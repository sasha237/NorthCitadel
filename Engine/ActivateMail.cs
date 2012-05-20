
using System;
using NerZul.Core.Utils;
using Core.Source.Network;
using Pop3;
using System.Collections.Generic;

namespace Engine
{
	public static class ActivateMail
	{
        public static int messNumber, messCount;
        public static Pop3Client mailclient;
        public static object locker = new object();
        public static List<Pop3Message> messagesToDelete;

		public static void Worker(string[] args)
		{
//#if PUBLIC_BUILD
            if (!Globals.IsValid())
                return;
//#endif

            if (args[0] == "activatemail" && args.Length != 4)
            {
                ConsoleLog.WriteLine("activatemail pop_server login password");
                return;
            }
            if (args[0] == "activatelist" && args.Length != 2)
            {
                ConsoleLog.WriteLine("activatelist mailboxes_file");
                return;
            }

            NickNameAndPasswordGenerator gen = new NickNameAndPasswordGenerator("", 0);
            List<string> mailBoxes = null;
            if (args[0] == "activatemail")
            {
                mailBoxes = new List<string>();
                mailBoxes.Add(args[1] + ';' + args[2] + ';' + args[3] + ";xxx");
            }
            if (args[0] == "activatelist")
            {
                mailBoxes = gen.GetMailListBoxs(args[1]);
            }

            foreach (var mailbox in mailBoxes)
            {
                mailclient = new Pop3.Pop3Client();
                messagesToDelete = new List<Pop3Message>();
                messNumber = 0;
                messCount = 0;
                ConsoleLog.WriteLine("Connecting to " + mailbox.Split(';')[0]);
                mailclient.Connect(mailbox.Split(';')[0], mailbox.Split(';')[1], mailbox.Split(';')[2]);
                ConsoleLog.WriteLine("Logged in. Retrieving message list...");
                List<Pop3Message> messages = mailclient.List();
                mailclient.Retrieve(messages);

                int poolsize = Globals.threadCount;
                if ((args[0] == "activatelist") || (Globals.BotConfig.useTOR))
                    poolsize = 1;

                try
                {
                    messCount = messages.Count;
                    NerZul.Core.Utils.Bicycles.ThreadPool.ExecInPool(ActivateProc, messages, poolsize, true, Globals.ShowDlg);

                    if (args[0] == "activatelist")
                    {
                        gen.FixBoxAct(mailbox.Split(';')[3]);
                    }
                }
                catch (System.Exception e)
                {
                    ConsoleLog.WriteLine("Activate mail error: " + e.Message);
                }

                ConsoleLog.WriteLine("Deleteing processed messages...");
                mailclient.Delete(messagesToDelete);
                ConsoleLog.WriteLine("Disconnecting...");
                mailclient.Disconnect();
                ConsoleLog.WriteLine("Finished");

                messagesToDelete.Clear();
            }
        }

        public static void ActivateProc(object messageObj)
        {
            const string regURL = "http://www.erepublik.com/en/register-validate";
            const string weaponURL = "http://www.erepublik.com/en/main/claimReward";

            Pop3Message message = (Pop3Message)messageObj;

            messNumber++;
            ConsoleLog.WriteLine("Processing message " + messNumber.ToString() + "/" + messCount);

            if (message.Body.Contains(regURL))
            {
                string sBuf = message.Body;
                sBuf = sBuf.Replace("\n", "");
                sBuf = sBuf.Replace("\r", "");
                sBuf = sBuf.Replace("=", "");
                string link = sBuf.Substring(sBuf.IndexOf(regURL));
                link = link.Remove(link.IndexOfAny(new char[] { '\r', '\n', '\"', '<', '>', '*' }));
                link = link.Replace("=", "");
                ConsoleLog.WriteLine("Activate: " + link);

                for (int i = 1; i <= 10; i++)
                {
                    if (OpenLink(link))
                    {
                        lock (locker)
                        {
                            messagesToDelete.Add(message);
                        }

                        System.Threading.Thread.Sleep(1000);

                        break;
                    }
                }

                return;
            }

            if (message.Body.Contains(weaponURL))
            {
                string link = weaponURL + CommonUtils.GetStringBetween(message.Body, weaponURL, "\"");
                ConsoleLog.WriteLine("Weapon claim: " + link);

                for (int i = 1; i <= 10; i++)
                {
                    if (OpenLink(link))
                    {
                        lock (locker)
                        {
                            messagesToDelete.Add(message);
                        }

                        System.Threading.Thread.Sleep(1000);

                        break;
                    }
                }

                return;
            }

            if (message.Body.Contains("erepublik"))
            {
                ConsoleLog.WriteLine("Unknown erepublik message!");
            }
            else
            {
                ConsoleLog.WriteLine("Unrelated message.");
                lock (locker)
                {
                    messagesToDelete.Add(message);
                }
            }
        }

		public static bool OpenLink(string link) 
        {
			NerZul.Core.Network.HttpClient http = new NerZul.Core.Network.HttpClient();
			if (!Globals.BotConfig.useTOR) 
            {
				http.SetProxy(Globals.BotConfig.ProxyList.GetRandomString(), Globals.BotConfig.proxyCredentials);
			} 
            else 
            {
                http.SetProxy("127.0.0.1:8118", null);//Globals.BotConfig.ProxyList.GetString(0);

				if (!(new TorClient(http.GetProxy(), "")).GetNewIP())
					throw new Exception("Error getting new TOR IP");

				ConsoleLog.WriteLine("TOR new IP obtained!");
			}

            try
            {
                link = http.DownloadString(link);

                if (link.Contains("Congratulations"))
                {
                    ConsoleLog.WriteLine("Activated");
                    return true;
                }
                if (link.Contains("has either expired"))
                {
                    ConsoleLog.WriteLine("Expired");
                    return true;
                }
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Error: " + e.Message);
            }

            return false;
		}
	}
}
