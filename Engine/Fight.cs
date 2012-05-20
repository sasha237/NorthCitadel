using System;
using System.Collections.Generic;
using NerZul.Core.Utils;
namespace Engine
{
    public static class Fight
    {
        static string group;
        static int iBattle = 0;
        static int iCountry = 0;
        static bool bBuyWeapon = false;
        static bool bBuyFood = false;
        static int iLeftHP = 0;
        static int iLeftFood = 0;
        static int iShotLimit = 0;
        static bool doNotChange = true;
        static bool cyclicFight = false;
        static int iExpLimit = 0;

        private static void PrintUsage()
        {
            ConsoleLog.WriteLine("Usage: fight group battle_id buy_weapon(true|false) buy_food(true|false) left_hp left_food country_id(id|0) do_not_change_weapon(true|false) shot_limit cyclic_fight(true|false) experience_limit");
        }
        public static void Worker(string[] args)
        {
//#if PUBLIC_BUILD
            if (!Globals.IsValid())
                return;
//#endif

            if (args.Length != 12)
            {
                PrintUsage();
                return;
            }
            group = args[1];
            int.TryParse(args[2], out iBattle);
            bool.TryParse(args[3], out bBuyWeapon);
            bool.TryParse(args[4], out bBuyFood);
            int.TryParse(args[5], out iLeftHP);
            int.TryParse(args[6], out iLeftFood);
            int.TryParse(args[7], out iCountry);
            bool.TryParse(args[8], out doNotChange);
            int.TryParse(args[9], out iShotLimit);
            bool.TryParse(args[10], out cyclicFight);
            int.TryParse(args[11], out iExpLimit);

            if (iLeftHP < 21)
                iLeftHP = 21;

            //ConsoleLog.WriteLine("DateTime: " + DateTime.Now.ToFileTime());
            //return;

            DbRows bots = null;
            lock (Globals.DBLocker)
            {
                Globals.Database.Reset();
                if (group.ToLower() != "all")
                    Globals.Database.Where("group", group);
                Globals.Database.Where("banned", 0);
                if (!cyclicFight && iLeftHP>0) Globals.Database.Where("wellness", iLeftHP, ">");
                if (iLeftFood > 0) Globals.Database.Where("food_qty", iLeftFood+1, ">");
                if (iExpLimit > 0) Globals.Database.Where("experience", iExpLimit, "<");
                if (!String.IsNullOrEmpty(Globals.addWhere))
                    Globals.Database.Where(Globals.addWhere);
                bots = Globals.Database.Select("bots");
                Globals.Database.Reset();
            }
            if (!Globals.webCitadel.SendLogInfo(args, bots.Count))
                return;

            int poolsize = Globals.threadCount;
            if (Globals.BotConfig.useTOR)
                poolsize = 1;

            try
            {
                Globals.totalBotCounter = bots.Count;
                Globals.processedBotCounter = 0;
                bots = DbRows.MixList(bots);
                NerZul.Core.Utils.Bicycles.ThreadPool.ExecInPool(BotProc, bots, poolsize, true, Globals.ShowDlg);
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Fight worker error: " + e.Message);
            }

            ConsoleLog.WriteLine("Воевать окончено!");
        }

        static void BotProc(object botnfo)
        {
            var botinfo = (DbRow)botnfo;

            Globals.processedBotCounter++;
            ConsoleLog.WriteLine(
                "Processing bot " +
                Globals.processedBotCounter.ToString() + "/" +
                Globals.totalBotCounter.ToString() + ": " +
                (string)botinfo["login"]);

            //инициализируем класс
            NerZul.Core.Utils.ManagedCitizen Bot = new NerZul.Core.Utils.ManagedCitizen(
                (string)botinfo["login"],
                (string)botinfo["email"],
                (string)botinfo["password"], 
                Globals.BotConfig);

            ManagedCitizen.LoginResult loginResult;
            loginResult = Bot.Login();

            if (loginResult == ManagedCitizen.LoginResult.Success)
            {
                ConsoleLog.WriteLine("Logged in - " + botinfo["login"]);
                botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);

                // Смотрим состояние инвентаря
                if (Bot.GetStorageInfo(true))
                {
                    botinfo = Utils.TryToUpdateDbWithStorageInfo(botinfo, Bot);
                }

                if ((iLeftFood == 0) ||
                    (Bot.storageInfo.foodQty >= iLeftFood))
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        if (cyclicFight)
                            ConsoleLog.WriteLine("Cyclic fight, try " + i.ToString());


                        int iter = 0;
                        int retryCount = 0;
                        float wellness = (float)botinfo["wellness"];
                        float oldWellness = 0;

                        do
                        {
                            System.Threading.Thread.Sleep(1500);
                            iter++;

                            if (iter > 10) break;

                            if (wellness > iLeftHP)
                            {
                                if (cyclicFight)
                                    ConsoleLog.WriteLine("Cyclic fight, iteration " + iter.ToString() + ". Fight!");

                                Bot.Bot.FightInBattle(iBattle, ((bBuyWeapon) ? 1 : 0), (int)botinfo["country"], iLeftHP, iCountry, doNotChange, iShotLimit);
                            }

                            Bot.GetInfoFromCommonResponse(true);
                            botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);

                            if (cyclicFight)
                            {
                                ConsoleLog.WriteLine("Cyclic fight, iteration " + iter.ToString() + ". Feed!");
                                Bot.Feed(!Globals.BotConfig.useTOR, (int)botinfo["country"], Globals.defaultFoodQ, 97, !bBuyFood, true);
                            }

                            Bot.GetInfoFromCommonResponse(true);
                            botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);

                            wellness = (float)botinfo["wellness"];

                            ConsoleLog.WriteLine("Fight finished. Wellness left: " + wellness.ToString());

                            if (oldWellness == wellness)
                            {
                                retryCount++;
                            }
                            else
                            {
                                oldWellness = wellness;
                                retryCount = 0;
                            }

                            if ((!cyclicFight) ||
                                (retryCount >= 3) ||
                                ((iExpLimit != 0) && ((int)botinfo["experience"]) >= iExpLimit))
                                break;
                        }
                        while (wellness > iLeftHP);

                        if ((!cyclicFight) ||
                           ((iExpLimit != 0) && ((int)botinfo["experience"]) >= iExpLimit))
                            break;
                    }
                }

                // Смотрим состояние инвентаря
                if (Bot.GetStorageInfo(true))
                {
                    botinfo = Utils.TryToUpdateDbWithStorageInfo(botinfo, Bot);
                }
            }
            else
                if ((loginResult == ManagedCitizen.LoginResult.Banned) ||
                    (loginResult == ManagedCitizen.LoginResult.Banned2))
                {
                    botinfo["banned"] = (loginResult == ManagedCitizen.LoginResult.Banned) ? 1 : 2;
                    Utils.UpdateDbWithCustomBotInfo(botinfo, "banned");
                    //botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);
                }
                //else
                //    if (Bot.Bot.GetLastResponse().Contains("infringement"))
                //    {
                //        botinfo["banned"] = 1;
                //        Utils.UpdateDbWithCustomBotInfo(botinfo, "banned");
                //        //botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);
                //        ConsoleLog.WriteLine(botinfo["login"].ToString() + ": Banned");
                //    }
                else
                {
                    //ConsoleLog.WriteLine(Bot.Bot.GetLastResponse(), "LoginLog.txt");
                    ConsoleLog.WriteLine(botinfo["login"].ToString() + ": Possibly dead, see LoginLog.txt");
                }
        }

    }
}
