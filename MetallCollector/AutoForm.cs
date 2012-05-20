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

namespace MetallCollector
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
            string sCount = textBox3.Text;
            StringBuilder sb1 = new StringBuilder();
            ManagementObjectSearcher searcher = null;
//             searcher = new ManagementObjectSearcher("Select * from Win32_IRQResource");
//             foreach (ManagementObject item in searcher.Get())
//             {
//                 StringBuilder sb = new StringBuilder(sCount);
//                 sb.Append(item.GetPropertyValue("Availability").ToString());
//                 sb.Append(item.GetPropertyValue("Hardware").ToString());
//                 sb.Append(item.GetPropertyValue("IRQNumber").ToString());
//                 sb.Append(item.GetPropertyValue("Name").ToString());
//                 sb.Append(item.GetPropertyValue("TriggerLevel").ToString());
//                 sb.Append(item.GetPropertyValue("TriggerType").ToString());
//                 string sBuf = MD5Hash(Convert.ToBase64String(StrToByteArray(sb.ToString())));
//                 sb1.Append(sBuf);
//             }
//             textBox1.Text = sb1.ToString();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str1 = textBox1.Text + textBox3.Text;
            textBox2.Text = MD5Hash(Convert.ToBase64String(StrToByteArray(str1)));
        }
    }
}
