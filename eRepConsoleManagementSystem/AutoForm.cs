using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Security;
using System.Management;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;

namespace eRepConsoleManagementSystem
{
    [assembly: SuppressIldasmAttribute()]
    public partial class AutoForm : Form
    {
        public string MD5Hash(string instr)
        {
            string strHash = string.Empty;

            foreach (byte b in new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(instr)))
            {
                strHash += b.ToString("X2");
            }
            return strHash;
        }
        public byte[] StrToByteArray(string str)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }

        public AutoForm()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str1 = textBox1.Text;
            string str2 = textBox2.Text;
            if (MD5Hash(Convert.ToBase64String(StrToByteArray(str1))) != str2)
            {
                MessageBox.Show("FAIL");
                return;
            }

            string DataDir = System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
            DataDir = System.IO.Path.GetDirectoryName(DataDir);
            NerZul.Core.Utils.INIFile Access = new NerZul.Core.Utils.INIFile(System.IO.Path.Combine(DataDir, "access.ini"));

            Access.SetValue("vals", "FirstKey", textBox1.Text);
            Access.SetValue("vals", "SecondKey", textBox2.Text);
            Access.Flush();

            MessageBox.Show("OK");
            MessageBox.Show("Пожалуйста перезапуститесь");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StringBuilder sb1 = new StringBuilder();
            ManagementObjectSearcher searcher = null;
            searcher = new ManagementObjectSearcher("Select * from Win32_IRQResource");
            foreach (ManagementObject item in searcher.Get())
            {
                StringBuilder sb = new StringBuilder("");
                sb.Append(item.GetPropertyValue("Availability").ToString());
                sb.Append(item.GetPropertyValue("Hardware").ToString());
                sb.Append(item.GetPropertyValue("IRQNumber").ToString());
                sb.Append(item.GetPropertyValue("Name").ToString());
                sb.Append(item.GetPropertyValue("TriggerLevel").ToString());
                sb.Append(item.GetPropertyValue("TriggerType").ToString());
                string sBuf = MD5Hash(Convert.ToBase64String(StrToByteArray(sb.ToString())));
                sb1.Append(sBuf);
            }
            textBox1.Text = sb1.ToString().Substring(0,200);
        }
    }
}
