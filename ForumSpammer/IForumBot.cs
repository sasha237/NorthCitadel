using System;
using System.Collections;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using NerZul.Core.Utils;
using System.Text.RegularExpressions;
using NerZul.Core.Network;

namespace ForumSpammer
{
    public class Pair2<T, U>
    {
        public Pair2()
        {
        }

        public Pair2(T first, U second)
        {
            this.First = first;
            this.Second = second;
        }

        public T First { get; set; }
        public U Second { get; set; }
    };
    class IForumBot
    {
        public string m_Response;
        public string m_sForumPath;
        public HttpClient m_Client = new HttpClient();
        public HttpClient HttpClient
        {
            get
            {
                return m_Client;
            }
        }
        public string ForumPath
        {
            get
            {
                return m_sForumPath;
            }
        }
        public string GetLastResponse()
        {
            return m_Response;
        }
        private IForumBot()
        {

        }
        public IForumBot(string sForumPath)
        {
            m_sForumPath = sForumPath;
            m_sForumPath.Trim();
            m_sForumPath.Trim('/');
        }
        public virtual bool Login(string sLogin, string sPassword)
        {
            return false;
        }
        public virtual Hashtable GetUserList(int iPage)
        {
            return null;
        }
        public virtual void SendMessageToUser(IDictionaryEnumerator pp, string sMes, string sTitle)
        {
            return;
        }
        public virtual bool SendMessagesToUsers(Hashtable lst, string sMes, string sTitle)
        {
            return false;
        }
        public virtual void WorkingCycle(string sLogin, string sPassword, string sTitle, string sMessage)
        {
            return;
        }
    }
}
