using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Engine;
using NerZul;
using Fuel;


namespace eRepConsoleManagementSystem
{
    public partial class ActivateMailForm : Form
    {
        public ActivateMailForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sLogin = LogintextBox.Text;
            string sPassword = PasswordtextBox.Text;
            string sServer = ServertextBox.Text;
            if (sLogin.Length==0)
            {
                MessageBox.Show("Не задан логин!");
                return;
            }
            if (sPassword.Length == 0)
            {
                MessageBox.Show("Не задан пароль!");
                return;
            }

            if (sServer.Length == 0)
            {
                MessageBox.Show("Не задан сервер!");
                return;
            }

            string[] args = new string[] { "activatemail", sServer, sLogin, sPassword };
            ActivateMail.Worker(args);
        }

		private void ManualActivationButton_Click(object sender, EventArgs e) {
			Random rnd = new Random();
			foreach (string i in ActivationLinksTextBox.Text.Split('\n')) {
				string link = i.Replace("\r", "");
				try {
					ActivateMail.OpenLink(link);
				} catch (Exception ex) {
					NerZul.Core.Utils.ConsoleLog.WriteLine("Ошибка:" + ex.Message);
				}
				System.Threading.Thread.Sleep(rnd.Next(3000));
			}
			MessageBox.Show("Все ссылки обработаны!");
			DialogResult = DialogResult.OK;
		}
    }
}
