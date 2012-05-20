using System;
using System.Collections.Generic;
using NerZul.Core.Utils;
namespace Engine
{
    public static class Comment
    {
        static string group;
        static int iId = 0;
        static string sMess;

        private static void PrintUsage()
        {
            ConsoleLog.WriteLine("Usage: comment group article_id message");
        }
        public static void Worker(string[] args)
        {
#if !PUBLIC_BUILD
            if (args.Length != 4)
            {
                PrintUsage();
                return;
            }
            group = args[1];
            int.TryParse(args[2], out iId);

            sMess = args[3];

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
                NerZul.Core.Utils.Bicycles.ThreadPool.ExecInPool(BotProc2, bots, poolsize, true, Globals.ShowDlg);
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Comment error: " + e.Message);
            }
        }

        static void BotProc2(object botnfo)
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
                    Bot.Bot.Comment(iId, sMess);
                }
                catch (System.Exception e)
                {
                    ConsoleLog.WriteLine("Error: " + e.Message);
                }
            }
            else
            {
                if (Bot.Bot.GetLastResponse().Contains("infringement"))
                {
                    botinfo["banned"] = 1;
                    Utils.UpdateDbWithCustomBotInfo(botinfo, "banned");
                    //botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);
                    ConsoleLog.WriteLine(botinfo["login"].ToString() + ": Banned");
                }
                else
                    ConsoleLog.WriteLine("Unable to login - " + botinfo["login"]);
            }
#else
            ConsoleLog.WriteLine("Думаешь самый умный? Сказано, не работает!");
#endif
        }
    }
}
