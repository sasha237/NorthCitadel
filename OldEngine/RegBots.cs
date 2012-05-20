using System;
using NerZul.Core.Utils;

namespace Engine
{
	class RegBots
	{
		static object lockable=new object();
		static object lockable2=new object();
		static int count;
		static int Needed=0; static string gmlogin="";
		static int country=0, region=0;
		static bool manual=false;
		static void WriteLog(string NickName, string Password)
		{
			lock(lockable2)
			{
				System.IO.File.AppendAllText("bots.txt", NickName+"|"+Password+";");
			}
			lock(lockable)
			{
				count++;
				ConsoleLog.WriteLine(count.ToString()+":"+NickName+" ready");
				if(count>=Needed) System.Diagnostics.Process.GetCurrentProcess().Kill();
			}
		}
		static void RegisterProc()
		{
			while(true)
			{
				NerZul.Core.Utils.NickNameAndPasswordGenerator generator=
					new NerZul.Core.Utils.NickNameAndPasswordGenerator("dic.txt");
				NerZul.Core.Network.BotRegister reg=new NerZul.Core.Network.BotRegister();
				//reg.Referal="Krez2010";		
				
				string NickName="";
				if(manual)
				{
					lock(lockable)
					{
					ConsoleLog.WriteLine("Enter the name for a new bot: ");
					NickName=Console.ReadLine();
					}
				}
				else NickName=generator.GenerateNick();
				string Password=generator.GeneratePassword();
				bool sOK=false;
				try
				{
                    string Response;
					sOK=reg.RegisterBot(NickName,Password,
                        generator.GenerateGMail(gmlogin), country, region, 192, "", 0, out Response);
				}catch(Exception e)
				{	
					ConsoleLog.WriteLine("Exception! "+e.Message);
				};
				if (sOK)
				{
					WriteLog(NickName,Password);
				};
			}
		}
		public static void Worker (string[] args)
		{
			if((args.Length<6)||(args.Length>7)) 
			{
				ConsoleLog.WriteLine("regbots base_gmail_login count threads country region [manual]");
				return;
			}
			gmlogin=args[1];
			Needed=int.Parse(args[2]);
			country=int.Parse(args[4]);
			region=int.Parse(args[5]);
			manual=((args.Length==7)&&(args[6]=="manual"));
			
			ConsoleLog.WriteLine("Starting registration threads...");
			for (int c=0; c<int.Parse(args[3]);c++)
			{
				System.Threading.Thread thread=new System.Threading.Thread(RegisterProc);
				thread.Start();
				System.Threading.Thread.Sleep(5000);
			};
			while(true)
			{
				System.Threading.Thread.Sleep(1000);
				//ConsoleLog.WriteLine("Type 'quit' to exit");
				//if(Console.ReadLine()=="quit")
					//System.Diagnostics.Process.GetCurrentProcess().Kill();
			}
		}
	}
	
}
