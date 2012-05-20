using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace PalBot.Features {
	class Daily : PBotFeature {
		public const int MaxProgress = 7;
		private string _csrfToken = "";
		private string _csrfTokenWork = "";

		public Daily(PBot bot)
			: base(bot) {
		}

		public override int GetMaxProgress() {
			return MaxProgress;
		}

		public override bool HasJob() {
			return Info.WorkDay != Info.Today || Info.TrainDay != Info.Today;
		}

		public override void Worker() {
			bool w, t;
			if (_rnd.Next(10) < 6) {
				w = Work();
				Thread.Sleep(_rnd.Next(500));
				t = Train();
			} else {
				t = Train();
				Thread.Sleep(_rnd.Next(500));
				w = Work();
			}
			if (w) {
				Info.WorkDay = Info.Today;
			}
			if (t) {
				Info.TrainDay = Info.Today;
			}
			GetTehReward();
			_progress = MaxProgress;
			Bot.UpdateProgress();
		}

		private void GetTehReward() {
			Log("получает награду");
			Client.Referer = "http://www.erepublik.com/en";
			string response = DownloadString("http://www.erepublik.com/daily_tasks_reward");
			if (response.Contains("success")) {
				Log("получил медаль");
			} else if (response.Contains("error")) {
				Log("медаль не дали");
			} else {
				Log("Daily reward shit happened\n" + response);
			}
		}

		private bool Train() {
			if (Info.TrainDay == Info.Today) {
				return true;
			}
			try {
				Log("идет тренироваться");
				string sBuf = "\" ";
				string sFind = "[_csrf_token]\" value=\"";
				string response = DownloadString("http://economy.erepublik.com/en/train");
				if (response.Contains("View train results")) {
					return true;
				}
				int ipos = response.IndexOf(sFind);
				response = response.Substring(ipos + sFind.Length, response.Length - (ipos + sFind.Length));
				string sToken = response.Substring(0, response.IndexOf(sBuf));
				Log("качает силу");
				string PostData = System.Web.HttpUtility.UrlEncode("train[boosterId]") + "=10"
					+ "&" + System.Web.HttpUtility.UrlEncode("train[_csrf_token]") + "=" + sToken
					+ "&" + System.Web.HttpUtility.UrlEncode("train[skillId]") + "=1"
					+ "&" + System.Web.HttpUtility.UrlEncode("train[friend1]") + "=0"
					+ "&" + System.Web.HttpUtility.UrlEncode("train[friend2]") + "=0";
				response = UploadString("http://economy.erepublik.com/en/train", PostData);
				for (int kk = 0; kk < 5 && response.Contains("manual_challenge"); kk++ ) {
					if (BotConfig.Get("skipCaptcha") != null) {
						return false;
					}
					Log("читает капчу");
					var captcha = MainForm.Instance.GetResolvedCaptcha();
					ipos = response.IndexOf(sFind);
					response = response.Substring(ipos + sFind.Length, response.Length - (ipos + sFind.Length));
					sToken = response.Substring(0, response.IndexOf(sBuf));
					PostData = System.Web.HttpUtility.UrlEncode("captcha_form[_csrf_token]") + "=" + sToken + "&" +
						"recaptcha_challenge_field=" + captcha.ChallengeID + "&recaptcha_response_field=" + System.Web.HttpUtility.UrlEncode(captcha.CaptchaText) + "&commit=Continue";
					response = UploadString("http://economy.erepublik.com/en/time-management/captcha/train", PostData);
				}
				Bot.UpdateBasicInfo(response);
				if (!response.Contains("charTooltipTd") || !response.Contains("Train results")) {
					Log("Train error: \n" + response);
					return false;
				} else {
					return true;
				}
			} catch (System.Exception e) {
				Log("Train error: " + e);
			}
			return false;
		}

		private bool Work() {
			if (Info.WorkDay == Info.Today) {
				return true;
			}
			try {
				Log("идет на работу");
				string response = DownloadString("http://economy.erepublik.com/en/work");
				if (response.Contains("You have already worked today")) {
					Log("уже работал");
					return true;
				}
				bool resign = false;
				bool fired = false;
				if (response.Contains("You do not have a job")) {
					fired = true;
				} else {
					UpdateCsrfToken(response);
					resign = _rnd.Next(10) > 3;
				}
				if (resign) {
					fired = Resign();
				}
				if (fired) {
					fired = !AppyForJob();
				}
				if (!fired) {
					return DoWork();
				}
			} catch (Exception ex) {
				Log("Work error: " + ex);
			}
			return false;
		}

		private bool DoWork() {
			bool ok = false;
			for (int attempt = 0; attempt < 2 && !ok; attempt++) {
				Log("пашет на фирме");
				string PostData = System.Web.HttpUtility.UrlEncode("work[boosterId]") + "=1&"
					+ System.Web.HttpUtility.UrlEncode("work[_csrf_token]") + "=" + _csrfTokenWork
					+ "&" + System.Web.HttpUtility.UrlEncode("work[friend1]") + "=0"
					+ "&" + System.Web.HttpUtility.UrlEncode("work[friend2]") + "=0";
				string response = UploadString("http://economy.erepublik.com/en/work", PostData);
				for (int kk = 0; kk < 3 && response.Contains("manual_challenge"); kk++) {
					UpdateCsrfToken(response);
					if (BotConfig.Get("skipCaptcha") != null) {
						return false;
					}
					Log("читает на работе капчу");
					var captcha = MainForm.Instance.GetResolvedCaptcha();
					PostData = System.Web.HttpUtility.UrlEncode("captcha_form[_csrf_token]") + "=" + _csrfTokenWork + "&" +
						"recaptcha_challenge_field=" + captcha.ChallengeID + "&recaptcha_response_field=" + System.Web.HttpUtility.UrlEncode(captcha.CaptchaText) + "&commit=Continue";
					response = UploadString("http://economy.erepublik.com/en/time-management/captcha/work", PostData);
				}
				ok = response.Contains("Workday results") && !response.Contains("CSRF attack detected");
				Bot.UpdateBasicInfo(response);
				if (!ok) {
					Log("Не смог отработать");
				}
			}
			return ok;
		}

		private void UpdateCsrfToken(string response) {
			Match m = Regex.Match(response, "_csrf_token'\\); m\\.setAttribute\\('value', '([^']+)'\\)");
			if (m.Success) {
				_csrfToken = m.Groups[1].Value;
			}
			m = Regex.Match(response, "\\[_csrf_token\\]\" value=\"([^\"]+)\"");
			if (m.Success) {
				_csrfTokenWork = m.Groups[1].Value;
			}

		}

		private Dictionary<int, int> _jobBlackList = new Dictionary<int, int>();

		private bool AppyForJob() {
			Log("ищет подходящую вакансию");
			double qFactor = 0.04;
			//скачать список вакансий
			string response = DownloadString("http://economy.erepublik.com/en/market/job/" + Info.Country);
			UpdateCsrfToken(response);
			MatchCollection matches = Regex.Matches(response, 
				"<div class=\"stared (\\w+)\"></div>.*?<td class=\"jm_salary\">(.*?)</td>.*?href=\"/en/job/apply/(\\d+)\"", RegexOptions.Singleline);
			int jobId = 0;
			double maxSalary = 0;
			foreach (Match m in matches) {
				int id = int.Parse(m.Groups[3].Value);
				//не более 3х ботов на 1 работу
				if (_jobBlackList.ContainsKey(id) && _jobBlackList[id] > 3) {
					continue;
				}
				int q = 1;
				switch (m.Groups[1].Value) {
					case "one": q = 1; break;
					case "two": q = 2; break;
					case "three": q = 3; break;
					case "four": q = 4; break;
					case "five": q = 5; break;
				}
				string sprice = Regex.Replace(m.Groups[2].Value, "[^0-9\\.]", "");
				double salary = double.Parse(sprice, System.Globalization.CultureInfo.InvariantCulture);
				salary = salary * (1 - q * qFactor);
				if (salary > maxSalary) {
					jobId = id;
					maxSalary = salary;
				}
			}
			if (jobId == 0) {
				return false;
			}
			Thread.Sleep(_rnd.Next(2000));
			if (_jobBlackList.ContainsKey(jobId)) {
				_jobBlackList[jobId] = _jobBlackList[jobId] + 1;
			} else {
				_jobBlackList.Add(jobId, 1);
			}
			Log("устраивается на работу");
			string PostData = "_csrf_token=" + _csrfToken;
			response = UploadString("http://economy.erepublik.com/en/job/apply/" + jobId, PostData);
			if (response.IndexOf("Congratulations") == -1) return false;
			UpdateCsrfToken(response);
			return true;
		}

		public bool Resign() {
			Log("пытается уволиться");
			string PostData = "_csrf_token=" + _csrfToken;
			string response = UploadString("http://economy.erepublik.com/en/time-management/resign", PostData);
			if (response.IndexOf("You have successfully resigned") == -1) return false;
			Log("уволился!");
			return true;
		}
	}
}
