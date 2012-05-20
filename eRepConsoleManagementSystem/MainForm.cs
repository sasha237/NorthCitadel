using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Engine;
using System.Globalization;
using NerZul.Core.Network;

namespace eRepConsoleManagementSystem
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
//#if PUBLIC_BUILD
            //Partybutton.Visible = false;
            //VoteTopicButton.Visible = false;
            //Subscribebutton.Visible = false;
            //Commentbutton.Visible = false;
//#endif

        }

        private void FillGlobals()
        {
            Engine.Globals.BotConfig.AntiGateKey = (AutocaptchacheckBox.Checked)?AutocaptchatextBox.Text:"";
            Engine.Globals.BotConfig.precaptchaBufferSize = Convert.ToInt32(PreCaptchatextBox.Text);
            Engine.Globals.BotConfig.useTOR = (TORcheckBox.Checked);

            Engine.Globals.addWhere = AddWheretextBox.Text;
        }

        private void RegBotsButton_Click(object sender, EventArgs e)
        {
            FillGlobals();

            RegForm pForm = new RegForm();
            pForm.ShowDialog();
        }

        private void ActivateBotsButton_Click(object sender, EventArgs e)
        {
            FillGlobals();

            ActivateMailForm pForm = new ActivateMailForm();
            pForm.ShowDialog();
        }

        private void DailyButton_Click(object sender, EventArgs e)
        {
            FillGlobals();

            DailyForm pForm = new DailyForm();
            pForm.ShowDialog();
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            FillGlobals();

            ImportForm pForm = new ImportForm();
            pForm.ShowDialog();
        }

        private void WarButton_Click(object sender, EventArgs e)
        {
            FillGlobals();

            FightForm pForm = new FightForm();
            pForm.ShowDialog();
        }

        private void VoteTopicButton_Click(object sender, EventArgs e)
        {
            FillGlobals();

            VoteForm pForm = new VoteForm();
            pForm.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(RegBotsButton, "Вызов процедуры регистрации в базу данных");
            toolTip1.SetToolTip(ActivateBotsButton, "Активация почты зарегистрированных ботов");
            toolTip1.SetToolTip(DailyButton, "Процедура ежедневной работы бототы");
            toolTip1.SetToolTip(ImportButton, "Импорт бототы из предыдущих версий, пока не реализовано");
            toolTip1.SetToolTip(WarButton, "Военный модуль, пока не реализовано");
            toolTip1.SetToolTip(VoteTopicButton, "Голосование за статью, пока не реализовано");
			AutocaptchacheckBox.Checked = Globals.BotConfig.AntiGateKey.Length > 0;
            AutocaptchatextBox.Text = Globals.BotConfig.AntiGateKey;
			AutocaptchatextBox.Enabled = AutocaptchacheckBox.Checked;
            PreCaptchatextBox.Text = Globals.BotConfig.precaptchaBufferSize.ToString();
			TORcheckBox.Checked = Globals.BotConfig.useTOR;
			LocaleCombo.SelectedIndex = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "en" ? 1 : 0;
			Timer t = new Timer();
			t.Interval = 1;
			t.Tick += new EventHandler(t_Tick);
			t.Enabled = true;
        }

		void t_Tick(object sender, EventArgs e)
		{
			((Timer)sender).Dispose();
			this.Activate();
		}

        private void Feedbutton_Click(object sender, EventArgs e)
        {
            FillGlobals();

            FeedForm pForm = new FeedForm();
            pForm.ShowDialog();
        }

        private void AutocaptchacheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AutocaptchatextBox.Enabled = AutocaptchacheckBox.Checked;
        }

        private void Questbutton_Click(object sender, EventArgs e)
        {
            FillGlobals();

            Quest_Form pForm = new Quest_Form();
            pForm.ShowDialog();
        }

        private void Commentbutton_Click(object sender, EventArgs e)
        {
            FillGlobals();

            Comment_Form pForm = new Comment_Form();
            pForm.ShowDialog();
        }

        private void Subscribebutton_Click(object sender, EventArgs e)
        {
            FillGlobals();

            Subscribe_Form pForm = new Subscribe_Form();
            pForm.ShowDialog();
        }

        private void Moneybutton_Click(object sender, EventArgs e)
        {
            FillGlobals();

            GoldForm pForm = new GoldForm();
            pForm.ShowDialog();
        }

		private void LocaleCombo_SelectedIndexChanged(object sender, EventArgs e) {
			string locale = LocaleCombo.SelectedIndex == 0 ? "ru-RU" : "en-US";
			CultureInfo ci = CultureInfo.GetCultureInfo(locale);
			if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName != ci.TwoLetterISOLanguageName) {
				System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
				this.Controls.Clear();
				InitializeComponent();
				MainForm_Load(this, EventArgs.Empty);
//#if PUBLIC_BUILD
//                 Partybutton.Visible = false;
//                 VoteTopicButton.Visible = false;
//                 Subscribebutton.Visible = false;
//                 Commentbutton.Visible = false;
//                 RegBotsButton.Visible = false;
//#endif
			}
		}

        private void Flybutton_Click(object sender, EventArgs e)
        {
            FillGlobals();

            FlyForm pForm = new FlyForm();
            pForm.ShowDialog();
        }

        private void Partybutton_Click(object sender, EventArgs e)
        {
            FillGlobals();

            PartyForm pForm = new PartyForm();
            pForm.ShowDialog();
        }

        private void Reportbutton_Click(object sender, EventArgs e)
        {
            FillGlobals();

            ReportForm pForm = new ReportForm();
            pForm.ShowDialog();
        }

        private void PreCaptchaTestbutton_Click(object sender, EventArgs e)
        {
            PreCaptcha.Init(AutocaptchatextBox.Text, Convert.ToInt32(PreCaptchatextBox.Text), Globals.BotConfig.bBeep);
        }
    }
}
