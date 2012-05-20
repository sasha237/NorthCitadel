using System;
using System.Collections.Generic;
using System.Text;
using NerZul.Core.Utils;

namespace Engine
{
    public static class Deploy
    {
        static string group;
        static int id = 0;
        private static void PrintUsage()
        {
            ConsoleLog.WriteLine("Usage: deploy group title");
        }
        public static void Worker(string[] args)
        {
//#if PUBLIC_BUILD
            if (!Globals.IsValid())
                return;
//#endif

            if (args.Length != 3)
            {
                PrintUsage();
                return;
            }
            group = args[1];
            group = args[2];
            int.TryParse(args[2], out id);

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

            int poolsize = Globals.threadCount;
            if (Globals.BotConfig.useTOR)
                poolsize = 1;

            try
            {
                bots = DbRows.MixList(bots);
                NerZul.Core.Utils.Bicycles.ThreadPool.ExecInPool(BotProc2, bots, poolsize, true, Globals.ShowDlg);
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Deploy error: " + e.Message);
            }
        }

        static void BotProc2(object botnfo)
        {
//             var botinfo = (DbRow)botnfo;
//             //инициализируем класс
//             NerZul.Core.Utils.ManagedCitizen Bot = new NerZul.Core.Utils.ManagedCitizen(
//                 (string)botinfo["login"], (string)botinfo["password"], Globals.BotConfig);
//             //Пытаемся залогиниться через проксики
//             if (Bot.Login() == NerZul.Core.Utils.ManagedCitizen.LoginResult.Success)
//             //Bot.Bot.Login();	//if(true)
//             {
//                 ConsoleLog.WriteLine("Logged in - " + botinfo["login"]);
//                 botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);
//                 try
//                 {
//                     if (((int)botinfo["activated"]) == 0)
//                     {
//                         Bot.Bot.Activate();
//                         botinfo["activated"] = 1;
//                         Utils.UpdateDbWithCustomBotInfo(botinfo, "activated");
//                         botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);
//                     }
//                 }
//                 catch (System.Exception e)
//                 {
//                     ConsoleLog.WriteLine("Error activation!");
//                 }
//                 List<int> lst = new List<int>();
//                 lst.Add(1);
//                 lst.Add(2);
//                 lst.Add(3);
//                 lst.Add(4);
//                 lst = MixList(lst);
//                 try
//                 {
//                     //АВАТАРКА
//                     if (!Bot.Info.HasAvatar)
//                     {
// 
//                         string Path = System.IO.Path.Combine(Globals.DataDir, "avatars");
//                         Path = System.IO.Path.Combine(Path, Globals.Avatars.GetRandomString());
//                         Bot.Bot.UploadAvatar(System.IO.File.ReadAllBytes(Path));
//                         botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);
//                     }
//                 }
//                 catch (System.Exception e)
//                 {
//                     ConsoleLog.WriteLine("Error uploading avatar!");
//                 }
// 
//                 for (List<int>.Enumerator e = lst.GetEnumerator(); e.MoveNext(); )
//                 {
//                     Random rnd = new System.Random();
//                     if (rnd.Next(101) > 75)
//                         continue;
//                     switch (e.Current)
//                     {
//                         case 1:
//                             try
//                             {
//                                 //Если light-режим, то работаем только 1 раз
//                                 if ((((int)botinfo["last_day_work"] < Globals.GetErepTime().Days) &&
//                                     (mode != "light")) || ((int)botinfo["last_day_work"] == 0))
//                                 {
//                                     //Вот тут мы разруливаем трудоустройство
//                                     int work = defwork;
//                                     if (work == 0)
//                                     {
//                                         var offers = Bot.Bot.GetJobOffers(Bot.Info.Country, industry, 1);
//                                         if (offers != null)
//                                         {
//                                             //ищем самое дорогое предложение с минимальным Q
//                                             int minq = 5;
//                                             foreach (var offer in offers)
//                                             {
//                                                 if (offer.GetValue("quality") < minq)
//                                                     minq = offer.GetValue("quality");
//                                             }
//                                             foreach (var offer in offers)
//                                             {
//                                                 if (offer.GetValue("quality") == minq)
//                                                 {
//                                                     work = offer.GetValue("id");
//                                                     break;
//                                                 }
//                                             }
//                                         }
//                                         if (work != 0)
//                                         {
//                                             if (!Bot.Bot.Hire(work))
//                                             {
//                                                 ConsoleLog.WriteLine("Unable to get job - " + botinfo["login"]);
//                                             }
//                                         }
//                                     }
// 
//                                     //РАБОТА
//                                     if (Bot.Bot.Work(5))
//                                     {
//                                         System.IO.File.WriteAllText("lol", Bot.Bot.GetLastResponse());
//                                         botinfo["last_day_work"] = Globals.GetErepTime().Days;
//                                         Utils.UpdateDbWithCustomBotInfo(botinfo, "last_day_work");
//                                     }
//                                     else
//                                         ConsoleLog.WriteLine("Unable to work - " + botinfo["login"]);
//                                 }
//                             }
//                             catch (System.Exception e1)
//                             {
// 
//                             }
//                             break;
//                         case 2:
//                             try
//                             {
//                                 //ТРЕНЬКА
//                                 //Если light-режим, то тренимся только 1 раз
//                                 if ((((int)botinfo["last_day_train"] < Globals.GetErepTime().Days) &&
//                                     (mode != "light")) || ((int)botinfo["last_day_train"] == 0))
//                                 {
//                                     if (Bot.Bot.Train(5))
//                                     {
//                                         botinfo["last_day_train"] = Globals.GetErepTime().Days;
//                                         Utils.UpdateDbWithCustomBotInfo(botinfo, "last_day_train");
//                                         botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);
//                                     }
//                                     else
//                                         ConsoleLog.WriteLine("Unable to train - " + botinfo["login"]);
//                                 }
//                             }
//                             catch (System.Exception e1)
//                             {
// 
//                             }
//                             break;
//                         case 3:
//                             try
//                             {
//                                 //УЧЕБА
//                                 //Если light-режим, то учимся только 1 раз
//                                 if ((((int)botinfo["last_day_study"] < Globals.GetErepTime().Days) &&
//                                     (mode != "light")) || ((int)botinfo["last_day_study"] == 0))
//                                 {
//                                     if (Bot.Bot.Study(5))
//                                     {
//                                         botinfo["last_day_study"] = Globals.GetErepTime().Days;
//                                         Utils.UpdateDbWithCustomBotInfo(botinfo, "last_day_study");
//                                         botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);
//                                     }
//                                     else
//                                         ConsoleLog.WriteLine("Unable to study - " + botinfo["login"]);
//                                 }
//                             }
//                             catch (System.Exception e1)
//                             {
// 
//                             }
//                             break;
//                         case 4:
//                             try
//                             {
//                                 //ОТДЫХ
//                                 //Если light-режим, то отдыхаем только 1 раз
//                                 if ((((int)botinfo["last_day_relax"] < Globals.GetErepTime().Days) &&
//                                     (mode != "light")) || ((int)botinfo["last_day_relax"] == 0))
//                                 {
//                                     if (Bot.Bot.Entertain(9))
//                                     {
//                                         botinfo["last_day_relax"] = Globals.GetErepTime().Days;
//                                         Utils.UpdateDbWithCustomBotInfo(botinfo, "last_day_relax");
//                                         botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);
//                                     }
//                                     else
//                                         ConsoleLog.WriteLine("Unable to relax - " + botinfo["login"]);
//                                 }
//                             }
//                             catch (System.Exception e1)
//                             {
// 
//                             }
//                             break;
//                     }
//                 }
// 
// 
// 
// 
// 
// 
// 
// 
// 
//             }
//             else
//             {
//                 ConsoleLog.WriteLine("Unable to login - " + botinfo["login"]);
//             }
        }
    }
}
