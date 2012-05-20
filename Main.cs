using System;

namespace Test
{
	class MainClass
	{
		static object lockable=new object();
		static int count;
		static void WriteLog(string NickName, string Password)
		{
			lock(lockable)
			{
				count++;
				Console.WriteLine(count.ToString()+":"+NickName+" ready");
				System.IO.File.AppendAllText("bots.txt", NickName+"|"+Password+"\n");
			}
		}
		static void RegisterProc()
		{
			while(true)
			{
				NerZul.Core.Utils.NickNameAndPasswordGenerator generator=
					new NerZul.Core.Utils.NickNameAndPasswordGenerator("dic.txt");
				NerZul.Network.BotRegister reg=new NerZul.Network.BotRegister();
			
				string NickName=generator.GenerateNick();
				string Password=generator.GeneratePassword();
				bool sOK=false;
				try
				{
					sOK=reg.RegisterBot(NickName,Password,generator.GenerateEmail(),39,242,192);
				}catch(Exception e)
				{};
				if (sOK)
				{
					WriteLog(NickName,Password);
				};
			}
		}
		public static void Main (string[] args)
		{
			Console.WriteLine("Starting registration threads...");
			for (int c=0; c<20;c++)
			{
				System.Threading.Thread thread=new System.Threading.Thread(RegisterProc);
				thread.Start();
				System.Threading.Thread.Sleep(2000);
			};
			while(true)
			{
				Console.WriteLine("Type 'quit' to exit");
				if(Console.ReadLine()=="quit")
					System.Diagnostics.Process.GetCurrentProcess().Kill();
			}
		}
	}
}
