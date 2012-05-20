using System;
using System.Collections.Generic;
using System.Text;
using NerZul.Core.Utils;
using System.Text.RegularExpressions;
using NerZul.Core.Network;

namespace Engine
{
    public class CollectQuests
    {
        static string group;
        static bool collectAll;
        static bool buyFood;
		static bool buyWeapons;

        private static void PrintUsage()
        {
            ConsoleLog.WriteLine("Usage: collectquests group collect_all buy_3_food buy_3_weapons");
            ConsoleLog.WriteLine("Example: collectquests my_group 0 1 0");
        }

        public static void Worker(string[] args)
        {
//#if PUBLIC_BUILD
            if (!Globals.IsValid())
                return;
//#endif

            if (args.Length != 5)
            {
                PrintUsage();
                return;
            }

            group = args[1];
			collectAll = args[2] == "1";
			buyFood = args[3] == "1";
			buyWeapons = args[4] == "1";

            DbRows bots = null;
            lock (Globals.DBLocker)
            {
                Globals.Database.Reset();
                if (group.ToLower() != "all")
                    Globals.Database.Where("group", group);
                Globals.Database.Where("banned", 0);
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
                ConsoleLog.WriteLine("CollectQuests error: " + e.Message);
            }

            ConsoleLog.WriteLine("Сбор квестов окончен!");
        }

        static void BotProc(object botnfo)
        {
            var botinfo = (DbRow)botnfo;
            Random rnd = new System.Random();

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
                if (Bot.Login() == ManagedCitizen.LoginResult.Success)
                //Bot.Bot.Login();	//if(true)
                {
                    botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);
                    ConsoleLog.WriteLine("Logged in - " + Bot.Bot.LoginName);

                    if (buyFood)
                    {
                        ConsoleLog.WriteLine("Покупает 3 булки");
                        Bot.Bot.BuyItem(int.Parse(botinfo["country"].ToString()), Goods.Food, 3, 0, 0, true);
                    }

					if (buyWeapons) 
                    {
						ConsoleLog.WriteLine("Покупает 3 пушки");
						Bot.Bot.BuyItem(int.Parse(botinfo["country"].ToString()), Goods.Weapon, 3, 0, 0, true);
					}

                    for (int i = 0; i < 5; i++)
                    {
                        string response = Bot.Bot.m_Client.DownloadString("http://www.erepublik.com/en");

                        Match m = Regex.Match(response, "var missionsJSON = (\\[.*?\\]);\\s*var csrfToken = '([^']+)';");
                        string MissionsJson = m.Groups[1].Value;
                        string MissionsToken = m.Groups[2].Value;

                        if (collectAll)
                        {
                            for (int q = 1; q < 16; i++)
                            {
                                Bot.Bot.DoMission(q, false, MissionsToken);
                            }
                            break;
                        }
                        else
                        {
                            if (Bot.Bot.DoMissionsFromJson(MissionsJson, MissionsToken) == 0)
                                break;
                        }
                    }


					/*
                    for (int i = 0; i < 2; i++)
                    {
                        ConsoleLog.WriteLine(
                            Bot.Bot.LoginName + " - try to buy 3 " + ((i == 0) ? "food" : "guns"));

                        try
                        {
                            System.Threading.Thread.Sleep(rnd.Next(1000, 2000));
                            // Первые 3 раза покупаем булки, вторые 3 раза пушки
                            Bot.Bot.BuyItem(int.Parse(botinfo["country"].ToString()), (i==0)?Goods.Food:Goods.Weapon, 3, 0, 0, 0, 0);
                            Bot.GetInfoFromCommonResponse();
                            ConsoleLog.WriteLine(
                                Bot.Bot.LoginName + " - after buy. " +
                                "Currency left: " + Bot.Info.Nat_occur.ToString());
                        }
                        catch (Exception e)
                        {
                            ConsoleLog.WriteLine("Error buying: " + e.Message);
                        }
                    }
					*/

					/*
                    //Utils.TryToUpdateDbWithCurrentInfo(botinfo, Bot);
                    //Вроде нечем апдейтить, к сожалению.

                    #region FEED
                    try
                    {
                        ConsoleLog.WriteLine(Bot.Bot.LoginName + " - collecting FEED quest");
                        Bot.Bot.RewardFeed();
                    }
                    catch (Exception e)
                    {
                        ConsoleLog.WriteLine("Error collecting FEED quest: " + e.Message);
                    }                    
                    #endregion

                    #region WEAPON
                    try
                    {
                        ConsoleLog.WriteLine(Bot.Bot.LoginName + " - collecting WEAPON quest");
                        Bot.Bot.RewardWeapoon();
                    }
                    catch (Exception e)
                    {
                        ConsoleLog.WriteLine("Error collecting WEAPON quest: " + e.Message);
                    } 
                    #endregion

                    #region HERO
                    try
                    {
                        ConsoleLog.WriteLine(Bot.Bot.LoginName + " - collecting HERO quest");
                        Bot.Bot.RewardHero();
                    }
                    catch (Exception e)
                    {
                        ConsoleLog.WriteLine("Error collecting HERO quest: " + e.Message);
                    } 
                    #endregion
                    
                    #region WORKING ROW
                    try
                    {
                        ConsoleLog.WriteLine(Bot.Bot.LoginName + " - collecting WORKING ROW quest");
                        Bot.Bot.RewardWorkingRow();
                    }
                    catch (Exception e)
                    {
                        ConsoleLog.WriteLine("Error collecting WORKING ROW quest: " + e.Message);
                    } 
                    #endregion

                    #region SOCIETY
                    try
                    {
                       ConsoleLog.WriteLine(Bot.Bot.LoginName + " - collecting SOCIETY quest");
                       Bot.Bot.RewardSociety();
                    }
                    catch (Exception e)
                    {
                       ConsoleLog.WriteLine("Error collecting SOCIETY quest: " + e.Message);
                    } 
                    #endregion
					*/
                }
                else
                {
                    ConsoleLog.WriteLine("Unable to login - " + botinfo["login"]);
                }
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("CollectQuests exception: " + e.Message);
            }
        }
    }
}
