using System;
using System.Collections.Generic;
using NerZul.Core.Utils;
using NerZul.Core.Network;
using Core.Source.Network;

namespace eRepCompanyChecker
{
    public static class Badi
    {
        private const string baseURL = "http://www.livemaster.ru";
        private const string svetikVoteURL = baseURL + "/item/571885-ukrasheniya-komplekt-yagodnyj";
        private const string konkursURL = baseURL + "/bestsp.php";

        private static HttpClient client;
        private static Random rnd = new Random();

        private static int delayMin = 0;
        private static int delayMax = 0;

        private static void PrintUsage()
        {
            ConsoleLog.WriteLine("Usage: badi delay_min delay_max");
        }

        private static void DoSleep(int sleepSeconds)
        {
            ConsoleLog.WriteLine("Waiting for " + sleepSeconds.ToString() + " seconds...");

            System.Threading.Thread.Sleep(sleepSeconds * 1000);
        }

        public static bool Visiting1(string startReferrer)
        {
            string response;
            bool result = false;

            try
            {
                ConsoleLog.WriteLine("Do visiting1: from " + startReferrer);

                result = Vote(svetikVoteURL, startReferrer, true);

                DoSleep(rnd.Next(5, 10));

                ConsoleLog.WriteLine("Visiting konkurs page");
                client.Headers.Remove("X-Requested-With");
                response = client.DownloadString(konkursURL);
                //ConsoleLog.WriteLine(response, "Response3.txt");

                DoSleep(rnd.Next(5, 20));

                for (int i = 1; i <= rnd.Next(5, 20); i++)
                {
                    string referrer = konkursURL + "?sort=0&sorder=&from=" + rnd.Next(2, 13) * 20;
                    ConsoleLog.WriteLine("Additional visit1 " + i.ToString() + ": " + referrer);
                    response = client.DownloadString(referrer);

                    response = CommonUtils.GetStringBetween(
                        response,
                        "<div class=\"clear sep-v-5\">",
                        "<div id=\"image");

                    response = baseURL + CommonUtils.GetStringBetween(
                        response,
                        "<a href=\"",
                        "\">");

                    DoSleep(rnd.Next(5, 20));

                    //Vote(response, referrer, (rnd.Next(1, 10) == 1));
                    Vote(response, referrer, false);

                    DoSleep(rnd.Next(5, 10));
                }
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Visiting1 error: " + e.Message);
            }
            return result;
        }

        public static string Visiting2()
        {
            string referrer = "";
            string URL = baseURL;
            string response;

            try
            {
                ConsoleLog.WriteLine("Do visiting2: from " + baseURL);

                client.Headers.Remove("X-Requested-With");
                client.Referer = referrer;
                response = client.DownloadString(URL);

                DoSleep(rnd.Next(5, 20));

                for (int i = 1; i <= rnd.Next(5, 20); i++)
                {
                    if (!response.Contains("<base href=\"http://www.livemaster.ru\">"))
                    {
                        ConsoleLog.WriteLine("Bad page loaded");
                    }

                    client.Referer = URL;

                    response = CommonUtils.GetStringBetween(
                        response,
                        "<body>",
                        "</body>");

                    URL = baseURL + "/" + CommonUtils.GetStringBetween(
                        response,
                        "href=\"/",
                        "\"",
                        0,
                        rnd.Next(30, 100));
                    ConsoleLog.WriteLine("Additional visit2 " + i.ToString() + ": " + URL);
                    response = client.DownloadString(URL);

                    DoSleep(rnd.Next(5, 20));
                }
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Visiting2 error: " + e.Message);
            }
            return URL;
        }

        public static bool Vote(string voteURL, string referrer, bool doVote)
        {
            ConsoleLog.WriteLine("Vote URL (doVote=" + doVote.ToString() + "): " + voteURL);

            string response;

            client.Referer = referrer;
            client.Headers.Remove("X-Requested-With");

            response = client.DownloadString(voteURL);
            //ConsoleLog.WriteLine(response, "Response1.txt");

            if (!doVote) 
                return true;

            if (!response.Contains("<div id=\"contestBlock\">"))
            {
                ConsoleLog.WriteLine("Bad page loaded");
                return false;
            }

            if (response.Contains("<div id=\"kaptchaBlock\">"))
            {
                ConsoleLog.WriteLine("Loaded");

                response = CommonUtils.GetStringBetween(
                    response,
                    "<div id=\"kaptchaBlock\">",
                    "<input type=\"image\"");

                string imageURL = baseURL + CommonUtils.GetStringBetween(
                        response,
                        "<img src=\"",
                        "\">");
                string challengeID = CommonUtils.GetStringBetween(
                    response,
                    "<input type=\"hidden\" id=\"itemvote\" value=\"",
                    "\">");

                response = "-1";

                while (response.Contains("-1"))
                {
                    ConsoleLog.WriteLine("Getting captcha...");

                    byte[] image = client.DownloadData(imageURL);
                    Captcha captcha = new Captcha(image);

                    ConsoleLog.WriteLine(
                        "imageURL=" + imageURL + ", " +
                        "challengeID=" + challengeID);

                    string challengeText = captcha.ResolveAntigate(Globals.BotConfig.AntiGateKey);

                    client.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    client.Referer = voteURL;
                    string PostData = "item=" + challengeID + "&votekey=" + challengeText;
                    response = client.UploadString(baseURL + "/contest/spvoting.php", PostData);

                    //ConsoleLog.WriteLine(response, "Response2.txt");
                }

                bool result = response.Contains("<a href=\"/bestsp.php\">");

                ConsoleLog.WriteLine("URL voted. Result=" + result);

                return result;
            }
            else
            {
                ConsoleLog.WriteLine("IP/URL allready voted");
                return false;
            }
        }

        public static void Worker(string[] args)
        {
            if (args.Length != 3)
            {
                PrintUsage();
                return;
            }

            int.TryParse(args[1], out delayMin);
            int.TryParse(args[2], out delayMax);

            Antigate.phrase = "0";

            try
            {
                int voteCounter = 0;

                while (true)
                {
                    try
                    {
                        voteCounter++;

                        ConsoleLog.WriteLine("Vote, try " + voteCounter.ToString());

                        client = new HttpClient();

                        if (!Globals.BotConfig.useTOR)
                        {
                            client.SetProxy(Globals.BotConfig.ProxyList.GetRandomString(), Globals.BotConfig.proxyCredentials);
                        }
                        else
                        {
                            client.SetProxy("127.0.0.1:8118", null);//m_Config.ProxyList.GetString(0);

                            if (!(new TorClient(client.GetProxy(), "")).GetNewIP())
                                throw new Exception("Error getting new TOR IP");

                            ConsoleLog.WriteLine("TOR new IP obtained!");
                        }

                        client.UserAgent = Globals.BotConfig.UserAgentList.GetRandomString();

                        ConsoleLog.WriteLine("Proxy: " + client.GetProxy());

                        string lastURL = "http://svetiksch.spb.ru";
                        if (rnd.Next(1, 3) == 1)
                        {
                            if (rnd.Next(1, 10) <= 5)
                                lastURL = "http://svetiksch.spb.ru";
                            else
                                lastURL = "http://svetiksch.spb.ru/news/2011-05-27-345";
                        }
                        else
                        {
                            lastURL = Visiting2();
                        }

                        if (lastURL == baseURL)
                        {
                            throw new Exception("Visiting2 failed");
                        }

                        if (!Visiting1(lastURL))
                        {
                            throw new Exception("Visiting1 failed");
                        }

                        ConsoleLog.WriteLine("Voting done.");
                        DoSleep(rnd.Next(delayMin, delayMax));
                        //ConsoleLog.WriteLine("Press any key...");
                        //Console.ReadKey();
                    }
                    catch (System.Exception e)
                    {
                        ConsoleLog.WriteLine("Local error: " + e.Message);
                    }
                }
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Worker error: " + e.Message);
            }
        }
    }
}


