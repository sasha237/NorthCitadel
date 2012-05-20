using System;
using NerZul.Core.Utils;
namespace Engine
{
	public static class Elections
	{
		/*
		jQuery.post('/vote-party-election', {
			_token: $j("#_token").val(),
			c: candidate,
			election: election	
		*/
		static string group, party, candidate,electiontype, operation;
        static int electionid;
		public static void Worker(string[] args)
		{
#if !PUBLIC_BUILD

            if (args.Length != 7)
			{
                ConsoleLog.WriteLine("Usage: " + args[0] + " operation(join|leave|vote) group party election_type electionid candidate");
				return;
			}
            operation = args[1];
            group = args[2]; 
            party = args[3];
            electiontype = args[4];
            int.TryParse(args[5], out electionid);
            candidate = args[6];
			DbRows bots=null;
			lock (Globals.DBLocker)
			{
				Globals.Database.Reset();
                if(operation=="vote")
				    Globals.Database.Where("last_day_vote",Globals.GetErepTime().Days,"<");
                if (group.ToLower() != "all")
				    Globals.Database.Where("group",group);
                Globals.Database.Where("banned", 0);
                Globals.Database.Where("activated", 1);
                if (!String.IsNullOrEmpty(Globals.addWhere))
                    Globals.Database.Where(Globals.addWhere);
                bots = Globals.Database.Select("bots");
			}
            Globals.webCitadel.SendLogInfo(args, bots.Count);

            int poolsize = Globals.threadCount;
            if (Globals.BotConfig.useTOR)
                poolsize = 1;
            System.Threading.Thread[] pool = new System.Threading.Thread[poolsize];
			
			foreach(var botnfo in bots)
			{
				while (true)
				{
					bool found=false;
					for(int i=0; i<pool.Length;i++)
					{
						if((pool[i]==null)||(pool[i].IsAlive==false))
						{
							pool[i]=new System.Threading.Thread(BotProcWrap);
							pool[i].Start(botnfo);
							found=true;
							break;
						}
					}
					if (found) break;
					System.Threading.Thread.Sleep(1000);
				}
				
			}
			//Ждём завершения потоков
			while(true)
			{
				bool found=false;
				for(int i=0; i<pool.Length;i++)
				{
				if((pool[i]!=null)&&(pool[i].IsAlive))
					{
						found=true;
						break;
					}
				}
				if(!found) break;
				System.Threading.Thread.Sleep(1000);
			}

        }
		static void BotProcWrap(object arg)
		{
			try
			{
				BotProc((DbRow)arg);
			}catch (Exception e)
			{
				ConsoleLog.WriteLine("Exception: "+e.GetType().ToString()+": "+e.Message+
				                  "\n"+e.StackTrace);
			}
		}
		static void BotProc(DbRow botinfo)
		{
            Random rnd = new Random();
			NerZul.Core.Utils.ManagedCitizen bot=new NerZul.Core.Utils.ManagedCitizen(
                (string)botinfo["login"],
                (string)botinfo["email"],
                (string)botinfo["password"], 
                Globals.BotConfig);
			
			if(bot.Login()!=NerZul.Core.Utils.ManagedCitizen.LoginResult.Success)			
			//if(!bot.Bot.Login())
			{
				ConsoleLog.WriteLine("Unable to login - "+ (string)botinfo["login"]);
			}
            if (bot.Bot.GetLastResponse().Contains("dead"))
            {
                ConsoleLog.WriteLine("Possible dead!");
                
            }
            
			ConsoleLog.WriteLine("Logged in - "+botinfo["login"]);
			//Join party if needed
//            bool bChangedParty = false;
			if((string)botinfo["party"]!=party)
			{
                try
                {
                    if ((string)botinfo["party"] != "")
                    {
                        bot.Bot.LeaveParty((string)botinfo["party"]);
                    }

                }
                catch (System.Exception e)
                {
                    ConsoleLog.WriteLine("LeaveParty error: " + e.ToString());
                }
                if ((operation == "join" || operation == "vote") && (string)botinfo["party"] != party)
                {
                    try
                    {
                        if (!bot.Bot.JoinParty(party))
                        {
                            ConsoleLog.WriteLine("Unable to join party - " + (string)botinfo["login"]);
                            return;
                        }
                    }
                    catch (System.Exception e)
                    {
                        ConsoleLog.WriteLine("JoinParty error: " + e.ToString());
                    }

                    botinfo["party"] = party;
//                    bChangedParty = true;
                }
                else
                    botinfo["party"] = "";
				Utils.UpdateDbWithCustomBotInfo(botinfo,"party");
			}
            if (operation == "vote")
            {

                    switch(electiontype)
                    {
                        case "vote-for-congress":
                            try
                            {
                                if (!bot.Bot.VoteCongress(electionid, candidate))
                                {
                                    ConsoleLog.WriteLine("Unable to vote- " + botinfo["login"]);
                                    return;
                                }
                                botinfo["last_day_vote"] = Globals.GetErepTime().Days;
                                Utils.UpdateDbWithCustomBotInfo(botinfo, "last_day_vote");

                            }
                            catch (System.Exception e)
                            {
                                ConsoleLog.WriteLine("VoteCongress error: " + e.ToString());
                            }
                            break;
                        case "vote-party-election":
                            try
                            {
                                if (!bot.Bot.VoteParty(electionid, candidate))
                                {
                                    ConsoleLog.WriteLine("Unable to vote- " + botinfo["login"]);
                                    return;
                                }
                                botinfo["last_day_vote"] = Globals.GetErepTime().Days;
                                Utils.UpdateDbWithCustomBotInfo(botinfo, "last_day_vote");

                            }
                            catch (System.Exception e)
                            {
                                ConsoleLog.WriteLine("VoteParty error: " + e.ToString());
                            }
                            break;
                        default:
                            break;
                    }


//                 botinfo["last_day_vote"] = Globals.GetErepTime().Days;
//                 Utils.UpdateDbWithCustomBotInfo(botinfo, "last_day_vote");
//                 if (bChangedParty)
//                 {
//                     try
//                     {
//                         if (!bot.Bot.LeaveParty(party))
//                         {
//                             ConsoleLog.WriteLine("Unable to leave party - " + botinfo["login"]);
//                             return;
//                         }
//                     }
//                     catch (System.Exception e)
//                     {
//                         ConsoleLog.WriteLine("LeaveParty error: " + e.ToString());
//                     }
// 
//                     botinfo["party"] = "";
//                     Utils.UpdateDbWithCustomBotInfo(botinfo, "party");
//                 }
            }
            //System.Threading.Thread.Sleep(20000);
#else
            ConsoleLog.WriteLine("Думаешь самый умный? Сказано, не работает!");
#endif
        }
	
	}
}

