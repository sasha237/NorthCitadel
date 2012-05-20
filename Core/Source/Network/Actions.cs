using System;
using System.Collections.Generic;
using System.Text;

namespace NerZul.Network
{
    static class Actions
    {
        private static string GetIDAntireg(string UserName)
        {
            HttpClient WC = new HttpClient();
            string sBuf= WC.DownloadString("http://"+UserName + ".antireg.ru");
            string sScan = "register-validate%2F";
            sBuf=sBuf.Remove(0, sBuf.IndexOf(sScan)+sScan.Length);
            sBuf=sBuf.Substring(0, sBuf.IndexOf("\""));
            return sBuf;
        }
        public static void ActivateAccount(string UserName, string MailService, string UserAgent, string Proxy)
        {
            //Get ID from mail service
            string sActivateID = "";
            try
            {
                switch (MailService)
                {
                    case ("antireg"):
                        sActivateID = GetIDAntireg(UserName);
                        break;
                    default:
                        throw new NotSupportedException("Mail service is not supported");

                };
            }
            catch (NotSupportedException e){throw e;}
            catch {
                throw new Exception("Unable to connect to the mail service");
            };

            if (sActivateID == "") throw new Exception("No Mail");
            
            //Try to activate
            string sBuf="";
            HttpClient WC = new HttpClient();
            WC.Proxy = new System.Net.WebProxy(Proxy);
            sBuf = WC.DownloadString("http://erepublik.com/en/register-validate/" + sActivateID);
            if (sBuf.IndexOf("You can login and start playing. Have fun!") == -1)
            {
                throw new Exception("Invalid link");     
            };
            return;
        }
    };
};