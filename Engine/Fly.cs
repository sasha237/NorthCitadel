using System;
using System.Collections.Generic;
using NerZul.Core.Utils;
using NerZul.Core.Network;
namespace Engine
{
    public static class Fly
    {
        static string group;
        static int iCountry;
        static int iRegion;
        static int iDistance;
        private static void PrintUsage()
        {
            ConsoleLog.WriteLine("Usage: fly group ticket_distance country_id region_id");
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
            int.TryParse(args[2], out iDistance);
            int.TryParse(args[3], out iCountry);
            int.TryParse(args[4], out iRegion);

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
                ConsoleLog.WriteLine("Fly error: " + e.Message);
            }

            ConsoleLog.WriteLine("Fly end!");
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
                botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);
                
                try
                {
                    Bot.Bot.Resign();
                }
                catch (System.Exception e)
                {
                    ConsoleLog.WriteLine("Bot resign error: " + e.Message);
                }

                try
                {
                    System.Threading.Thread.Sleep(rnd.Next(1000, 2000));
                    Bot.Bot.BuyItem(int.Parse(botinfo["country"].ToString()), Goods.Ticket, 1, iDistance, 0, true);
                }
                catch (System.Exception e)
                {
                    ConsoleLog.WriteLine("Bot buy ticket error: " + e.Message);
                }

                try
                {
                    Bot.Bot.Fly(iCountry, iRegion);
                    botinfo = Utils.TryToUpdateDbWithCurrentInfo(botinfo, Bot);
                }
                catch (System.Exception e)
                {
                    ConsoleLog.WriteLine("Bot fly error: " + e.Message);
                }
            }
        }
    }
}
