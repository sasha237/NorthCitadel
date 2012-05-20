using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

namespace NerZul.Core.Network
{
    public class HttpClient
    {
        /// <summary>
        /// Счетчик произошедших таймаутов
        /// </summary>
        public int Timeouts = 0;

        /// <summary>
        /// Приудительная задержка между двумя открытиями URL
        /// </summary>
        public int ForcedDelay = 1000;

        /// <summary>
        ///Таймаут запросов. Если запрос не был выполнен в течении этого времени, генерируется исключение 
        /// </summary>
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
		public string Referer="";
		
        public CookieContainer Cookies = new CookieContainer();

        private DateTime previousLoad = DateTime.Now;

		/// <summary>
		/// Прокся 
		/// </summary>
/*
        public string Proxy
		{
			get { return m_Proxystring; }
            set {
				m_Proxystring = value;
				if ((value==null)||(value.Length==0)) 
                    m_Proxy=null;
				else 
                    m_Proxy=new WebProxy("http://"+value+"/");
			}
		}
 */
        public string GetProxy()
        {
            return m_Proxystring;
        }

        public void SetProxy(string proxyAddress, NetworkCredential proxyCredentials)
        {
            m_Proxystring = proxyAddress;
            if (String.IsNullOrEmpty(m_Proxystring))
                m_Proxy = null;
            else
            {
                m_Proxy = new WebProxy("http://" + proxyAddress + "/");
                m_Proxy.Credentials = proxyCredentials;
            }
        }

        private IWebProxy m_Proxy=null;
		private string m_Proxystring="";
		
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
        public string UploadString(string URL, string Data, string ContentType)
        {
            TimeSpan ts = DateTime.Now - previousLoad;
            int delay = (ForcedDelay - ts.Milliseconds);
            if (delay > 0)
                System.Threading.Thread.Sleep(delay);
            previousLoad = DateTime.Now;

            HttpWebRequest req = PresetRequest(URL);
            if(ContentType!=null) req.ContentType=ContentType;
            req.Method = "POST";
            int len = Encoding.UTF8.GetByteCount(Data);
            req.ContentLength=len;
            //req.GetRequestStream().Write(Encoding.UTF8.GetBytes(Data), 0, len);
            return GetResponseText(req,Encoding.UTF8.GetBytes(Data));
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
        public string UploadString(string URL, string Data)
        {
            return UploadString(URL, Data, "application/x-www-form-urlencoded");
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
        public string UploadMultipartData(string URL, byte[] Data, string boundary)
        {
            HttpWebRequest req = PresetRequest(URL);
            if (boundary != null)
                req.ContentType = "multipart/form-data; boundary=" + boundary;
            req.Method = "POST";
            req.ContentLength = Data.Length;
            //req.GetRequestStream().Write(Data,0,Data.Length);

            return GetResponseText(req,Data);
        }

        public Stream UploadMultipartDataStream(string URL, byte[] Data, string boundary)
        {
            HttpWebRequest req = PresetRequest(URL);
            if (boundary != null)
                req.ContentType = "multipart/form-data; boundary=" + boundary;
            req.Method = "POST";
            req.ContentLength = Data.Length;
            //req.GetRequestStream().Write(Data,0,Data.Length);

            return GetResponseStream(req, Data);
        }


        private void FloodThread(object arg)
        {
            HttpWebRequest req = (HttpWebRequest)arg;
            try
            {
                GetResponseText(req,null);
            }
            catch { };
        }

		/// <summary>
		///Посылает энное количество запросов 
		/// </summary>
        public void Flood(string URL, int ThreadCount, string Referer, bool UsePost, string PostData)
        {

            HttpWebRequest[] reqs = new HttpWebRequest[ThreadCount];
            System.Threading.Thread[] threads = new System.Threading.Thread[ThreadCount];
            for (int c = 0; c < ThreadCount; c++)
            {
                reqs[c] = PresetRequest(URL);
                reqs[c].ServicePoint.ConnectionLimit = ThreadCount;
                reqs[c].Referer = Referer;
                
                if(UsePost)
                {
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

        public void Flood(string URL, int ThreadCount, string  Referer) { Flood(URL, ThreadCount, Referer,false, ""); }
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
		public string DownloadString(string URL)
        {
            DateTime now = DateTime.Now;
            TimeSpan ts = DateTime.Now - previousLoad;
            int delay = (ForcedDelay - ts.Milliseconds);
            if (delay > 0)
            {
                System.Threading.Thread.Sleep(delay);
            }
            previousLoad = DateTime.Now;

            HttpWebRequest req = PresetRequest(URL);
            return GetResponseText(req,null);
        }

		private string GetResponseText(HttpWebRequest req,byte[] data)
		{
			while(true)
			{
				try
				{
					return GetResponseText2(req,data);
				}
				catch(Exception e)
				{
					//TODO: Add error handling stuff here
					throw e;
				}
			}
		}
        private string GetResponseText2(HttpWebRequest req,byte[] data)
        {
			if(data!=null)	req.GetRequestStream().Write(data, 0, data.Length);
            HttpWebResponse resp = GetResponseWithTimeout(req);
            if (resp == null)
            {
                Timeouts++;
                throw new Exception("GetResponseText2: response wait timeout!");
            }
            Stream stream = resp.GetResponseStream();
            if (resp.GetResponseHeader("Content-Encoding").ToLower().Contains("gzip"))
                stream = new System.IO.Compression.GZipStream(stream,
                    System.IO.Compression.CompressionMode.Decompress);
            TextReader reader = new StreamReader(stream, UTF8Encoding.UTF8);
            return reader.ReadToEnd();
        }

        private Stream GetResponseStream(HttpWebRequest req, byte[] data)
        {
            while (true)
            {
                try
                {
                    return GetResponseStream2(req, data);
                }
                catch (Exception e)
                {
                    //TODO: Add error handling stuff here
                    throw e;
                }
            }
        }
        private Stream GetResponseStream2(HttpWebRequest req, byte[] data)
        {
            if (data != null) req.GetRequestStream().Write(data, 0, data.Length);
            HttpWebResponse resp = GetResponseWithTimeout(req);
            Stream stream = resp.GetResponseStream();
            if (resp.GetResponseHeader("Content-Encoding").ToLower().Contains("gzip"))
                stream = new System.IO.Compression.GZipStream(stream,
                    System.IO.Compression.CompressionMode.Decompress);
            return stream;
        }

		public byte[] DownloadData(string URL)
		{	
            HttpWebRequest req = PresetRequest(URL);
			HttpWebResponse resp = GetResponseWithTimeout(req);
            Stream stream = resp.GetResponseStream();
            if (resp.GetResponseHeader("Content-Encoding").ToLower().Contains("gzip"))
                stream = new System.IO.Compression.GZipStream(stream,
                    System.IO.Compression.CompressionMode.Decompress);
			int length=int.Parse(resp.GetResponseHeader("Content-Length"));
            
			byte[] rv=new byte[length];
			int roff=0,r=0;
			while((roff+r)!=length)
			{
				r=stream.Read(rv,roff,length-roff);
				roff+=r;
				r=0;
			}
			return rv;
		}

        private void GetResponseCallback(IAsyncResult ar)
        {
            ((AutoResetEvent)ar.AsyncState).Set();
        }


        private HttpWebResponse GetResponseWithTimeout(HttpWebRequest req)
        {
            AutoResetEvent ev=new AutoResetEvent(false);
            IAsyncResult result =req.BeginGetResponse(GetResponseCallback, ev);

            if (!ev.WaitOne(ResponseTimeout))
            {
                req.Abort();
                return null;
            }
            /*
            if (!HttpClientParams.WaitOneEmpty)
            {
                if (!ev.WaitOne(ResponseTimeout))
                {
                    req.Abort();
                    return null;
                }
            }
            else
                if (!ev.WaitOne())
                {
                    req.Abort();
                    return null;
                }
             */
                
            /*if (!m_Ajax) */Referer = req.RequestUri.ToString();
            return (HttpWebResponse)req.EndGetResponse(result);

        }

        private HttpWebRequest PresetRequest(string URL)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
			
            //req.KeepAlive = true;
            req.Expect = "";
            //Headers.Add("Keep-Alive: 300");
            req.Headers.Add(Headers);
            req.Accept="text/html, application/xml;q=0.9, application/xhtml+xml, image/png, image/jpeg, image/gif, image/x-xbitmap";
            req.Headers.Add("Accept-Encoding: gzip, identity");
            req.KeepAlive = false;
            req.CookieContainer = Cookies;
            req.Referer = Referer;
            req.UserAgent = UserAgent;
            req.Method = "GET";
            req.Proxy = m_Proxy;
			
            return req;
        }
        public HttpClient()
        {
            System.Net.ServicePointManager.Expect100Continue = false;
        }
    }
}
