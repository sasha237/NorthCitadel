using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using MySql.Data.MySqlClient;
using PalBot.Core;

namespace PalBot {
	public partial class MainForm : Form, ICaptchaProvider {
		public static MainForm Instance;

		public int CurrentBot = 0;
		public int TotalBots = 1;
		private int _botProgress;
		private int _botTotal;
		private string _botStatus;

		private System.IO.StreamWriter fs = null;

		public MainForm() {
			InitializeComponent();
			Instance = this;
		}

		private void MainForm_Load(object sender, EventArgs e) {
			new Thread(new ThreadStart(AddCaptcha)).Start();
			new Thread(new ThreadStart(Worker)).Start();
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
			if (fs != null) {
				fs.Close();
			}
		}

		#region Обработка капчи
		private List<ResolvedCaptcha> _captchaBuffer = new List<ResolvedCaptcha>();
		private Captcha _currentCaptcha = null;

		public ResolvedCaptcha GetResolvedCaptcha() {
			if (BotConfig.Get("skipCaptcha") != null) {
				throw new Exception("отхватил капчу!");
			}
			//ждать пока оператор не введет капчу
			while (true) {
				lock (_captchaBuffer) {
					if (_captchaBuffer.Count > 0) {
						ResolvedCaptcha c = _captchaBuffer[0];
						_captchaBuffer.RemoveAt(0);
						BeginInvoke(new EventHandler(UpdateCaptchaCount));
						return c;
					}
				}
				Invoke(new EventHandler(RedBg));
				Thread.Sleep(200);
			}
		}

		private void CaptchaText_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter) {
				if (_currentCaptcha != null) {
					lock (_captchaBuffer) {
						_captchaBuffer.Add(new ResolvedCaptcha(_currentCaptcha.ChallengeID, CaptchaText.Text));
						BeginInvoke(new EventHandler(UpdateCaptchaCount));
					}
				}
				CaptchaText.Text = "";
				CaptchaImage.Image = null;
				new Thread(new ThreadStart(AddCaptcha)).Start();
			}
			if (e.KeyCode == Keys.Escape) {
				CaptchaText.Text = "";
				CaptchaImage.Image = null;
				new Thread(new ThreadStart(AddCaptcha)).Start();
			}
		}

		private void RedBg(object o, EventArgs e) {
			PaneCaptcha.BackColor = Color.FromArgb(200, 150, 150);
		}

		private void AddCaptcha() {
			_currentCaptcha = new Captcha("http://www.erepublik.com/en");
			BeginInvoke(new EventHandler(ShowCaptcha));
		}

		private void ShowCaptcha(object o, EventArgs e) {
			var stream = new System.IO.MemoryStream(_currentCaptcha.Image);
			var img = System.Drawing.Image.FromStream(stream);
			CaptchaImage.Image = img;
			CaptchaImage.Width = img.Width;
			CaptchaImage.Height = img.Height;
			PaneCaptcha.Height = CaptchaImage.Height + 31;
			CaptchaText.Top = CaptchaImage.Height;
			CaptchaText.Width = CaptchaImage.Width;
			CaptchaText.Height = 30;
			CaptchaText.Text = "";
			CaptchaText.Focus();
			PaneCaptcha.BackColor = SystemColors.Control;
		}

		private void UpdateCaptchaCount(object o, EventArgs e) {
			CaptchasCount.Text = "" + _captchaBuffer.Count;
		}
		#endregion

		#region Вывод прогресса
		public static void SetProgress(int progress) {
			Instance._botProgress = progress;
			Instance.BeginInvoke(new EventHandler(Instance.SetProgress));
		}

		public static void SetStatus(string status) {
			Instance._botStatus = status;
			Instance.Log(status);
			Instance.BeginInvoke(new EventHandler(Instance.SetStatus));
		}

		private void SetProgress(object o, EventArgs e) {
			if (CurrentBot > TotalBots - 1) {
				TotalBots = CurrentBot + 1;
			}
			if (_botProgress > _botTotal - 1) {
				_botTotal = _botProgress + 1;
			}
			PBTotal.Value = (int)((double)PBTotal.Maximum / TotalBots * (CurrentBot + (double)_botProgress / _botTotal));
			PBBot.Value = PBBot.Maximum * _botProgress / _botTotal;
		}

		private void SetStatus(object o, EventArgs e) {
			LbStatus.Text = _botStatus;
		}

		private void Log(string s) {
			lock (this) {
				if (fs == null) {
					fs = new System.IO.StreamWriter("log.txt", false);
				}
				fs.WriteLine(DateTime.Now.ToString("dd.MM HH:mm:ss ")+s);
			}
		}

		#endregion

		#region Братья

		public static MySqlConnection Connection;
		public static MySqlCommand Command;

		public void Worker() {
			string strCon = BotConfig.Get("ConnectionString");
			Connection = new MySqlConnection(strCon);
			Connection.Open();
			Command = Connection.CreateCommand();

			string where = "";
			string group = BotConfig.Get("group");
			if (!string.IsNullOrEmpty(group)) {
				where += " AND `group`='" + group + "'";
			}

			Command.CommandText = "SELECT * FROM bots WHERE 1 " + where + " ORDER BY RAND()";
			DataTable brothers = new DataTable();
			using(MySqlDataAdapter ad = new MySqlDataAdapter(Command)) {
				ad.Fill(brothers);
			}
			Random rnd = new Random();
			TotalBots = brothers.Rows.Count;
			for (CurrentBot = 0; CurrentBot < brothers.Rows.Count; CurrentBot ++) {
				PBot bot = new PBot(brothers.Rows[CurrentBot]);
				_botTotal = bot.MaxProgress;
				bot.Worker();
				if (bot.HasTasks) {
					Thread.Sleep(rnd.Next(2000));
				}
			}
			MessageBox.Show("Все сделано, босс!");
			Invoke(new EventHandler(delegate(object o, EventArgs e) {
				Application.Exit();
			}));
		}

		#endregion
	}
}
