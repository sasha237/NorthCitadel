using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Text.RegularExpressions;
using PalBot.Features;
using PalBot.Core;

namespace PalBot {

	class PBot {
		public readonly HttpClient Client = new HttpClient();
		public readonly PBotInfo Info = new PBotInfo();

		private DataRow _dbData;
		private Random _rnd = new Random();
		private long _jsonCallback = 0;
		public bool HasTasks = false;

		private Login _login = null;
		private List<PBotFeature> _features = new List<PBotFeature>();

		public int MaxProgress {
			get {
				int p = _login.GetMaxProgress();
				foreach (PBotFeature f in _features) {
					p += f.GetMaxProgress();
				}
				return p;
			}
		}

		public int Progress {
			get {
				int p = _login.Progress;
				foreach (PBotFeature f in _features) {
					p += f.Progress;
				}
				return p;
			}
		}


		public PBot(DataRow dbData) {
			_dbData = dbData;
			LoadDBData();
			_login = new Login(this);
			_features.Add(new Devour(this));
			_features.Add(new Daily(this));
			_features.Add(new Quests(this));
		}

		public void Worker() {
			MainForm.SetProgress(0);
			//проверка есть ли у бота задания
			HasTasks = false;
			foreach (PBotFeature f in _features) {
				if (f.HasJob()) {
					HasTasks = true;
					break;
				}
			}
			if (!HasTasks) {
				MainForm.SetStatus(Info.Login + " уже все сделал");
				return;
			}
			try {
				//фичи запускаются асинхронно, т.к. они не мешают друг другу работать.
				_login.Worker();

				List<Thread> trs = new List<Thread>();
				foreach (PBotFeature f in _features) {
					Thread t = new Thread(new ThreadStart(f.Worker));
					trs.Add(t);
					t.Start();
					Thread.Sleep(_rnd.Next(2000));
				}

				foreach (Thread tt in trs) {
					tt.Join();
				}
			} catch (Exception ex) {
				MainForm.SetStatus(Info.Login + " cхлопотал ошибку :" + ex);
			} finally {
				SaveDBData();
			}
		}

		private int SafeParseInt(object value) {
			if (value == null || value is DBNull) {
				return 0;
			}
			int v = 0;
			try {
				v = Convert.ToInt32(value);
			} catch { }
			return v;
		}

		private void LoadDBData() {
			Info.Login = "" + _dbData["login"];
			Info.Password = "" + _dbData["password"];
			Info.WorkDay = SafeParseInt(_dbData["last_day_work"]);
			Info.TrainDay = SafeParseInt(_dbData["last_day_train"]);
			Info.ReadDay = SafeParseInt(_dbData["last_day_relax"]);
			Info.WaitingCaptcha = SafeParseInt(_dbData["disabled"]) > 0;
			Info.Banned = SafeParseInt(_dbData["banned"]) == 1;
			Info.Disabled = SafeParseInt(_dbData["banned"]) > 1;

			//авто день
			DateTime _erepStart = new DateTime(2007, 11, 20);
			int diff = DateTime.Now.IsDaylightSavingTime() ? -7 : -8;
			Info.Today = (int)DateTime.Now.ToUniversalTime().AddHours(diff).Subtract(_erepStart).TotalDays;
		}

		private void SaveDBData() {
			Dictionary<string, object> data = new Dictionary<string, object>();
			data.Add("last_day_work", Info.WorkDay);
			data.Add("last_day_train", Info.TrainDay);
			data.Add("last_day_relax", Info.ReadDay);
			data.Add("disabled", Info.WaitingCaptcha ? 1 : 0);
			data.Add("banned", Info.Banned ? 1 : Info.Disabled ? 2 : 0);
			data.Add("citizen_id", Info.CitizenID);
			data.Add("wellness", Info.Wellness);
			data.Add("experience", Info.Experience);
			data.Add("activated", Info.Activated ? 1 : 0);
			data.Add("country", Info.Country);
			data.Add("gold", Info.Gold.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.Add("nat_occur", Info.Currency.ToString(System.Globalization.CultureInfo.InvariantCulture));
			string sql = "";
			foreach (string key in data.Keys) {
				string value = ""+data[key];
				sql += ", `" + key + "` = '" + value.Replace("\"", "\\\"") + "'";
			}
			MainForm.Command.CommandText = "UPDATE bots SET " + sql.Substring(1) + " WHERE id=" + _dbData["id"];
			MainForm.Command.ExecuteNonQuery();
		}

		/*
		public int Level = 0; //уровень
		public int Experience = 0; //экспа
		public double Wellness = 0; //велл
		public double Gold = 0; //голд, обчно обрезано до целых
		public double GoldExact = 0; //точное число глда, актуально только после вызова LoadExcactCurrency()
		public double Currency = 0; //нац валюта, обчно обрезано до целых
		public double CurrencyExact = 0;  //точное число нац валюты, актуально только после вызова LoadExcactCurrency()
		public string CurrencyCode = ""; //код нац валюты
		public int CitizenID = 0; //ид
		public bool HasAvatar = false; //имеет авку
		public int Country = 0; //страна
		public string Email = null; //мыло
		public bool Activated = true; //активировал почту?
		*/

		public void LoadInventory() {
			_login.LoadInventory();
		}

		public void LoadExcactCurrency() {
			_login.LoadExcactCurrency();
		}

		public void UpdateBasicInfo(string response) {
			_login.FillBasicBotInfo(response);
		}

		public void UpdateProgress() {
			MainForm.SetProgress(Progress);
		}

		public string JSONCallback {
			get {
				if (_jsonCallback == 0) {
					string sR = "128";
					for (int i = 3; i < 13; i++) sR += _rnd.Next(1, 9);
					_jsonCallback = long.Parse(sR);
				}
				_jsonCallback++;
				return "jsonp" + _jsonCallback;
			}
		}

	}
}
