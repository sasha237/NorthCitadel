using System;
using System.Text;
using NerZul.Core.Utils;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using System.Security;
using NerZul.Core.Network;
using System.Security;
using System.Management;

namespace Engine
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
        public static bool TryAnotherWork;
        public static bool ShowDlg;
		public static string DataDir;
		public static Database Database;
        public static object DBLocker = new object();
		public static int ErepTZ;
		public static DateTime ErepAgeStart;
		public static TimeSpan GetErepTime()
		{
			DateTime time=DateTime.UtcNow;
			time=time.AddHours(ErepTZ);
			TimeSpan tsx=time-ErepAgeStart;
			return tsx;
		}
        public static int timeoutsLimit;
        public static int threadCount;
        public static string webNickURL;

        public static int totalBotCounter = 0;
        public static int processedBotCounter = 0;

        public static string addWhere = "";

        public static int defaultFoodQ = 1;

		public static ManagedBotConfig BotConfig = new ManagedBotConfig();

        public static WebCitadel webCitadel;
		
		public static NerZul.Core.Utils.StringSelector Avatars;
        

		public static void Init()
		{
			DataDir = System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
			DataDir = System.IO.Path.GetDirectoryName(DataDir);
//#if PUBLIC_BUILD
            AccessFile = new NerZul.Core.Utils.INIFile(System.IO.Path.Combine(DataDir, "access.ini"));
//#endif
            Config = new NerZul.Core.Utils.INIFile(System.IO.Path.Combine(DataDir, "config.ini"));
			DataDir=System.IO.Path.Combine(DataDir, "data");
			NerZul.Core.Network.HttpClient.ResponseTimeout = Config.GetValue("misc", "responsetimeout", 10) * 1000;
            timeoutsLimit = Config.GetValue("misc", "timeoutslimit", 9999);
            ShowDlg = false;
            threadCount = Config.GetValue("misc", "threadCount", 10);
            webNickURL = Config.GetValue("misc", "webnickurl", "");
            
            //Init database
			Database=new Database();
			Database.ConnectToDb();
			//Init time
			ErepTZ=Config.GetValue("time","timezone",-7);
			ErepAgeStart=new DateTime(Config.GetValue("time","zeroyear",2007),
			                          Config.GetValue("time","zeromounth",11),
			                          Config.GetValue("time","zeroday",20));
			//Init bots configuration 
			BotConfig.UserAgentList=new NerZul.Core.Utils.StringSelector(
				System.IO.Path.Combine(DataDir, "useragents.txt"));
			BotConfig.DisableProxyAfterLogin = false;
            BotConfig.AntiGateKey = Config.GetValue("misc", "autocaptcha", "");
            BotConfig.precaptchaBufferSize = Config.GetValue("misc", "precaptchaBufferSize", 0);
            BotConfig.useTOR = Config.GetValue("misc", "TOR", false);
            BotConfig.proxyAuthorisation = Config.GetValue("misc", "proxyAuthorisation", false);
            BotConfig.proxyLogin = Config.GetValue("misc", "proxyLogin", "");
            BotConfig.proxyPassword = Config.GetValue("misc", "proxyPassword", "");
            BotConfig.bBeep = Config.GetValue("misc", "beep", false);

            defaultFoodQ = Config.GetValue("gameplay", "defaultFoodQ", 1);
            TryAnotherWork = Config.GetValue("gameplay", "TryAnotherWork", false);

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

//#if PUBLIC_BUILD

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

//#endif
            //Load avatars list
			Avatars=new NerZul.Core.Utils.StringSelector(
				System.IO.Directory.GetFiles(System.IO.Path.Combine(DataDir,"avatars")));

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
            return true;
//#if PUBLIC_BUILD
            return FirstRealKey == FirstKey && MD5Hash(Convert.ToBase64String(StrToByteArray(FirstKey))) == SecondKey;
//#endif
            return true;
        }

	}

    public static class MainClass
	{
		public static void Main (string[] args)
		{
//#if !PUBLIC_BUILD			
			Globals.Init(); 
			TimeSpan now=Globals.GetErepTime();  

			ConsoleLog.WriteLine("Ner-Zul Bot Management Engine v2.");
			
			if(Environment.OSVersion.Platform!=PlatformID.Unix)
			{
				ConsoleLog.WriteLine("Running on "+Environment.OSVersion.VersionString);	
			}
            else
			{
				System.Diagnostics.Process uname=new System.Diagnostics.Process();
				uname.StartInfo.UseShellExecute=false;
				uname.StartInfo.FileName="uname";
				uname.StartInfo.Arguments="-a";
				uname.StartInfo.RedirectStandardOutput=true;
				uname.Start();
				ConsoleLog.WriteLine("Running on "+uname.StandardOutput.ReadToEnd());
			}	   
			ConsoleLog.WriteLine(string.Format("Started at {0:00}:{1:00}",now.Hours,now.Minutes)+
			                  ", day " + now.Days.ToString() + " of the New World");
			
			switch (args[0])
			{
			case "import":
				Import.ImportBots(args);
				break;
			case "daily":
				Daily.Worker(args);
				break;
			case "fire_all":
				FireAll.Worker(args);
				break;
			case "election":
				Elections.Worker(args);
				break;
            case "votearticle":
                VoteArticle.Worker(args);
                break;
            case "fight":
                Fight.Worker(args);
                break;
            case "comment":
                Comment.Worker(args);
                break;
            default:
				ConsoleLog.WriteLine("Unknown command: "+args[0]);
				break;
			}
//#endif
		}
	}
}
