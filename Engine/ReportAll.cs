using System;
using System.Collections.Generic;
using NerZul.Core.Utils;

namespace Engine
{
    public static class ReportAll
    {
        static string group;
        static int iId;
        static int iType;
        static string lang;
        static string stype;
        private static void PrintUsage()
        {
            ConsoleLog.WriteLine("Usage: report group id type lang itemtype");
        }
        public static void Worker(string[] args)
        {
//#if PUBLIC_BUILD
            if (!Globals.IsValid())
                return;
//#endif

            if (args.Length != 6)
            {
                PrintUsage();
                return;
            }

            group = args[1];
            int.TryParse(args[2], out iId);
            int.TryParse(args[3], out iType);
            lang = args[4];
            stype = args[5];

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
                ConsoleLog.WriteLine("Worker error: " + e.Message);
            }

            ConsoleLog.WriteLine("ReportAll done!");

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

            ManagedCitizen.LoginResult loginResult;

            loginResult = Bot.Login();

            if (loginResult == ManagedCitizen.LoginResult.Success)
            {
                ConsoleLog.WriteLine("Logged in - " + botinfo["login"]);
                try
                {
                    botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);
                    Bot.Bot.ReportTicket(iId, iType, lang, stype);
                }
                catch (System.Exception e2)
                {
                    ConsoleLog.WriteLine("ReportAll error - " + botinfo["login"] + e2.ToString());
                }
            }
        }
    }
}
