
using System;
using NerZul.Core.Utils;

namespace Engine
{


	public static class Daily
	{
		public static void Worker(string[] args)
		{
			if(args.Length!=2)
			{
				Console.Write("daily botslist.txt offerid");
				return;
			}

			NerZul.Core.Utils.ManagedBotConfig config=new NerZul.Core.Utils.ManagedBotConfig();
			config.ProxyList=new NerZul.Core.Utils.StringSelector("data/proxy.txt");
			config.UserAgentList=new NerZul.Core.Utils.StringSelector("data/ua.txt");
			config.DisableProxyAfterLogin=true;

            string[] bots = System.IO.File.ReadAllLines(args[1]);
            foreach (string bot in bots)
            {
                if (bot.Contains("|"))
                {

                    string[] pair = bot.Split('|');
                    ConsoleLog.WriteLine(pair[0] + "|" + pair[1]);
                    NerZul.Core.Network.Bot nbot = new NerZul.Core.Network.Bot(pair[0], pair[1],
                        "Opera/9.62 (Windows NT 6.1; U; ru) Presto/2.1.1", "",0);
                    bool ok = false;
                    while (!ok)
                    {
                        try
                        {

                            if (nbot.Login())
                            {
                                ConsoleLog.WriteLine("Logged in");
                                nbot.Work(false);
                                nbot.Train();
                            }
                        }
                        catch (Exception e) 
                        {
                            ConsoleLog.WriteLine("Daily error: " + e.Message);
                        }
                        if (!ok) System.Threading.Thread.Sleep(1000 * int.Parse(args[3]));

                    }
                    System.Threading.Thread.Sleep(1000 * int.Parse(args[3]));
                }
            }
			
		}
	}
}
