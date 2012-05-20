using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using NerZul.Core.Utils;

namespace eRepConsoleVoter
{
    public partial class MainForm : Form
    {
        Dictionary<string, string> m_ssUsers = new Dictionary<string, string>();
        public MainForm()
        {
            InitializeComponent();
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"Software\sasha237\eRepVoter\Path", true);
            if (rk!=null)
            {
                FiletextBox.Text = (string)rk.GetValue("Path", "");
            }
        }

        Dictionary<string, string> GetDict(string f)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            string [] s = System.IO.File.ReadAllLines(f);
            if (s.Length % 2 != 0)
            {
                MessageBox.Show("Неверный формат файла!");
                return d;
            }
            for (int i = 0; i < s.Length; i += 2)
            {
                d.Add(s[i], s[i+1]);
            }
            return d;
        }
        
        private void Filebutton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofn = new OpenFileDialog();
            ofn.Filter = "All files (*.*)|*.*";
            ofn.Title = "Выберите файл";
            if(ofn.ShowDialog()==DialogResult.OK)
            {
                FiletextBox.Text = ofn.FileName;
                RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"Software\sasha237\eRepVoter\Path", true);
                if (rk == null)
                    rk = Registry.CurrentUser.CreateSubKey(@"Software\sasha237\eRepVoter\Path");
                rk.SetValue("Path", FiletextBox.Text, RegistryValueKind.String);
            }
        }

        private void Closebutton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Gobutton_Click_1(object sender, EventArgs e)
        {
            string sLogin, sPassword, sFile, sLink;
            sLink = TopictextBox.Text;
            sFile = FiletextBox.Text;
            int iLink = 0;
            if (sFile.Length == 0)
            {
                MessageBox.Show("Не указан файл!");
                return;
            }
            if (sLink.Length == 0)
            {
                MessageBox.Show("Отсуствует номер на статьи!");
                return;
            }
            iLink = int.Parse(sLink);
            if (iLink.ToString() != sLink)
            {
                MessageBox.Show("Неверно задан номер статьи!");
                return;
            }
            m_ssUsers = GetDict(sFile);
            if (m_ssUsers.Count == 0)
            {
                MessageBox.Show("В указанном файле ничего нет!");
                return;
            }
            foreach (var d in m_ssUsers)
            {
                throw new NotImplementedException("Login not updated to email");
                sLogin = d.Key;
                sPassword = d.Value;
                ConsoleLog.WriteLine(sLogin);
                NerZul.Core.Network.Bot bt = new NerZul.Core.Network.Bot(sLogin, sLogin, sPassword, false);
                try
                {
                    if (!bt.Login())
                    {
                        ConsoleLog.WriteLine("Wrong login or password!");
                        continue;
                    }
                    if(bt.GetLastResponse().Contains("dead"))
                    {
                        ConsoleLog.WriteLine("Possible dead!");
                        bt.Revive();
                    }
                }
                    
                catch (System.Exception e1)
                {
                    ConsoleLog.WriteLine(sLogin+": Possible dead!");
                    ConsoleLog.WriteLine(e1.Message);
                    try
                    {
                        bt.Revive();
                    }
                    catch (System.Exception e2)
                    {
                        ConsoleLog.WriteLine(sLogin+": Possible banned!");
                        ConsoleLog.WriteLine(e2.Message);
                        continue;
                    }
                }
                try
                {
                    bt.VoteArticle(iLink);
                    ConsoleLog.WriteLine(sLogin + ": voted!");
                }
                catch (System.Exception e2)
                {
                    ConsoleLog.WriteLine(e2.Message);
                    continue;
                }
            }
            MessageBox.Show("Готово!");

        }
    }
}
