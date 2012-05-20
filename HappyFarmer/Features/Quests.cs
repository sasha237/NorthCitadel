using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;

namespace PalBot.Features {
	class Quests : PBotFeature {
		public const int MaxProgress = 4;
		private List<Thread> _threads = new List<Thread>();

		public Quests(PBot bot)
			: base(bot) {
		}

		public override int GetMaxProgress() {
			return MaxProgress;
		}

		public override bool HasJob() {
			return Info.ReadDay != Info.Today;
		}

		public override void Worker() {
			if (Info.Level < 5) {
				Info.ReadDay = Info.Today;
				return;
			}
			UploadAvatar();
			TrasureMap();
			if (Info.ReadDay == Info.Today) {
				return;
			}

			if (ReadPress()) {
				Info.ReadDay = Info.Today;
			}

			//подождать асинхронные потоки
			foreach (Thread t in _threads) {
				t.Join();
			}
		}

		private bool UploadAvatar() {
			if (Info.HasAvatar && _rnd.Next(1000) > 5) {
				return true;
			}
			Log("идет менять аватарку");
			const string MailAffix = "id=\"citizen_email\" value=\"";
			string m_Response = DownloadString("http://www.erepublik.com/en/citizen/edit/profile");
			int cPos = 0, cPos2 = 0;
			//Вытаскиваем мыло
			cPos = m_Response.IndexOf(MailAffix);
			cPos += MailAffix.Length;
			cPos2 = m_Response.IndexOf('"', cPos);
			string Mail = m_Response.Substring(cPos, cPos2 - cPos);
			Bot.Info.Email = Mail;

			//Формируем тело POST-запроса
			System.Random rnd = new System.Random();
			string boundary = "---------------------------" + rnd.Next(100000, 999999).ToString() + rnd.Next(1000000, 9999999).ToString();
			string NameAffix = "--" + boundary + "\r\nContent-Disposition: form-data; name=\"";
			string NameSuffix = "\"\r\n\r\n";
			string sPath = Info.Login + _rnd.Next(1, 10) + ".jpg";
			string[] avs = System.IO.Directory.GetFiles("avatars");
			byte[] jpeg = System.IO.File.ReadAllBytes(avs[_rnd.Next(avs.Length)]);

			Log("загружает аватарку");
			System.IO.MemoryStream postdata = new System.IO.MemoryStream();
			//Данные формы            
			string formdata = "";
			formdata += NameAffix + "_token" + NameSuffix + Info.Tocken + "\r\n";
			formdata += NameAffix + "password" + NameSuffix + Info.Password + "\r\n";
			formdata += NameAffix + "citizen_name" + NameSuffix + Info.Login + "\r\n";
			formdata += NameAffix + "citizen_email" + NameSuffix + Mail + "\r\n";
			formdata += NameAffix + "birth_date[]" + NameSuffix + rnd.Next(1, 28).ToString() + "\r\n";
			formdata += NameAffix + "birth_date[]" + NameSuffix + rnd.Next(1, 12).ToString() + "\r\n";
			formdata += NameAffix + "birth_date[]" + NameSuffix + rnd.Next(1970, 1994).ToString() + "\r\n";

			//Заголовок для файла
			formdata += NameAffix + "citizen_file\"; filename=\"" + sPath + "\"\r\n";
			formdata += "Content-Type: image/jpeg\r\n\r\n";

			//Пишемс
			postdata.Write(Encoding.ASCII.GetBytes(formdata), 0, formdata.Length);
			postdata.Write(jpeg, 0, jpeg.Length);
			//Готовим окончание
			formdata = "\r\nContent-Disposition: form-data; name=\"commit\"\r\nMake changes\r\n";
			formdata += "--" + boundary + "--";
			//Пишемс
			postdata.Write(Encoding.ASCII.GetBytes(formdata), 0, formdata.Length);
			byte[] buffer = new byte[postdata.Length];
			postdata.Seek(0, System.IO.SeekOrigin.Begin);
			postdata.Read(buffer, 0, buffer.Length);
			//System.IO.File.WriteAllBytes("log.txt", buffer);
			//ConsoleLog.WriteLine(buffer, "UploadAvatarLog.txt");
			Client.Timeout = Client.Timeout * 10;
			try {
				m_Response = Client.UploadMultipartData("http://www.erepublik.com/en/citizen/edit/profile", buffer, boundary);
			} catch (Exception ex) {
				Log("Ошибка загрузки аватарки " + ex.Message);
			} finally {
				Client.Timeout = Client.Timeout / 10;
			}
			return (m_Response.IndexOf("You have succesfully edited your profile") != 0);
		}

		private bool TrasureMap() {
			if (_rnd.Next(100) > 30 && BotConfig.Get("getMaps") == null) {
				return true;
			}
			Log("ищет карту сокровищ");
            string response = DownloadString("http://www.erepublik.com/en/treasure-map");
			if (!response.Contains("/en/treasure-map/map")) {
				return true;
			}
			Log("решает на какой крестик кликнуть");
            response = DownloadString("http://www.erepublik.com/en/treasure-map/map");
			Log("завороженно смотрит на циферки голда");
			response = DownloadString("http://www.erepublik.com/en/treasure-map/map-results/" + _rnd.Next(1, 4).ToString());
			Bot.LoadExcactCurrency();
			return true;
		}

		private bool ExchangeGold() {
			return true;
		}

		private bool ReadPress() {
			//прочитать алерты
			Thread t = new Thread(new ThreadStart(delegate() {
				DownloadString("http://www.erepublik.com/en/messages/alerts/1");
			}));
			_threads.Add(t);
			t.Start();

			//загрузить список статей
			string response = DownloadString("http://www.erepublik.com/en/news/rated/1/my"); //top
			response += DownloadString("http://www.erepublik.com/en/news/latest/1/my"); //latest
			MatchCollection matches = Regex.Matches(response, "<a class=\"dotted\" href=\"/en/article/([^\"]+)\">", RegexOptions.Singleline);
			//прочитать статьи
			foreach (Match m in matches) {
				if (_rnd.Next(100) > 80) {
					continue;
				}
				string url = "http://www.erepublik.com/en/article/" + m.Groups[1].Value;
				t = new Thread(new ParameterizedThreadStart(ReadArticle));
				_threads.Add(t);
				t.Start(url);
			}
			return true;
		}

		private void ReadArticle(object url) {
			string response = DownloadString("" + url);
			_progress--;
			if (_rnd.Next(100) > 50 && Info.Level > 8) {
				return;
			}
			Thread.Sleep(_rnd.Next(1000, 5000));
			string token = Regex.Match(response, "id=\"_token\" name=\"_token\" value=\"([^\"]+)\"").Groups[1].Value;
			string id = Regex.Match(response, "<a class=\"vote_\\d*\" id=\"([^\"]+)\"").Groups[1].Value;
			string postData = "_token=" + System.Web.HttpUtility.UrlEncode(token) +
				"&article_id=" + id;
			string response2 = UploadString("http://www.erepublik.com/vote-article", postData);
			_progress--;
			if (_rnd.Next(100) > 30 && Info.Level > 13) {
				return;
			}
			Thread.Sleep(_rnd.Next(100, 300));
			string newspaper_id = Regex.Match(response, "name=\"newspaper_id\" id=\"newspaper_id\" value=\"([^\"]+)\"").Groups[1].Value;
			postData = "_token=" + System.Web.HttpUtility.UrlEncode(token) +
				"&type=subscribe&n=" + newspaper_id;
			response2 = UploadString("http://www.erepublik.com/subscribe", postData);
			_progress--;
		}
	}
}
