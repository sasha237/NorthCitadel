using System;
using System.Collections.Generic;
using System.Text;
using NerZul.Core.Network;
using NerZul.Core.Utils;

namespace eRepCompanyChecker
{
    public class TraderBot : Bot
    {
        public TraderBot(string UserName, string Email, string Password) :
            base(UserName, Email, Password, false)
        {
        }
        public TraderBot(string UserName, string Email, string Password, string UserAgent, string autocaptcha, int captchaBufferSize) :
            base(UserName, Email, Password, UserAgent, autocaptcha, captchaBufferSize, false)
        {
        }

        public int FindGoodOffer()
        {
            m_Response = m_Client.DownloadString("http://www.erepublik.com/en/economy/inventory");

            if (!m_Response.Contains("id=\"offer_"))
            {
                return 0;
            }

            const string offerStr = "<strong class=\"offer_amount\">";
            string offerAmount;
            offerAmount = m_Response.Substring(m_Response.IndexOf(offerStr) + offerStr.Length);
            offerAmount = offerAmount.Substring(0, offerAmount.IndexOf("</strong>"));
            offerAmount = offerAmount.Replace(",", "");

            //ConsoleLog.WriteLine("offerAmount=" + offerAmount);

            return Convert.ToInt32(offerAmount);
        }

        public int GetRemains(string industry, string quality, bool loadInventory)
        {
            //ConsoleLog.WriteLine("m_Response=" + m_Response);
            if (loadInventory)
            {
                m_Response = m_Client.DownloadString("http://www.erepublik.com/en/economy/inventory");
            }

            string itemPositionStr = "stock_" + industry + "_" + quality;

            if (!m_Response.Contains(itemPositionStr))
            {
                return 0;
            }

            string remainsStr = "<strong id=\"" + itemPositionStr + "\">";
            string remainsAmount;
            remainsAmount = m_Response.Substring(m_Response.IndexOf(remainsStr) + remainsStr.Length);
            remainsAmount = remainsAmount.Substring(0, remainsAmount.IndexOf("</strong>"));
            remainsAmount = remainsAmount.Replace(",", "");

            //ConsoleLog.WriteLine("remainsAmount=" + remainsAmount);

            return Convert.ToInt32(remainsAmount);
        }

        public bool SetOnSale(string country, string industry, string quality, string amount, string price, bool loadInventory)
        {
            //ConsoleLog.WriteLine("m_Response=" + m_Response);

            if (loadInventory)
            {
                m_Response = m_Client.DownloadString("http://www.erepublik.com/en/economy/inventory");
            }

            m_Client.Headers.Add("X-Requested-With", "XMLHttpRequest");
            string PostData =
                "industryId=" + industry +
                "&customization=" + quality +
                "&amount=" + amount +
                "&price=" + price +
                "&countryId=" + country +
                "&_token=" + ms_Token2;
            m_Response = m_Client.UploadString("http://www.erepublik.com/en/economy/postMarketOffer", PostData);

            //ConsoleLog.WriteLine("Responce: " + m_Response);
            m_Client.Headers.Remove("X-Requested-With");

            //ConsoleLog.WriteLine("SetOnSale=" + m_Response);

            return (m_Response.Contains("success"));
        }
    }
}
