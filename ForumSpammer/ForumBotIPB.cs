using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace ForumSpammer
{
    class ForumBotIPB : IForumBot
    {
        string m_sSession;
        public ForumBotIPB(string sForumPath)
            : base(sForumPath)
        {
            m_sSession = "";
        }
        public override bool Login(string sLogin, string sPassword)
        {
            m_Response = m_Client.DownloadString(m_sForumPath);
            m_Client.Referer = ForumPath + "/index.php?act=Login&CODE=00";
            string POSTDATA = "referer=" + m_sForumPath + "/"
                + "&UserName=" + sLogin
                + "&PassWord=" + sPassword
                + "&CookieDate=1";
            m_Response = m_Client.UploadString(m_sForumPath + "/index.php?act=Login&CODE=01", POSTDATA);
            return m_Response.Contains("CODE=03");
        }
        public override Hashtable GetUserList(int iPage)
        {
            Hashtable lst = new Hashtable();
            m_Client.Referer = ForumPath + "/index.php?act=Login&CODE=01";
            m_Response = m_Client.DownloadString(m_sForumPath + "/index.php?&name_box=&sort_key=members_display_name&sort_order=asc&filter=ALL&act=members&max_results=20&aim=&yahoo=&icq=&msn=&posts=&joined=&lastpost=&lastvisit=&signature=&homepage=&name=&photoonly=&st=" + ((int)(iPage*20)).ToString());
            Match mc1 = Regex.Match(m_Response, "showuser\\=(\\d*)");
            Match mc2 = Regex.Match(m_Response, "showuser\\=\\d*\"\\>(.*)\\<\\/a");
            while (mc1.Success && mc2.Success)
            {
                try
                {
                    Pair2<int, string> pp = new Pair2<int, string>(int.Parse(mc1.Groups[1].Value), mc2.Groups[1].Value);
                    if(!lst.Contains(int.Parse(mc1.Groups[1].Value)))
                        lst.Add(int.Parse(mc1.Groups[1].Value),mc2.Groups[1].Value);
                }
                catch (System.Exception e)
                {
                    Console.WriteLine("GetUserList ForumBotIPB error: " + e.Message);
                }
                mc1 = mc1.NextMatch();
                mc2 = mc2.NextMatch();
            }
            return lst;
        }

        public override void SendMessageToUser(IDictionaryEnumerator pp, string sMes, string sTitle)
        {
            Console.WriteLine("SendMessageToUser " + pp.Value.ToString());
            string attach_post_key = "";
            string auth_key = "";
            m_Client.Referer = ForumPath + "/index.php?act=Msg&CODE=4&MID=" + pp.Key.ToString();
            m_Response = m_Client.DownloadString(m_sForumPath + "/index.php?act=Msg&CODE=4&MID=" + pp.Key.ToString());
            Match mc1 = Regex.Match(m_Response, "attach_post_key\" value\\=\"(.*)\"\\s*\\/\\>");
            if (mc1.Success)
            {
                attach_post_key = mc1.Groups[1].Value;
            }

            mc1 = Regex.Match(m_Response, "auth_key\" value\\=\"(.*)\"\\s*\\/\\>");
            if (mc1.Success)
            {
                auth_key = mc1.Groups[1].Value;
            }

            string POSTDATA =
                "removeattachid=0" +
                "&OID=0" +
                "&act=Msg" +
                "&CODE=04" +
                "&MODE=01" +
                "&attach_post_key=" + attach_post_key +
                "&auth_key=" + auth_key +
                "&entered_name=" + pp.Value.ToString() +
                "&carbon_copy=" +
                "&msg_title=" + sTitle +
                "&ed-0_wysiwyg_used=0" +
                "&editor_ids%5B%5D=ed-0" +
                "&Post=" + sMes;
            m_Response = m_Client.UploadString(m_sForumPath + "/index.php?act=msg", POSTDATA);
        }

        public override bool SendMessagesToUsers(Hashtable lst, string sMes, string sTitle)
        {
            if (lst == null || lst.Count == 0)
                return false;
            for (IDictionaryEnumerator e = lst.GetEnumerator(); e.MoveNext(); )
            {
                try
                {

                    SendMessageToUser(e, sMes, sTitle);
                }
                catch (System.Exception e1)
                {
                    Console.WriteLine("SendMessagesToUsers: " + e1.ToString());
                }
            }
            return true;
        }
        public override void WorkingCycle(string sLogin, string sPassword, string sTitle, string sMessage)
        {
            Console.WriteLine("WorkingCycle Begin");
            try
            {
                if (Login(sLogin, sPassword))
                {
                    Console.WriteLine("Logged in");
                    int i = 0;
                    while (SendMessagesToUsers(GetUserList(i++), sMessage, sTitle)) ;
                }

            }
            catch (System.Exception e)
            {
                Console.WriteLine("WorkingCycle error: " + e.ToString());
            }
            Console.WriteLine("WorkingCycle End");
        }
    }
}
