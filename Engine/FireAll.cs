using System;
using NerZul.Core.Utils;

namespace Engine
{
	public static class FireAll
	{
		public static void Worker(string[] args)	
		{
#if !PUBLIC_BUILD
			if(args.Length!=4)
			{
                ConsoleLog.WriteLine("Usage: fire_all login(email) password company");
				return;
			}
            Globals.webCitadel.SendLogInfo(new string[] { "fire_all" }, 0);
            args[1] = args[1].Replace("%", " ");
            NerZul.Core.Network.Bot bot = new NerZul.Core.Network.Bot(args[1], args[1], args[2], Globals.BotConfig.bBeep);
			if(!bot.Login()) 
			{
				ConsoleLog.WriteLine("Unable to login");
				return;
			}
			string scan=bot.CustomRequest("http://www.erepublik.com/en/company-employees/"+
			                              args[3]+"/1/1/1");
			while(true)
			{
				System.Text.RegularExpressions.Match m=
					System.Text.RegularExpressions.Regex.Match(scan,
					@"/en/fire-employee/company-\w+-employee-\w+");
				if(m.Groups[0].Value.Length==0) break;
				ConsoleLog.WriteLine("Found: "+m.Groups[0].Value);
				System.Threading.Thread.Sleep(3000);
				scan=bot.CustomRequest("http://www.erepublik.com"+m.Groups[0].Value);
			}
				
			return;

            //http://api.erepublik.com/v1/feeds/companies/COMPANY_ID}
#else
            ConsoleLog.WriteLine("Думаешь самый умный? Сказано, не работает!");
#endif
        }
	}
}

