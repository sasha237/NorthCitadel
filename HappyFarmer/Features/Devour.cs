using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;

namespace PalBot.Features {
	class Devour : PBotFeature {
		public const int MaxProgress = 3;

		public Devour(PBot bot) : base(bot) {
		}

		public override int GetMaxProgress() {
			return MaxProgress;
		}

		public override bool HasJob() {
			//хавка не критичный процесс
			return false;
		}

		public override void Worker() {
			Log("зашел в столовку");
			Work();
			_progress = MaxProgress;
			Bot.UpdateProgress();
		}

		private void Work() {
			for (var pass = 0; pass < 3; pass++) {
				//если здоров, то пропустить
				if (Info.Wellness >= 92) {
					Log("уже недавно кушал, уходит");
					return;
				}
				//подгрузить инвентарь
				Bot.LoadInventory();
				if (Info.Wellness >= 92) {
					Log("наелся до отвала");
					return;
				}
				//можем ли вылечиться запасом булок?
				int healAmount = 100 - (int)Info.Wellness;
				int invWellness = 0;
				int invCount = 0;
				foreach (InventoryItem item in Info.Inventory) {
					if (item.Type == 1) {
						invWellness += item.Q * 2;
						if (invWellness > healAmount) {
							break;
						} else {
							invCount++;
						}
					}
				}
				if (invCount > 0) {
					//загрызть нужное колво булок из инвентаря.
					Client.Headers.Add("X-Requested-With", "XMLHttpRequest");
					List<Thread> eats = new List<Thread>();
					for (int i = 0; i < invCount; i++) {
						Thread t = new Thread(new ThreadStart(AcyncEat));
						eats.Add(t);
						t.Start();
						Thread.Sleep(100);
					}
					Log("жрет " + invCount + " пирожков");
					//подождать пока булки грызутся
					foreach (Thread t in eats) {
						t.Join();
					}
					Client.Headers.Remove("X-Requested-With");
				} else {
					//Закупка еды
					Log("стоит в очереди за едой");
					string customFood = BotConfig.Get("food" + Info.Country);
					string url = "http://economy.erepublik.com/en/market/" + Info.Country + "/1/" + _rnd.Next(0, 19) + "/0/0/0/citizen/0/price_asc/1";
					if (!string.IsNullOrEmpty(customFood)) {
						url = "http://economy.erepublik.com/en/market/offer/" + customFood;
					}
					string market = DownloadString(url);
					string csrfToken = Regex.Match(market, "<input type=\"hidden\" name=\"buyMarketOffer\\[_csrf_token\\]\" value=\"([^\"]+)\"").Groups[1].Value;
					int toBuy = 19 - Info.Inventory.Count;
					MatchCollection matches = Regex.Matches(market,
						"<td class=\"m_stock\">\\s*(\\d+)\\s*</td>\\s*<td class=\"m_price stprice\">\\s*<strong>(.*?)</strong></sup>.*?id=\"amount_(\\d+)\"",
						RegexOptions.Singleline);
					foreach (Match m in matches) {
						try {
							int stock = int.Parse(m.Groups[1].Value);
							string sprice = m.Groups[2].Value;
							int id = int.Parse(m.Groups[3].Value);
							sprice = Regex.Replace(sprice, "[^0-9\\.]", "");
							double price = double.Parse(sprice, System.Globalization.CultureInfo.InvariantCulture);
							int afford = (int)Math.Min(Info.Currency / price, toBuy);
							if (stock < afford) continue;
							Log("покупает " + afford + " пирожков");
							string PostData =
								System.Web.HttpUtility.UrlEncode("buyMarketOffer[amount]") + "=" + afford + "&" +
								System.Web.HttpUtility.UrlEncode("buyMarketOffer[_csrf_token]") + "=" + csrfToken + "&" +
								System.Web.HttpUtility.UrlEncode("buyMarketOffer[offerId]") + "=" + id;
							string resp = UploadString("http://economy.erepublik.com/en/market/" + Info.Country + "/1", PostData);
							break;
						} catch (Exception ex) {
							ex.ToString();
						}
					}
				}
			}
		}

		private void AcyncEat() {
			string resp = UploadString("http://www.erepublik.com/eat?format=json&_token=" + Info.FoodKey + "&jsoncallback=" + Bot.JSONCallback, "");
			_progress--;
		}
	}
}
