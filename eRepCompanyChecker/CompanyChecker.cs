using System;
using System.Collections.Generic;
using NerZul.Core.Utils;
using NerZul.Core.Network;

namespace eRepCompanyChecker
{
    public static class CompanyChecker
    {
        private static int weekNum = 19;

        public static void CompanyCheck(string[] args)
        {
            if (args.Length != 7)
            {
                ConsoleLog.WriteLine("Usage: companycheck login password companyname/companynum botscount sleeplengthsec firelast");
                ConsoleLog.WriteLine("Example: companycheck ololo alala tratata/54321 9 30 false");
                return;
            }
            string sLogin = args[1];
            string sPassword = args[2];
            string sCompanyNum = args[3];
            string sBotsNum = args[4];
            string sSec = args[5];
            string sFireLast = args[6];
            int iBotsNum = 0;
            iBotsNum = int.Parse(sBotsNum);
            bool bFireLast = false;
            bFireLast = bool.Parse(sFireLast);

            CheckerBot bt = new CheckerBot(sLogin, sLogin, sPassword, "Mozilla//4.0 (compatible; MSIE 7.0; Windows NT 6.0)", "", 0);
            bt.HttpClient.SetProxy(null, null);
            bool loggedIn = false;
            int iTryToConnect = 0;
            System.Random rnd = new System.Random();

            while (true)
            {
                try
                {
                    if (!loggedIn)
                    {
                        iTryToConnect++;
                        if (iTryToConnect > 10)
                            break;

                        ConsoleLog.WriteLine("Trying to login (" + (iTryToConnect).ToString() + ")...");
                        if (bt.Login())
                        {
                            ConsoleLog.WriteLine("Logged in!");
                            iTryToConnect = 0;
                            loggedIn = true;
                        }
                        else
                        {
                            ConsoleLog.WriteLine("Login failed!");
                            System.Threading.Thread.Sleep(1000);
                            continue;
                        }
                    }

                    ConsoleLog.WriteLine("Getting workers list");
                    List<int> lst = bt.GetCompanyWorkers(sCompanyNum, weekNum);

                    if (lst == null)
                    {
                        loggedIn = false;
                        throw new Exception("Error: lst == null! Try to relogin");
                    }

                    ConsoleLog.WriteLine(lst.Count.ToString() + " workers onboard");

                    if ((iBotsNum == 0) && (lst.Count == 0))
                    {
                        ConsoleLog.WriteLine("All workers fired.");
                        break;
                    }

                    while (lst.Count > iBotsNum)
                    {
                        System.Threading.Thread.Sleep(1000);
                        int idx;
                        if (!bFireLast)
                            idx = rnd.Next(0, lst.Count - 1);
                        else
                            idx = lst.Count - 1;
                        ConsoleLog.WriteLine("Fire a worker " + (idx+1).ToString());
                        bt.FireWorker(sCompanyNum, lst[idx], weekNum);
                        lst.RemoveAt(idx);

                        if (bt.GetLastResponse().IndexOf("logout") == -1)
                        {
                            loggedIn = false;
                            throw new Exception("Logged out! Try to relogin");
                        }
                    }
                }
                catch (System.Exception e)
                {
                    ConsoleLog.WriteLine("Company check error: " + e.Message);
                    ConsoleLog.WriteLine(bt.GetLastResponse(), "Responses.txt");
                }

                ConsoleLog.WriteLine("Waiting for next check");
                System.Threading.Thread.Sleep(int.Parse(sSec)*1000);
            }
        }

        public static void DeleteAlerts(string[] args)
        {
            if (args.Length != 3)
            {
                ConsoleLog.WriteLine("Usage: deletealerts login password");
                ConsoleLog.WriteLine("Example: deletealerts ololo alala");
                return;
            }
            string sLogin = args[1];
            string sPassword = args[2];

            CheckerBot bt = new CheckerBot(sLogin, sLogin, sPassword, "Mozilla//4.0 (compatible; MSIE 7.0; Windows NT 6.0)", "", 0);
            bool loggedIn = false;
            int iTryToConnect = 0;
            System.Random rnd = new System.Random();

            while (!loggedIn)
            {
                try
                {
                    iTryToConnect++;
                    if (iTryToConnect > 1)
                        break;

                    ConsoleLog.WriteLine("Trying to login (" + (iTryToConnect).ToString() + ")...");
                    if (bt.Login())
                    {
                        ConsoleLog.WriteLine("Logged in!");
                        iTryToConnect = 0;
                        loggedIn = true;
                    }
                    else
                    {
                        ConsoleLog.WriteLine("Login failed!");
                        System.Threading.Thread.Sleep(1000);
                    }
                }
                catch (Exception e)
                {
                    ConsoleLog.WriteLine("Login for delete alerts error: " + e.Message);
                    ConsoleLog.WriteLine(bt.GetLastResponse(), "Responses.txt");
                }
            }

            if (loggedIn)
            {
                try
                {
                    ConsoleLog.WriteLine("Deleteing all alerts");
                    bt.DeleteAllAlerts();
                    ConsoleLog.WriteLine("All alerts deleted");
                }
                catch (Exception e)
                {
                    ConsoleLog.WriteLine("DeleteAllAlerts error: " + e.Message);
                    ConsoleLog.WriteLine(bt.GetLastResponse(), "Responses.txt");
                }
            }
        }
    }
}
