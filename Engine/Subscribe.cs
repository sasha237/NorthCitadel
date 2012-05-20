using System;
using System.Collections.Generic;
using System.Text;
using NerZul.Core.Utils;

namespace Engine
{
    public class Subscribe
    {
        static string group;
        static int id = 0;
        private static void PrintUsage()
        {
            ConsoleLog.WriteLine("Usage: subscribe group newspaper_id");
        }
        public static void Worker(string[] args)
        {
#if !PUBLIC_BUILD
            if (args.Length != 3)
            {
                PrintUsage();
                return;
            }
            group = args[1];
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
            Globals.webCitadel.SendLogInfo(args, bots.Count);

            int poolsize = Globals.threadCount;
            if (Globals.BotConfig.useTOR)
                poolsize = 1;

            try
            {
                bots = DbRows.MixList(bots);
                NerZul.Core.Utils.Bicycles.ThreadPool.ExecInPool(BotProc, bots, poolsize, true, Globals.ShowDlg);
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Error: " + e.Message);
            }
        }

        static void BotProc(object botnfo)
        {
            var botinfo = (DbRow)botnfo;
            //инициализируем класс
            NerZul.Core.Utils.ManagedCitizen Bot = new NerZul.Core.Utils.ManagedCitizen(
                (string)botinfo["login"],
                (string)botinfo["email"],
                (string)botinfo["password"],
                Globals.BotConfig);
            //Пытаемся залогиниться через проксики
            if (Bot.Login() == NerZul.Core.Utils.ManagedCitizen.LoginResult.Success)
            //Bot.Bot.Login();	//if(true)
            {
                ConsoleLog.WriteLine("Logged in - " + botinfo["login"]);
                botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);
                try
                {
                    if (((int)botinfo["activated"]) == 0)
                    {
                        Bot.Bot.Activate();
                        botinfo["activated"] = 1;
                        Utils.UpdateDbWithCustomBotInfo(botinfo, "activated");
                        botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);
                    }
                }
                catch (System.Exception e)
                {
                    ConsoleLog.WriteLine("Error: " + e.Message);
                }

                Bot.Bot.SubscribeNewspaper(id);
            }
            else
            {
                ConsoleLog.WriteLine("Unable to login - " + botinfo["login"]);
            }
#else
            ConsoleLog.WriteLine("Думаешь самый умный? Сказано, не работает!");
#endif
        }
    }
}
