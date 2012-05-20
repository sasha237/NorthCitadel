using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Engine;
using NerZul.Core.Utils;

namespace eRepConsoleManagementSystem
{
    public partial class ImportForm : Form
    {
        public ImportForm()
        {
            InitializeComponent();
        }
        Dictionary<string, string> GetDict(string f)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            string[] s = System.IO.File.ReadAllLines(f);
            if (s.Length!=0)
            {
                if (s.Length % 2 == 0 && (s[0].IndexOfAny(new char[] { '|', ';' }) == -1))
                {
                    for (int i = 0; i < s.Length; i += 2)
                    {
                        if (!d.ContainsKey(s[i]))
                            d.Add(s[i], s[i + 1]);
                    }
                }
                else
                {
                    if (s.Length == 1)
                        s = s[0].Split(';');
                    for (int i = 0; i < s.Length; i ++)
                    {
                        string[] LPPair = s[i].Split('|');
                        if (LPPair.Length == 2)
                        {
                            if(!d.ContainsKey(LPPair[0]))
                                d.Add(LPPair[0], LPPair[1]);
                        }
                    }
                }
            }
            return d;
        }
        Dictionary<string, string> m_ssUsers = new Dictionary<string, string>();

        private void Selectbutton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofn = new OpenFileDialog();
            ofn.Filter = "All files (*.*)|*.*";
            ofn.Title = "Выберите файл";
            if (ofn.ShowDialog() == DialogResult.OK)
            {
                FilePathtextBox.Text = ofn.FileName;
            }
       }

        private void StartButton_Click(object sender, EventArgs e)
        {
            string sFile, sGroup;
            sGroup = GrouptextBox.Text;
            sFile = FilePathtextBox.Text;
            if (sFile.Length == 0)
            {
                MessageBox.Show("Не указан файл!");
                return;
            }
            if (sGroup.Length == 0)
            {
                MessageBox.Show("Отсуствует название группы!");
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
                try
                {
                    if (d.Key.Length==0||d.Value.Length==0)
                        continue;
                    Globals.Database.Insert("bots", "login", d.Key, "password", d.Value,
                                    "group", sGroup);
                }
                catch (System.Exception e1)
                {
                    ConsoleLog.WriteLine("Error: " + e1.Message);
                }
            }
            MessageBox.Show("Готово!");
        }
    }
}
