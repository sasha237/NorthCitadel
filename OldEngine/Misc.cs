
using System;
using NerZul.Core.Utils;

namespace Engine
{


	public static class Misc
	{

		public static void vote_subscribe(string[] args)
		{
			
			if((args.Length!=4) ||
			   ((args[0].ToLower() != "vote") && (args[0].ToLower() != "subscribe")
			    && (args[0].ToLower() != "unsubscribe")))
			{
				ConsoleLog.WriteLine("vote/subscribe list.txt articleid interval");
			}
			string[] bots=System.IO.File.ReadAllLines(args[1]);
			foreach (string bot in bots)
			{
				if (bot.Contains("|"))
				{
					
					string[] pair=bot.Split('|');
					ConsoleLog.WriteLine(pair[0]+"|"+pair[1]);
					NerZul.Core.Network.Bot nbot=new NerZul.Core.Network.Bot(pair[0],pair[1],
                        "Opera/9.62 (Windows NT 6.1; U; ru) Presto/2.1.1","",0);
					bool ok=false;
					while(!ok)
					{
						try
						{
					
							if(nbot.Login())
							{
								ConsoleLog.WriteLine("Logged in");
								if(args[0].ToLower()=="vote")
									nbot.VoteArticle(int.Parse(args[2]));
								else if(args[0].ToLower()=="subscribe")
									nbot.SubscribeNewspaper(int.Parse(args[2]));
								else
									nbot.UnsubscribeNewspaper(int.Parse(args[2]));
								ok=true;
							}
						}catch (Exception e) 
                        {
                            ConsoleLog.WriteLine("Vote error: " + e.Message);
                        }
						if(!ok) System.Threading.Thread.Sleep(1000*int.Parse(args[3]));
				
					}
					System.Threading.Thread.Sleep(1000*int.Parse(args[3]));
				}
			}
		}
		
		public static void filter_min_exp(string [] args)
		{
			if(args.Length!=4)
			{
				ConsoleLog.WriteLine("checkbanned list out_list minexp");
				return;
			}
			string[] Bots=System.IO.File.ReadAllLines(args[1]);
			System.IO.TextWriter outs=new System.IO.StreamWriter(args[2],true);
			//System.Net.WebClient client=new System.Net.WebClient();
			NerZul.Core.Network.HttpClient client=new NerZul.Core.Network.HttpClient();
			int cnt=1;
			foreach (string Bot in Bots)
			{
				ConsoleLog.WriteLine("["+cnt.ToString()+"/"+Bots.Length.ToString()+"] "+
				                  "Checking "+Bot.Split('|')[0]); cnt++;
				string botinfo=Bot.Split('|')[0].Replace(" ","%20");
				botinfo="http://api.erepublik.com/v1/feeds/citizens/"+botinfo+"?by_username=true";
				botinfo=client.DownloadString(botinfo);
				botinfo=System.Text.RegularExpressions.Regex.
					Match(botinfo,@"\<experience-points\>(\w+)\<").Groups[1].Value;
				if (int.Parse(botinfo)>=int.Parse(args[3]))
				{
					outs.WriteLine(Bot);
					outs.Flush();
					ConsoleLog.WriteLine("OK");
				}
				System.Threading.Thread.Sleep(3000);
			};
			
		}

	}
}
