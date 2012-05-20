
using System;
using NerZul.Core.Utils;

namespace Engine
{


	public class PartyJoin
	{
		class BotHolder
		{
			System.Collections.Generic.List<string> BotList;
			public BotHolder(string Path)
			{
				string[] BL=System.IO.File.ReadAllLines(Path);
				BotList=new System.Collections.Generic.List<string>(BL);
			}
			public string GetNextBot()
			{
				
				string rv;
				lock(BotList)
				{
					if(BotList.Count==0) return null;
					rv=BotList[0];
					BotList.RemoveAt(0);
				}
				return rv;
			}
		}
		//private static int threadcount=0; 
		private static object lockable=new System.Exception();
		private static void join_thread(object xarg)
		{
			join_thread_arg arg=(join_thread_arg) xarg;
			string bot=arg.BotList.GetNextBot();
			while(bot!=null)
			{
				string[] pair=bot.Split('|');
				bool sOK=false;
				while (!sOK)
				{
					NerZul.Core.Network.Bot Bot=new NerZul.Core.Network.Bot(pair[0],pair[1],
                        "Opera/9.62 (Windows NT 6.1; U; ru) Presto/2.1.1", "",0);
					Bot.HttpClient.Proxy=arg.ProxyList.GetRandomString();
					Bot.HttpClient.Timeout=20000;
					ConsoleLog.WriteLine("Try \""+pair[0]+"\" with"+Bot.HttpClient.Proxy);
					try
					{
						if(Bot.Login())
						{
							//Bot.HttpClient.Proxy=null;
							if(arg.Leave)
							{
								ConsoleLog.WriteLine(pair[0]+" logged in");
								string PostData=Bot.GetTokenArg(1);
								Bot.CustomRequest("http://www.erepublik.com/en/resign-party/"+
								                  arg.Party,PostData);
							}else{
							Bot.CustomRequest("http://www.erepublik.com/en/join-party/"+arg.Party);
							}
							sOK=true;
							ConsoleLog.WriteLine(pair[0]+" OK");
						}
							
					}catch (Exception){};
				}
				bot=arg.BotList.GetNextBot();
				
			}
			
		}
		class join_thread_arg
		{
			public BotHolder BotList;
			public NerZul.Core.Utils.StringSelector ProxyList;
			public string Party;
			public bool Leave=false;
				
		}

		public static void join_party(string[] args)
		{
			join_thread_arg arg=new join_thread_arg();
			arg.BotList=new BotHolder (args[1]);
			arg.ProxyList=new NerZul.Core.Utils.StringSelector(args[2]);
			arg.Party=args[3];
			if(args[0].ToLower()=="leaveparty") arg.Leave=true;

			for( int i=0; i<20; i++)
				
			{
				System.Threading.Thread th=new System.Threading.Thread(join_thread);
				th.Start(arg);
				lock(lockable)
				{
				}
			}
			System.Threading.Thread.Sleep(5000000);
		}

	}
}
