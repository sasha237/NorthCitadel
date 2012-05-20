using System;

namespace NerZul.Core.Network
{

    using System.Text;
    using Core;
    using NerZul.Core.Utils;
    using System.Collections.Generic;
    using System.Threading;

    public class ResolvedCaptcha
    {
        public string ChallengeID, CaptchaText;
        public ResolvedCaptcha(string ID, string Text) { ChallengeID = ID; CaptchaText = Text; }
    }

    public interface ICaptchaProvider
    {
        ResolvedCaptcha GetResolvedCaptcha();
    }

    public class AntigateCaptchaProvider : ICaptchaProvider
    {
        private string m_Key;
        public AntigateCaptchaProvider(string Key)
        {
            m_Key = Key;
        }
        public ResolvedCaptcha GetResolvedCaptcha()
        {
            Captcha c = new Captcha("http://www.erepublik.com/en");
            return new ResolvedCaptcha(c.ChallengeID, c.ResolveAntigate(m_Key));
        }
    }

    public class PrecaptchaCaptchaProvider : ICaptchaProvider
    {
        public PrecaptchaCaptchaProvider(string antigateKey, int bufferSize, bool bBeep)
        {
            PreCaptcha.Init(antigateKey, bufferSize, bBeep);
        }
        public ResolvedCaptcha GetResolvedCaptcha()
        {
            return PreCaptcha.GetCapcha();
        }
    }

    public class WinFormsCaptchaProvider : ICaptchaProvider
    {
        private static System.Windows.Forms.Form Form = new System.Windows.Forms.Form();
        private static System.Windows.Forms.PictureBox PicBox = new System.Windows.Forms.PictureBox();
        private static System.Windows.Forms.TextBox TextBox = new System.Windows.Forms.TextBox();
        private static System.Windows.Forms.Button OKBtn = new System.Windows.Forms.Button();
        private static bool init = false;
        private static bool m_bBeep = false;
        private static object locker = new object();
        public WinFormsCaptchaProvider(bool bBeep)
        {
            m_bBeep = bBeep;
        }
        private static void Init()
        {
            if (init) return;
            init = true;
            PicBox.AutoSize = true;
            Form.Controls.Add(PicBox);
            Form.Controls.Add(TextBox);
            Form.Controls.Add(OKBtn);
            //OKBtn.Visible=false;
            OKBtn.Click += OnClick;
            Form.AcceptButton = OKBtn;

        }
        private static void OnClick(object sender, EventArgs args)
        {
            Form.Close();
        }


        public ResolvedCaptcha GetResolvedCaptcha()
        {
            lock (locker)
            {
                Init();
                Captcha c = new Captcha("http://www.erepublik.com/en");
                var stream = new System.IO.MemoryStream(c.Image);
                var img = System.Drawing.Image.FromStream(stream);
                PicBox.Image = img;
                PicBox.Width = img.Width;
                PicBox.Height = img.Height;
                Form.Width = PicBox.Width + 5;
                Form.Height = PicBox.Height + Form.Height - Form.ClientSize.Height + 30;
                TextBox.Top = PicBox.Height;
                TextBox.Width = PicBox.Width;
                TextBox.Height = 30;
                TextBox.Focus();
                TextBox.Text = "";
                if (m_bBeep)
                {
                    Console.Beep(262, 400);
                }
                Form.ShowDialog();
                return new ResolvedCaptcha(c.ChallengeID, TextBox.Text);
            }
        }
    }

    public class Captcha
    {
        private static string CaptchaAPIURL =
            "http://api.recaptcha.net/challenge?k=6LeWK7wSAAAAAA_QFoHVnY5HwVCb_CETsvrayFhu";
        private static string CaptchaURLBase = "http://api.recaptcha.net/image?c=";
        //private static string CaptchaGAPIURL=
        //	"http://www.google.com/recaptcha/api/challenge?k=6LfEGgIAAAAAABSqQ0rzMd3t-rOU_XrZlC2C9WG2&darklaunch=1";

        private static string ChallengeScanString = "challenge:'";
        private static string ChallengeScanString2 = "challenge :";
        public readonly string ChallengeID;
        public readonly byte[] Image;
        public Captcha(string Referer)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.Referer = Referer;
                //Вытаскиваем токен каптчи
                string CaptchaData = client.DownloadString(Captcha.CaptchaAPIURL);
                CaptchaData = CaptchaData.Remove(0, CaptchaData.IndexOf(Captcha.ChallengeScanString) +
                                               ChallengeScanString.Length);
                CaptchaData = CaptchaData.Remove(0, CaptchaData.IndexOf(Captcha.ChallengeScanString2) +
                                               ChallengeScanString2.Length);
                CaptchaData = CaptchaData.Trim();
                if (CaptchaData[0] == '\'') CaptchaData = CaptchaData.Remove(0, 1);
                CaptchaData = CaptchaData.Remove(CaptchaData.IndexOf('\''));
                /*
                client.Referer=Referer;
                string CaptchaImageURL=client.DownloadString(Captcha.CaptchaGAPIURL);
                CaptchaImageURL=CaptchaImageURL.Substring(CaptchaImageURL.IndexOf('\'')+1);
                CaptchaImageURL=CaptchaImageURL.Remove(CaptchaImageURL.IndexOf('\''));                                          
                */
                string CaptchaImageURL = Captcha.CaptchaURLBase + CaptchaData;
                //Грузим картинку с каптчей
                client.Referer = null;
                Image = client.DownloadData(CaptchaImageURL);
                ChallengeID = CaptchaData;
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLine("Unable to load captcha: " + e.Message);
                throw e;
            }
        }

        public Captcha(byte[] image)
        {
            Image = image;
        }

        public string ResolveAntigate(string Key)
        {
            try
            {
                Antigate ag = new Antigate();
                ag.Key = Key;
                if (ag.ResolveCaptcha(Image)) return ag.CaptchaText;
                ConsoleLog.WriteLine("Captcha not resolved (no error)");
                return null;
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLine("Unable to resolve captcha: " + e.Message);
                throw e;
            }
        }
    }

    public class Antigate
    {
        public static string phrase = "1";

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
            HttpClient m_Client = new HttpClient();
            string m_Response = "";
            //Формируем тело POST-запроса
            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            string NameAffix = "--" + boundary + "\r\nContent-Disposition: form-data; name=\"";
            string NameSuffix = "\"\r\n\r\n";


            System.IO.MemoryStream postdata = new System.IO.MemoryStream();
            //Данные формы            
            string formdata = "";
            formdata += NameAffix + "method" + NameSuffix + "post" + "\r\n";
            formdata += NameAffix + "soft_id" + NameSuffix + "223\r\n";
            formdata += NameAffix + "key" + NameSuffix + Key + "\r\n";
            formdata += NameAffix + "phrase" + NameSuffix + phrase + "\r\n";


            //Заголовок для файла
            formdata += NameAffix + "file\"; filename=\"captcha.jpg\"\r\n";
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
            //ConsoleLog.WriteLine(buffer, "CapchaLog.txt");
            //m_Client.Timeout = m_Client.Timeout * 10;
            m_Response = m_Client.UploadMultipartData(
                "http://antigate.com/in.php", buffer, boundary);
            if (m_Response.Substring(0, 2) == "OK")
            {
                CaptchaID = m_Response.Substring(3);
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
            string resp = client.DownloadString("http://antigate.com/res.php?key=" +
                                              Key + "&action=get&id=" + CaptchaID.ToString());
            if (resp.Contains("CAPCHA_NOT_READY"))
            {
                CaptchaStatus = Status.NotReady;
                return Status.NotReady;
            }
            if (resp.Substring(0, 2) == "OK")
            {
                CaptchaText = resp.Substring(3);
                CaptchaStatus = Status.Success;
                return Status.Success;
            }
            CaptchaStatus = Status.Error;
            return Status.Error;
        }

        public bool ResolveCaptcha(byte[] Image)
        {
            UploadCaptcha(Image);
            while (true)
            {
                System.Threading.Thread.Sleep(10000);
                GetStatus();
                ConsoleLog.WriteLine("ResolveCaptcha status: " + CaptchaStatus.ToString());
                if (CaptchaStatus == Status.Error) return false;
                if (CaptchaStatus == Status.Success) return true;
            }
        }
    }

    public static class PreCaptcha
    {
        private static List<ResolvedCaptcha> captchaBuf;
        private static bool inited = false;
        private static bool doStop;
        private static object locker = new object();
        private static int buffSize;
        private static int threadPoolSize;
        private static Thread[] threadPool;
        private static int inResolve;
        private static ICaptchaProvider captchaProvider;

        public static void Init(string antigateKey, int bufferSize, bool bBeep)
        {
            if (inited)
                return;

            lock (locker)
            {
                if (inited)
                    return;

                inited = true;
                doStop = false;

                captchaBuf = new List<ResolvedCaptcha>();
                buffSize = bufferSize;

                threadPoolSize = 5;
                threadPool = new Thread[threadPoolSize];
                inResolve = 0;

                if (String.IsNullOrEmpty(antigateKey))
                {
                    captchaProvider = new WinFormsCaptchaProvider(bBeep);
                }
                else
                {
                    captchaProvider = new AntigateCaptchaProvider(antigateKey);
                }

                var preCaptchaThread = new Thread(Worker);
                preCaptchaThread.Start();
            }
        }

        private static void ResolveWorker()
        {
            try
            {
                // Try to get captcha
                ConsoleLog.WriteLine("PreCaptcha. Getting new captcha...");
                ResolvedCaptcha capcha = captchaProvider.GetResolvedCaptcha();

                if (!String.IsNullOrEmpty(capcha.CaptchaText))
                {
                    ConsoleLog.WriteLine("PreCaptcha. New captcha loaded");
                    lock (locker)
                    {
                        captchaBuf.Add(capcha);
                    }
                }
                else
                {
                    ConsoleLog.WriteLine("PreCaptcha. New captcha loading failed");
                }
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLine("PreCaptcha. New captcha loading error: " + e.Message);
            }

            ConsoleLog.WriteLine("PreCaptcha. Ready captchas: " + captchaBuf.Count.ToString() + " / " + buffSize.ToString());

            lock (locker)
            {
                inResolve--;
            }
        }

        private static void Worker()
        {
            int captchasToGet = 0;

            doStop = false;

            while (true)
            {
                if (doStop) break;

                lock (locker)
                {
                    captchasToGet = (buffSize - captchaBuf.Count - inResolve);
                }

                while (captchasToGet > 0)
                {
                    if (doStop) break;

                    int freeThread = -1;

                    for (int ft = 0; ft < threadPoolSize; ft++)
                    {
                        if ((threadPool[ft] == null) || (!threadPool[ft].IsAlive))
                        {
                            freeThread = ft;
                            break;
                        }
                    }

                    if (freeThread == -1)
                    {
                        System.Threading.Thread.Sleep(1000);
                        continue;
                    }

                    captchasToGet--;
                    lock (locker)
                    {
                        inResolve++;
                    }
                    threadPool[freeThread] = new System.Threading.Thread(ResolveWorker);
                    threadPool[freeThread].Start();
                }

                if (doStop) break;

                Thread.Sleep(1000);
            }

            ConsoleLog.WriteLine("PreCaptcha. Stopped");
        }

        public static ResolvedCaptcha GetCapcha()
        {
            while (true)
            {
                if (captchaBuf.Count > 0)
                {
                    ResolvedCaptcha captcha = null;

                    lock (locker)
                    {
                        if (captchaBuf.Count > 0)
                        {
                            captcha = captchaBuf[0];
                            captchaBuf.RemoveAt(0);
                        }
                    }

                    if (captcha != null)
                    {
                        ConsoleLog.WriteLine("PreCaptcha. Consuming captcha. Ready captchas left: " + captchaBuf.Count.ToString());
                        return captcha;
                    }
                }

                Thread.Sleep(1000);
            }
        }

        public static void StopWork()
        {
            ConsoleLog.WriteLine("PreCaptcha. Stopping...");

            doStop = true;
        }
    }
}
