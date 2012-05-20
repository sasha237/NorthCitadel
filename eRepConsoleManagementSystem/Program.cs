using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Engine;
using NerZul;
using Fuel;
using NerZul.Core.Utils;
using System.Runtime.CompilerServices;

namespace eRepConsoleManagementSystem
{
    [assembly: SuppressIldasmAttribute()]
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Globals.Init();
//#if PUBLIC_BUILD

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
//#endif
            TimeSpan now = Globals.GetErepTime();

            ConsoleLog.WriteLine("Ner-Zul Bot Management Engine v2.0");


            if (Environment.OSVersion.Platform != PlatformID.Unix)
            {
                ConsoleLog.WriteLine("Running on " + Environment.OSVersion.VersionString);
            }
            else
            {
                System.Diagnostics.Process uname = new System.Diagnostics.Process();
                uname.StartInfo.UseShellExecute = false;
                uname.StartInfo.FileName = "uname";
                uname.StartInfo.Arguments = "-a";
                uname.StartInfo.RedirectStandardOutput = true;
                uname.Start();
                ConsoleLog.WriteLine("Running on " + uname.StandardOutput.ReadToEnd());
            }
            ConsoleLog.WriteLine(string.Format("Started at {0:00}:{1:00}", now.Hours, now.Minutes) +
                              ", day " + now.Days.ToString() + " of the New World");

  			System.Threading.Thread.Sleep(2000);
            if (args.Length < 1)
            {
                Globals.ShowDlg = true;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            else
            {
                args[0] = args[0].ToLower();
                switch (args[0])
                {
                    case "regbots":
                    case "regactbots":
                        RegBots.Worker(args);
                        break;
                    case "activatemail":
                    case "activatelist":
                        ActivateMail.Worker(args);
                        break;
                    case "daily":
                        Engine.Daily.Worker(args);
                        break;
                    case "feed":
                        Engine.Feed.Worker(args);
                        break;
                    case "fight":
                        Fight.Worker(args);
                        break;
                    case "buymoneyfromoffer":
                        MoneyProcessor.WorkerBuyFromOffer(args);
                        break;
                    case "collectmoney":
                        MoneyProcessor.WorkerCollectMoney(args);
                        break;
                    case "collectmoneytocountry":
                        MoneyProcessor.WorkerCollectMoneyToCountry(args);
                        break;
                    case "donatemoney":
                        MoneyProcessor.WorkerDonateMoney(args);
                        break;
                    case "convertmoney":
                        MoneyProcessor.WorkerConvertMoney(args);
                        break;
                    case "fly":
                        Fly.Worker(args);
                        break;

                    case "votearticle":
                        Engine.VoteArticle.Worker(args);
                        break;
                    case "election":
                        Elections.Worker(args);
                        break;
                    case "subscribe":
                        Subscribe.Worker(args);
                        break;
                    case "report":
                        ReportAll.Worker(args);
                        break;

                    case "import":
                        Import.ImportBots(args);
                        break;

                    case "tutorial":
                        Engine.Tutorial.Worker(args);
                        break;
                    case "deploy":
                        Engine.Deploy.Worker(args);
                        break;
                    
                    default:
                        ConsoleLog.WriteLine("Unknown command: " + args[0]);
                        break;
                }
            }

            NerZul.Core.Network.PreCaptcha.StopWork();
        }
    }
}
