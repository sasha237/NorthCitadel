using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NerZul;
using NerZul.Core.Utils;
using System.Runtime.CompilerServices;
using System.Security;
using System.Management;
using System.Security.Cryptography;
using NerZul.Core.Network;

namespace eRepCompanyChecker
{
    [assembly: SuppressIldasmAttribute()]
    public static class Globals
    {
        public static NerZul.Core.Utils.INIFile Config;
        //#if PUBLIC_BUILD
        public static NerZul.Core.Utils.INIFile AccessFile;
        public static string FirstKey;
        public static string SecondKey;
        public static string FirstRealKey;
        //#endif
        public static string DataDir;
        public static int ErepTZ;
        public static DateTime ErepAgeStart;
        public static TimeSpan GetErepTime()
        {
            DateTime time = DateTime.UtcNow;
            time = time.AddHours(ErepTZ);
            TimeSpan tsx = time - ErepAgeStart;
            return tsx;
        }
        public static int timeoutsLimit;

        public static ManagedBotConfig BotConfig = new ManagedBotConfig();

        public static WebCitadel webCitadel;

        public static void Init()
        {
            DataDir = System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
            DataDir = System.IO.Path.GetDirectoryName(DataDir);
            //#if PUBLIC_BUILD
            AccessFile = new NerZul.Core.Utils.INIFile(System.IO.Path.Combine(DataDir, "access.ini"));
            //#endif
            Config = new NerZul.Core.Utils.INIFile(System.IO.Path.Combine(DataDir, "config.ini"));
            DataDir = System.IO.Path.Combine(DataDir, "data");
            NerZul.Core.Network.HttpClient.ResponseTimeout = Config.GetValue("misc", "responsetimeout", 10) * 1000;

            //Init time
            ErepTZ = Config.GetValue("time", "timezone", -7);
            ErepAgeStart = new DateTime(Config.GetValue("time", "zeroyear", 2007),
                                      Config.GetValue("time", "zeromounth", 11),
                                      Config.GetValue("time", "zeroday", 20));
            //Init bots configuration 
            BotConfig.UserAgentList = new NerZul.Core.Utils.StringSelector(
                System.IO.Path.Combine(DataDir, "useragents.txt"));

            BotConfig.AntiGateKey = Config.GetValue("misc", "autocaptcha", "");
            BotConfig.precaptchaBufferSize = Config.GetValue("misc", "precaptchaBufferSize", 0);
            BotConfig.useTOR = Config.GetValue("misc", "TOR", false);
            BotConfig.proxyAuthorisation = Config.GetValue("misc", "proxyAuthorisation", false);
            BotConfig.proxyLogin = Config.GetValue("misc", "proxyLogin", "");
            BotConfig.proxyPassword = Config.GetValue("misc", "proxyPassword", "");
            BotConfig.bBeep = Config.GetValue("misc", "beep", false);

            string proxyURL = Config.GetValue("misc", "proxyURL", "");
            if (String.IsNullOrEmpty(proxyURL))
            {
                BotConfig.ProxyList = new NerZul.Core.Utils.StringSelector(
                    System.IO.Path.Combine(DataDir, "proxy.txt"));
            }
            else
            {
                ConsoleLog.WriteLine("Loading proxy list from " + proxyURL);
                BotConfig.ProxyList = Core.Source.Network.ProxyLoader.LoadFromURL(proxyURL, DataDir);
                ConsoleLog.WriteLine(BotConfig.ProxyList.Count.ToString() + " proxies loaded");
            }

            FirstKey = AccessFile.GetValue("vals", "FirstKey", "");
            SecondKey = AccessFile.GetValue("vals", "SecondKey", ""); ;

            StringBuilder sb1 = new StringBuilder();
            ManagementObjectSearcher searcher = null;
            searcher = new ManagementObjectSearcher("Select * from Win32_IRQResource");
            foreach (ManagementObject item in searcher.Get())
            {
                StringBuilder sb = new StringBuilder("");
                sb.Append(item.GetPropertyValue("Availability").ToString());
                sb.Append(item.GetPropertyValue("Hardware").ToString());
                sb.Append(item.GetPropertyValue("IRQNumber").ToString());
                sb.Append(item.GetPropertyValue("Name").ToString());
                sb.Append(item.GetPropertyValue("TriggerLevel").ToString());
                sb.Append(item.GetPropertyValue("TriggerType").ToString());
                string sBuf = MD5Hash(Convert.ToBase64String(StrToByteArray(sb.ToString())));
                sb1.Append(sBuf);
            }
            FirstRealKey = sb1.ToString().Substring(0, 200);

            webCitadel = new WebCitadel();
            webCitadel.Init(SecondKey);
        }

        public static string MD5Hash(string instr)
        {
            //#if PUBLIC_BUILD
            string strHash = string.Empty;

            foreach (byte b in new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(instr)))
            {
                strHash += b.ToString("X2");
            }
            return strHash;
            //#endif
            return "";
        }
        public static byte[] StrToByteArray(string str)
        {
            //#if PUBLIC_BUILD
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
            //#endif
            return null;
        }
        public static bool IsValid()
        {
            //#if PUBLIC_BUILD
            return FirstRealKey == FirstKey && MD5Hash(Convert.ToBase64String(StrToByteArray(FirstKey))) == SecondKey;
            //#endif
            return true;
        }
    }

    [assembly: SuppressIldasmAttribute()]
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Globals.Init();

            if (Globals.IsValid())
            {
                ConsoleLog.WriteLine("Ner-Zul Keys Is Ok");
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new AutoForm());
                return;
            }
            
            if (args.Length < 1)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new CheckerForm());
            }
            else
            {
                if(!Globals.webCitadel.SendLogInfo(args, 0))
                {
                    return;
                }
                args[0] = args[0].ToLower();
                switch (args[0])
                {
                    case "companycheck":
                        CompanyChecker.CompanyCheck(args);
                        break;
                    case "deletealerts":
                        CompanyChecker.DeleteAlerts(args);
                        break;

                    case "sellgoods":
                        Trader.SellGoods(args);
                        break;
                    case "buygoods":
                        Trader.BuyGoods(args);
                        break;

                    case "armysupply":
                        Donater.GDocSupply(args);
                        break;

                    case "badi":
                        Badi.Worker(args);
                        break;

                    default:
                        ConsoleLog.WriteLine("Unknown command: " + args[0]);
                        break;
                }
            }
        }
    }
}
