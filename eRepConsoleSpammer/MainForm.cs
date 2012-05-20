using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NerZul.Core.Utils;
using SimpleDOMParserCSharp;
using System.Xml;

namespace eRepConsoleSpammer
{
    public partial class SpamForm : Form
    {
        string m_Response;
        bool bBeep = false;
        Dictionary<string, string> m_ssUsers = new Dictionary<string, string>();
        public SpamForm()
        {
            InitializeComponent();
        }
        string GetLine(Dictionary<string, string> d)
        {
            // Build up each line one-by-one and them trim the end
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in d)
            {
                builder.Append(pair.Key).Append(":").Append(pair.Value).Append(',');
            }
            string result = builder.ToString();
            // Remove the final delimiter
            result = result.TrimEnd(',');
            return result;
        }

        Dictionary<string, string> GetDict(string f)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            string s = System.IO.File.ReadAllText(f);
            // Divide all pairs (remove empty strings)
            string[] tokens = s.Split(new char[] { ':', ',' },
                StringSplitOptions.RemoveEmptyEntries);
            // Walk through each item
            for (int i = 0; i < tokens.Length; i += 2)
            {
                string name = tokens[i];
                string freq = tokens[i + 1];
                name.Trim('\r');
                name.Trim('\n');
                name.Trim('\r');
                freq.Trim('\r');
                freq.Trim('\n');
                freq.Trim('\r');

                // Fill the value in the sorted dictionary
                d.Add(name, freq);
            }
            return d;
        }
        private void Partybutton_Click(object sender, EventArgs e)
        {
            string sbuf;
            int iTotal;
            string sCountFind;
            string sFind;
            int ipos;
            int i;
            int iVal;
            string sVal1, sVal2, sFind1;
            string sB1 = "\">", sB2 = "</";
            if(PartytextBox.Text.Length==0)
            {
                MessageBox.Show("Имя партии не задано!");
                return;
            }
            PartytextBox.Text.Trim('/');
            string sLogin = LogintextBox.Text;
            string sPassword = PasswordtextBox.Text;
            if (sLogin.Length == 0 || sPassword.Length == 0)
            {
                MessageBox.Show("Логин и пароль не заданы!");
                return;
            }

            throw new NotImplementedException("Login not updated to email");
            NerZul.Core.Network.Bot bt = new NerZul.Core.Network.Bot(sLogin, sLogin, sPassword, bBeep);
            if (!bt.Login())
                return;
            
            m_Response = bt.CustomRequest("http://www.erepublik.com/en/party-members/" + PartytextBox.Text + "/1");
            if (m_Response.Contains("Page not found"))
            {
                MessageBox.Show("Имя партии должно быть задано в формате (название-логин)!");
                return;
            }
            sCountFind = "class=\"last \" title=\"Go to page ";
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
            ConsoleLog.WriteLine("Total=" + iTotal.ToString());
            for (i = 1; i <= iTotal; i++ )
            {
                ConsoleLog.WriteLine("http://www.erepublik.com/en/party-members/" + PartytextBox.Text + "/" + i.ToString());
                m_Response = bt.CustomRequest("http://www.erepublik.com/en/party-members/" + PartytextBox.Text + "/" + i.ToString());
                sFind = "class=\"nameholder\" href=\"/en/citizen/profile/";
                while ((ipos = m_Response.IndexOf(sFind)) !=-1)
                {
                    m_Response = m_Response.Substring(ipos+sFind.Length);
                    ipos = m_Response.IndexOf("\"");
                    iVal = int.Parse(m_Response.Substring(0, ipos));
                    sVal1 = iVal.ToString();
                    sFind1 = sVal1 + sB1;
                    ipos = m_Response.IndexOf(sFind1);
                    m_Response = m_Response.Substring(ipos + sFind1.Length);
                    ipos = m_Response.IndexOf(sB2);
                    sVal2 = m_Response.Substring(0,ipos);
                    m_ssUsers[sVal1] = sVal2;
                }
                ConsoleLog.WriteLine("ok");
            }
            ConsoleLog.WriteLine("Total in " + m_ssUsers.Count.ToString());
            System.IO.File.WriteAllText(PartytextBox.Text + ".txt", GetLine(m_ssUsers));
            ConsoleLog.WriteLine("Saved to " + PartytextBox.Text + ".txt");
            MessageBox.Show("Готово!");
        }

        private void Filebutton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofn = new OpenFileDialog();
            ofn.Filter = "All files (*.*)|*.*";
            ofn.Title = "Выберите файл";
            if(ofn.ShowDialog()==DialogResult.OK)
            {
                FiletextBox.Text = ofn.FileName;
            }
        }

        private void Gobutton_Click(object sender, EventArgs e)
        {
            string sLogin, sPassword, sFile, sBufSubj, sSubj, sBufBody, sBody;
            sLogin = LogintextBox.Text;
            sPassword = PasswordtextBox.Text;
            sFile = FiletextBox.Text;
            sBufSubj = SubjtextBox.Text;
            sBufBody = TexttextBox.Text;
            sSubj = System.Web.HttpUtility.UrlEncode(SubjtextBox.Text);
            sBody = System.Web.HttpUtility.UrlEncode(TexttextBox.Text);
            if (sLogin.Length == 0)
            {
                MessageBox.Show("Не задан пароль!");
                return;
            }
            if (sPassword.Length == 0)
            {
                MessageBox.Show("Не задан логин!");
                return;
            }
            if (sFile.Length == 0)
            {
                MessageBox.Show("Не указан файл!");
                return;
            }
            if (sSubj.Length == 0)
            {
                MessageBox.Show("Отсуствует заголовок сообщения!");
                return;
            }
            if (SubjtextBox.Text.Length > 30)
            {
                MessageBox.Show("Длина заголовка не должна быть больше 30 символов!");
                return;
            }
            if (sBody.Length == 0)
            {
                MessageBox.Show("Отсутствует текст сообщения!");
                return;
            }
            m_ssUsers = GetDict(sFile);
            if (m_ssUsers.Count == 0)
            {
                MessageBox.Show("В указанном файле ничего нет!");
                return;
            }
            throw new NotImplementedException("Login not updated to email");
            NerZul.Core.Network.Bot bt = new NerZul.Core.Network.Bot(sLogin, sLogin, sPassword, "Opera/9.99 (Windows NT 5.1; U; pl) Presto/9.9.9", (AutocaptchacheckBox.Checked) ? AutocaptchatextBox.Text : "", 0, bBeep);
            if (!bt.Login())
            {
                MessageBox.Show("Неверно указаны логин или пароль!");
                return;
            }
            int i = 0;
            ConsoleLog.WriteLine("Total to send " + m_ssUsers.Count.ToString());
            foreach (var p in m_ssUsers)
            {
                i++;
                ConsoleLog.WriteLine(i.ToString() + " of " + m_ssUsers.Count.ToString());
                int iId = int.Parse(p.Key);
                string sName = p.Value;
                sSubj = sBufSubj.Replace("%username%", sName);
                sSubj = System.Web.HttpUtility.UrlEncode(sSubj);

                sBody = sBufBody.Replace("%username%", sName);
                sBody = System.Web.HttpUtility.UrlEncode(sBody);
               
                bt.SendMessage(iId, sName, sSubj, sBody);
            }
            MessageBox.Show("Готово!");
        }

        private void Closebutton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Userbutton_Click(object sender, EventArgs e)
        {
            string sbuf;
            int iTotal;
            string sCountFind;
            string sFind;
            int ipos;
            int i;
            int iVal;
            string sVal1, sVal2, sFind1;
            string sB1 = "\" href=\"/en/citizen/profile/", sB2 = "\">";
            if (UsertextBox.Text.Length == 0)
            {
                MessageBox.Show("Id пользователя не задано!");
                return;
            }
            string sLogin = LogintextBox.Text;
            string sPassword = PasswordtextBox.Text;
            if (sLogin.Length == 0 || sPassword.Length == 0)
            {
                MessageBox.Show("Логин и пароль не заданы!");
                return;
            }

            throw new NotImplementedException("Login not updated to email");
            NerZul.Core.Network.Bot bt = new NerZul.Core.Network.Bot(sLogin, sLogin, sPassword, bBeep);
            if (!bt.Login())
                return;
            m_Response = bt.CustomRequest("http://www.erepublik.com/en/citizen/friends/"+UsertextBox.Text+"/0/100");
            if (m_Response.Contains("Page not found"))
            {
                MessageBox.Show("Имя партии должно быть задано в формате (название-логин)!");
                return;
            }
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
            ConsoleLog.WriteLine("Total=" + iTotal.ToString());
            for (i = 0; i <= iTotal; i++)
            {
                m_Response = bt.CustomRequest("http://www.erepublik.com/en/citizen/friends/"+UsertextBox.Text+"/"+i.ToString()+"/100");
                ConsoleLog.WriteLine("http://www.erepublik.com/en/citizen/friends/" + UsertextBox.Text + "/" + i.ToString() + "/100");
                sFind = "<div class=\"avatarholder\">\n\t\t\t\t<a title=\"";
                while ((ipos = m_Response.IndexOf(sFind)) != -1)
                {
                    m_Response = m_Response.Substring(ipos + sFind.Length);
                    ipos = m_Response.IndexOf("\"");
                    
                    sVal2 = m_Response.Substring(0, ipos);
                    sFind1 = sVal2 + sB1;
                    ipos = m_Response.IndexOf(sFind1);
                    m_Response = m_Response.Substring(ipos + sFind1.Length);
                    ipos = m_Response.IndexOf(sB2);
                    iVal = int.Parse(m_Response.Substring(0, ipos));
                    sVal1 = iVal.ToString();
                    sVal2.Trim();
                    if (iVal!=2&&sVal2.Length!=0)
                    {
                        m_ssUsers[sVal1] = sVal2;
                    }
                }
                ConsoleLog.WriteLine("ok");
            }
            ConsoleLog.WriteLine("Total in " + m_ssUsers.Count.ToString());
            System.IO.File.WriteAllText(UsertextBox.Text + ".txt", GetLine(m_ssUsers));
            ConsoleLog.WriteLine("Saved to " + UsertextBox.Text + ".txt");
            MessageBox.Show("Готово!");
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void XMLbutton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofn = new OpenFileDialog();
            ofn.Filter = "All files (*.*)|*.*";
            ofn.Title = "Выберите файл";
            if (ofn.ShowDialog() == DialogResult.OK)
            {
                XMLtextBox.Text = ofn.FileName;
            }
        }

        private void ParseXMLbutton_Click(object sender, EventArgs e)
        {
            m_ssUsers = new Dictionary<string, string>();
            SimpleDOMParser dp = new SimpleDOMParser();
            SimpleElement elRoot = dp.parse(new XmlTextReader(XMLtextBox.Text));
            foreach (SimpleElement elCh in elRoot.ChildElements)
            {
                string sName = "";
                string sId = "";
                foreach (SimpleElement elSubCh in elCh.ChildElements)
                {
                    if (elSubCh.TagName=="name")
                    {
                        sName = elSubCh.Text;
                    }
                    if (elSubCh.TagName=="id")
                    {
                        sId = elSubCh.Text;
                    }
                    if (sId!=""&& sName!="")
                    {
                        break;
                    }
                }
                m_ssUsers.Add(sId, sName);
            }
            System.IO.File.WriteAllText("ParsedXML.txt", GetLine(m_ssUsers));
        }

        private void SpamForm_Load(object sender, EventArgs e)
        {
            string DataDir = System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
            DataDir = System.IO.Path.GetDirectoryName(DataDir);
            NerZul.Core.Utils.INIFile Config = new NerZul.Core.Utils.INIFile(System.IO.Path.Combine(DataDir, "config.ini"));
            bBeep = Config.GetValue("misc", "beep", false);
        }

        private void AutocaptchacheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AutocaptchatextBox.Enabled = AutocaptchacheckBox.Checked;
        }

        private void Regionbutton_Click(object sender, EventArgs e)
        {
            string sbuf;
            int iTotal;
            string sCountFind;
            string sFind;
            int ipos;
            int i;
            int iVal;
            string sVal1, sVal2, sFind1;
            string sB1 = "\">", sB2 = "</";
            if (RegiontextBox.Text.Length == 0)
            {
                MessageBox.Show("Номер региона на задан!");
                return;
            }
            PartytextBox.Text.Trim('/');
            string sLogin = LogintextBox.Text;
            string sPassword = PasswordtextBox.Text;
            if (sLogin.Length == 0 || sPassword.Length == 0)
            {
                MessageBox.Show("Логин и пароль не заданы!");
                return;
            }

            throw new NotImplementedException("Login not updated to email");
            NerZul.Core.Network.Bot bt = new NerZul.Core.Network.Bot(sLogin, sLogin, sPassword, bBeep);
            if (!bt.Login())
                return;

            m_Response = bt.CustomRequest("http://www.erepublik.com/en/rankings/citizens/region/1/" + RegiontextBox.Text);
            if (m_Response.Contains("Page not found"))
            {
                MessageBox.Show("Нужно ввести номер региона (цифру)!");
                return;
            }
            sCountFind = "class=\"last \" title=\"Go to page ";
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
            ConsoleLog.WriteLine("Total=" + iTotal.ToString());
            for (i = 1; i <= iTotal; i++)
            {
                ConsoleLog.WriteLine("http://www.erepublik.com/en/rankings/citizens/region/" + i.ToString() + "/" + RegiontextBox.Text);
                m_Response = bt.CustomRequest("http://www.erepublik.com/en/rankings/citizens/region/" + i.ToString() + "/" + RegiontextBox.Text);
                sFind = "\"dotted\" href=\"/en/citizen/profile/";
                while ((ipos = m_Response.IndexOf(sFind)) != -1)
                {
                    m_Response = m_Response.Substring(ipos + sFind.Length);
                    ipos = m_Response.IndexOf("\"");
                    iVal = int.Parse(m_Response.Substring(0, ipos));
                    sVal1 = iVal.ToString();
                    sFind1 = sVal1 + sB1;
                    ipos = m_Response.IndexOf(sFind1);
                    m_Response = m_Response.Substring(ipos + sFind1.Length);
                    ipos = m_Response.IndexOf(sB2);
                    sVal2 = m_Response.Substring(0, ipos);
                    m_ssUsers[sVal1] = sVal2;
                }
                ConsoleLog.WriteLine("ok");
            }
            ConsoleLog.WriteLine("Total in " + m_ssUsers.Count.ToString());
            System.IO.File.WriteAllText(RegiontextBox.Text + ".txt", GetLine(m_ssUsers));
            ConsoleLog.WriteLine("Saved to " + RegiontextBox.Text + ".txt");
            MessageBox.Show("Готово!");
        }
    }
}
