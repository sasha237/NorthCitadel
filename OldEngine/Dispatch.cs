
using System;
using NerZul.Core.Utils;

namespace Engine
{


	public static class Dispatch
	{

	
		public static void test()
		{
		}
		

		
		public static void Main (string[] args)
		{

			//System.Windows.Forms.MessageBox.Show("Какого хуя?");
			System.Threading.Thread.Sleep(2000);
			if(args.Length<1) return;
			args[0]=args[0].ToLower();
			switch (args[0])
			{
			case "activatemail":
				ActivateMail.Worker(args);
				break;
			case "regbots":
				RegBots.Worker(args);
				break;
			case "vote":
				Misc.vote_subscribe(args);
				break;
			case "subscribe":
				Misc.vote_subscribe(args);
				break;
			case "unsubscribe":
				Misc.vote_subscribe(args);
				break;
			case "test":
				test();
				break;
			case "filter_min_exp":
				Misc.filter_min_exp(args);
				break;
			case "joinparty":
			case "leaveparty":
				PartyJoin.join_party(args);
				break;
			case "zombolize":
				Zomboloid.zombolize(args);
				break;
            case "daily":
                Daily.Worker(args);
                break;
			default:
				ConsoleLog.WriteLine("Available subprograms: regbots, activatemail");
				break;
			}
		}
	}
}
