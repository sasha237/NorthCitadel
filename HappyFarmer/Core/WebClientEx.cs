using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

namespace PalBot.Core {
	public class HttpClient {
		public static int ResponseTimeout = 30000;
		/// <summary>
		///Заголовки, которые будут использованы при следующем запроса 
		/// </summary>
		public readonly WebHeaderCollection Headers = new WebHeaderCollection();
		/// <summary>
		///Содержимое заголовка User-Agent  
		/// </summary>
		public string UserAgent = "Opera/9.99 (Windows NT 5.1; U; pl) Presto/9.9.9";
		/// <summary>
		///Referer, который будет использован при следующем запросе. Обычно выставляется автоматически 
		/// </summary>
		public string Referer = "";
		/// <summary>
		///Таймаут запросов. Если запрос не был выполнен в течении этого времени, генерируется исключение 
		/// </summary>
		public int Timeout = 20000;

		public CookieContainer Cookies = new CookieContainer();
		/// <summary>
		///Если это поле установлено, то Referer не будет изменяться автоматически
		///TODO: Выпилить к ебеням и сделать более вменяемо
		/// </summary>
		public bool Ajax = false;

#if !PUBLIC_BUILD
		/// <summary>
		/// Прокся 
		/// </summary>
		public string Proxy {
			get { return m_Proxystring; }
			set {
				m_Proxystring = value;
				if ((value == null) || (value.Length == 0)) m_Proxy = null;
				else m_Proxy = new WebProxy("http://" + value + "/");
			}
		}
#endif
		private IWebProxy m_Proxy = null;
		private string m_Proxystring = "";




		/// <summary>
		///Выполняет POST-запрос 
		/// </summary>
		/// <param name="URL">
		/// URL полностью
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="Data">
		/// Данные, которые будут переданы
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="ContentType">
		/// Содержимое Content-Type заголовка
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Результат запроса
		/// A <see cref="System.String"/>
		/// </returns>       
		public string UploadString(string URL, string Data, string ContentType) {
			HttpWebRequest req = PresetRequest(URL);
			if (ContentType != null) req.ContentType = ContentType;
			req.Method = "POST";
			int len = Encoding.UTF8.GetByteCount(Data);
			req.ContentLength = len;
			//req.GetRequestStream().Write(Encoding.UTF8.GetBytes(Data), 0, len);
			return GetResponseText(req, Encoding.UTF8.GetBytes(Data));
		}
		/// <param name="URL">
		/// URL полностью
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="Data">
		/// Данные, которые будут переданы
		/// A <see cref="System.String"/>
		/// </param>
		/// </param>
		/// <returns>
		/// Результат запроса
		/// A <see cref="System.String"/>
		/// </returns>       
		public string UploadString(string URL, string Data) {
			return UploadString(URL, Data, "application/x-www-form-urlencoded");
		}

		public string UploadStringSafe(string URL, string Data) {
			for (int i = 0; i < 5; i++) {
				try {
					return UploadString(URL, Data);
				} catch { }
			}
			return null;
		}
		/// <summary>
		/// POST-запрос с предварительно сформированными данными multipart-типа 
		/// </summary>
		/// <param name="URL">
		/// URL полностью
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="Data">
		/// Данные, которые будут переданы
		/// A <see cref="System.Byte[]"/>
		/// </param>
		/// <param name="boundary">
		/// Разделитель. См. доку по HTTP
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Результат запроса
		/// A <see cref="System.String"/>
		/// </returns>        
		public string UploadMultipartData(string URL, byte[] Data, string boundary) {
			HttpWebRequest req = PresetRequest(URL);
			if (boundary != null)
				req.ContentType = "multipart/form-data; boundary=" + boundary;
			req.Method = "POST";
			req.ContentLength = Data.Length;
			//req.GetRequestStream().Write(Data,0,Data.Length);

			return GetResponseText(req, Data);
		}

		public Stream UploadMultipartDataStream(string URL, byte[] Data, string boundary) {
			HttpWebRequest req = PresetRequest(URL);
			if (boundary != null)
				req.ContentType = "multipart/form-data; boundary=" + boundary;
			req.Method = "POST";
			req.ContentLength = Data.Length;
			//req.GetRequestStream().Write(Data,0,Data.Length);

			return GetResponseStream(req, Data);
		}


		private void FloodThread(object arg) {
			HttpWebRequest req = (HttpWebRequest)arg;
			try {
				GetResponseText(req, null);
			} catch { };
		}

		/// <summary>
		///Посылает энное количество запросов 
		/// </summary>
		public void Flood(string URL, int ThreadCount, string Referer, bool UsePost, string PostData) {

			HttpWebRequest[] reqs = new HttpWebRequest[ThreadCount];
			System.Threading.Thread[] threads = new System.Threading.Thread[ThreadCount];
			for (int c = 0; c < ThreadCount; c++) {
				reqs[c] = PresetRequest(URL);
				reqs[c].ServicePoint.ConnectionLimit = ThreadCount;
				reqs[c].Referer = Referer;

				if (UsePost) {
					reqs[c].Method = "POST";
					reqs[c].ContentType = "application/x-www-form-urlencoded";
					int len = Encoding.UTF8.GetByteCount(PostData);
					reqs[c].ContentLength = len;
					reqs[c].GetRequestStream().Write(Encoding.UTF8.GetBytes(PostData), 0, len);

				};
				threads[c] = new Thread(FloodThread);
			}

			for (int c = 0; c < ThreadCount; c++) threads[c].Start(reqs[c]);

		}

		public void Flood(string URL, int ThreadCount, string Referer) { Flood(URL, ThreadCount, Referer, false, ""); }
		public void Flood(string URL, int ThreadCount) { Flood(URL, ThreadCount, Referer); }
		/// <summary>
		///Посылает GET-запрос 
		/// </summary>
		/// <param name="URL">
		/// URL полностью
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Результат запроса
		/// A <see cref="System.String"/>
		/// </returns>
		public string DownloadString(string URL) {
			HttpWebRequest req = PresetRequest(URL);
			return GetResponseText(req, null);
		}

		public string DownloadStringSafe(string url) {
			for (int i = 0; i < 5; i++) {
				try {
					return DownloadString(url);
				} catch { }
			}
			return null;
		}

		private string GetResponseText(HttpWebRequest req, byte[] data) {
			while (true) {
				try {
					return GetResponseText2(req, data);
				} catch (Exception e) {
					//TODO: Add error handling stuff here
					throw e;
				}
			}
		}
		private string GetResponseText2(HttpWebRequest req, byte[] data) {
			if (data != null) req.GetRequestStream().Write(data, 0, data.Length);
			HttpWebResponse resp = GetResponseWithTimeout(req);
			if (resp == null)
				throw new Exception("GetResponseText2: response wait timeout!");
			Stream stream = resp.GetResponseStream();
			if (resp.GetResponseHeader("Content-Encoding").ToLower().Contains("gzip"))
				stream = new System.IO.Compression.GZipStream(stream,
					System.IO.Compression.CompressionMode.Decompress);
			TextReader reader = new StreamReader(stream, UTF8Encoding.UTF8);
			return reader.ReadToEnd();
		}

		private Stream GetResponseStream(HttpWebRequest req, byte[] data) {
			while (true) {
				try {
					return GetResponseStream2(req, data);
				} catch (Exception e) {
					//TODO: Add error handling stuff here
					throw e;
				}
			}
		}
		private Stream GetResponseStream2(HttpWebRequest req, byte[] data) {
			if (data != null) req.GetRequestStream().Write(data, 0, data.Length);
			HttpWebResponse resp = GetResponseWithTimeout(req);
			Stream stream = resp.GetResponseStream();
			if (resp.GetResponseHeader("Content-Encoding").ToLower().Contains("gzip"))
				stream = new System.IO.Compression.GZipStream(stream,
					System.IO.Compression.CompressionMode.Decompress);
			return stream;
		}

		public byte[] DownloadData(string URL) {
			HttpWebRequest req = PresetRequest(URL);
			HttpWebResponse resp = GetResponseWithTimeout(req);
			Stream stream = resp.GetResponseStream();
			if (resp.GetResponseHeader("Content-Encoding").ToLower().Contains("gzip"))
				stream = new System.IO.Compression.GZipStream(stream,
					System.IO.Compression.CompressionMode.Decompress);
			int length = int.Parse(resp.GetResponseHeader("Content-Length"));

			byte[] rv = new byte[length];
			int roff = 0, r = 0;
			while ((roff + r) != length) {
				r = stream.Read(rv, roff, length - roff);
				roff += r;
				r = 0;
			}
			return rv;
		}

		private void GetResponseCallback(IAsyncResult ar) {
			((AutoResetEvent)ar.AsyncState).Set();
		}


		private HttpWebResponse GetResponseWithTimeout(HttpWebRequest req) {

			bool m_Ajax = Ajax; Ajax = false;
			AutoResetEvent ev = new AutoResetEvent(false);
			IAsyncResult result = req.BeginGetResponse(GetResponseCallback, ev);

			if (!ev.WaitOne(ResponseTimeout)) {
				req.Abort();
				return null;
			}
			if (!m_Ajax) Referer = req.RequestUri.ToString();
			return (HttpWebResponse)req.EndGetResponse(result);

		}

		private HttpWebRequest PresetRequest(string URL) {
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);

			//req.KeepAlive = true;
			req.Expect = "";
			//Headers.Add("Keep-Alive: 300");
			req.Headers.Add(Headers);
			req.Accept = "text/html, application/xml;q=0.9, application/xhtml+xml, image/png, image/jpeg, image/gif, image/x-xbitmap";
			req.Headers.Add("Accept-Encoding: gzip, identity");
			req.KeepAlive = false;
			req.CookieContainer = Cookies;
			req.Referer = Referer;
			if (Proxy != null)
				req.Proxy = m_Proxy;
			req.UserAgent = UserAgent;
			req.Method = "GET";

			return req;
		}
		public HttpClient() {

			System.Net.ServicePointManager.Expect100Continue = false;
		}

		/// <summary>
		/// Клон клинета, эмулирует работу в отдельной вкладке браузера.
		/// </summary>
		public HttpClient Clone() {
			HttpClient clone = new HttpClient();
			clone.Cookies = Cookies;
			clone.Proxy = Proxy;
			clone.UserAgent = UserAgent;
			clone.Referer = Referer;
			clone.Timeout = Timeout;
			return clone;
		}

	}
	/*
    class WebClientEx :WebClient
    {
        public CookieContainer Cookies=new CookieContainer();
        public string UserAgent;
        public string Referer="";
        public uint Timeout = 10000;
        public bool Ajax = false;
        private HttpWebRequest m_LastReq;
        
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest req = (HttpWebRequest)base.GetWebRequest(address);
            req.CookieContainer = Cookies;
            req.UserAgent = UserAgent;
            req.Referer = Referer;
            m_LastReq = req;
            return req;
        }
        #region timeout
        private ManualResetEvent allDone = new ManualResetEvent(false);
        private void RespCallback(IAsyncResult asynchronousResult)
        {
            allDone.Set();
        }
        private class CBCont
        {
            public HttpWebRequest req;
            public bool Aborted = false;
            public CBCont(HttpWebRequest r) { req = r; }
        }

        private static void TimeoutCallback(object state, bool timedOut)
        {
            if (timedOut)
            {

                CBCont cont = state as CBCont;
                if (cont.req != null)
                {
                    cont.Aborted = true;
                    cont.req.Abort();
                }
            }
        }
        #endregion


       
         protected override WebResponse GetWebResponse(WebRequest request)
        {
            try
            {
                IAsyncResult result =
                  (IAsyncResult)request.BeginGetResponse(new AsyncCallback(RespCallback), null);
                CBCont cbcont = new CBCont((HttpWebRequest)request);
                ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, new WaitOrTimerCallback(TimeoutCallback), cbcont, Timeout, true);
                allDone.WaitOne();
                if (cbcont.Aborted) return null;
                string XReferer = request.RequestUri.ToString();
                WebResponse resp= request.EndGetResponse(result);
                if(Ajax==false)   Referer = XReferer;
                Ajax = false;
                return resp;
            }
            catch
            {
                Ajax = false;
                return null;
            }

        }
        
    }
*/
}
