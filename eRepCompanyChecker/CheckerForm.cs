using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace eRepCompanyChecker
{
    public partial class CheckerForm : Form
    {
        public CheckerForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int iNum;
            string sLogin = LogintextBox.Text;
            string sPassword = PasswordtextBox.Text;
            string sCompanyNum = CompanyNumtextBox.Text;
            string sBotsNum = BotNumtextBox.Text;
            string sSec = SectextBox.Text;
            if (sLogin.Length == 0 ||
                sPassword.Length == 0 ||
                sCompanyNum.Length == 0 ||
                sSec.Length == 0 ||
                sBotsNum.Length == 0 || !int.TryParse(sBotsNum, out iNum))
            {
                MessageBox.Show("Что-то не заполнено");
                return;
            }
            string[] args = new string[] { "companycheck", sLogin, sPassword, sCompanyNum, sBotsNum, sSec, FireLastcheckBox.Checked.ToString() };
            if(Globals.webCitadel.SendLogInfo(args, 0))
                CompanyChecker.CompanyCheck(args);
        }
    }
}
