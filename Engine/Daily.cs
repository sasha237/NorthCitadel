using System;
using System.Collections.Generic;
using NerZul.Core.Utils;
namespace Engine
{
	public static class Daily
	{
		static string mode;
		static string group;
		static int exactVacancy=0;
		static string industry;
        static bool resignBeforeWork;
        static int doFeed;

		private static void PrintUsage()
		{
            ConsoleLog.WriteLine("Usage: daily group exact_vacancy(0|id) industry mode(normal|light|self) resignBeforeWork(true|false) doFeed(0|1|2)");
		}

		public static void Worker(string[] args)
		{
//#if PUBLIC_BUILD
            if (!Globals.IsValid())
                return;
//#endif

			if(args.Length!=7)
			{
				PrintUsage();
				return;
			}
            if ((args[4] != "normal") && (args[4] != "light") && (args[4] != "self"))
			{
				PrintUsage();
				return;
			}
			group=args[1];
            int.TryParse(args[2], out exactVacancy);
            industry = args[3];
            mode = args[4];
            bool.TryParse(args[5], out resignBeforeWork);
            int.TryParse(args[6], out doFeed);

            DbRows bots=null;
			lock (Globals.DBLocker)
			{
				Globals.Database.Reset();
                if (mode == "light")
                {
                    /*
					//Дохляки нам не интересны при таком раскладе
					Globals.Database.Where("( isnull(wellness) OR wellness>40)");
                     */
                    // ВСЕХ смотрим (Badiboy)
                    Globals.Database.Where("(1 = 1)");
                }
                else
				{
					Globals.Database.Where(String.Format(
                        "(last_day_work < {0} or last_day_train < {0})",
					    Globals.GetErepTime().Days));
				}
				if(group.ToLower()!="all")
					Globals.Database.Where("group",group);
                Globals.Database.Where("banned", 0);
                if (!String.IsNullOrEmpty(Globals.addWhere))
                    Globals.Database.Where(Globals.addWhere);
				bots=Globals.Database.Select("bots");
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
                ConsoleLog.WriteLine("Worker error: " + e.Message);
			}

            ConsoleLog.WriteLine("Ежедневная работа окончена!");
            /*
            ConsoleLog.WriteLine("Daily. ThreadCount: " + System.Diagnostics.Process.GetCurrentProcess().Threads.Count.ToString());
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            ConsoleLog.WriteLine("Daily. ThreadCount: " + System.Diagnostics.Process.GetCurrentProcess().Threads.Count.ToString());
             */
        }

        /// <summary>
        /// Режим Light. Проверка состояния бота
        /// </summary>
        static void BotLight(DbRow botinfo, ManagedCitizen Bot, System.Random rnd)
        {
            if (resignBeforeWork)
            {
                if (!Bot.Bot.Resign())
                {
                    ConsoleLog.WriteLine("Unable to resign from job - " + botinfo["login"]);
                }
                if (!Globals.BotConfig.useTOR) System.Threading.Thread.Sleep(rnd.Next(1000, 2000));
            }
        }

        /// <summary>
        /// Ежедневная работа
        /// </summary>
        static void BotWork(DbRow botinfo, ManagedCitizen Bot, System.Random rnd)
        {
            int workindustry = 1;
            int curVacancy = exactVacancy;

            for (int i = 0; i < 3; i++)
            {
                if ((int)botinfo["last_day_work"] < Globals.GetErepTime().Days)
                {
                    if (resignBeforeWork)
                    {
                        if (!Bot.Bot.Resign())
                        {
                            ConsoleLog.WriteLine("Unable to resign from job - " + botinfo["login"]);
                        }
                        if (!Globals.BotConfig.useTOR) System.Threading.Thread.Sleep(rnd.Next(1000, 2000));
                    }

                    if (mode == "self")
                    {
                        Daily.BotWorkSelf(botinfo, Bot, rnd);
                        return;
                    }

                    //Вот тут мы разруливаем трудоустройство
                    int work = 0;
                    var offers = Bot.Bot.GetJobOffers(Bot.Info.Country, industry, 0, 1, curVacancy);
                    if (!Globals.BotConfig.useTOR) System.Threading.Thread.Sleep(rnd.Next(2000, 3000));
                    //if (curVacancy != 0)
                    //    ConsoleLog.WriteLine(Bot.Bot.GetLastResponse(), "FindJobLog.txt");
                    if (offers != null)
                    {
                        if (curVacancy == 0)
                        {
                            int minq = 5;
                            /*
                             Ищем самое дорогое предложение с минимальным Q, меньше или равно заданному.
                             ПРОПУСКАЕМ первые два предложения. Там частенько бывают предложения,
                             на которые не дают устраиваться. Денег нет на фирме или еще что... 
                             */

                            int skip = 3;
                            foreach (var offer in offers)
                            {
                                skip--;
                                if (skip > 0) continue;

                                // Берем вот ту, что стоит третьей. Мало ли лучше не найдем
                                if (skip == 0)
                                {
                                    work = offer.GetValue("id");
                                    workindustry = offer.GetValue("industry");
                                }

                                // Если нашли с заданным Q -- то и совсем хорошо
                                if (offer.GetValue("quality") <= minq)
                                {
                                    work = offer.GetValue("id");
                                    workindustry = offer.GetValue("industry");
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (offers.Count != 0)
                            {
                                work = offers[0].GetValue("id");
                                workindustry = offers[0].GetValue("industry");
                            }
                            else
                            {
                                work = 0;
                                workindustry = 0;
                            }

                        }

                        if (work != 0)
                        {
                            if (!Bot.Bot.Hire(work))
                            {
                                ConsoleLog.WriteLine("Unable to get job - " + botinfo["login"]);
                                break;
                            }
                            else
                            {
                                if (!Globals.BotConfig.useTOR) System.Threading.Thread.Sleep(rnd.Next(1000, 2000));
                                botinfo["industry"] = workindustry;
                                Utils.UpdateDbWithCustomBotInfo(botinfo, "industry");
                            }
                        }
                        else
                        {
                            ConsoleLog.WriteLine("Вакансии с ID=" + curVacancy.ToString() + " не удалось найти!");
                            if ((curVacancy == 0) || (!Globals.TryAnotherWork))
                            {
                                break;
                            }
                            else
                            {
                                ConsoleLog.WriteLine("Ищем первую попавшуюся вакансию.");
                                curVacancy = 0;
                                continue;
                            }
                        }
                    }

                    //РАБОТА
                    if (Bot.Bot.Work((uint)botinfo["citizen_id"]) != 0)
                    {
                        botinfo["last_day_work"] = Globals.GetErepTime().Days;
                        Utils.UpdateDbWithCustomBotInfo(botinfo, "last_day_work");
                    }
                    else
                    {
                        ConsoleLog.WriteLine("Unable to work - " + botinfo["login"]);
                        //ConsoleLog.WriteLine(Bot.Bot.GetLastResponse(), "WorkLog.txt");
                    }
                }

                break;
            }
        }

        /// <summary>
        /// Ежедневная работа на своих фирмах
        /// </summary>
        static void BotWorkSelf(DbRow botinfo, ManagedCitizen Bot, System.Random rnd)
        {
            for (int i = 0; i < 6; i++)
            {
                string land = "http://economy.erepublik.com/en/land/overview/" + ((uint)botinfo["citizen_id"]).ToString();
                string m_Response = Bot.Bot.CustomRequest(land);
                //ConsoleLog.WriteLine(m_Response, "LandOVerviewLog.txt");

                if (!m_Response.Contains("/en/company/restore"))
                {
                    break;
                }

                ConsoleLog.WriteLine("Not restored firm found. Restoring.");

                Bot.Bot.RestoreSelfFirm((uint)botinfo["citizen_id"]);
            }

            for (int i = 0; i < 6; i++)
            {
                //РАБОТА
                int workRes = Bot.Bot.Work((uint)botinfo["citizen_id"]);
                
                ConsoleLog.WriteLine("Work try " + i.ToString() + " - " + botinfo["login"] + " - Result: " + workRes.ToString());

                if (workRes == 0)
                {
                    ConsoleLog.WriteLine("Unable to work - " + botinfo["login"] + " - " + Bot.Bot.GetLastResponse());
                    //ConsoleLog.WriteLine(Bot.Bot.GetLastResponse(), "WorkLog.txt");
                }
                if (workRes == 1)
                {
                    // Успешно где-то отработали
                }
                if (workRes == 2)
                {
                    botinfo["last_day_work"] = Globals.GetErepTime().Days;
                    Utils.UpdateDbWithCustomBotInfo(botinfo, "last_day_work");
                    break;
                }
                if (workRes == 3)
                {
                    ConsoleLog.WriteLine("\"Not enough slots\" message, visiting inventory - " + botinfo["login"] + " - " + Bot.Bot.GetLastResponse());
                    Bot.Bot.VisitInventory();
                }
            }
        }

        /// <summary>
        /// Ежедневная тренька
        /// </summary>
        static void BotTrain(DbRow botinfo, ManagedCitizen Bot, System.Random rnd)
        {
            //Если light-режим, то тренимся только 1 раз
            if ((int)botinfo["last_day_train"] < Globals.GetErepTime().Days)
            {
                if (Bot.Bot.Train((uint)botinfo["citizen_id"]))
                {
                    botinfo["last_day_train"] = Globals.GetErepTime().Days;
                    //Utils.UpdateDbWithCustomBotInfo(botinfo, "last_day_train");
                    //botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);
                }
                else
                    ConsoleLog.WriteLine("Unable to train - " + botinfo["login"]);
            }
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

            //инициализируем класс
            NerZul.Core.Utils.ManagedCitizen Bot = new NerZul.Core.Utils.ManagedCitizen(
                (string)botinfo["login"],
                (string)botinfo["email"],
                (string)botinfo["password"],
                Globals.BotConfig);
            //Пытаемся залогиниться через проксики

            ManagedCitizen.LoginResult loginResult = Bot.Login();
            if (loginResult == ManagedCitizen.LoginResult.Success)
            {
                try
                {
                    Bot.Bot.m_Client.Timeouts = 0;

                    ConsoleLog.WriteLine("Logged in - " + botinfo["login"]);
                    botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);

                    #region Активируем, если в базе нет отметки что активен. Посещаем диснейленд в первый раз
                    try
                    {
                        if (((int)botinfo["activated"]) == 0)
                        {
                            if (!Globals.BotConfig.useTOR) System.Threading.Thread.Sleep(rnd.Next(2000, 5000));
                            Bot.Bot.Activate();
                            botinfo["activated"] = 1;
                            Utils.UpdateDbWithCustomBotInfo(botinfo, "activated");
                            botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);

                            ConsoleLog.WriteLine("Visit 'Land' for the first time");
                            Bot.Bot.VisitLand((uint)botinfo["citizen_id"]);

                        }
                    }
                    catch (System.Exception e)
                    {
                        ConsoleLog.WriteLine("Error activation: " + e.Message);
                    }

                    if (Bot.Bot.m_Client.Timeouts >= Globals.timeoutsLimit)
                        throw new Exception("Timeout limit reached.");
                    #endregion

                    // Смотрим состояние инвентаря
                    if (Bot.GetStorageInfo(true))
                    {
                        botinfo = Utils.TryToUpdateDbWithStorageInfo(botinfo, Bot);
                    }

                    #region Light mode. Light так light. НИЧЕГО не делаем, только собираем инфу и выходим
                    if (mode == "light")
                    {
                        if (!Globals.BotConfig.useTOR) System.Threading.Thread.Sleep(rnd.Next(1000, 2000));
                        Daily.BotLight(botinfo, Bot, rnd);
                        return;
                    }
                    #endregion


                    #region Покупаем и едим еду 1
                    if (doFeed == 1)
                        Bot.Feed(!Globals.BotConfig.useTOR, (int)botinfo["country"], Globals.defaultFoodQ, 99, true, true);

                    if (Bot.Bot.m_Client.Timeouts >= Globals.timeoutsLimit)
                        throw new Exception("Timeout limit reached.");
                    #endregion


                    #region Работаем, тренькаемся, читаем алерты, читаем статьи, смотрим TreasureMap
                    //, учимся, отдыхаем
                    List<int> lst = new List<int>();
                    lst.Add(1);
                    lst.Add(2);
                    lst.Add(3);
                    lst.Add(4);
                    lst = CommonUtils.MixList(lst);
                    for (List<int>.Enumerator e = lst.GetEnumerator(); e.MoveNext(); )
                    {
                        if (!Globals.BotConfig.useTOR) System.Threading.Thread.Sleep(rnd.Next(1000, 2000));
                        switch (e.Current)
                        {
                            case 1: // Работа
                                for (int i = 1; i <= 3; i++)
                                {
                                    try
                                    {
                                        Daily.BotWork(botinfo, Bot, rnd);
                                        break;
                                    }
                                    catch (System.Exception e1)
                                    {
                                        ConsoleLog.WriteLine("Bot work error: " + e1.Message);
                                    }
                                }
                                break;
                            case 2: // Тренька
                                for (int i = 1; i <= 3; i++)
                                {
                                    try
                                    {
                                        Daily.BotTrain(botinfo, Bot, rnd);
                                        break;
                                    }
                                    catch (System.Exception e1)
                                    {
                                        ConsoleLog.WriteLine("Bot train error: " + e1.Message);
                                    }
                                }
                                break;
                            case 3: // Аллерты
                                try
                                {
                                    Bot.Bot.LookAlerts();
                                }
                                catch (System.Exception e1)
                                {
                                    ConsoleLog.WriteLine("Bot look alerts error: " + e1.Message);
                                }
                                break;
                            case 4: // Статьи
                                try
                                {
                                    Bot.Bot.ReadRandomTopArticle();
                                    //if (!Globals.BotConfig.useTOR) System.Threading.Thread.Sleep(rnd.Next(15000, 20000));
                                    Bot.Bot.ReadRandomTopArticle();
                                }
                                catch (System.Exception e1)
                                {
                                    ConsoleLog.WriteLine("Bot read article error: " + e1.Message);
                                }
                                break;
                        }

                        if (Bot.Bot.m_Client.Timeouts >= Globals.timeoutsLimit)
                            throw new Exception("Timeout limit reached.");
                    }
                    #endregion


                    #region Получаем дневной reward
                    if (!Globals.BotConfig.useTOR) System.Threading.Thread.Sleep(rnd.Next(1000, 2000));
                    for (int i = 1; i <= 3; i++)
                    {
                        try
                        {
                            Bot.Bot.DailyReward();
                            break;
                        }
                        catch (System.Exception e)
                        {
                            ConsoleLog.WriteLine("Bot get daily reward error: " + e.Message);
                        }

                        if (Bot.Bot.m_Client.Timeouts >= Globals.timeoutsLimit)
                            throw new Exception("Timeout limit reached.");
                    }
                    #endregion


                    #region Покупаем и едим еду 2
                    if (doFeed == 2)
                        Bot.Feed(!Globals.BotConfig.useTOR, (int)botinfo["country"], Globals.defaultFoodQ, 99, true, true);

                    if (Bot.Bot.m_Client.Timeouts >= Globals.timeoutsLimit)
                        throw new Exception("Timeout limit reached.");
                    #endregion


                    // Смотрим состояние инвентаря
                    if (Bot.GetStorageInfo(true))
                    {
                        botinfo = Utils.TryToUpdateDbWithStorageInfo(botinfo, Bot);
                    }


                    #region Грузим аватару, если не загружена
                    try
                    {
                        //АВАТАРКА
                        if (/*((int)botinfo["experience"] >= 61) && */(!Bot.Info.HasAvatar))
                        //if (false)
                        {
                            ConsoleLog.WriteLine(botinfo["login"] + ": Upload avatar");
                            if (!Globals.BotConfig.useTOR) System.Threading.Thread.Sleep(rnd.Next(1000, 2000));
                            string Path = System.IO.Path.Combine(Globals.DataDir, "avatars");
                            Path = System.IO.Path.Combine(Path, Globals.Avatars.GetRandomString());
                            Bot.Bot.UploadAvatar(System.IO.File.ReadAllBytes(Path), Path);
                            botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);
                        }
                    }
                    catch (System.Exception e)
                    {
                        ConsoleLog.WriteLine("Error uploading avatar: " + e.Message);
                    }
                    #endregion
                }
                finally
                {
                    // Апдейтим статус бота в базе
                    botinfo = Utils.TryToUpdateDbWithCurrentInfo(botinfo, Bot);
                }

                // Logout
                Bot.Bot.Logout();
            }
            else
                if ((loginResult == ManagedCitizen.LoginResult.Banned) ||
                    (loginResult == ManagedCitizen.LoginResult.Banned2))
                {
                    botinfo["banned"] = (loginResult == ManagedCitizen.LoginResult.Banned)?1:2;
                    Utils.UpdateDbWithCustomBotInfo(botinfo, "banned");
                }
                else
                {
                    //ConsoleLog.WriteLine(Bot.Bot.GetLastResponse(), "LoginLog.txt");
                    ConsoleLog.WriteLine(botinfo["login"].ToString() + ": Possibly dead, see LoginLog.txt");
                }
            ConsoleLog.WriteLine(botinfo["login"].ToString() + ": daily proc finished");
        }
	}
} 


