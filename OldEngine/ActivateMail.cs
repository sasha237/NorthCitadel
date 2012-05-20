
using System;
using NerZul.Core.Utils;

namespace Engine
{


	public static class ActivateMail
	{

		public static void Worker(string[] args)
		{
			Pop3.Pop3Client mailclient=new Pop3.Pop3Client();
			ConsoleLog.WriteLine("Connecting to "+args[1]);
			mailclient.Connect(args[1],args[2],args[3]);
			ConsoleLog.WriteLine("Logged in. Retrieving message list...");
			System.Collections.Generic.List<Pop3.Pop3Message> messages=mailclient.List();
			foreach (Pop3.Pop3Message message in messages)
			{
				mailclient.Retrieve(message);
				if(!message.Body.Contains("erepublik"))
				{
					mailclient.Delete(message);
					ConsoleLog.WriteLine("Unrelated message. Deleted.");
				}else
				{
                    string sBuf = message.Body;
                    sBuf = sBuf.Replace("\n", "");
                    sBuf = sBuf.Replace("\r", "");
                    string link = sBuf.Substring(sBuf.IndexOf("http://www.erepublik.com/en/register-validate/"));
					link=link.Remove(link.IndexOfAny(new char[]{'\r','\n', '\"', '<'}));
                    link = link.Replace("=", "");
					ConsoleLog.WriteLine(link);
					NerZul.Core.Network.HttpClient http =new NerZul.Core.Network.HttpClient();
                    try
                    {
                        link = http.DownloadString(link);
                    }
                    catch (System.Exception e)
                    {
                        ConsoleLog.WriteLine("Activation error: " + e.Message);
                    }
                    if (link.Contains("Congratulations"))
					{
						ConsoleLog.WriteLine("Activated");
					};
					if (link.Contains("has either expired"))
					{
						ConsoleLog.WriteLine("Expired");
					};
					mailclient.Delete(message);
                    ConsoleLog.WriteLine("Deleted.");
				
					System.Threading.Thread.Sleep(1000);
				}
			}
		}
	}
}
