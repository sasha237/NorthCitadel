using System;
using NerZul.Core.Utils;
namespace Engine
{
	public static class Import
	{
		public static void ImportBots (string[] args)
		{
//#if PUBLIC_BUILD
            if (!Globals.IsValid())
                return;
//#endif

			if(args.Length!=3)
			{
				ConsoleLog.WriteLine("import filename group");
				ConsoleLog.WriteLine("Format: login|password\\n");
				return;
			}
			string[] Botota=System.IO.File.ReadAllLines(args[1]);
			foreach (string Bot in Botota)
			{
				if(Bot.Length!=0)
				{
					string[] LPPair=Bot.Split('|');
					if(LPPair.Length!=2)
					{
						ConsoleLog.WriteLine("Unable to import "+Bot+" - use login|password syntax");
					}
					else
					{
						Globals.Database.Reset();
						try
						{
							Globals.Database.Insert("bots","login",LPPair[0],"password", LPPair[1],
						                        "group",args[2]);
						}
						catch (Exception e)
						{
							ConsoleLog.WriteLine("Unable to import "+LPPair[0]+": "+e.Message);
						}
					}
				}
			}
		}
	}
}

