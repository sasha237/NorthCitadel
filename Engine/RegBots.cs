using System;
using NerZul.Core.Utils;
using Core.Source.Network;
using NerZul.Core.Network;

namespace Engine
{
    public static class NameType
    {
        public const string Manual = "manual";
        public const string Dictionary = "dictionary";
        public const string Web = "web";
    }

    public static class RegType
    {
        public const string GMail = "gmail";
        public const string Domain = "domain";
        public const string MailList = "maillist";
    }

    public enum RegResult
    {
        Success,
        CaptchaFail,
        NameExist,
        EmailExist,
        UnknownFail
    }

    public class RegBots
    {
        static object lockable = new object();
        static object lockable2 = new object();
        static string regType;
        static int count;
        static int Needed = 0;
        static string mail_parameter = "";
        static int country = 0;
        static int region = 0;
        static string nameType = "dictionary";
        static int delay = 0;
        public static string m_sGroup;

        static bool activate;
        static string popServer = "";
        static string mailLogin = "";
        static string mailPassword = "";

        static void WriteToDB(string NickName, string Password, string Email)
        {
//#if PUBLIC_BUILD
            if (!Globals.IsValid())
                return;
//#endif

            lock (lockable2)
            {
                Globals.Database.Reset();
                try
                {
                    Globals.Database.Insert(
                        "bots", "login", NickName, "password", Password, "group", m_sGroup, "email", Email);
                }
                catch (Exception e)
                {
                    ConsoleLog.WriteLine("Unable to insert " + NickName + ": " + e.Message);
                }
            }
            lock (lockable)
            {
                count++;
                if (count >= Needed) System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        public static void Worker(string[] args)
        {
//#if PUBLIC_BUILD
            if (!Globals.IsValid())
                return;
//#endif
            activate = (args[0] == "regactbots");

            if (args.Length < 2)
            {
                ConsoleLog.WriteLine("regbots/regactbots reg_type(gmail|domain|maillist) ...");
                return;
            }

            regType = args[1];
            string errorHelp = "";

            switch (regType)
            {
                case RegType.GMail:
                    errorHelp = "base_mailbox(aaa@gmail.com)";
                    break;
                case RegType.Domain:
                    errorHelp = "mailbox_suffix(@mydomain.com)";
                    break;
                case RegType.MailList:
                    errorHelp = "mailboxes_file(maillist.txt)";
                    break;
                default:
                    if (activate)
                    {
                        ConsoleLog.WriteLine("For MailList registration use 'regbots'");
                        return;
                    }
                    ConsoleLog.WriteLine("Unknown registration type.");
                    return;
            }

            if (!activate && args.Length != 10)
            {
                ConsoleLog.WriteLine("regbots reg_type(gmail|domain|maillist) " + errorHelp + " count_bots count_threads country_id region_id group_name nicknames(manual|dictionary|web) delay");
                return;
            }
            if (activate && args.Length != 13)
            {
                ConsoleLog.WriteLine("regactbots reg_type(gmail|domain) " + errorHelp + " count_bots count_threads country_id region_id group_name nicknames(manual|dictionary|web) delay pop_server mail_login mail_password");
                return;
            }

            mail_parameter = args[2];
            Needed = int.Parse(args[3]);
            int threadCount = int.Parse(args[4]);
            country = int.Parse(args[5]);
            region = int.Parse(args[6]);
            m_sGroup = args[7];
            nameType = args[8];
            delay = int.Parse(args[9]);

            if (activate)
            {
                Globals.threadCount = 1;

                popServer = args[10];
                mailLogin = args[11];
                mailPassword = args[12];
            }

            if (!Globals.webCitadel.SendLogInfo(args, Needed))
                return;

            if (Globals.BotConfig.useTOR)
                threadCount = 1;

            ConsoleLog.WriteLine("Starting registration threads...");
            for (int c = 0; c < threadCount; c++)
            {
                System.Threading.Thread thread = new System.Threading.Thread(RegisterProc);
                thread.Start();
                System.Threading.Thread.Sleep(5000);
            };
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                //ConsoleLog.WriteLine("Type 'quit' to exit");
                //if(Console.ReadLine()=="quit")
                //System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        public static void RegisterProc()
        {
//#if PUBLIC_BUILD
            if (!Globals.IsValid())
                return;
//#endif

            string Response = "";

            NerZul.Core.Utils.NickNameAndPasswordGenerator generator = null;

            if (nameType == NameType.Manual)
                generator = new NerZul.Core.Utils.NickNameAndPasswordGenerator("");
            if (nameType == NameType.Dictionary)
                generator = new NerZul.Core.Utils.NickNameAndPasswordGenerator("dic.txt");
            if (nameType == NameType.Web)
                generator = new NerZul.Core.Utils.NickNameAndPasswordGenerator(Globals.webNickURL, 1);

            while (true)
            {
                string NickName = "";
                if (nameType == NameType.Manual)
                {
                    lock (lockable)
                    {
                        Console.WriteLine("Enter the name for a new bot: ");
                        NickName = Console.ReadLine();
                    }
                }
                else NickName = generator.GenerateNick();
                string Password = generator.GeneratePassword();
                string Mail = "";
                if (regType == RegType.GMail)
                    Mail = generator.GenerateGMail(mail_parameter);
                if (regType == RegType.Domain)
                    Mail = generator.GenerateDomain(NickName, mail_parameter);
                if (regType == RegType.MailList)
                    Mail = generator.GenerateMailList(mail_parameter);
                ConsoleLog.WriteLine("Mail: " + Mail);
                RegResult regResult = RegResult.UnknownFail;
                try
                {
                    regResult = RegisterBot(
                        NickName,
                        Password,
                        Mail,
                        192,
                        out Response);
                }
                catch (Exception e)
                {
                    ConsoleLog.WriteLine("Exception! " + e.Message);
                };
                if (regResult == RegResult.Success)
                {
                    ConsoleLog.WriteLine(count.ToString() + ": " + NickName + " ready");

                    if (regType == RegType.MailList)
                    {
                        generator.FixBoxReg(Mail);
                    }

                    if (activate)
                    {
                        try
                        {
                            ActivateMail.Worker(new string[] { "activatemail", popServer, mailLogin, mailPassword });
                        }
                        catch (Exception e)
                        {
                            ConsoleLog.WriteLine("Activation failed. Error: " + e.Message);
                        }
                    }

                    WriteToDB(NickName, Password, Mail);
                    if (delay > 0)
                        System.Threading.Thread.Sleep(delay * 1000);
                }
                else
                {
                    ConsoleLog.WriteLine(count.ToString() + ": " + NickName + " registration failed. " + regResult.ToString());
                    //ConsoleLog.WriteLine(count.ToString() + ": " + NickName + " registration failed", "RegisterLog.txt");
                    //ConsoleLog.WriteLine(Response, "RegisterLog.txt");

                    if ((regResult == RegResult.EmailExist) && (regType == RegType.MailList))
                    {
                        generator.FixBoxReg(Mail);
                    }
                }
            }
        }

        private static RegResult RegisterBot(
                                string NickName, string Password, int Nationality,
                                string EMail, int BirthDay, int BirthMonth, int BirthYear, bool Female,
                                out string Response)
        {
            string Referal = null;
            HttpClient client = new HttpClient();
            string TokenScanString = "id=\"_token\" name=\"_token\" value=\"";
            string Token;
            //Грузим главную

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
            if (!String.IsNullOrEmpty(Referal))
            {
                Response = client.DownloadString("http://www.erepublik.com/en/referrer/" + Referal);
                Response = client.DownloadString("http://www.erepublik.com/en/referrer/" + Referal);
            }
            else
            {
                Response = client.DownloadString("http://www.erepublik.com/");
                Response = client.DownloadString("http://www.erepublik.com/en/register");
            }
            //Выковыриваем токен
            Response = Response.Remove(0, Response.IndexOf(TokenScanString) + TokenScanString.Length);
            Token = Response.Substring(0, Response.IndexOf("\""));


            //Строим запрос
            //Понеслася
            ICaptchaProvider CaptchaProvider;

            if (Globals.BotConfig.precaptchaBufferSize == 0)
            {
                if (String.IsNullOrEmpty(Globals.BotConfig.AntiGateKey))
                {
                    CaptchaProvider = new WinFormsCaptchaProvider(Globals.BotConfig.bBeep);
                }
                else
                {
                    CaptchaProvider = new AntigateCaptchaProvider(Globals.BotConfig.AntiGateKey);
                }
            }
            else
            {
                CaptchaProvider = new PrecaptchaCaptchaProvider(Globals.BotConfig.AntiGateKey, Globals.BotConfig.precaptchaBufferSize, Globals.BotConfig.bBeep);
            }

            bool ok = false;
            string PostData;
            ResolvedCaptcha captcha = null;
            for (int attempt = 0; attempt < 3; attempt++)
            {
                captcha = CaptchaProvider.GetResolvedCaptcha();
                PostData = "_token=" + Token +
                    "&recaptcha_challenge_field=" + captcha.ChallengeID +
                    "&recaptcha_response_field=" + captcha.CaptchaText;

                Response = client.UploadString("http://www.erepublik.com/ajax_captcha", PostData);
                if (Response.Contains("success"))
                {
                    ok = true;
                    break;
                }
            }
            if (!ok)
            {
                return RegResult.CaptchaFail;
            }

            PostData = "_token=" + Token + "&citizen_name=" + System.Web.HttpUtility.UrlEncode(NickName) +
                "&citizen_password=" + System.Web.HttpUtility.UrlEncode(Password) +
                "&country_selected_id=" + country.ToString() +
                "&country_list=" + country.ToString() +
                "&region_list=" + region.ToString() + "&region_selected_id=" + region.ToString() +
                "&nationality_list=" + Nationality + "&citizen_email=" + EMail.Replace("@", "%40") +
                "&recaptcha_challenge_field=" + captcha.ChallengeID +
                "&recaptcha_response_field=" + captcha.CaptchaText;
            Response = client.UploadString("http://www.erepublik.com/en/register", PostData);
            if (Response.Contains("Email validation"))
                return RegResult.Success;
            if (Response.Contains("Citizen name already exists"))
                return RegResult.NameExist;
            if (Response.Contains("There is already an account created on this email address"))
                return RegResult.EmailExist;
            return RegResult.UnknownFail;
        }

        private static RegResult RegisterBot(
            string NickName,
            string Password,
            string EMail,
            int Nationality,
            out string Response)
        {
            Random m_Random = new Random();

            return RegisterBot(NickName, Password, Nationality, EMail, m_Random.Next(1, 29),
                        m_Random.Next(1, 13), m_Random.Next(1970, 1995), (m_Random.Next(0, 2) > 0), 
                        out Response);
        }
    }

}
