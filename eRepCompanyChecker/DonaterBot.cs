using System;
using System.Collections.Generic;
using System.Text;
using NerZul.Core.Network;
using System.Text.RegularExpressions;
using NerZul.Core.Utils;

namespace eRepCompanyChecker
{
    public class DonaterBot : Bot
    {
        private string Pin = "";

        public DonaterBot(string UserName, string Email, string Password, string Pin) :
            base(UserName, Email, Password, false)
        {
            this.Pin = Pin;
        }
        public DonaterBot(string UserName, string Email, string Password, string Pin, string UserAgent, string autocaptcha, int captchaBufferSize) :
            base(UserName, Email, Password, UserAgent, autocaptcha, captchaBufferSize, false)
        {
            this.Pin = Pin;
        }

        public bool CheckPin(bool showMessage)
        {
            bool Res = m_Response.Contains("Personal Security PIN");

            if (Res && showMessage)
            {
                ConsoleLog.WriteLine("PIN code page opened...");
            }

            return Res;
        }

        public void SubmitPin()
        {
            m_Client.Referer = "http://www.erepublik.com/en/pin";
            string PostData = "_token=" + ms_Token2;
            PostData += "&pin=" + Pin;
            PostData += "&commit=Unlock";
            //ConsoleLog.WriteLine(PostData, "DeletePostData.txt");
            m_Response = m_Client.UploadString("http://www.erepublik.com/en/pin", PostData);
            if (CheckPin(false))
            {
                ConsoleLog.WriteLine(m_Response, "Responses.txt");
                throw new Exception("PIN entering failed, see file Responses.txt");
            }
            else
            {
                ConsoleLog.WriteLine("PIN entered successfully");
            }
        }

        public bool DonateItem(string amount, string industry, string quality, string token, string srcPage)
        {
            bool needCaptcha = m_Response.Contains("has_captcha = 1");
            //The CAPTCHA solution is incorrect.
            m_Client.Referer = srcPage;

            do
            {
                string PostData =
                    System.Web.HttpUtility.UrlEncode("donate_form[amount]") + "=" + amount + "&" +
                    System.Web.HttpUtility.UrlEncode("donate_form[industryId]") + "=" + industry + "&" +
                    System.Web.HttpUtility.UrlEncode("donate_form[quality]") + "=" + quality + "&" +
                    System.Web.HttpUtility.UrlEncode("donate_form[_csrf_token]") + "=" + token;

                if (needCaptcha)
                {
                    ConsoleLog.WriteLine("Donate captcha");
                    var captcha = CaptchaProvider.GetResolvedCaptcha();
                    string sToken = CommonUtils.GetToken(m_Response);
                    PostData =
                        PostData + "&" +
                        "recaptcha_challenge_field=" + captcha.ChallengeID + "&" +
                        "&recaptcha_response_field=" + System.Web.HttpUtility.UrlEncode(captcha.CaptchaText) + "&" +
                        "&commit=Continue";
                }

                //ConsoleLog.WriteLine(PostData);
                m_Response = m_Client.UploadString(srcPage, PostData);
                //ConsoleLog.WriteLine(m_Response, "DonateMoney.txt");

                needCaptcha = (needCaptcha && m_Response.Contains("has_captcha = 1"));
                if (needCaptcha)
                    ConsoleLog.WriteLine(m_Response, "DonateItems.txt");
            }
            while (needCaptcha);

            return (m_Response.Contains("Successfuly transfered"));
        }
    }
}
