using System;

namespace PalBot.Core
{
	
	using System.Text;
    using Core;
	
	public class ResolvedCaptcha
	{
		public string ChallengeID,CaptchaText;
		public ResolvedCaptcha(string ID, string Text) {ChallengeID=ID; CaptchaText=Text;}
	}
	public interface ICaptchaProvider
	{
		ResolvedCaptcha GetResolvedCaptcha();
	}
	#if !PUBLIC_BUILD
	public class AntigateCaptchaProvider:ICaptchaProvider
	{
		private string m_Key;
		public AntigateCaptchaProvider(string Key)
		{
			m_Key=Key;
		}
		public ResolvedCaptcha GetResolvedCaptcha ()
		{
			Captcha c=new Captcha("http://www.erepublik.com/en");
			return new ResolvedCaptcha(c.ChallengeID, c.ResolveAntigate(m_Key));
		}
	}
	public class WinFormsCaptchaProvider:ICaptchaProvider
	{
		private static System.Windows.Forms.Form Form=new System.Windows.Forms.Form();
		private static System.Windows.Forms.PictureBox PicBox=new System.Windows.Forms.PictureBox();
		private static System.Windows.Forms.TextBox TextBox=new System.Windows.Forms.TextBox();
		private static System.Windows.Forms.Button OKBtn=new System.Windows.Forms.Button();
		private static bool init=false;
		private static void Init()
		{
			if(init) return;
			init=true;
			PicBox.AutoSize=true;
			Form.Controls.Add (PicBox);
			Form.Controls.Add(TextBox);
			Form.Controls.Add(OKBtn);
			//OKBtn.Visible=false;
			OKBtn.Click+=OnClick;
			Form.AcceptButton=OKBtn;

		}
		private static void OnClick(object sender, EventArgs args)
		{
			Form.Close();
		}
		
		
		public ResolvedCaptcha GetResolvedCaptcha ()
		{
			lock(Form)
			{
				Init();
				Captcha c=new Captcha("http://www.erepublik.com/en");
				var stream=new System.IO.MemoryStream(c.Image);
				var img=System.Drawing.Image.FromStream(stream);
				PicBox.Image=img;
				PicBox.Width=img.Width;
				PicBox.Height=img.Height;
				Form.Width=PicBox.Width+5;
				Form.Height=PicBox.Height+Form.Height-Form.ClientSize.Height+30;
				TextBox.Top=PicBox.Height;
				TextBox.Width=PicBox.Width;
				TextBox.Height=30;
				TextBox.Focus();
				TextBox.Text="";
				Form.ShowDialog();
				return new ResolvedCaptcha(c.ChallengeID,TextBox.Text);
			}
		}
	}
	
	public class Captcha
	{
		private static string CaptchaAPIURL=
            "http://api.recaptcha.net/challenge?k=6LeWK7wSAAAAAA_QFoHVnY5HwVCb_CETsvrayFhu";
		private static string CaptchaURLBase="http://api.recaptcha.net/image?c=";
		//private static string CaptchaGAPIURL=
		//	"http://www.google.com/recaptcha/api/challenge?k=6LfEGgIAAAAAABSqQ0rzMd3t-rOU_XrZlC2C9WG2&darklaunch=1";
	
		private static string ChallengeScanString="challenge:'";
		private static string ChallengeScanString2="challenge :";
		public readonly string ChallengeID;
		public readonly byte[] Image;
		public Captcha(string Referer)
		{
			try
			{
			HttpClient client=new HttpClient();
			client.Referer=Referer;
			//Вытаскиваем токен каптчи
			string CaptchaData=client.DownloadString(Captcha.CaptchaAPIURL);
			CaptchaData=CaptchaData.Remove(0,CaptchaData.IndexOf(Captcha.ChallengeScanString)+
			                               ChallengeScanString.Length);
			CaptchaData=CaptchaData.Remove(0,CaptchaData.IndexOf(Captcha.ChallengeScanString2)+
			                               ChallengeScanString2.Length);
			CaptchaData=CaptchaData.Trim();
			if(CaptchaData[0]=='\'') CaptchaData=CaptchaData.Remove(0,1);
			CaptchaData=CaptchaData.Remove(CaptchaData.IndexOf('\''));
			/*
			client.Referer=Referer;
			string CaptchaImageURL=client.DownloadString(Captcha.CaptchaGAPIURL);
			CaptchaImageURL=CaptchaImageURL.Substring(CaptchaImageURL.IndexOf('\'')+1);
			CaptchaImageURL=CaptchaImageURL.Remove(CaptchaImageURL.IndexOf('\''));                                          
			*/
			string CaptchaImageURL=Captcha.CaptchaURLBase+CaptchaData;
			//Грузим картинку с каптчей
			client.Referer=null;
			Image=client.DownloadData(CaptchaImageURL);
			ChallengeID=CaptchaData;
			}catch (Exception e)
			{
				MainForm.SetStatus("Unable to load captcha: " + e.Message);
				throw e;
			}
		}
		
		public string ResolveAntigate(string Key)
		{
            try
            {
                Antigate ag = new Antigate();
                ag.Key = Key;
                if (ag.ResolveCaptcha(Image)) return ag.CaptchaText;
				MainForm.SetStatus("Captcha not resolved (no error)");
                return null;
            }
            catch (Exception e)
            {
				MainForm.SetStatus("Unable to resolve captcha: " + e.Message);
                //ConsoleLog.WriteLine(e.StackTrace);
                throw e;
            }
		}
	}
	public class Antigate
	{
		public enum Status
		{
			NotReady, Success, Error
		}
		public string CaptchaID;
		public string Key;
		public Status CaptchaStatus;
		public string CaptchaText;
		public bool UploadCaptcha(byte[] jpeg)
		{
			HttpClient m_Client=new HttpClient();
			string m_Response="";
            //Формируем тело POST-запроса
            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            string NameAffix = "--" + boundary + "\r\nContent-Disposition: form-data; name=\"";
            string NameSuffix = "\"\r\n\r\n";

            
            System.IO.MemoryStream postdata = new System.IO.MemoryStream();
            //Данные формы            
            string formdata = "";
            formdata += NameAffix + "method" + NameSuffix + "post" + "\r\n";
			formdata += NameAffix + "key" + NameSuffix + Key + "\r\n";
			formdata += NameAffix + "phrase" + NameSuffix + "1" + "\r\n";
			

            //Заголовок для файла
            formdata += NameAffix + "file\"; filename=\"captcha.jpg\"\r\n";
            formdata += "Content-Type: image/jpeg\r\n\r\n";

            //Пишемс
            postdata.Write(Encoding.ASCII.GetBytes(formdata),0,formdata.Length);
            postdata.Write(jpeg, 0, jpeg.Length);
            //Готовим окончание
            formdata="\r\nContent-Disposition: form-data; name=\"commit\"\r\nMake changes\r\n";
            formdata += "--" + boundary + "--";
            //Пишемс
            postdata.Write(Encoding.ASCII.GetBytes(formdata), 0, formdata.Length);
            byte[] buffer=new byte[postdata.Length];
            postdata.Seek(0, System.IO.SeekOrigin.Begin);
            postdata.Read(buffer, 0, buffer.Length);
            //System.IO.File.WriteAllBytes("log.txt", buffer);
            //ConsoleLog.WriteLine(buffer, "CapchaLog.txt");
            m_Client.Timeout = m_Client.Timeout * 10;
            m_Response = m_Client.UploadMultipartData(
				"http://antigate.com/in.php", buffer, boundary);
			if(m_Response.Substring(0,2)=="OK")
			{
				CaptchaID=m_Response.Substring(3);
				return true;
			}
			return false;
		}
		public Status GetStatus()
		{
            HttpClient client = new HttpClient();
            if (Key == null)
            {
                throw new Exception("Captcha.GetStatus: Key is null");
            }
            if (CaptchaID == null)
            {
                throw new Exception("Captcha.GetStatus: CaptchaID is null");
            }
			string resp=client.DownloadString("http://antigate.com/res.php?key="+
			                                  Key+"&action=get&id="+CaptchaID.ToString());
            if (resp.Contains("CAPCHA_NOT_READY"))
			{
                CaptchaStatus = Status.NotReady;
				return Status.NotReady;
			}
			if(resp.Substring(0,2)=="OK")
			{
                CaptchaText = resp.Substring(3);
				CaptchaStatus=Status.Success;
				return Status.Success;
			}
			CaptchaStatus=Status.Error;
			return Status.Error;
		}
		
		public bool ResolveCaptcha(byte[] Image)
		{
			UploadCaptcha(Image);
			while(true)
			{
				System.Threading.Thread.Sleep(10000);
				GetStatus();
				MainForm.SetStatus("ResolveCaptcha status: " + CaptchaStatus.ToString());
				if(CaptchaStatus==Status.Error) return false;
				if(CaptchaStatus==Status.Success) return true;
			}
		}
	}
	#endif	
}
