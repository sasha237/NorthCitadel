using System;
using System.Collections.Generic;
using System.Text;
using NerZul.Core.Utils;

namespace Engine
{
    public class Tutorial
    {
		static string group;
		private static void PrintUsage()
		{
			ConsoleLog.WriteLine("Usage: group");
		}
        public static void Worker(string[] args)
        {
//#if PUBLIC_BUILD
            if (!Globals.IsValid())
                return;
//#endif

            if (args.Length != 1)
            {
                PrintUsage();
                return;
            }
            group = args[1];
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

            bots = DbRows.MixList(bots);
            NerZul.Core.Utils.Bicycles.ThreadPool.ExecInPool(BotProc, bots, poolsize, true, Globals.ShowDlg);
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

                //++что-то делаем

            }
            else
            {
                ConsoleLog.WriteLine("Unable to login - " + botinfo["login"]);
            }
        }
    }
}
