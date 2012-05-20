using System;
using System.Collections.Generic;
using System.Text;
using NerZul.Core.Network;
using NerZul.Core.Utils;
using System.Drawing;
using System.IO;

namespace Core.Source.Network
{
    internal class HideMyAssProxyStruct
    {
        public string IP;
        public string PortImgURL;
        public string Port;

        public string FullProxy
        {
            get { return IP + ':' + Port; }
        }
    }

    public static class ProxyLoader
    {
        private static object locker = new object();

        private static string storePath = "";

        private static string[] delim0A = { "\x000A" };
        private static string[] delimBR = { "<br />" };

        private static List<Bitmap> hidemyassSamples = new List<Bitmap>();
        private static int hidemyassSamplesMaxWidth = 0;

        public static StringSelector LoadFromURL(string proxyURL, string proxyFilePath)
        {
            storePath = proxyFilePath;

            if (proxyURL.Contains("hidemyass.com"))
            {
                string proxyFile = storePath + @"\proxy.txt";
                if ((File.Exists(proxyFile)) &&
                    (File.GetLastWriteTime(proxyFile).AddHours(3) > DateTime.Now))
                {
                    return new StringSelector(proxyFile);
                }
                return new StringSelector(LoadHideMyAssText(proxyURL));
            }

            string response = (new HttpClient()).DownloadString(proxyURL);
            //ConsoleLog.WriteLine(response, "ProxyLog.txt");

            if (proxyURL.Contains("fineproxy.ru"))
                return new StringSelector(LoadFineProxy(response));

            return new StringSelector(LoadPlainList(response, delim0A));
        }

        public static string[] LoadPlainList(string data, string[] delimiters)
        {
            return data.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] LoadFineProxy(string data)
        {
            var localData = CommonUtils.GetStringBetween(data, "<strong>Анонимные прокси:</strong>", "</p>");
            return LoadPlainList(localData, delimBR);
        }

        public static string[] LoadHideMyAssText(string proxyURL)
        {
            HttpClient loader = new HttpClient();
            List<string> proxies = new List<string>();

            string localData;
            string proxyInfo;

            string response = "";

            try
            {
                response = loader.DownloadString(proxyURL);
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLine("LoadHideMyAss: load 1 error. " + e.Message);
            }

            int page = 1;
            //ConsoleLog.WriteLine(response, "ProxyLog1.txt");

            while (response.Contains("leftborder timestamp"))
            {
                ConsoleLog.WriteLine(proxyURL + '/' + page.ToString());

                localData = CommonUtils.GetStringBetween(response, "Anonymity</td>", "pagination");
                //ConsoleLog.WriteLine(localData, "ProxyLog2.txt");

                while (localData.Contains("<tr class=\"\"  rel=\"") || 
                       localData.Contains("<tr class=\"altshade\"  rel=\""))
                {
                    string proxyIP = "";
                    string proxyPort = "";

                    int index = 0, index1 = 0, index2 = 0;
                    index1 = localData.IndexOf("<tr class=\"\"  rel=\"");
                    index2 = localData.IndexOf("<tr class=\"altshade\"  rel=\"");
                    if (index1 == -1)
                    {
                        index = index2 +18;
                    }
                    else if (index2 == -1)
                    {
                        index = index1 + 10;
                    }
                    else
                    {
                        index = Math.Min(index1 + 10, index2 + 18);
                    }
                    localData = localData.Substring(index);

                    proxyInfo = CommonUtils.GetStringBetween(localData, "<!--", "</tr>");
                    //ConsoleLog.WriteLine(proxyInfo, "ProxyLog3.txt");
                    if (proxyInfo.Contains("planetlab"))
                    {
                        proxyIP = CommonUtils.GetStringBetween(proxyInfo, "title=\"Planet Lab proxy\">", "</span>");
                        ConsoleLog.WriteLine(proxyIP + " - PlanetLab");
                        continue;
                    }

                    proxyIP = CommonUtils.GetStringBetween(proxyInfo, "<td><span>", "</span></td>");
                    proxyPort = CommonUtils.GetStringBetween(proxyInfo, "<td>" + (char)10, "</td>");

                    if (proxyInfo.Contains("None"))
                    {
                        ConsoleLog.WriteLine(proxyIP + " - not anonymous");
                        continue;
                    }

                    proxies.Add(proxyIP + ":" + proxyPort);
                }

                ConsoleLog.WriteLine("Proxies loaded: " + proxies.Count.ToString());
                page++;
                //if (page > 3) break; // Debug
                try
                {
                    response = loader.DownloadString(proxyURL + '/' + page.ToString());
                    //ConsoleLog.WriteLine(response, "ProxyLog1.txt");
                }
                catch (Exception e)
                {
                    ConsoleLog.WriteLine("LoadHideMyAss: load 2 error. " + e.Message);
                }
            }

            if (proxies.Count > 0)
            {
                var streamWriter = new StreamWriter(
                    storePath + "\\proxy.txt",
                    false);
                foreach (string proxy in proxies)
                {
                    streamWriter.WriteLine(proxy);
                }
                streamWriter.Close();
            }

            return proxies.ToArray();
        }

        public static string[] LoadHideMyAss(string proxyURL)
        {
            HttpClient loader = new HttpClient();
            List<HideMyAssProxyStruct> proxies = new List<HideMyAssProxyStruct>();
            string baseURL = "http://hidemyass.com";
            string portImg = "/proxy-list/img/port";
            
            string localData;
            string proxyInfo;

            string response = "";

            try
            {
                response = loader.DownloadString(proxyURL);
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLine("LoadHideMyAss: load 1 error. " + e.Message);
            }

            int page = 1;
            ConsoleLog.WriteLine(response, "ProxyLog1.txt");

            while (response.Contains(portImg))
            {
                ConsoleLog.WriteLine(proxyURL + '/' + page.ToString());

                localData = CommonUtils.GetStringBetween(response, "Anonymity</td>", "pagination");
                ConsoleLog.WriteLine(localData, "ProxyLog2.txt");

                while (localData.Contains("<tr class=\"row"))
                {
                    localData = localData.Substring(localData.IndexOf("<tr class=\"row") + 10);

                    //proxyInfo = CommonUtils.GetStringBetween(localData, "<!--", "style=\"margin");
                    proxyInfo = CommonUtils.GetStringBetween(localData, "<!--", "</tr>");
                    ConsoleLog.WriteLine(proxyInfo, "ProxyLog3.txt");
                    if (proxyInfo.Contains("planetlab"))
                    {
                        string proxyIP = CommonUtils.GetStringBetween(proxyInfo, "gif\">", "</td>");
                        ConsoleLog.WriteLine(proxyIP + " - PlanetLab");
                        continue;
                    }

                    HideMyAssProxyStruct proxy = new HideMyAssProxyStruct();

                    proxy.IP = CommonUtils.GetStringBetween(proxyInfo, "<td>", "</td>");

                    if (proxyInfo.Contains("None"))
                    {
                        ConsoleLog.WriteLine(proxy.IP + " - not anonymous");
                        continue;
                    }

                    proxy.PortImgURL = baseURL + CommonUtils.GetStringBetween(proxyInfo, "<img src=\"", "\"");

                    proxies.Add(proxy);
                }

                page++;
                //if (page > 3) break; // Debug
                try
                {
                    response = loader.DownloadString(proxyURL + '/' + page.ToString());
                    ConsoleLog.WriteLine(response, "ProxyLog1.txt");
                }
                catch (Exception e)
                {
                    ConsoleLog.WriteLine("LoadHideMyAss: load 2 error. " + e.Message);
                }
            }

            LoadHideMyAssCaptchaPreloader();

            const int poolSize = 10;
            System.Threading.Thread[] pool = new System.Threading.Thread[poolSize];
            int count = proxies.Count;
            int i;
            
            i = 0;
            while (i < count)
            {
                int freeThread = -1;

                for (int ft = 0; ft < poolSize; ft++)
                {
                    if ((pool[ft] == null) || (!pool[ft].IsAlive))
                    {
                        freeThread = ft;
                        break;
                    }
                }

                if (freeThread == -1)
                {
                    System.Threading.Thread.Sleep(100);
                    continue;
                }

                ConsoleLog.WriteLine(
                    "Processing proxy " + (i + 1).ToString() + "/" + count.ToString() + 
                    ", thread " + (freeThread+1).ToString());
                pool[freeThread] = new System.Threading.Thread(LoadHideMyAssCaptcha);
                pool[freeThread].Start(proxies[i]);
                
                i++;
            }

            i = 0;
            while (i < poolSize)
            {
                if ((pool[i % poolSize] != null) && (pool[i % poolSize].IsAlive))
                {
                    System.Threading.Thread.Sleep(200);
                    continue;
                }

                i++;
            }

            List<string> result = new List<string>();
            foreach (HideMyAssProxyStruct proxy in proxies)
            {
                if (!String.IsNullOrEmpty(proxy.Port))
                    result.Add(proxy.FullProxy);
            }

            if (result.Count > 0)
            {
                var streamWriter = new StreamWriter(
                    storePath + "\\proxy.txt",
                    false);
                foreach (string proxy in result)
                {
                    streamWriter.WriteLine(proxy);
                }
                streamWriter.Close();
            }

            return result.ToArray();
        }

        private static Color LoadHideMyAssConvertBW(Color color, int whiteLevel)
        {
            if (color.R + color.G + color.B >= whiteLevel * 3)
                return Color.White;
            else
                return Color.Black;
        }

        private static Bitmap LoadHideMyAssCaptchaCutSource(Bitmap source, int whiteLevel)
        {
            if (whiteLevel == 0)
                whiteLevel = source.GetPixel(0, 0).R;

            int yStart = 0, yEnd = 0;
            bool found;

            found = false;
            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    if (LoadHideMyAssConvertBW(source.GetPixel(x, y), whiteLevel) != Color.White)
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    yStart = y;
                    break;
                }
            }

            for (int y = yStart+1; y < source.Height; y++)
            {
                found = false;
                for (int x = 0; x < source.Width; x++)
                {
                    if (LoadHideMyAssConvertBW(source.GetPixel(x, y), whiteLevel) != Color.White)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    yEnd = y;
                    break;
                }
            }

            if (yEnd == 0)
                yEnd = source.Height - 1;

            Bitmap result = new Bitmap(source.Width, yEnd - yStart);

            for (int y = 0; y < result.Height; y++)
            {
                for (int x = 0; x < result.Width; x++)
                {
                    result.SetPixel(x, y, LoadHideMyAssConvertBW(source.GetPixel(x, y + yStart), whiteLevel));
                }
            }

            return result;
        }

        private static bool LoadHideMyAssCaptchaCompareSamples(Bitmap sampleBigBitmap, int sampleBigOffset, Bitmap sampleSmallBitmap)
        {
            bool fail = false;
            for (int x = 0; x < Math.Min(sampleBigBitmap.Width, sampleSmallBitmap.Width); x++)
            {
                for (int y = 0; y < Math.Min(sampleBigBitmap.Height, sampleSmallBitmap.Height); y++)
                {
                    var color1 = sampleBigBitmap.GetPixel(sampleBigOffset + x, y).ToArgb();
                    var color2 = sampleSmallBitmap.GetPixel(x, y).ToArgb();
                    if (color1 != color2)
                    {
                        fail = true;
                        break;
                    }
                }

                if (fail) 
                    break;
            }
            return (!fail);
        }

        private static void LoadHideMyAssCaptchaPreloader()
        {
            if (hidemyassSamples.Count == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Bitmap loadSample = LoadHideMyAssCaptchaCutSource(new Bitmap(storePath + @"\hma\" + i.ToString() + ".bmp"), 255);
                    hidemyassSamplesMaxWidth = Math.Max(hidemyassSamplesMaxWidth, loadSample.Width);
                    hidemyassSamples.Add(loadSample);
                }
            }
        }

        // update Port in proxy class
        private static void LoadHideMyAssCaptcha(HideMyAssProxyStruct proxy)
        {
            HttpClient connection = new HttpClient();

            string num = "";
            byte[] bitmapData = null;

            try
            {
                bitmapData = connection.DownloadData(proxy.PortImgURL);
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLine("LoadHideMyAss: load 3 error. " + e.Message);
                return;
            }

            //Bitmap source = new Bitmap(new MemoryStream(bitmapData));
            //source.Save(@"C:\source" + count.ToString() + ".bmp");
            Bitmap captcha = LoadHideMyAssCaptchaCutSource(new Bitmap(new MemoryStream(bitmapData)), 0);
            //Bitmap captcha = CaptchaCutSource(new Bitmap(@"C:\source.bmp"));
            //captcha.Save(@"C:\cut" + count.ToString() + ".bmp");

            for (int captchaX = 0; captchaX < captcha.Width - hidemyassSamplesMaxWidth; captchaX++)
            {
                foreach (Bitmap trySample in hidemyassSamples)
                {
                    Bitmap lSample;
                    lock (locker)
                    {
                        lSample = new Bitmap(trySample);
                    }

                    if (LoadHideMyAssCaptchaCompareSamples(captcha, captchaX, lSample))
                    {
                        num += hidemyassSamples.IndexOf(trySample).ToString();
                        break;
                    }
                }
            }

            proxy.Port = num.ToString();

            return;
        }

        // Wrapper for threading
        private static void LoadHideMyAssCaptcha(object proxy)
        {
            LoadHideMyAssCaptcha(proxy as HideMyAssProxyStruct);
        }
    }
}
