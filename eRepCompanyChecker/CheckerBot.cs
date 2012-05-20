using System;
using System.Collections.Generic;
using System.Text;
using NerZul.Core.Network;
using System.Text.RegularExpressions;
using NerZul.Core.Utils;

namespace eRepCompanyChecker
{
    public class CheckerBot: Bot
    {
        public CheckerBot(string UserName, string Email, string Password) :
            base(UserName, Email, Password, false)
        {
        }
        public CheckerBot(string UserName, string Email, string Password, string UserAgent, string autocaptcha, int captchaBufferSize) :
            base(UserName, Email, Password, UserAgent, autocaptcha, captchaBufferSize, false)
        {
        }

        public List<int> GetCompanyWorkers(string sCompany, int weekNum)
        {
            if (m_Response.IndexOf("logout") == -1)
                return null;
            int i;
            //int iPos = sCompany.IndexOf("/");
            //string sBuf = sCompany.Substring(iPos+1, sCompany.Length - iPos-1);
            string sBuf = sCompany.Split('/')[1];

            List<int> list = new List<int>();

            m_Response = m_Client.DownloadString("http://economy.erepublik.com/en/company/employees/" + sCompany + "/" + weekNum.ToString());

            if (m_Response.Contains("This company has no employees"))
                return list;

            ms_companyToken = Regex.Match(m_Response, "m.setAttribute\\('value', '(.*)'\\)").Groups[1].Value;

            string sPageCount = Regex.Match(m_Response, "<a href=\"/en/company/employees/" + sCompany + "/" + weekNum.ToString() + "/([0-9.]{1,9})\" class=\"last \" title=").Groups[1].Value;
            if (String.IsNullOrEmpty(sPageCount))
                sPageCount = "1";
            int iPageCounts = int.Parse(sPageCount);
            if (ms_companyToken.Length == 0)
                return null;
            for (i = 1; i <= iPageCounts; i++)
            {
                m_Response = m_Client.DownloadString("http://economy.erepublik.com/en/company/employees/" + sCompany + "/" + weekNum.ToString() + "/" + i.ToString());
                Match mc = Regex.Match(m_Response, "/en/company/fire-employee/" + sBuf + "/(.*)/" + weekNum.ToString() + "/" + i.ToString());
                while (mc.Success)
                {
                    try
                    {
                        list.Add(int.Parse(mc.Groups[1].Value));
                    }
                    catch (System.Exception e)
                    {
                        ConsoleLog.WriteLine("GetCompanyWorkers error: " + e.Message);
                    }
                    mc = mc.NextMatch();
                }
            }

            return list;
        }

        public void FireWorker(string sCompany, int iWorker, int weekNum)
        {
            //int iPos = sCompany.IndexOf("/");
            //string sBuf = sCompany.Substring(iPos + 1, sCompany.Length - iPos - 1);
            string sBuf = sCompany.Split('/')[1];

            m_Client.Referer = "http://economy.erepublik.com/en/company/employees/" + sCompany + "/" + weekNum.ToString();
            string PostData = "_token=" + ms_companyToken;
            m_Response = m_Client.UploadString("http://economy.erepublik.com/en/company/fire-employee/" + sBuf + "/" + iWorker.ToString() + "/" + weekNum.ToString() + "/1", PostData);

        }

        private List<int> GetAlerts()
        {
            List<int> lst = new List<int>();
            m_Response = m_Client.DownloadString("http://www.erepublik.com/en/messages/alerts/1");
            Match mc = Regex.Match(m_Response, "delete_message_([0-9.]{1,9})");
            while (mc.Success)
            {
                int iId = int.Parse(mc.Groups[1].Value);
                lst.Add(iId);
                mc = mc.NextMatch();
            }
            return lst;
        }

        private bool DeleteAlerts(List<int> lst)
        {
            if (lst.Count == 0)
                return false;
            m_Client.Referer = "http://www.erepublik.com/en/messages/alerts/1";
            string PostData = "_token=" + ms_Token2;
            foreach (int el in lst)
            {
                PostData += "&delete_message[]=" + el.ToString();
            }
            PostData += "&delete_messages_all=";
            PostData += "&commit=Delete";
            //ConsoleLog.WriteLine(PostData, "DeletePostData.txt");
            m_Response = m_Client.UploadString("http://www.erepublik.com/en/messages/alerts/1", PostData);
            //ConsoleLog.WriteLine(m_Response, "DeleteResponses.txt");
            return true;
        }

        public void DeleteAllAlerts()
        {
            int counter = 0;

            do
            {
                if (counter > 0)
                {
                    System.Threading.Thread.Sleep(400);
                    //break;
                }
                counter++;
                ConsoleLog.WriteLine("Deleteing alerts portion " + counter.ToString());
            }
            while (DeleteAlerts(GetAlerts()));
        }
    }
}
