using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PalBot.Core;

namespace PalBot.Features {
	abstract class PBotFeature {
		protected readonly PBot Bot;
		protected readonly PBotInfo Info;
		public readonly HttpClient Client;
		protected int _progress;
		protected System.Random _rnd = new System.Random();

		public PBotFeature(PBot bot) {
			Bot = bot;
			Info = bot.Info;
			Client = bot.Client.Clone();
		}

		public int Progress { get { return Math.Min(GetMaxProgress(), _progress); } }

		public abstract int GetMaxProgress();
		public abstract void Worker();
		public abstract bool HasJob();

		protected void Log(string s) {
			MainForm.SetStatus(Info.Login + " " + s);
		}

		protected string DownloadString(string url) {
			string ret = Client.DownloadStringSafe(url);
			_progress++;
			Bot.UpdateProgress();
			if (ret == null) {
				Log("Неудача загрузки: GET " + url);
				return "";
			}
			return ret;
		}

		protected string UploadString(string url, string data) {
			string ret = Client.UploadStringSafe(url, data);
			_progress++;
			Bot.UpdateProgress();
			if (ret == null) {
				Log("Неудача загрузки: POST " + url + "\n" + data);
				return "";
			}
			return ret;
		}
	}
}
