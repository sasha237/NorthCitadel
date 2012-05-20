using System;
using NerZul.Core.Network;
using System.Text.RegularExpressions;
using Core.Source.Network;
using System.Net;

namespace NerZul.Core.Utils
{
    public class ManagedBotConfig
    {
        public StringSelector UserAgentList;
        public StringSelector ProxyList;
        public bool DisableProxyAfterLogin = false;
        public bool Verbose = false;
        public int precaptchaBufferSize;
        public string AntiGateKey;
        public bool useTOR = false;

        public static string proxyURL = "";
        public bool proxyAuthorisation = false;
        public string proxyLogin = "";
        public string proxyPassword = "";
        private NetworkCredential m_proxyCredentials = null;
        public bool bBeep = false;
        public NetworkCredential proxyCredentials
        {
            get
            {
                if ((m_proxyCredentials == null) && 
                    (proxyAuthorisation) &&
                    (!String.IsNullOrEmpty(proxyLogin)))
                    m_proxyCredentials = new NetworkCredential(proxyLogin, proxyPassword);

                return m_proxyCredentials;
            }
        }
    }

    public class BasicBotInfo
    {
        public int Level = 0;
        public int Experience = 0;
        public double Wellness = 0;
        public double Gold = 0;
        public double Nat_occur = 0;

        public int CitizenID = 0;
        public bool HasAvatar = false;
        public int Country = 0;
    }

    public class StorageBotInfo
    {
        /// <summary>
        /// Amount of food items in storage
        /// </summary>
        public int foodQty = -1;

        /// <summary>
        /// Total amount of items in storage
        /// </summary>
        public int itemsQty = -1;
    }

    public class ManagedCitizen
    {
        public enum LoginResult
        {
            Success, InvalidPassword, Fail, Banned, Banned2, ResendMail
        }
        public enum WorkResult
        {
            Success, Unemployed, Fail
        }
        private ManagedBotConfig m_Config;

        public readonly Citizen Bot;

        public readonly BasicBotInfo Info = new BasicBotInfo();

        public readonly StorageBotInfo storageInfo = new StorageBotInfo();

        public ManagedCitizen(string Login, string Email, string Password, ManagedBotConfig config)
        {
            if ((config == null) || (Email == null) || (Password == null))
                throw new ArgumentNullException();
            m_Config = config;
            Bot = new Citizen(Login, Email, Password, config.UserAgentList.GetRandomString(), m_Config.AntiGateKey, m_Config.precaptchaBufferSize, config.bBeep);
        }

        public bool GetInfoFromCommonResponse()
        {
            return GetInfoFromCommonResponse(false);
        }

        public bool GetInfoFromCommonResponse(bool loadMainPage)
        {
            int xLine = 0;
            string m_Response = String.Empty;
            string sLevel = String.Empty;
            string sWellness = String.Empty;
            string sNat_occur = String.Empty;
            string sExperience = String.Empty;
            string sGold = String.Empty;
            string tmpResponse;

            try
            {
                /*  0 */
                if (loadMainPage)
                    Bot.CustomRequest("http://www.erepublik.com/en");

                /*  1 */
                m_Response = Bot.GetLastResponse(); xLine++;
                if (m_Response.Contains("Internal Server Error"))
                {
                    xLine = 0;
                    throw new Exception("500 - Internal Server Error");
                }
                /*  2 */
                sLevel = CommonUtils.GetStringBetween(
                    m_Response,
                    "Experience level: <strong>",
                    "</strong>"); xLine++;
                //sLevel = Regex.Match(m_Response, @"Experience level:\s*<strong>(\w+)<").Groups[1].Value; xLine++;
                /*  3 */
                Info.Level = int.Parse(sLevel); xLine++;
                /*  4 */
                sExperience = CommonUtils.GetStringBetween(
                    m_Response,
                    "Experience points: <strong>",
                    "</strong>"); xLine++;
                //sExperience = Regex.Match(m_Response, @"Experience points:\s*<strong>(\w+)<").Groups[1].Value; xLine++;
                /*  5 */
                Info.Experience = int.Parse(sExperience); xLine++;
                /*  6 */
                sWellness = CommonUtils.GetStringBetween(
                    m_Response,
                    "<strong id=\"current_health\">",
                    "</strong>"); xLine++;
                /*
                sWellness = Regex.Match(m_Response,
                    "wellness_small.png\" alt=\"\" />\n\\s*<span>(.*?)<")
                   .Groups[1].Value; xLine++;
                if (String.IsNullOrEmpty(sWellness))
                    /  7 /
                    sWellness = Regex.Match(m_Response,
                    "wellness_small.png\" alt=\"\" />\n\\s*<span>(.*?)<")
                       .Groups[1].Value; xLine++;
                 */
                /*  7 */
                Info.Wellness = double.Parse(sWellness, System.Globalization.CultureInfo.InvariantCulture); xLine++;

                /*  8 */
                string sCountry = CommonUtils.GetStringBetween(
                    m_Response,
                    "<a href=\"http://economy.erepublik.com/en/market/job/",
                    "\">"); xLine++;
                //string sCountry = Regex.Match(m_Response, "erepublik.com/en/market/job/(\\w+)\"").Groups[1].Value; xLine++;
                /* 9 */
                int.TryParse(sCountry, out Info.Country); xLine++;

                ///* 11 */
                //Regex regex = new Regex(
                //                "<div class=\"avatarholder\">\\s*<div class=\"backwhite\">\\s" +
                //                "*<a href=\"\\/\\w{2}\\/citizen\\/profile\\/(\\d{1,10})\"><img al" +
                //                "t=\".*\" width=\"55\" height=\"55\" src=\"(.*)\" \\/><\\/a>",
                //        RegexOptions.Multiline
                //        );
                //Match match = regex.Match(m_Response);
                //xLine++;
                //if (match.Success)
                //{
                //    /* 12 */
                //    Info.CitizenID = Convert.ToInt32(match.Groups[1].Value);
                //    xLine++;
                //    /* 13 */
                //    Info.HasAvatar = ((match.Groups[2].Value == "http://static.erepublik.com/images/default_avatars/Citizens/default.gif") ? false : true);
                //    xLine++;
                //}
                //else
                //    throw new Exception("CitizenID not detected");

                tmpResponse = CommonUtils.GetStringBetween(
                    m_Response,
                    "<div class=\"user_section\">",
                    "</div>");

                /* 10 */
                Info.HasAvatar = tmpResponse.Contains("http://static.erepublik.com/uploads/avatars/Citizens"); xLine++;
                /* 11 */
                string sProfile = CommonUtils.GetStringBetween(
                    tmpResponse,
                    "/en/citizen/profile/",
                    "\""); xLine++;
                //string sProfile = Regex.Match(m_Response, "/en/citizen/profile/(\\w+)\">").Groups[1].Value; xLine++;
                /* 12 */
                Info.CitizenID = int.Parse(sProfile); xLine++;

                /* 13 */
                sGold = CommonUtils.GetStringBetween(
                    m_Response,
                    "<strong id=\"side_bar_gold_account_value\">",
                    "</strong>"); xLine++;
                //sGold = Regex.Match(m_Response, "side_bar_gold_account_value\">\\s*(\\w+)\\s*<").Groups[1].Value; xLine++;
                //ConsoleLog.WriteLine("sGold = " + sGold);
                /* 14 */
                Info.Gold = double.Parse(sGold, System.Globalization.CultureInfo.InvariantCulture); xLine++;

                /* 15 */
                tmpResponse = CommonUtils.GetStringBetween(
                    m_Response,
                    "<div class=\"currency_amount\">",
                    "</span>");
                sNat_occur = CommonUtils.GetStringBetween(
                    tmpResponse,
                    "<strong>",
                    "</strong>"); xLine++;
                //sNat_occur = Regex.Match(m_Response, "<img\\s*class=\"flag\"\\s*alt=\"\\w*\"\\s*title=\"\\w*\"\\s*src=\"/images/flags/S/[\\w\\-]*.gif\"\\s*/>\\s*(\\d+)\\s*</a>").Groups[1].Value; xLine++;
                //if (String.IsNullOrEmpty(sNat_occur))
                //    /*  17 */
                //    sNat_occur = Regex.Match(m_Response, "side_bar_natural_account_value\">(\\w*)<").Groups[1].Value; xLine++;
                //ConsoleLog.WriteLine("sNat_occur = " + sNat_occur);
                /* 16 */
                Info.Nat_occur = double.Parse(sNat_occur, System.Globalization.CultureInfo.InvariantCulture); xLine++;
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLine("GetInfoFromCommonResponse error, (xLine=" + xLine.ToString() + "): " + e.Message);
                if (xLine != 0)
                {
                    ConsoleLog.WriteLine("sLevel=" + sLevel);
                    ConsoleLog.WriteLine("sWellness=" + sWellness);
                    ConsoleLog.WriteLine("sGold=" + sGold);
                    ConsoleLog.WriteLine("sNat_occur=" + sNat_occur);

                    ConsoleLog.WriteLine(m_Response, "GetInfoFromCommonResponseLog.txt");
                    //Console.ReadKey();
                }

                return false;
            }
            return true;

        }

        public bool GetHealthWellInfoFromCommonResponse()
        {
            string m_Response = "";

            try
            {
                m_Response = Bot.GetLastResponse();
                string sWellness = CommonUtils.GetStringBetween(
                    m_Response,
                    "\"health\":",
                    ",");
                //string sWellness = Regex.Match(m_Response,
                //    //                    "\"health\":\"([0-9.]{1,9}).")
                //    "\"health\":\"(.*?)\"")
                //    .Groups[1].Value;
                Info.Wellness = double.Parse(sWellness, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLine("GetHealthWellInfoFromCommonResponse error: " + e.Message);
                ConsoleLog.WriteLine("m_Response=" + m_Response);
                return false;
            }
            return true;
        }

        public bool GetFoodRemainingInfoFromEatFoodResponse()
        {
            string m_Response = "";

            m_Response = Bot.GetLastResponse();
            string sFoodRemains = CommonUtils.GetStringBetween(
                m_Response,
                "\"has_food_in_inventory\":",
                ",");
            //string sFoodRemains = Regex.Match(m_Response,
            //    @"""has_food_in_inventory""\s?:\s?(\d+)")
            //    .Groups[1].Value;
            return (Convert.ToInt32(sFoodRemains) > 0);
        }

        public bool GetStorageInfo(bool loadInventoryPage)
        {
            if (loadInventoryPage)
                Bot.VisitInventory();

            string response = Bot.GetLastResponse();

            if (!response.Contains("inventory_overview"))
                return false;

            response = CommonUtils.GetStringBetween(
                response,
                "inventory_overview",
                "inventory_sell");

            int foodQty = 0;
            int itemsQty = 0;

            while (response.Contains("<strong id=\"stock_"))
            {
                string item = CommonUtils.GetStringBetween(
                    response,
                    "<strong id=\"stock",
                    "/strong");

                string sType = CommonUtils.GetStringBetween(
                    item,
                    "_",
                    "_");

                string sQty = CommonUtils.GetStringBetween(
                    item,
                    ">",
                    "<");

                ConsoleLog.WriteLine("sType=" + sType + ", sQty=" + sQty);

                int qty = Convert.ToInt32(sQty);

                itemsQty += qty;

                if (sType == "1")
                    foodQty += qty;

                response = CommonUtils.GetStringFrom(
                    response,
                    "<strong id=\"stock_");
            }

            storageInfo.foodQty = foodQty;
            storageInfo.itemsQty = itemsQty;

            return true;
        }

        public LoginResult Login()
        {
            return Login(10);
        }

        public LoginResult Login(int retryCount)
        {
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    if (!m_Config.useTOR)
                    {
                        Bot.HttpClient.SetProxy(m_Config.ProxyList.GetRandomString(), m_Config.proxyCredentials);
                    }
                    else
                    {
                        Bot.HttpClient.SetProxy("127.0.0.1:8118", null);//m_Config.ProxyList.GetString(0);

                        if (!(new TorClient(Bot.HttpClient.GetProxy(), "")).GetNewIP())
                            throw new Exception("Error getting new TOR IP");

                        ConsoleLog.WriteLine("TOR new IP obtained!");
                    }

                    if (Bot.Login())
                    {
                        if (m_Config.DisableProxyAfterLogin)
                        {
                            //Bot.HttpClient.Proxy=null;
                            //Bot.ConnectionTimeout=40;
                        }
                        GetInfoFromCommonResponse();
                        return LoginResult.Success;
                    }
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("Resending mail"))
                    {
                        return LoginResult.ResendMail;
                    }
                    if (e.Message.Contains("Banned2"))
                    {
                        ConsoleLog.WriteLine("Return to login page: login suspected to be banned");
                        return LoginResult.Banned2;
                    }
                    if (e.Message.Contains("Banned"))
                    {
                        ConsoleLog.WriteLine("Login is banned");
                        return LoginResult.Banned;
                    }

                    ConsoleLog.WriteLine("Bad proxy - " + Bot.HttpClient.GetProxy() + " (" + e.Message + ")");
                };
                //TODO: проверка на левый пасс и НЁХ
                //if(Bot.GetLastResponse().Contains
            }
            return LoginResult.Fail;
        }

        public WorkResult Work()
        {
            Bot.CustomRequest("http://www.erepublik.com/en/my-places/company/1");
            if ((Bot.GetLastResponse().Contains("You have not worked today")) ||
               (Bot.GetLastResponse().Contains("You are now an employee of this company")))
            {
                //Можно и поработать
                if (Bot.TryToWork())
                    return WorkResult.Success;
                else return WorkResult.Fail;
            }
            else
                if (Bot.GetLastResponse().Contains("You have worked today"))
                    return WorkResult.Success;
                else
                    if (Bot.GetLastResponse().Contains("You do not have a job"))
                        return WorkResult.Unemployed;
                    else
                        return WorkResult.Fail;
        }

        public bool Heal()
        {
            Bot.CustomRequest("http://www.erepublik.com/en/citizen/profile/" + Info.CitizenID);
            string resp = Bot.GetLastResponse();
            string Country = Regex.Match(resp, "en/country/(\\w+)\"").Groups[1].Value;
            string Region = Regex.Match(resp, "en/region/(([A-Za-z\\-]{1,500}))\"").Groups[1].Value;
            string PostData = Bot.GetTokenArg(1) + "&region=" + Region;
            Bot.CustomRequest("http://www.erepublik.com/en/hospital/" + Country, PostData);
            return Bot.GetLastResponse().Contains("You have recovered ");
        }

        public void Feed(bool doDelay, int country, int food_quality, int for_health, bool justEat, bool fastFood)
        {
            Random rnd = new System.Random();

            const int reTryCount = 3;
            int tryCount = 30;    /* Если по параметрам должен есть но ничего не происходит -- 
                                     делаем не больше tryCount попыток */
            int reTryNum = 0;

            bool forceBuyOne = false;

            // Стартовые значение для запуска, дальше не используется
            double iOldWell = 0, iOldCurrLeft = 0;
            bool iOldHasFood = false;
            double iNewWell = Info.Wellness;
            double iNewCurrLeft = Info.Nat_occur;
            bool iNewHasFood = true;

            ConsoleLog.WriteLine(Bot.LoginName + " - Buy and eat food");

            while ((Info.Wellness < for_health) &&
                   (tryCount > 0) &&
                   ((iNewHasFood) ||
                    (iNewCurrLeft > 0)))
            {
                iOldWell = iNewWell;
                iOldHasFood = iNewHasFood;
                iOldCurrLeft = iNewCurrLeft;

                if ((!justEat) && (!iOldHasFood))
                {
                    // Buy food
                    int buyCount = 
                        Convert.ToInt32(
                            Math.Ceiling(
                                Convert.ToDouble((for_health - Convert.ToInt32(iOldWell))) / (food_quality * 2))); // Need for health

                    //ConsoleLog.WriteLine("buyCount = " + buyCount.ToString());

                    if (buyCount > 10) buyCount = 10;
                    if ((forceBuyOne) || (buyCount < 1)) buyCount = 1;
                    if (forceBuyOne) forceBuyOne = false;

                    ConsoleLog.WriteLine(
                        Bot.LoginName + " - Buy " + buyCount.ToString() + " food.");

                    int buyRes = 0;

                    for (int i = 1; i <= 3; i++)
                    {
                        try
                        {
                            if (doDelay) System.Threading.Thread.Sleep(rnd.Next(500, 1000));
                            buyRes = Bot.BuyItem(int.Parse(country.ToString()), Goods.Food, buyCount, food_quality, iOldCurrLeft, true);
                            if (buyRes == -1)
                                forceBuyOne = true;
                            GetInfoFromCommonResponse();
                            ConsoleLog.WriteLine(
                                Bot.LoginName + " - after buy food. " +
                                "Currency left: " + Info.Nat_occur.ToString());
                            break;
                        }
                        catch (Exception e)
                        {
                            ConsoleLog.WriteLine("Error buy food: " + e.Message);
                        }
                    }

                    if (buyRes == 0)
                        break;
                }

                for (int i = 1; i <= 3; i++)
                {
                    try
                    {
                        // Eat food
                        if (doDelay) System.Threading.Thread.Sleep(rnd.Next(250, 500));
                        Bot.EatFood();

                        GetHealthWellInfoFromCommonResponse();
                        iNewHasFood = GetFoodRemainingInfoFromEatFoodResponse();
                        iNewWell = Info.Wellness;
                        iNewCurrLeft = Info.Nat_occur;

                        ConsoleLog.WriteLine(
                            Bot.LoginName + " - after eat food. " +
                            "Health=" + iNewWell.ToString() +
                            ", FoodLeft: " + iNewHasFood.ToString() +
                            ", Currency: " + iNewCurrLeft.ToString());

                        break;
                    }
                    catch (Exception e)
                    {
                        ConsoleLog.WriteLine("Error eat food: " + e.Message);
                    }
                }

                tryCount--;

                /* Если булки и/или деньги есть но ничего не меняеся --
                 * пробуем еще reTryCount. Если нет -- то отваливаем */
                if ((iOldWell == iNewWell) &&
                    (iOldHasFood == iNewHasFood) &&
                    (iOldCurrLeft == iNewCurrLeft))
                {
                    reTryNum++;
                }
                else
                {
                    reTryNum = 0;
                }
                if (reTryNum == reTryCount)
                    break;
            }
        }
    }
}
