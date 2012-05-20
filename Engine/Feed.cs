using System;
using System.Collections.Generic;
using System.Text;
using NerZul.Core.Utils;

namespace Engine
{
    public class Feed
    {
        static string group;
        static int food_quality = 0;
        static int for_health = 0;
        static int less_then = 0;
        static bool justEat = false;
        static bool fastFood = false;
        static bool hungryFirst = false;

        private static void PrintUsage()
        {
            ConsoleLog.WriteLine("Usage: feed group food_quality for_health less_then just_eat fast_food hungry_first");
            ConsoleLog.WriteLine("Example: feed my_group 2 98 80 false true false");
        }

        public static void Worker(string[] args)
        {
//#if PUBLIC_BUILD
            if (!Globals.IsValid())
                return;
//#endif

            if (args.Length != 8)
            {
                PrintUsage();
                return;
            }
            group = args[1];
            if (!int.TryParse(args[2], out food_quality))
            {
                ConsoleLog.WriteLine("Wrong food_health");
                return;
            }
            if (food_quality == 0) food_quality = 1;
            if (!int.TryParse(args[3], out for_health))
            {
                ConsoleLog.WriteLine("Wrong food_quality");
                return;
            }
            args[4] = (String.IsNullOrEmpty(args[4]) || (args[4] == "0")) ? "101" : args[4];
            if (!int.TryParse(args[4], out less_then))
            {
                ConsoleLog.WriteLine("Wrong less_then");
                return;
            }
            if (!bool.TryParse(args[5], out justEat))
            {
                ConsoleLog.WriteLine("Wrong just_eat");
                return;
            }
            if (!bool.TryParse(args[6], out fastFood))
            {
                ConsoleLog.WriteLine("Wrong fast_food");
                return;
            }
            if (!bool.TryParse(args[7], out hungryFirst))
            {
                ConsoleLog.WriteLine("Wrong hungry_first");
                return;
            }

            DbRows bots = null;
            lock (Globals.DBLocker)
            {
                Globals.Database.Reset();
                if (group.ToLower() != "all")
                    Globals.Database.Where("group", group);
                Globals.Database.Where("banned", 0);
                Globals.Database.Where("(wellness < " + less_then.ToString() + " )");
                if (!String.IsNullOrEmpty(Globals.addWhere))
                    Globals.Database.Where(Globals.addWhere);
                if (hungryFirst)
                    Globals.Database.Order("wellness asc");
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
                if (!hungryFirst)
                    bots = DbRows.MixList(bots);
                NerZul.Core.Utils.Bicycles.ThreadPool.ExecInPool(BotProc, bots, poolsize, true, Globals.ShowDlg);
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Feed error: " + e.Message);
            }

            ConsoleLog.WriteLine("ЖРАТ окончено!");
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

            try
            {
                //инициализируем класс
                ManagedCitizen Bot = new NerZul.Core.Utils.ManagedCitizen(
                    (string)botinfo["login"],
                (string)botinfo["email"],
                    (string)botinfo["password"], 
                    Globals.BotConfig);
                //Пытаемся залогиниться через проксики

                ManagedCitizen.LoginResult loginResult;

                loginResult = Bot.Login();

                if (loginResult == ManagedCitizen.LoginResult.Success)
                {
                    botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);
                    ConsoleLog.WriteLine("Logged in - " + Bot.Bot.LoginName);

                    Bot.Feed(!Globals.BotConfig.useTOR, (int)botinfo["country"], food_quality, for_health, justEat, fastFood);

                    Utils.TryToUpdateDbWithCurrentInfo(botinfo, Bot);
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
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Feed exception: " + e.Message);
            }
        }
    }
}
