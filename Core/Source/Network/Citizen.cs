
using System;
using System.Text;
using NerZul.Core.Utils;
namespace NerZul.Core.Network
{
	public class Citizen:Bot
	{
        public string LoginName
        {
            get { return m_AuthLogin; }
        }

        public string Password
        {
            get { return m_AuthPassword; }
        }

        public Citizen(string Login, string Email, string Password, string UserAgent, string autocaptcha, int captchaBufferSize, bool bBeep) :
            base(Login, Email, Password, UserAgent, autocaptcha, captchaBufferSize, bBeep)
		{
			
		}

        /// <summary>
        /// Найм на работу по вакансии
        /// </summary>
        /// <param name="ID">Номер вакансии. Можно получить через GetJobOffers</param>
        /// <returns></returns>
        public bool Hire(int ID)
        {
            string PostData = "_csrf_token=" + ms_csrfToken;
            m_Response = m_Client.UploadString("http://economy.erepublik.com/en/job/apply/"+ID.ToString(), PostData);
            // ConsoleLog.WriteLine(m_Response, "HireResignLog.txt");
            if (m_Response.IndexOf("Congratulations")==-1) return false;
            ConsoleLog.WriteLine("Hired!");
            return true;
        }

        /// <summary>
        /// Увольняемся нафиг
        /// </summary>
        public bool Resign()
        {
            string PostData = "_csrf_token=" + ms_csrfToken;
            m_Response = m_Client.UploadString("http://economy.erepublik.com/en/time-management/resign", PostData);
            // ConsoleLog.WriteLine(m_Response, "HireResignLog.txt");
            if (m_Response.IndexOf("You have successfully resigned") == -1) return false;
            ConsoleLog.WriteLine("Resigned!");
            return true;
        }

        /// <summary>
        /// Попробовать поработать.
        /// </summary>
		//TODO: Проверка на каптчу
        public bool TryToWork()
        {
            m_Response=m_Client.DownloadString("http://www.erepublik.com/en/work");
            return m_Response.Contains("Total productivity");
        }
		
		public bool TryToTrain()
		{
            m_Response = m_Client.DownloadString("http://economy.erepublik.com/en/train");
            if (m_Response.Contains("You have already trained today")) return true;
			m_Response=m_Client.DownloadString("http://www.erepublik.com/en/my-places/train/0");
			while(m_Response.Contains("Please prove you are not a machine"))
			{
				if(CaptchaProvider==null) return false;
				var captcha=CaptchaProvider.GetResolvedCaptcha();
				string PostData="_token="+ms_Token2+
				"&recaptcha_challenge_field="+captcha.ChallengeID+
				"&recaptcha_response_field="+captcha.CaptchaText.Replace(" ","+")+
				"&commit=Continue";
				m_Response=m_Client.UploadString("http://www.erepublik.com/en/train_human_check/0",PostData);
			}
			if(m_Response.Contains("Strength gained")) return true;
			return false;
		}
		public void UnlockFeatures()
		{
			string PostData = "_token=" + ms_Token2;
			PostData+="&buy_what=features&buy_which=1&payment_method=gold";
			m_Response= m_Client.UploadString("http://www.erepublik.com/en/get-gold/unlock_features",PostData);
		}
        public void FightFlood(int Battle)
        {
            m_Client.Flood("http://www.erepublik.com/en/battles/fight/" + Battle.ToString()+"/0", 10,
                "http://www.erepublik.com/en/battles/fight/" + Battle.ToString() + "/0", true, "_token=" + ms_Token2);
        }
        public bool Fight(int Battle)
        {
            string PostData = "_token=" + ms_Token2;
            m_Client.Referer = "http://www.erepublik.com/en/battles/fight/" + Battle.ToString() + "/0";
            m_Response=m_Client.UploadString(m_Client.Referer, PostData);
			return m_Response.Contains("Total damage");

        }

        /// <summary>
        /// Обновление аватарки аккаунта
        /// </summary>
        /// <param name="jpeg">JPG-изображение с аватарой</param>
        /// <returns>true в случае успеха, false при несетевой ошибке</returns>
        public bool UploadAvatar(byte[] jpeg, string sPath)
        {
            //            return true;
            const string MailAffix = "id=\"citizen_email\" value=\"";
            m_Response = m_Client.DownloadString("http://www.erepublik.com/en/citizen/edit/profile");
            int cPos = 0, cPos2 = 0;
            //Вытаскиваем мыло
            cPos = m_Response.IndexOf(MailAffix);
            cPos += MailAffix.Length;
            cPos2 = m_Response.IndexOf('"', cPos);
            string Mail = m_Response.Substring(cPos, cPos2 - cPos);

            //Формируем тело POST-запроса
            System.Random rnd = new System.Random();
            string boundary = "---------------------------" + rnd.Next(100000, 999999).ToString() + rnd.Next(1000000, 9999999).ToString();
            string NameAffix = "--" + boundary + "\r\nContent-Disposition: form-data; name=\"";
            string NameSuffix = "\"\r\n\r\n";
            int ipos = sPath.LastIndexOf("\\");
            if (ipos != -1)
            {
                sPath = sPath.Substring(ipos + 1, sPath.Length - ipos - 1);
            }

            System.IO.MemoryStream postdata = new System.IO.MemoryStream();
            //Данные формы            
            string formdata = "";
            formdata += NameAffix + "_token" + NameSuffix + ms_Token2 + "\r\n";
            formdata += NameAffix + "password" + NameSuffix + m_AuthPassword + "\r\n";
            formdata += NameAffix + "citizen_name" + NameSuffix + m_AuthLogin + "\r\n";
            formdata += NameAffix + "citizen_email" + NameSuffix + Mail + "\r\n";
            formdata += NameAffix + "birth_date[]" + NameSuffix + rnd.Next(1, 28).ToString() + "\r\n";
            formdata += NameAffix + "birth_date[]" + NameSuffix + rnd.Next(1, 12).ToString() + "\r\n";
            formdata += NameAffix + "birth_date[]" + NameSuffix + rnd.Next(1950, 1990).ToString() + "\r\n";

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
//            m_Client.Timeout = m_Client.Timeout * 10;
            try
            {
                m_Response = m_Client.UploadMultipartData("http://www.erepublik.com/en/citizen/edit/profile", buffer, boundary);
            }
            finally
            {
//                m_Client.Timeout = m_Client.Timeout / 10;
            }
            return (m_Response.IndexOf("You have succesfully edited your profile") != 0);
        }	
		
		public bool JoinParty(string Party)
		{
			m_Response=m_Client.DownloadString("http://www.erepublik.com/en/join-party/"+Party);
			return m_Response.Contains("Congratulations, you are now a party member");
        }
        public bool LeaveParty(string Party)
		{
			string PostData=GetTokenArg(1);
			m_Response=m_Client.UploadString("http://www.erepublik.com/en/resign-party/"+
								                  Party,PostData);
			return m_Response.Contains("You are not a member of a party");
		}
		public bool VoteCongress(int election, string candidate)
		{
			string resp = m_Client.DownloadString("http://www.erepublik.com/vote-for-congress?candidate_id=" + candidate);
			string search = "name=\"_token\" value=\"";
			resp = resp.Remove(0, resp.IndexOf(search) + search.Length);
			string tocken = resp.Substring(0, resp.IndexOf("\""));

			if (tocken.Length < 20) {
				ConsoleLog.WriteLine("invalid tocken:" + tocken);
				tocken = ms_Token2;
			}
			ResolvedCaptcha rc = CaptchaProvider.GetResolvedCaptcha();
			string PostData = "_token=" + tocken + 
				"&candidate_id=" + candidate +
				"&recaptcha_challenge_field=" + rc.ChallengeID + 
				"&recaptcha_response_field="+System.Web.HttpUtility.UrlEncode(rc.CaptchaText);
            m_Response = m_Client.UploadString("http://www.erepublik.com/vote-for-congress", PostData);
			return m_Response.Contains("display_captcha = false");
		}

        public bool VoteParty(int election, string candidate)
        {
            ResolvedCaptcha rc = CaptchaProvider.GetResolvedCaptcha();
            m_Client.Headers.Add("X-Requested-With", "XMLHttpRequest");
            string PostData = GetTokenArg(2) +
                "&c=" + candidate +
                "&election=" + election +
                "&recaptcha_challenge_field=" + rc.ChallengeID +
                "&recaptcha_response_field=" + System.Web.HttpUtility.UrlEncode(rc.CaptchaText);
            m_Response = m_Client.UploadString("http://www.erepublik.com/vote-party-election", PostData);
            m_Client.Headers.Remove("X-Requested-With");
            return m_Response.Contains("display_captcha = false");
        }
    }
}
