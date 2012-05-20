using System;
using System.Collections.Generic;
using System.Text;
using NerZul.Core.Utils;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;

namespace NerZul.Core.Network
{
    [assembly: SuppressIldasmAttribute()]
    public class WebCitadel
    {
        private string key;

        private const string validateKey = @"<RSAKeyValue><Modulus>lXkh4rdEjLRAD31NXw+GwotQ9RhlB6B3kGkmIHytMeddPEUy85BiLBhIGYdJYs8zSppvSQZTMFcYaoyVNXk1DlByYmqzzbhyxAqFn42UHlgs+lyZ1YKrrzS6R0TZu8QE/JsFqnrtA6Kjz7JKFvd5ySaKWX+L8pSYghEt5+qmZpc=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        private const string logURL = @"http://northcitadel-5.hosting.parking.ru/Home/Log?";

        public WebCitadel()
        {
        }

        public bool Init(string key)
        {
            this.key = key;

            if (!String.IsNullOrEmpty(this.key))
            {
                // Do logon procedure
                // return false on failure
            }

            //if (!SendLogInfo(new string[] { "login" }, 0))
            //    return false;

            return true;
        }

        private byte[] encodeMD5(string data)
        {
            return //encryptedBytes1 =
                (new MD5CryptoServiceProvider()).
                    ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));

            //return Convert.ToBase64String(encryptedBytes1);
        }

        private bool validateReply(string signedData, byte[] dataToValidate)
        {
            RSACryptoServiceProvider vRSA = new RSACryptoServiceProvider();
            vRSA.FromXmlString(validateKey);

            var decryptedBytes =
                Convert.FromBase64String(signedData);

            return vRSA.VerifyHash(dataToValidate, CryptoConfig.MapNameToOID("MD5"), decryptedBytes);
        }

        public bool SendLogInfo(string[] taskAndParams, int botCount)
        {
            return true;
            StringBuilder logCommand = new StringBuilder();

            string ticks = System.DateTime.Today.Ticks.ToString();

            logCommand.Append(logURL);
            logCommand.Append("HashKey=" + key);
            logCommand.Append("&task=" + taskAndParams[0]);
            //logCommand.Append("&taskParams=" + "here_will_be_params");
            logCommand.Append("&nBot=" + botCount.ToString());
            logCommand.Append("&nData=" + ticks);

            byte[] expectedReply = encodeMD5(ticks);
            
            string callURL = logCommand.ToString();

            //ConsoleLog.WriteLine("Log URL: " + callURL);
            HttpClient client = new HttpClient();
            string response = "";

            for (int i = 1; i <= 20; i++)
            {
                try
                {
                    ConsoleLog.WriteLine("Authorising on NorthCitadel server, try " + i.ToString());
                    response = client.DownloadString(callURL);
                    string[] respLines = response.Split('\n');

                    if ((client.Referer == callURL) && (respLines.Length >= 2))
                    {
                        if (validateReply(respLines[0], expectedReply))
                        {
                            if (respLines[1].Contains("ALLOWED"))
                            {
                                ConsoleLog.WriteLine("Authorisation successfull");
                                return true;
                            }
                            if (respLines[1].Contains("DENIED"))
                            {
                                ConsoleLog.WriteLine("Authorisation failed: " + respLines[1]);
                                return false;
                            }
                            ConsoleLog.WriteLine("Authorisation error: " + respLines[1]);
                        }
                        else
                            ConsoleLog.WriteLine("Authorisation error: signature validation failed.");
                    }
                }
                catch (Exception e)
                {
                    //ConsoleLog.WriteLine(response, "AuthorisationLog.txt");
                    ConsoleLog.WriteLine("Authorisation failed: " + e.Message);
                }

                Thread.Sleep(15 * 1000);
            }

            return false;
        }
    }
}
