using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PalBot.Core;
using System.Text.RegularExpressions;

namespace PalBot.Features {
	class Login : PBotFeature {

		public Login(PBot bot) : base(bot) { }

		public override int GetMaxProgress() {
			return 2;
		}

		public override bool HasJob() {
			return false;
		}

		public override void Worker() {
			bool ok = false;
			//логинимся
			for (int i = 0; i < 3 && !ok; i++) {
				ok = DoLogin();
			}
			if (!ok) {
				throw new Exception("Никак не удалось залогиниться");
			}
		}

		/// <summary>
		/// Выполняет процедуру авторизации в eRepublik. В это же время получает токены сессии.
		/// Для авторизации используются ранее запомненные параметры
		/// </summary>
		/// <returns></returns>
		public bool DoLogin() {
			Log("получает TOR IP");
			Client.Proxy = "127.0.0.1:8118";
			Client.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)";

			if (!TorClient.GetNewIP()) {
				return false;
			}
			if (Info.Banned || Info.Disabled || !Info.Activated) {
				Log("сокрушается по поводу бана");
				return false;
			}
			Log("зашел в игру");
			string mc_TokenScanString = "id=\"_token\" name=\"_token\" value=\"";
			string response = DownloadString("http://www.erepublik.com/en");
			response = response.Remove(0, response.IndexOf(mc_TokenScanString) + mc_TokenScanString.Length);
			Info.Tocken = response.Substring(0, response.IndexOf("\""));
			Log("логинится");
			string PostData = "_token=" + Info.Tocken +
				"&citizen_name=" + System.Web.HttpUtility.UrlEncode(Info.Login) +
				"&citizen_password=" + System.Web.HttpUtility.UrlEncode(Info.Password);
			response = UploadString("http://www.erepublik.com/en/login", PostData);
			bool bOK = (response.IndexOf("logout") != -1);
			if (!bOK) {
				if (response.Contains("Internal Server Error"))
					Log("500 - Internal Server Error");
				if (response.Contains("infringement")) {
					Info.Banned = true;
					Log("с ужасом смотрит на страницу бана");
				}
				if (response.Contains("Citizen name")) {
					Info.Disabled = true;
					Log("с ужасом смотрит на страницу бана");
				}

				if (response.Contains("Email validation")) {
					Info.Activated = false;
					Log("забыл активировать почту");
					string sBuf = response;
					string sTokenPref = "name=\"_token\" value=\"";
					string sEmailPref = "id=\"citizen_email\" value=\"";
					sBuf = sBuf.Remove(0, sBuf.IndexOf(sTokenPref) + sTokenPref.Length);
					sBuf = sBuf.Remove(sBuf.IndexOf("\""));
					string sToken = sBuf;
					sBuf = response;
					sBuf = sBuf.Remove(0, sBuf.IndexOf(sEmailPref) + sEmailPref.Length);
					sBuf = sBuf.Remove(sBuf.IndexOf("\""));
					string sEmail = sBuf;
					PostData = "_token=" + sToken + "&citizen_email=" + System.Web.HttpUtility.UrlEncode(sEmail);
					response = UploadString("http://www.erepublik.com/en/register-validate-new-account", PostData);
				}
				if (response.Contains("dead")) {
					Client.Referer = "http://www.erepublik.com/en";
					PostData = "_token=" + Info.Tocken;
					UploadString("http://www.erepublik.com/en/dead_revive", PostData);
					Log("ERROR: Dead!");
				}
				return false;
			}
			string _tocken = response.Remove(0, response.IndexOf(mc_TokenScanString) + mc_TokenScanString.Length);
			Info.Tocken = _tocken.Substring(0, _tocken.IndexOf("\""));
			Log("залогинился");
			return FillBasicBotInfo(response);
		}

		internal bool FillBasicBotInfo(string response) {
			try {
				string s = Regex.Match(response, @"Experience level: <strong>(\d+)<").Groups[1].Value;
				Info.Level = int.Parse(s);
				s = Regex.Match(response, @"Experience points: <strong>\s*(\d+)<").Groups[1].Value;
				Info.Experience = int.Parse(s);
				s = Regex.Match(response, @"wellness_small.png.*?<span>([0-9\.]{1,9})", RegexOptions.Singleline).Groups[1].Value;
				Info.Wellness = double.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
				s = Regex.Match(response, "erepublik.com/en/market/job/(\\w+)\"").Groups[1].Value;
				int.TryParse(s, out Info.Country);
				Info.HasAvatar = response.Contains("http://static.erepublik.com/uploads/avatars/Citizens");
				s = Regex.Match(response, "/en/citizen/profile/(\\w+)\">").Groups[1].Value;
				Info.CitizenID = int.Parse(s);
				s = Regex.Match(response, "side_bar_gold_account_value\">(\\w+)<").Groups[1].Value;
				Info.Gold = double.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
				s = Regex.Match(response, "side_bar_natural_account_value\">(\\w+)<").Groups[1].Value;
				Info.Currency = double.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
				Info.CurrencyCode = Regex.Match(response, "<div class=\"item sidebarCurrency\">.*?alt=\"(\\w+)\"", RegexOptions.Singleline).Groups[1].Value;
				Info.FoodKey = Regex.Match(response, "id=\"DailyConsumtion\">\\s*<input type=\"hidden\" value=\"([^\"]+)\"", RegexOptions.Singleline).Groups[1].Value;
				Info.Today = int.Parse(Regex.Match(response, "<span class=\"eday\">Day <strong>([\\d,]+)</strong> of the New World").Groups[1].Value.Replace(",", ""));
			} catch (Exception e) {
				Log(e.ToString());
				return false;
			}
			return true;
		}

		internal void LoadInventory() {
			Log("Загрузка инвентаря");
			string inventoryMain = DownloadString("http://economy.erepublik.com/en/inventory");
			FillBasicBotInfo(inventoryMain);
			string key = "offersJSON = ";
			string inventory = inventoryMain;
			inventoryMain = inventoryMain.Substring(inventoryMain.IndexOf(key) + key.Length);
			inventoryMain = inventoryMain.Remove(inventoryMain.IndexOf(";"));
			MatchCollection matches = Regex.Matches(inventoryMain,
				"\"img\":\"http:\\\\/\\\\/www.erepublik.com\\\\/images\\\\/icons\\\\/industry\\\\/(\\d)+\\\\/q(\\d)_");
			Info.Inventory = new List<InventoryItem>();
			foreach (Match m in matches) {
				InventoryItem item = new InventoryItem();
				item.Type = int.Parse(m.Groups[1].Value);
				item.Q = int.Parse(m.Groups[2].Value);
				Info.Inventory.Add(item);
			}
		}

		internal void LoadExcactCurrency() {
			Log("Загрузка данных по счетам");
			string accounts = DownloadString("http://economy.erepublik.com/en/accounts/" + Info.CitizenID);
			FillBasicBotInfo(accounts);

			string s = Regex.Match(accounts,
				"<strong>" + Info.CurrencyCode + "</strong>\\s*</div>\\s*<div class=\"push_right\">\\s*([0-9\\.]+)\\s*</div>",
				RegexOptions.Singleline).Groups[1].Value;
			Info.CurrencyExact = Double.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
			Info.Currency = Info.CurrencyExact;

			s = Regex.Match(accounts,
				"<span>Get Gold</span>\\s*</a>\\s*<div class=\"push_right\">\\s*([0-9\\.]+)\\s*</div>",
				RegexOptions.Singleline).Groups[1].Value;
			Info.GoldExact = Double.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
			Info.Gold = Info.GoldExact;
		}
	}
}
