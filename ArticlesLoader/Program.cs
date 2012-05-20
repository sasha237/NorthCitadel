using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NerZul.Core.Utils;
using System.Xml;


namespace ArticlesLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            List<string> listArts = new List<string>();
            List<string> listConts = new List<string>();
            int i;
            string sbuf;
            int iTotal;
            string sCountFind;
            string sFind;
            int ipos;
            NerZul.Core.Network.Bot bt = new NerZul.Core.Network.Bot("sasha240", "ebi@ebi.ee", "vfvfvskfhfve", false);
            string m_Response;
            string sNumPaper;
            sNumPaper = "the-independent-nowadays-191782";
            if (!bt.Login())
                return;
            m_Response = bt.CustomRequest("http://www.erepublik.com/en/newspaper/" + sNumPaper + "/1");
            sCountFind = "class=\"last\" title=\"Go to page ";
            ipos = m_Response.IndexOf(sCountFind);
            if (ipos == -1)
            {
                iTotal = 5;
            }
            else
            {
                sbuf = m_Response.Substring(ipos + sCountFind.Length);
                ipos = sbuf.IndexOf("\"");
                iTotal = int.Parse(sbuf.Substring(0, ipos));
            }
            //int iVal;
            //string sVal1;
            //string sFind1;
            //string sB1;
            //string sVal2;
            //string sB2;
            string sFullLink;
            ConsoleLog.WriteLine("Total=" + iTotal.ToString());
            for (i = 1; i <= iTotal; i++)
            {
                sFullLink = "http://www.erepublik.com/en/newspaper/" + sNumPaper + "/" + i.ToString();
                ConsoleLog.WriteLine(sFullLink);
                m_Response = bt.CustomRequest(sFullLink);
                sFind = "class=\"padded\"><a href=\"/en/article/";
                sbuf = "class=\"padded\"><a href=\"";
                while ((ipos = m_Response.IndexOf(sFind)) != -1)
                {
                    m_Response = m_Response.Substring(ipos + sbuf.Length);
                    sFind = "/1/20\">";
                    ipos = m_Response.IndexOf(sFind);
                    listArts.Add(m_Response.Substring(0, ipos + sFind.Length-2));
                    sFind = "class=\"padded\"><a href=\"/en/article/";
                }
                ConsoleLog.WriteLine("ok");
            }
            foreach(string sLink in listArts)
            {
                System.Threading.Thread.Sleep(2000);
                sFullLink = "http://www.erepublik.com" + sLink;
                ConsoleLog.WriteLine(sFullLink);
                try
                {
                    m_Response = bt.CustomRequest(sFullLink);
                }
                catch (System.Exception e)
                {
                    Console.WriteLine("Error");
                    continue;
                }
                
                sFind = "<p class=\"preview\">";
                ipos = m_Response.IndexOf(sFind);
                if (ipos!=-1)
                {
                    m_Response = m_Response.Substring(ipos + sFind.Length);
                    sFind = "<p class=\"bottomcontrol\">";
                    ipos = m_Response.IndexOf(sFind);
                    listConts.Add(m_Response.Substring(0, ipos));
                }
            }
            foreach(string sCont in listConts)
            {
                sb.Append(sCont);
            }
            System.IO.File.WriteAllText("c:\\2\\" + sNumPaper + ".txt", sb.ToString());
        }
    }
}
