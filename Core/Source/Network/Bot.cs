using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using NerZul.Core.Utils;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NerZul.Core.Network
{
    [assembly: SuppressIldasmAttribute()]
    public class Pair2<T, U>
    {
        public Pair2()
        {
        }

        public Pair2(T first, U second)
        {
            this.First = first;
            this.Second = second;
        }

        public T First { get; set; }
        public U Second { get; set; }
    };

    public class OfferInfo : Dictionary<string, int>
    {
        public int GetValue(string key)
        {
            int o;
            if (!this.TryGetValue(key, out o))
            {
                throw new KeyNotFoundException();
            }
            return o;
        }
    }

    public class Bot
    {
        public ICaptchaProvider CaptchaProvider;
        protected string m_Response = "", m_AuthLogin, m_AuthEmail, m_AuthPassword;
        public HttpClient m_Client = new HttpClient();
        protected string ms_Token = "";
        protected string ms_Token2 = "";
        protected string ms_csrfToken = "";
        protected string ms_feedToken = "";
        protected string ms_companyToken = "";
        protected int m_RegionID = 0;

        public HttpClient HttpClient
        {
            get
            {
                return m_Client;
            }
        }
        protected const string mc_TokenScanString = "id=\"_token\" name=\"_token\" value=\"";

        /// <summary>
        /// ID аккаунта в eRepublik
        /// </summary>
        public readonly int AccountID = 0;

        /// <summary>
        /// Возвращает последний ответ, полученный с сервера
        /// </summary>
        /// <returns></returns>
        public string GetLastResponse()
        {
            return m_Response;
        }

        /// <summary>
        /// Основной конструктор с указанием данных, необходимых для логина бота
        /// </summary>
        /// <param name="UserName">Логин в eRepublik</param>
        /// <param name="Password">Пароль в eRepublik</param>
        /// <param name="UserAgent">Используемый UserAgent</param>
        public Bot(string UserName, string Email, string Password, string UserAgent, string autocaptcha, int captchaBufferSize, bool bBeep)
        {
            m_AuthLogin = UserName;
            m_AuthEmail = Email;
            m_AuthPassword = Password;
            m_Client.UserAgent = UserAgent;
            if (captchaBufferSize == 0)
            {
                if (autocaptcha.Length == 0)
                {
                    CaptchaProvider = new WinFormsCaptchaProvider(bBeep);
                }
                else
                {
                    CaptchaProvider = new AntigateCaptchaProvider(autocaptcha);
                }
            }
            else
            {
                CaptchaProvider = new PrecaptchaCaptchaProvider(autocaptcha, captchaBufferSize, bBeep);
            }
        }
        public Bot(string UserName, string Email, string Password, bool bBeep)
        {
            m_AuthLogin = UserName;
            m_AuthEmail = Email;
            m_AuthPassword = Password;
            CaptchaProvider = new WinFormsCaptchaProvider(bBeep);
        }
        /*
        /// <summary>
        /// Свойство, определяющее то время, которое будет производиться ожидания ответа на запрос.
        /// Устанавливается в миллисекундах
        /// </summary>
        public int ConnectionTimeout
        {
            get { return m_Client.Timeout; }
            set { m_Client.Timeout = value; }
        }
         */
        /// <summary>
        /// Выполняет процедуру авторизации в eRepublik. В это же время получает токены сессии.
        /// Для авторизации используются ранее запомненные параметрыXXX
        /// </summary>
        /// <returns></returns>
        public bool Login()
        {
            return Login("", "");
        }

        public bool Login(string captchaChallenge, string captchaText)
        {
            ConsoleLog.WriteLine(m_AuthLogin + ": Loggin in (" + m_AuthEmail + ")");
            m_Response = m_Client.DownloadString("http://www.erepublik.com/en");
            ms_Token = m_Response.Remove(0, m_Response.IndexOf(mc_TokenScanString) + mc_TokenScanString.Length);
            ms_Token = ms_Token.Substring(0, ms_Token.IndexOf("\""));
            string PostData = 
                "_token=" + ms_Token +
                "&citizen_email=" + System.Web.HttpUtility.UrlEncode(m_AuthEmail) + 
                "&citizen_password=" + System.Web.HttpUtility.UrlEncode(m_AuthPassword) +
                "&commit=Login";
            if (!String.IsNullOrEmpty(captchaChallenge))
            {
                PostData = 
                    PostData +
                    "&recaptcha_challenge_field=" + captchaChallenge +
                    "&recaptcha_response_field=" + System.Web.HttpUtility.UrlEncode(captchaText);
            }
            m_Response = m_Client.UploadString("http://www.erepublik.com/en/login", PostData);
            bool bOK = (m_Response.IndexOf("logout") != -1);
            if (!bOK)
            {
                if (m_Response.Contains("recaptcha"))
                {
                    ConsoleLog.WriteLine("Captcha required");
                    ConsoleLog.WriteLine(m_AuthLogin + ": Loggin in (" + m_AuthEmail + ")", "CaptchaRequired.txt");
                    if (String.IsNullOrEmpty(captchaChallenge))
                    {
                        ResolvedCaptcha captcha = CaptchaProvider.GetResolvedCaptcha();
                        return Login(captcha.ChallengeID, captcha.CaptchaText);
                    }
                    else
                        throw new WebException("Captcha not recognized");
                }

                if (m_Response.Contains("infringement"))
                    throw new WebException("Banned");
                //if (m_Response.Contains("Citizen name"))
                //    throw new WebException("Banned2");

                if (m_Response.Contains("Email validation"))
                {
                    ResendMail();
                    throw new WebException(m_AuthLogin + ": Resending mail");
                }

                if (m_Response.Contains("dead"))
                {
                    Revive();
                }
                else
                    Activate();

                bOK = (m_Response.IndexOf("logout") != -1);
                if (!bOK)
                {
                    if (m_Response.Contains("Wrong password") ||
                        m_Response.Contains("Wrong citizen name") ||
                        m_Response.Contains("Join now"))
                        return false;
                    if (!m_Response.Contains("dead"))
                    {
                        //ConsoleLog.WriteLine(m_Response, "LoginLog.txt");
                        throw new WebException("Login: erepublik-сервер вернул неизвестный ответ. См. LoginLog.txt");
                    }
                }
            }

            if (m_Response.IndexOf(mc_TokenScanString) == -1)
            {
                m_Response = m_Client.DownloadString("http://www.erepublik.com/en/main/messages-inbox");
                if (m_Response.IndexOf(mc_TokenScanString) == -1)
                {
                    throw new Exception("Token (ms_Token2) not found!");
                }
            }
            ms_Token2 = m_Response.Remove(0, m_Response.IndexOf(mc_TokenScanString) + mc_TokenScanString.Length);
            ms_Token2 = ms_Token2.Substring(0, ms_Token2.IndexOf("\""));
            ConsoleLog.WriteLine(m_AuthLogin + ": Logged in!");
            return true;
        }

        /// <summary>
        /// Получает список предложений работы
        /// </summary>
        /// <param name="Country">Страна. Если задать 0, то используется страна пребывания</param>
        /// <param name="Type">Отрасль. Допустимые значения - "all" ("a"), "land" ("l"),
        /// "manufacturing" ("m"), "constructions" ("c")</param>
        /// <param name="Skill">Уровень навыка. Нулевому скиллу соответствует 1. 0-автофильтр по текущему скиллу.</param>
        /// <param name="Page">Страница с рынка вакансий, которую требуется загрузить.</param>
        /// <param name="Vacancy">Выводит только заданное предложение. 0-обычный режим</param>
        /// <returns></returns>
        public List<OfferInfo> GetJobOffers(int Country, string Type, int Skill, int Page, int Vacancy)
        {
            int iBuf;
            const string CompanyAffix = "href=\"/en/company/";
            const string QualityAffix = "stared ";
            const string QualitySuffix = "\">";
            //const string IndustryAffix = "class=\"tooltip\" alt=\"";
            //const string IndustrySuffix = "\" ";
            const string LevelAfffix = "<span class=\"skiller\"><strong>";
            const string LevelSuffix = "</strong>";
            const string SalaryAfffix1 = "<strong>";
            const string SalarySuffix1 = "</strong>";
            const string SalaryAfffix2 = "<sup>.";
            const string SalarySuffix2 = " <strong>";
            const string ApplyAffix = "href=\"/en/job/apply/";

            if (Type == "a") Type = "all";
            Type = "0";
            string JobURL = "";
            if (Vacancy == 0)
            {
                if (Skill == 0)
                    JobURL = "http://economy.erepublik.com/en/market/job/"
                        + Country.ToString();
                else
                    JobURL = "http://economy.erepublik.com/en/market/job/"
                        + Country.ToString() + "/" + Type + "/" + Skill.ToString();
            }
            else
                JobURL = "http://economy.erepublik.com/en/market/job/offer/"
                    + Vacancy.ToString();
            ConsoleLog.WriteLine("Find job for vacancy (0-all): " + Vacancy.ToString());
            ConsoleLog.WriteLine("Job URL: " + JobURL);
            //ConsoleLog.WriteLine(JobURL, "FingJobLog.txt");
            string html = m_Response = m_Client.DownloadString(JobURL);
            if (m_Response.Contains("You already have a job"))
            {
                // Мы уже устроены на работу, ничего делать не надо. 
                // Возвращаем пустой лист, чтобы бот не пытался трудоустроитсья.
                ConsoleLog.WriteLine("Allready have a job.");
                return null;
            }

            //ConsoleLog.WriteLine(m_Response, "JobOffersLog.txt");

            List<OfferInfo> list = new List<OfferInfo>();

            int cPos = 0; int cPos2; string sVal;

            ms_csrfToken = CommonUtils.GetToken2(html);
            if (ms_csrfToken.Length != 32)
            {
                ConsoleLog.WriteLine("Error parsing job list (token): " + ms_csrfToken);
                return list;
            }

            cPos = 0;

            while ((cPos = html.IndexOf(CompanyAffix, cPos)) != -1)
            {
                OfferInfo offer = new OfferInfo();
                cPos += CompanyAffix.Length;
                if (cPos == -1)
                {
                    ConsoleLog.WriteLine(html, "JobOffersList2.txt");
                    ConsoleLog.WriteLine("Error parsing job list (2)");
                    return list;
                }

                //Вытаскиваем ID компании
                cPos2 = html.IndexOf('"', cPos);
                sVal = html.Substring(cPos, cPos2 - cPos);
                sVal = sVal.Substring(sVal.IndexOf("/", 0) + 1);
                offer.Add("company", int.Parse(sVal));
                //Вытаскиваем звёздочки
                int saveCPos = cPos;
                cPos = html.IndexOf(QualityAffix, cPos2);
                if (cPos != -1)
                {
                    cPos += QualityAffix.Length;
                    cPos2 = html.IndexOf(QualitySuffix, cPos);
                    sVal = html.Substring(cPos, cPos2 - cPos);
                }
                else
                {
                    cPos = saveCPos;
                    sVal = "one";
                }

                iBuf = 1;
                switch (sVal)
                {
                    case "one":
                        iBuf = 1;
                        break;
                    case "two":
                        iBuf = 2;
                        break;
                    case "three":
                        iBuf = 3;
                        break;
                    case "four":
                        iBuf = 4;
                        break;
                    case "five":
                        iBuf = 5;
                        break;
                    default:
                        iBuf = 1;
                        break;
                }
                offer.Add("quality", iBuf);

                //Вытаскиваем индустрию
                /*
                cPos = html.IndexOf(IndustryAffix, cPos);
                if (cPos == -1)
                {
                    ConsoleLog.WriteLine("Error parsing job list (4)");
                    return list;
                }
                cPos += IndustryAffix.Length;
                cPos2 = html.IndexOf(IndustrySuffix, cPos);
                sVal = html.Substring(cPos, cPos2 - cPos);

                switch (sVal)
                {
                    case "Producer": iBuf = 1;
                        break;
                    case "Marketing Manager": iBuf = 2;
                        break;
                    case "Project Manager": iBuf = 3;
                        break;
                    case "Carpenter": iBuf = 4;
                        break;
                    case "Builder": iBuf = 5;
                        break;
                    case "Architect": iBuf = 6;
                        break;
                    case "Engineer": iBuf = 7;
                        break;
                    case "Mechanic": iBuf = 8;
                        break;
                    case "Fitter": iBuf = 9;
                        break;
                    case "Technician": iBuf = 10;
                        break;
                    case "Harvester": iBuf = 11;
                        break;
                }
                 */
                iBuf = 1;

                offer.Add("industry", iBuf);
                //Вытаскиваем уровень
                cPos = html.IndexOf(LevelAfffix, cPos);
                if (cPos == -1)
                {
                    ConsoleLog.WriteLine(html, "JobOffersList3.txt");
                    ConsoleLog.WriteLine("Error parsing job list (3)");
                    return list;
                }

                cPos += LevelAfffix.Length;
                cPos2 = html.IndexOf(LevelSuffix, cPos);
                sVal = html.Substring(cPos, cPos2 - cPos);
                offer.Add("level", int.Parse(sVal));
                //Вытаскиваем ЗП
                //Целая часть
                cPos = html.IndexOf(SalaryAfffix1, cPos);
                if (cPos == -1)
                {
                    ConsoleLog.WriteLine(html, "JobOffersList4.txt");
                    ConsoleLog.WriteLine("Error parsing job list (4)");
                    return list;
                }

                cPos += SalaryAfffix1.Length;
                cPos2 = html.IndexOf(SalarySuffix1, cPos);
                sVal = html.Substring(cPos, cPos2 - cPos);
                //Дробная
                cPos = html.IndexOf(SalaryAfffix2, cPos);
                if (cPos == -1)
                {
                    ConsoleLog.WriteLine(html, "JobOffersList5.txt");
                    ConsoleLog.WriteLine("Error parsing job list (5)");
                    return list;
                }

                cPos += SalaryAfffix2.Length;
                cPos2 = html.IndexOf(SalarySuffix2, cPos);
                sVal += html.Substring(cPos, cPos2 - cPos);
                offer.Add("salary", int.Parse(sVal));
                //ID предложения
                cPos = html.IndexOf(ApplyAffix, cPos);
                if (cPos == -1)
                {
                    ConsoleLog.WriteLine(html, "JobOffersList6.txt");
                    ConsoleLog.WriteLine("Error parsing job list (6)");
                    return list;
                }

                cPos += ApplyAffix.Length;
                cPos2 = html.IndexOf('"', cPos);
                sVal = html.Substring(cPos, cPos2 - cPos);
                offer.Add("id", int.Parse(sVal));

                list.Add(offer);
            };

            return list;
        }
        #region Overloads
        /// <summary>
        /// Получает список предложений работы
        /// </summary>
        /// <param name="Country">Страна</param>
        /// <param name="Type">Отрасль. Допустимые значения - "all" ("a"), "land" ("l"),
        /// "manufacturing" ("m"), "constructions" ("c")</param>
        /// <param name="Skill">Уровень навыка. 0 - без фильтра. Нулевому скиллу соответствует 1.</param>
        /// <returns></returns>
        public List<OfferInfo> GetJobOffers(int Country, string Type, int Skill, int Page)
        { return GetJobOffers(Country, Type, Skill, Page, 0); }
        /// <summary>
        /// Получает список предложений работы
        /// </summary>
        /// <param name="Country">Страна</param>
        /// <param name="Type">Отрасль. Допустимые значения - "all" ("a"), "land" ("l"),
        /// "manufacturing" ("m"), "constructions" ("c")</param>
        /// <param name="Skill">Уровень навыка. 0 - без фильтра. Нулевому скиллу соответствует 1.</param>
        /// <returns></returns>
        public List<OfferInfo> GetJobOffers(int Country, string Type, int Skill)
        { return GetJobOffers(Country, Type, Skill, 1); }
        /// <summary>
        /// Получает список предложений работы
        /// </summary>
        /// <param name="Country">Страна</param>
        /// <param name="Type">Отрасль. Допустимые значения - "all" ("a"), "land" ("l"),
        /// "manufacturing" ("m"), "constructions" ("c")</param>
        /// <returns></returns>
        public List<OfferInfo> GetJobOffers(int Country, string Type)
        { return GetJobOffers(Country, Type, 0); }
        /// <summary>
        /// Получает список предложений работы
        /// </summary>
        /// <param name="Country">Страна</param>
        /// <returns></returns>
        public List<OfferInfo> GetJobOffers(int Country)
        { return GetJobOffers(Country, "all"); }
        public List<OfferInfo> GetJobOffers()
        { return GetJobOffers(0); }
        #endregion

        public List<OfferInfo> GetMarketOffers(int Country, int Type, int Quality)
        {
            const string CompanyAffix = "/en/company/";
            const string QualityAffix = "class=\"qllevel\" style=\"width: ";
            const string CountAfffix = "<span class=\"special\">";
            const string CountSuffix = "</span>";
            const string PriceAfffix1 = "<span class=\"special\">";
            const string PriceSuffix1 = "</span>";
            const string PriceAfffix2 = "<sup>.";
            const string PriceSuffix2 = "</sup>";
            const string IDAffix = "name=\"amount_offer\" id=\"amount_";
            m_Response = m_Client.DownloadString("http://www.erepublik.com/en/market/country-" +
                Country.ToString() + "-industry-" + Type.ToString() + "-quality-" + Quality.ToString() +
                "-citizen_account-/1");
            int cPos = 0, cPos2 = 0;
            List<OfferInfo> list = new List<OfferInfo>();
            while ((cPos = m_Response.IndexOf(CompanyAffix, cPos)) != -1)
            {
                OfferInfo offer = new OfferInfo();
                //ID компании
                cPos += CompanyAffix.Length;
                cPos2 = m_Response.IndexOf('"', cPos);
                string sVal = m_Response.Substring(cPos, cPos2 - cPos);
                sVal = sVal.Substring(sVal.LastIndexOf('-') + 1);
                offer.Add("company", int.Parse(sVal));

                //Качество
                cPos = m_Response.IndexOf(QualityAffix, cPos);
                cPos += QualityAffix.Length;
                sVal = m_Response.Substring(cPos, 2);
                offer.Add("quality", int.Parse(sVal) / 20);
                //Количество
                cPos = m_Response.IndexOf(CountAfffix, cPos);
                cPos += PriceAfffix1.Length;
                cPos2 = m_Response.IndexOf(CountSuffix, cPos);
                sVal = m_Response.Substring(cPos, cPos2 - cPos);
                offer.Add("count", int.Parse(sVal));
                //Вытаскиваем цену
                //Целая часть
                cPos = m_Response.IndexOf(PriceAfffix1, cPos);
                cPos += PriceAfffix1.Length;
                cPos2 = m_Response.IndexOf(PriceSuffix1, cPos);
                sVal = m_Response.Substring(cPos, cPos2 - cPos);
                //Дробная
                cPos = m_Response.IndexOf(PriceAfffix2, cPos);
                cPos += PriceAfffix2.Length;
                cPos2 = m_Response.IndexOf(PriceSuffix2, cPos);
                sVal += m_Response.Substring(cPos, cPos2 - cPos);
                offer.Add("price", int.Parse(sVal));

                //ID
                cPos = m_Response.IndexOf(IDAffix, cPos);
                cPos += IDAffix.Length;
                cPos2 = m_Response.IndexOf('"', cPos);
                sVal = m_Response.Substring(cPos, cPos2 - cPos);
                offer.Add("id", int.Parse(sVal));
                list.Add(offer);
            }

            return list;
        }

        public bool BuyProducts(int offerid, int amount, int industry)
        {
            const string TargetAffix = "<form id=\"buy_offer\" method=\"post\" action=";
            if (m_Client.Referer.Contains("http://erepublik.com/en/market/country") == false)
            {
                if (m_Response.Contains(TargetAffix) == false) m_Response = m_Client.DownloadString
                      ("http://www.erepublik.com/en/market/country-0-industry-1-quality-1-citizen_account-");
            }

            string link = m_Response.Remove(0, m_Response.IndexOf(TargetAffix) + TargetAffix.Length + 1);
            link = link.Remove(link.IndexOf('\"'));

            string ptr = link.Substring(link.IndexOf("industry-"), 10);
            link = link.Replace(ptr, "industry-" + industry.ToString());
            //ptr=link.Substring(link.IndexOf("quality-"),9);
            //link = link.Replace(ptr, "quality-" + quality.ToString());

            //m_Client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            string PostData = "_token=" + ms_Token + "&offer_id=" + offerid.ToString() +
                "&amount=" + amount.ToString();
            //link = "en/market/country-41-industry-1-quality-1-citizen_account-9994143/1";
            m_Response = m_Client.UploadString("http://www.erepublik.com" + link, PostData);
            return (m_Response.Contains("You have succesfully bought"));

        }

        public void ChooseFightSide(int iBattleId, int iCountry)
        {
            try
            {
                m_Response = m_Client.DownloadString("http://www.erepublik.com/en/military/battlefield-choose-side/" + iBattleId.ToString() + "/" + iCountry.ToString());
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("ChooseFightSide error: " + e.Message);
            }
        }

        public int FightInBattle(int iBattleId, int iBuyWeaponAmount, int iBuyCountry, int iLeftHP, int iFightCountry, bool doNotChange, int iShotLimit)
        {
            ConsoleLog.WriteLine("Preparing to fight in battle");
            if (iBuyWeaponAmount != 0)
                BuyItem(iBuyCountry, Goods.Weapon, iBuyWeaponAmount, 0, 0, true);
            if (iFightCountry != 0)
                ChooseFightSide(iBattleId, iFightCountry);
            int i;
            int well = -1;
            if (iShotLimit == 0)
                iShotLimit = 9999;
            Random rnd = new System.Random();
            for (i = 0; i < iShotLimit; i++)
            {
                System.Threading.Thread.Sleep(rnd.Next(10, 500));
                if (!doNotChange) 
                    ChangeWeapon(iBattleId);
                well = FightShoot(iBattleId, i+1);
                if (well < iLeftHP) break;
            }
            return well;
        }

        public int FightShoot(int iBattleId, int iShootNum)
        {
            int wellness;
            for (int tryNum = 1; tryNum <= 3; tryNum++)
            {
                try
                {
                    ConsoleLog.WriteLine("Shoot " + iShootNum.ToString());
                    m_Client.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    m_Client.Referer = "http://www.erepublik.com/en/military/battlefield/" + iBattleId.ToString();
                    string PostData =
                        "battleId=" + iBattleId.ToString() +
                        "&instantKill=0" +
                        "&_token=" + ms_Token2;
                    m_Response = m_Client.UploadString("http://www.erepublik.com/en/military/fight-shoot", PostData);
                    //ConsoleLog.WriteLine(m_Response, "FightShootLog.txt");
                    if (!m_Response.Contains("health"))
                        throw new Exception("shoot error. Possibly this bot cannot fight in this battle");
                    string sWellness = Regex.Match(m_Response,
                        "\"health\":(.*?),")
                        .Groups[1].Value;
                    int.TryParse(sWellness, out wellness);
                }
                catch (System.Exception e)
                {
                    ConsoleLog.WriteLine("Fight shoot error: " + e.Message + " Retry " + (tryNum + 1).ToString());
                    continue;
                }
                ConsoleLog.WriteLine("Health after shoot: " + wellness.ToString());
                m_Client.Headers.Remove("X-Requested-With");
                return wellness;
            }
            return -1;
        }
        public void ChangeWeapon(int iBattleId)
        {
            try
            {
                m_Client.Headers.Add("X-Requested-With", "XMLHttpRequest");
                m_Client.Referer = "http://www.erepublik.com/en/military/battlefield/" + iBattleId.ToString();
                string PostData = "_token=" + ms_Token2 + "&battleId=" + iBattleId.ToString();
                m_Response = m_Client.UploadString("http://www.erepublik.com/en/military/change-weapon", PostData);
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Change weapon error: " + e.Message);
            }
            m_Client.Headers.Remove("X-Requested-With");
        }

        /// <summary>
        /// Отправляет POST=запрос с указанными параметрами
        /// </summary>
        /// <param name="url">URL запроса(полный)</param>
        /// <param name="data">Данные POST-запроса. Возможно, понадобится включить в них токен</param>
        /// <returns></returns>

        public string CustomRequest(string url, string data)
        {
            m_Response = m_Client.UploadString(url, data);
            return m_Response;
        }
        public string CustomRequest(string url)
        {
            m_Response = m_Client.DownloadString(url);
            return m_Response;
        }
        /// <summary>
        /// Получает токен сессии.
        /// </summary>
        /// <param name="ind"><para>0 - авторизационный, полученный со страницы логина</para>
        /// <para>1 - основной, полученный после авторизации</para></param>
        /// <returns></returns>
        public string GetTokenArg(int ind)
        {
            string ret = "_token=";
            if (ind == 0) ret += ms_Token;
            else if (ind == 1) ret += ms_Token2;
            else throw new IndexOutOfRangeException();
            return ret;
        }
        private void ParseRegionID()
        {
            string RegionIDAffix = "current_region_id\" value=\"";
            m_Response = m_Client.DownloadString("http://www.erepublik.com/en/citizen/change-residence");
            int cPos = m_Response.IndexOf(RegionIDAffix);
            cPos += RegionIDAffix.Length;
            int cPos2 = m_Response.IndexOf('\"', cPos);
            m_RegionID = int.Parse(m_Response.Substring(cPos, cPos2 - cPos));
        }
        /// <summary>
        /// ID региона, в котором находится персонаж. Если регион не известен, будет произведена попытка его
        /// определить
        /// </summary>
        public int RegionID
        {
            get { if (m_RegionID == 0) ParseRegionID(); return m_RegionID; }
        }
        /// <summary>
        /// Перелёт из одного региона в другой. В кармане должен быть билет
        /// </summary>
        /// <param name="Country">ID страны</param>
        /// <param name="Region">ID региона</param>
        /// <returns></returns>
        public bool Fly(int Country, int Region)
        {
            m_Client.Referer = "http://www.erepublik.com/en/citizen/change-residence";
            string PostData = "_token=" + ms_Token2
            + "&country_list=" + Country.ToString()
            + "&region_list=" + Region.ToString();

//            m_Client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            m_Response = m_Client.UploadString("http://www.erepublik.com/en/citizen/change-residence", PostData);
            return m_Response.Contains("You have successfully moved to");

        }
        /// <summary>
        /// Голосует за статью
        /// </summary>
        /// <param name="ArticleID">ID статьи</param>
        public void VoteArticle(int ArticleID)
        {
#if !PUBLIC_BUILD
            m_Client.Referer = "http://www.erepublik.com/en/article/a-" + ArticleID.ToString() + "/1/20";
            string PostData = "_token=" + ms_Token2 + "&article_id=" + ArticleID.ToString();
            m_Response = m_Client.UploadString("http://www.erepublik.com/vote-article", PostData);
#endif
        }
        public void SubscribeNewspaper(int NewspaperID)
        {
#if !PUBLIC_BUILD
            m_Client.Referer = "http://www.erepublik.com/en/newspaper/a-" + NewspaperID.ToString() + "/1";
            string PostData = "_token=" + ms_Token2 + "&type=subscribe&n=" + NewspaperID.ToString();
            m_Response = m_Client.UploadString("http://www.erepublik.com/subscribe", PostData);
#endif
        }
        public void UnsubscribeNewspaper(int NewspaperID)
        {
#if !PUBLIC_BUILD
            m_Client.Referer = "http://www.erepublik.com/en/newspaper/a-" + NewspaperID.ToString() + "/1";
            string PostData = "_token=" + ms_Token2 + "&type=unsubscribe&n=" + NewspaperID.ToString();
            m_Response = m_Client.UploadString("http://www.erepublik.com/subscribe", PostData);
#endif
        }
        public int GetAvailableBattle()
        {
            m_Response = m_Client.DownloadString("http://www.erepublik.com/en/battles/mybattlelist");
            string battle = System.Text.RegularExpressions.Regex.Match(m_Response,
                "battles/show/(\\w+)\"").Groups[1].Value;
            return int.Parse(battle);

        }
        private bool SubPreSendMessage(int citId)
        {
            m_Client.Referer = "http://www.erepublik.com/en/";
            m_Response = m_Client.DownloadString("http://www.erepublik.com/en/main/messages-compose/" + citId.ToString());
            return m_Response.Contains("Recaptcha");
        }
        private bool SubSendMessage(int citId, string citName, string subj, string body, string captch)
        {
            m_Client.Referer = "http://www.erepublik.com/en/main/messages-compose/" + citId.ToString();
            string PostData = "_token=" + ms_Token2 + "&citizen_name=" + citName + ",&citizen_subject=" + subj + "&citizen_message=" + body;
            m_Response = m_Client.UploadString("http://www.erepublik.com/en/main/messages-compose/" + citId.ToString(), PostData);
            return m_Response.Contains("Recaptcha");
        }
        private bool SubSendCapthaMessage(int citId, string citName, string subj, string body, string captch1, string captch2)
        {
            m_Client.Referer = "http://www.erepublik.com/en/main/messages-compose/" + citId.ToString();
            string PostData = "_token=" + ms_Token2 + "&citizen_name=" + citName + ",&citizen_subject=" + subj + "&citizen_message=" + body + "&recaptcha_challenge_field=" + captch1 + "&recaptcha_response_field=" + System.Web.HttpUtility.UrlEncode(captch2);
            m_Response = m_Client.UploadString("http://www.erepublik.com/en/main/messages-compose/" + citId.ToString(), PostData);
            return m_Response.Contains("Recaptcha");
        }
        public void Revive()
        {
			m_Client.Referer = "http://www.erepublik.com/en/dead";
			string PostData = "_token=" + ms_Token;
            m_Response = m_Client.UploadString("http://www.erepublik.com/en/dead_revive", PostData);
            ms_Token2 = m_Response.Remove(0, m_Response.IndexOf(mc_TokenScanString) + mc_TokenScanString.Length);
            ms_Token2 = ms_Token2.Substring(0, ms_Token2.IndexOf("\""));

        }
        public void SendMessage(int citId, string citName, string subj, string body)
        {
            ConsoleLog.WriteLine(m_AuthLogin + ": SendMessage to " + citName);
            bool b = SubPreSendMessage(citId);
            if (b || !SubSendMessage(citId, citName, subj, body, "1"))
            {
                while (m_Response.Contains("Recaptcha"))
                {
                    var captcha = CaptchaProvider.GetResolvedCaptcha();
                    SubSendCapthaMessage(citId, citName, subj, body, captcha.ChallengeID, captcha.CaptchaText);
                }
            }
        }

        public int Work(uint citizenID)
        {
            try
            {
                ConsoleLog.WriteLine(m_AuthLogin + ": Work");
                string land = "http://economy.erepublik.com/en/land/overview/" + citizenID;
                m_Response = m_Client.DownloadString(land);
                string captchaToken = CommonUtils.GetToken(m_Response);

                if (!m_Response.Contains("myland/tip_icons/work.png"))
                {
                    ConsoleLog.WriteLine("Allready worked today");
                    return 2;
                }

                m_Response = CommonUtils.GetStringFrom(
                    m_Response,
                    "myland/tip_icons/work.png");

                string companyID = CommonUtils.GetStringBetween(
                    m_Response,
                    "companyId=\"",
                    "\"");

                m_Client.Referer = land;
                m_Client.Headers.Add("X-Requested-With", "XMLHttpRequest");
                m_Response = m_Client.DownloadString(
                    "http://economy.erepublik.com/en/work/" + companyID + "?isForced=0");

                while (m_Response.Contains("captcha"))
                {
                    ConsoleLog.WriteLine(m_AuthLogin + ": work-captcha");
                    var captcha = CaptchaProvider.GetResolvedCaptcha();
                    string PostData = 
                        System.Web.HttpUtility.UrlEncode("captcha_form[_csrf_token]") + "=" + captchaToken + "&" +
                        "recaptcha_challenge_field=" + captcha.ChallengeID + "&" +
                        "recaptcha_response_field=" + System.Web.HttpUtility.UrlEncode(captcha.CaptchaText) + "&" + 
                        "commit=Continue";
                    m_Response = m_Client.UploadString("http://economy.erepublik.com/en/time-management/captcha?actionType=work", PostData);
                    ///////////ConsoleLog.WriteLine("Work response: " + m_Response);
                }

                if (m_Response.Contains("timeManagement_work_anyway"))
                {
                    ConsoleLog.WriteLine(m_AuthLogin + ": not enough resources, force working");
                    ConsoleLog.WriteLine(m_Response);
                    m_Response = m_Client.DownloadString(
                        "http://economy.erepublik.com/en/work/" + companyID + "?isForced=1");
                }

                //ConsoleLog.WriteLine("Work response: " + m_Response);
                m_Client.Headers.Remove("X-Requested-With");

                if (m_Response.Contains("\"message\":true"))
                    return 1;
                
                if (m_Response.Contains("timeManagement_work_notEnoughSlots"))
                    return 3;

                return 0;
            }
            catch (System.Exception e)
            {
                m_Client.Headers.Remove("X-Requested-With");
                throw new Exception(e.Message);
            }
        }

        public bool TryGold()
        {
            /*
            try
            {
                System.Random rnd = new System.Random();
                m_Response = m_Client.DownloadString("http://www.erepublik.com/en/treasure-map");
                m_Response = m_Client.DownloadString("http://www.erepublik.com/en/treasure-map/map");
                m_Response = m_Client.DownloadString("http://www.erepublik.com/en/treasure-map/map-results/" + rnd.Next(1, 3).ToString());
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Gold error: " + e.Message);
            }
             */
            return true;
        }

        public bool Activate()
        {
            m_Response = m_Client.DownloadString("http://economy.erepublik.com/en/tutorial/battles/mybattlelist");
            bool bOK = (m_Response.IndexOf("logout") != -1);
            if (!bOK)
            {
                if ((m_Response.IndexOf("Wrong password") != -1) ||
                    (m_Response.IndexOf("Wrong citizen name") != -1) ||
                    (m_Response.IndexOf("infringement") != -1) ||
                    (m_Response.IndexOf("Join now") != 1))
                    return false;
                if (!m_Response.Contains("dead"))
                {
                    //ConsoleLog.WriteLine(m_Response, "LoginLog.txt");
                    throw new WebException("Activate: erepublik-сервер вернул неизвестный ответ. См. LoginLog.txt");
                }
            }
            ms_Token2 = m_Response.Remove(0, m_Response.IndexOf(mc_TokenScanString) + mc_TokenScanString.Length);
            ms_Token2 = ms_Token2.Substring(0, ms_Token2.IndexOf("\""));
            ConsoleLog.WriteLine(m_AuthLogin + ": Activated!");
            return true;
        }

        public bool ResendMail()
        {
            string sBuf = m_Response;
            string sTokenPref = "name=\"_token\" value=\"";
            string sEmailPref = "id=\"citizen_email\" value=\"";
            sBuf = sBuf.Remove(0, sBuf.IndexOf(sTokenPref) + sTokenPref.Length);
            sBuf = sBuf.Remove(sBuf.IndexOf("\""));
            string sToken = sBuf;
            sBuf = m_Response;
            sBuf = sBuf.Remove(0, sBuf.IndexOf(sEmailPref) + sEmailPref.Length);
            sBuf = sBuf.Remove(sBuf.IndexOf("\""));
            string sEmail = sBuf;
            string PostData = "_token=" + sToken + "&citizen_email=" + System.Web.HttpUtility.UrlEncode(sEmail);
            m_Response = m_Client.UploadString("http://www.erepublik.com/en/register-validate-new-account", PostData);
            ConsoleLog.WriteLine(m_AuthLogin + ": Mail Resended!");
            return true;
        }

        public bool Comment(int iId, string sMess)
        {
            sMess = System.Web.HttpUtility.UrlEncode(sMess);
            string PostData = "_token=" + ms_Token2 + "&article_comment=" + sMess + "&commit=Post+a+comment";
            m_Response = m_Client.UploadString("http://www.erepublik.com/en/add-comment/" + iId.ToString(), PostData);
            return true;
        }

        public bool Train(uint citizenID)
        {
            try
            {
                ConsoleLog.WriteLine(m_AuthLogin + ": Train");
                string land = "http://economy.erepublik.com/en/land/overview/" + citizenID;

                System.Random rnd = new System.Random();
                string sR = "";

                sR = "131";
                for (int i = 3; i < 13; i++)
                {
                    sR += rnd.Next(1, 9).ToString();
                }

                m_Client.Referer = land;
                m_Response = m_Client.DownloadString("http://www.erepublik.com/en/main/train?format=json&jsoncallback=jsonp" + sR + "&isResult=false&buildingId=0");
                //ConsoleLog.WriteLine("Train resp: " + m_Response);
                return (m_Response.Contains("\"status\":true"));
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Train error: " + e.Message);
                return false;
            }
        }

        public bool TrainX()
        {
            try
            {
                ConsoleLog.WriteLine(m_AuthLogin + ": Train");
                m_Response = m_Client.DownloadString("http://economy.erepublik.com/en/train");
                string sToken = CommonUtils.GetToken(m_Response);
                string PostData = System.Web.HttpUtility.UrlEncode("train[boosterId]") + "=10"
                    + "&" + System.Web.HttpUtility.UrlEncode("train[_csrf_token]") + "=" + sToken
                    + "&" + System.Web.HttpUtility.UrlEncode("train[skillId]") + "=1"
                    + "&" + System.Web.HttpUtility.UrlEncode("train[friend1]") + "=0"
                    + "&" + System.Web.HttpUtility.UrlEncode("train[friend2]") + "=0"
                    /* +"&" +
System.Web.HttpUtility.UrlEncode("train[hours]") + "=" + iH.ToString()*/
                    ;
                m_Response = m_Client.UploadString("http://economy.erepublik.com/en/train", PostData);
                while (m_Response.Contains("manual_challenge"))
                {
                    ConsoleLog.WriteLine(m_AuthLogin + ": train-captcha");
                    var captcha = CaptchaProvider.GetResolvedCaptcha();
                    sToken = CommonUtils.GetToken(m_Response);
                    PostData = System.Web.HttpUtility.UrlEncode("captcha_form[_csrf_token]") + "=" + sToken + "&" +
                        "recaptcha_challenge_field=" + captcha.ChallengeID + "&recaptcha_response_field=" + System.Web.HttpUtility.UrlEncode(captcha.CaptchaText) + "&commit=Continue";
                    m_Response = m_Client.UploadString("http://economy.erepublik.com/en/time-management/captcha/train", PostData);
                }

                //ConsoleLog.WriteLine("Train response: " + m_Response);
                return (m_Response.Contains("charTooltipTd"));
                //return true;
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Train error: " + e.Message);
                return false;
            }
        }

        public bool Study(int iH, int iSkill)
        {
            try
            {
                iSkill = 0;
                ConsoleLog.WriteLine(m_AuthLogin + ": Study");
                m_Response = m_Client.DownloadString("http://economy.erepublik.com/en/study");
                string sToken = CommonUtils.GetToken(m_Response);
                string PostData = System.Web.HttpUtility.UrlEncode("study[boosterId]") + "=6&" +
                    System.Web.HttpUtility.UrlEncode("study[_csrf_token]") + "=" + sToken + "&" +
                    System.Web.HttpUtility.UrlEncode("study[skillId]") + "=" + iSkill.ToString() + "&" +
                    System.Web.HttpUtility.UrlEncode("study[hours]") + "=" + iH.ToString();
                m_Response = m_Client.UploadString("http://economy.erepublik.com/en/study", PostData);
                while (m_Response.Contains("manual_challenge"))
                {
                    ConsoleLog.WriteLine(m_AuthLogin + ": study-captcha");
                    var captcha = CaptchaProvider.GetResolvedCaptcha();
                    sToken = CommonUtils.GetToken(m_Response);
                    PostData = System.Web.HttpUtility.UrlEncode("captcha_form[_csrf_token]") + "=" + sToken + "&" +
                        "recaptcha_challenge_field=" + captcha.ChallengeID + "&recaptcha_response_field=" + System.Web.HttpUtility.UrlEncode(captcha.CaptchaText) + "&commit=Continue";
                    m_Response = m_Client.UploadString("http://economy.erepublik.com/en/time-management/captcha/study", PostData);

                }
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Study error: " + e.Message);
                return false;
            }
            return true;
        }

        public bool Entertain(int iH)
        {
            try
            {
                ConsoleLog.WriteLine(m_AuthLogin + ": Entertain");
                m_Response = m_Client.DownloadString("http://economy.erepublik.com/en/entertain");
                string sToken = CommonUtils.GetToken(m_Response);
                string PostData = System.Web.HttpUtility.UrlEncode("entertain[boosterId]") + "=14&" +
                    System.Web.HttpUtility.UrlEncode("entertain[_csrf_token]") + "=" + sToken + "&" +
                    System.Web.HttpUtility.UrlEncode("entertain[hours]") + "=" + iH.ToString();
                m_Response = m_Client.UploadString("http://economy.erepublik.com/en/entertain", PostData);
                while (m_Response.Contains("manual_challenge"))
                {
                    ConsoleLog.WriteLine(m_AuthLogin + ": entertain-captcha");
                    var captcha = CaptchaProvider.GetResolvedCaptcha();
                    sToken = CommonUtils.GetToken(m_Response);
                    PostData = System.Web.HttpUtility.UrlEncode("captcha_form[_csrf_token]") + "=" + sToken + "&" +
                        "recaptcha_challenge_field=" + captcha.ChallengeID + "&recaptcha_response_field=" + System.Web.HttpUtility.UrlEncode(captcha.CaptchaText) + "&commit=Continue";
                    m_Response = m_Client.UploadString("http://economy.erepublik.com/en/time-management/captcha/entertain", PostData);

                }
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Entertain error: " + e.Message);
                return false;
            }
            return true;
        }

        public double GetMinPrice(int iCountry, int iIndustry, int iQuality, bool LoadPage, out string sOffer, out int amount)
        {
            if (iQuality == 0)
                iQuality = 1;

            sOffer = "";
            amount = 0;

            try
            {
                if (LoadPage)
                    m_Response = m_Client.DownloadString("http://economy.erepublik.com/en/market/" + iCountry.ToString() + "/" + iIndustry.ToString() + "/" + iQuality + "/citizen/0/price_asc/1");
                //ConsoleLog.WriteLine(m_Response, "BuyItemLog.txt");

                sOffer = CommonUtils.GetStringBetween(m_Response, "buyOffer\" title=\"Buy\" id=\"", "\"><");

                //Match mPrice = Regex.Match(m_Response, "\"m_price stprice\">[\\r\\n\\s]*<strong>[\\r\\n\\s]*(\\d)*</strong>[\\r\\n\\s]*<sup>\\.(\\d*)[\\r\\n\\s]*<strong>");
                Match mPrice = Regex.Match(m_Response, "\"m_price stprice\">[\\r\\n\\s]*<strong>[\\r\\n\\s]*(\\d*)</strong>[\\r\\n\\s]*<sup>\\.(\\d*)[\\r\\n\\s]*<strong>");
                string sPrice =
                    mPrice.Groups[1].Value +
                    Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator +
                    mPrice.Groups[2].Value;
                ConsoleLog.WriteLine("Detected price: " + sPrice);

                string sAmount = CommonUtils.GetStringBetween(m_Response, "<td class=\"m_stock\">", "</td>");
                sAmount = sAmount.Replace(" ", "").Replace("\n", "").Replace("\r", "");
                ConsoleLog.WriteLine("Detected amount: " + sAmount);
                amount = Convert.ToInt32(sAmount);

                return Convert.ToDouble(sPrice);
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLine("Price detection error: " + e.Message);
                return 0;
            }
        }

        public int BuyItem(int iCountry, int iIndustry, int iAmount, int iQuality, double availMoney, bool LoadPage)
        {
            if (iQuality == 0)
                iQuality = 1;

            int buyAmount = iAmount;

            int i;
            for (i = 1; i <= 3; i++)
            {
                try
                {
                    ConsoleLog.WriteLine(m_AuthLogin + ": BuyItem, try " + i.ToString());

                    if (LoadPage)
                        m_Response = m_Client.DownloadString("http://economy.erepublik.com/en/market/" + iCountry.ToString() + "/" + iIndustry.ToString() + "/" + iQuality + "/citizen/0/price_asc/1");
                    //ConsoleLog.WriteLine("BuyItem MarketPage: " + Environment.NewLine + m_Response, "BuyItemLog.txt");

                    string sToken = CommonUtils.GetToken(m_Response);
                    string sOffer;
                    int foundAmount;
                    double price = GetMinPrice(iCountry, iIndustry, iQuality, false, out sOffer, out foundAmount);

                    if (price == 0)
                        throw new Exception("price detection error");

                    if (availMoney > 0)
                    {
                        buyAmount = Convert.ToInt32(Math.Floor(Math.Min(iAmount, availMoney / price)));
                        ConsoleLog.WriteLine(
                            "Required amount=" + iAmount.ToString() + "; " +
                            "available money=" + availMoney.ToString() + "; " +
                            "Buy amount=" + buyAmount.ToString());

                        if (buyAmount == 0)
                            return 0;
                    }

                    string PostData =
                        System.Web.HttpUtility.UrlEncode("buyMarketOffer[amount]") + "=" + buyAmount.ToString() + "&" +
                        System.Web.HttpUtility.UrlEncode("buyMarketOffer[_csrf_token]") + "=" + sToken + "&" +
                        System.Web.HttpUtility.UrlEncode("buyMarketOffer[offerId]") + "=" + sOffer;

                    //ConsoleLog.WriteLine("BuyItem UPL: " + Environment.NewLine + "http://economy.erepublik.com/en/market/" + iCountry.ToString() + "/" + iItem.ToString() + "/0/0", "BuyItemLog.txt");
                    //ConsoleLog.WriteLine("BuyItem PostData: " + Environment.NewLine + PostData, "BuyItemLog.txt");
                    m_Response = m_Client.UploadString("http://economy.erepublik.com/en/market/" + iCountry.ToString() + "/" + iIndustry.ToString() + "/" + iQuality + "/citizen/0/price_asc/1", PostData);
                    //ConsoleLog.WriteLine("BuyItem Response: " + Environment.NewLine + m_Response, "BuyItemLog.txt");

                    if (m_Response.Contains("success_message"))
                    {
                        ConsoleLog.WriteLine("BuyItem: Succesfull.");
                        break;
                    }
                    if (m_Response.Contains("You do not have enough money"))
                    {
                        ConsoleLog.WriteLine("BuyItem: No money.");
                        break;
                    }
                    if (!m_Response.Contains("_message"))
                    {
                        ConsoleLog.WriteLine("BuyItem: No effect.");
                    }
                }
                catch (Exception e)
                {
                    ConsoleLog.WriteLine("BuyItem error: " + e.Message);
                }
            }
            if (i > 3)
            {
                ConsoleLog.WriteLine("BuyItem: failed.");
                return -1;
            }
            else return buyAmount;
        }

        public void EatFood()
        {
            System.Random rnd = new System.Random();
            string sR = "";
            if (ms_feedToken == "")
            {
                ConsoleLog.WriteLine(m_AuthLogin + ": EatFood");
                m_Response = m_Client.DownloadString("http://www.erepublik.com/en");
                ms_feedToken = CommonUtils.GetStringBetween(m_Response, "<input type=\"hidden\" value=\"", "\" ");
                //ConsoleLog.WriteLine("Feed token: " + ms_feedToken);
            }

            sR = "131";
            for (int i = 3; i < 13; i++)
            {
                sR += rnd.Next(1, 9).ToString();
            }

            m_Client.Headers.Add("X-Requested-With", "XMLHttpRequest");
            //m_Response = m_Client.UploadString("http://economy.erepublik.com/eat?format=json&_token=" + ms_feedToken + "&jsoncallback=jsonp" + sR, "");
            //m_Response = m_Client.DownloadString("http://economy.erepublik.com/eat?format=json&_token=" + ms_feedToken + "&jsoncallback=jsonp" + sR);
            m_Response = m_Client.DownloadString("http://www.erepublik.com/en/main/eat?format=json&_token=" + ms_feedToken + "&jsoncallback=jsonp" + sR);
            //ConsoleLog.WriteLine("Eat food resp: " + m_Response);
            m_Client.Headers.Remove("X-Requested-With");
        }

        public byte[] Combine(byte[] a, byte[] b)
        {
            byte[] c = new byte[a.Length + b.Length];
            System.Buffer.BlockCopy(a, 0, c, 0, a.Length);
            System.Buffer.BlockCopy(b, 0, c, a.Length, b.Length);
            return c;
        }

        public static byte[] StrToByteArray(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        public Stream WatchBattleField(int iId)
        {
            byte[] before = new byte[] { 0, 3, 0, 0, 0, 1, 0, 0x1a };
            byte[] bytes1 = Encoding.ASCII.GetBytes("RegionAPI.getFirstLoadData");
            byte[] mid = new byte[] { 0, 2 };
            byte[] bytes2 = Encoding.ASCII.GetBytes("/1");
            byte[] after = new byte[] { 0, 0, 0, 0xe, 0xa, 0, 0, 0, 1, 0 };
            double dVal = iId;
            byte[] db = BitConverter.GetBytes(dVal);
            int i;
            for (i = 0; i < db.Length / 2; i++)
            {
                byte t = db[i];
                db[i] = db[db.Length - 1 - i];
                db[db.Length - 1 - i] = t;
            }
            byte[] bytes = Combine(before, Combine(bytes1, Combine(mid, Combine(bytes2, Combine(after, db)))));

            m_Client.Referer = "http://erepublik.com/flash/military/military.swf?rand=89";
            Stream readStream = m_Client.UploadMultipartDataStream("http://military.erepublik.com/", bytes, null);
            return readStream;
            //             MemoryStream writeStream = new MemoryStream();
            //             int Length = 256;
            //             Byte[] buffer = new Byte[Length];
            //             int bytesRead = readStream.Read(buffer, 0, Length);
            //             
            //             while (bytesRead > 0)
            //             {
            //                 writeStream.Write(buffer, 0, bytesRead);
            //                 bytesRead = readStream.Read(buffer, 0, Length);
            //             }
            //             System.IO.File.WriteAllBytes("1.txt",writeStream.ToArray());

            //             BinaryReader br = new BinaryReader(response);
            //             FileStream fs = System.IO.File.Create("1.txt", (int)response.Length);
            //             byte[] bytesInStream = new byte[response.Length];
            //             response.Read(bytesInStream, 0, (int)bytesInStream.Length);
            //             fs.Write(bytesInStream, 0, bytesInStream.Length);

            //             MemoryStream str = (MemoryStream)response;
            //             System.IO.File.WriteAllBytes("1.txt", str.ToArray());
        }


		public void DoMission(int id, bool doNext, string MissionsTocken) {
			ConsoleLog.WriteLine("Проверяет миссию №" + id);
			string response = "" + m_Client.UploadString("http://www.erepublik.com/missions/check",
				"missionId=" + id + "&_token=" + MissionsTocken);
			if (response.IndexOf("\"isFinished\":1,") < 0) {
				return;
			}
			//выполнить миссию
			response = "" + m_Client.UploadString("http://www.erepublik.com/missions/solve",
				"sf_culture=en&missionId=" + id + "&_token=" + MissionsTocken);
			//попробовать новые
			ConsoleLog.WriteLine(">>> Выполнил миссию №" + id + "!!!!!");
			if (doNext) {
				DoMissionsFromJson(response, MissionsTocken);
			}
		}

		Dictionary<int, bool> missionStatuses = new Dictionary<int, bool>();
		public int DoMissionsFromJson(string json, string MissionsTocken) 
        {
			MatchCollection matches = Regex.Matches(json, "{\"id\":(\\d+),");
            int counter = 0;
			foreach (Match m in matches) 
            {
				int id = int.Parse(m.Groups[1].Value);
				if (!missionStatuses.ContainsKey(id)) 
                {
                    counter++;
					missionStatuses.Add(id, false);
					DoMission(id, true, MissionsTocken);
					System.Threading.Thread.Sleep(500);
				}
			}
            return counter;
		}

		/*
        public bool RewardCheck(int i)
        {
            try
            {
                m_Client.Headers.Add("X-Requested-With", "XMLHttpRequest");
                m_Client.Referer = "http://www.erepublik.com/en";
                string PostData = "missionId=" + i.ToString() + "&_token=" + ms_Token2;
                m_Response = m_Client.UploadString("http://www.erepublik.com/missions/check", PostData);

            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("RewardCheck reward failed." + e.ToString());
            }
            m_Client.Headers.Remove("X-Requested-With");
            return m_Response.Contains("\"isFinished\":1");
        }

        public void RewardMission(int i)
        {
            m_Client.Referer = "http://www.erepublik.com/en";
            string PostData = "sf_culture=en&missionId=" + i.ToString() + "&_token=" + ms_Token2;
            m_Response = m_Client.UploadString("http://www.erepublik.com/missions/solve", PostData);
        }

        public void RewardFirstTrain()
        {
            if (RewardCheck(1))
            {
                RewardMission(1);
                ConsoleLog.WriteLine("RewardFirstTrain" + Environment.NewLine + m_Response, "RewardLog.txt");
            }
        }

        public void RewardFeed()
        {
            if (RewardCheck(2))
            {
                RewardMission(2);
                ConsoleLog.WriteLine("RewardFeed" + Environment.NewLine + m_Response, "RewardLog.txt");
            }
        }

        public void RewardFirstWork()
        {
            if (RewardCheck(3))
            {
                RewardMission(3);
                ConsoleLog.WriteLine("RewardFirstWork" + Environment.NewLine + m_Response, "RewardLog.txt");
            }
        }

        public void RewardAvatar()
        {
            if (RewardCheck(4))
            {
                RewardMission(4);
                ConsoleLog.WriteLine("RewardAvatar" + Environment.NewLine + m_Response, "RewardLog.txt");
            }
        }

        public void RewardWeapoon()
        {
            if (RewardCheck(5))
            {
                RewardMission(5); 
                ConsoleLog.WriteLine("RewardWeapoon" + Environment.NewLine + m_Response, "RewardLog.txt");
            }
        }

        public void RewardHero()
        {
            if (RewardCheck(6))
            {
                RewardMission(6); 
                ConsoleLog.WriteLine("RewardHero" + Environment.NewLine + m_Response, "RewardLog.txt");
            }
        }

        public void RewardWorkingRow()
        {
            if (RewardCheck(7))
            {
                RewardMission(7); 
                ConsoleLog.WriteLine("RewardWorkingRow" + Environment.NewLine + m_Response, "RewardLog.txt");
            }
        }

        public void RewardSociety()
        {
            if (RewardCheck(8))
            {
                RewardMission(8); 
                ConsoleLog.WriteLine("RewardFeed" + Environment.NewLine + m_Response, "RewardLog.txt");
            }
        }

        public void RewardFindEldorado1()
        {
            if (RewardCheck(9))
            {
                RewardMission(9);
                ConsoleLog.WriteLine("RewardFindEldorado1" + Environment.NewLine + m_Response, "RewardLog.txt");
            }
        }

        public void RewardFindEldorado2()
        {
            if (RewardCheck(10))
            {
                RewardMission(10);
                ConsoleLog.WriteLine("RewardFindEldorado2" + Environment.NewLine + m_Response, "RewardLog.txt");
            }
        }

        public void RewardFindEldorado3()
        {
            if (RewardCheck(11))
            {
                RewardMission(11);
                ConsoleLog.WriteLine("RewardFindEldorado3" + Environment.NewLine + m_Response, "RewardLog.txt");
            }
        }

        public void RewardResistance()
        {
            if (RewardCheck(12))
            {
                RewardMission(12);
                ConsoleLog.WriteLine("RewardResistance" + Environment.NewLine + m_Response, "RewardLog.txt");
            }
        }

        public void RewardHealthyWorker()
        {
            if (RewardCheck(13))
            {
                RewardMission(13);
                ConsoleLog.WriteLine("RewardHealthyWorker" + Environment.NewLine + m_Response, "RewardLog.txt");
            }
        }

        public void RewardMercenary()
        {
            if (RewardCheck(14))
            {
                RewardMission(14);
                ConsoleLog.WriteLine("RewardMercenary" + Environment.NewLine + m_Response, "RewardLog.txt");
            }
        }
		*/

        public void DailyReward()
        {
            try
            {
                ConsoleLog.WriteLine(m_AuthLogin + ": Daily reward");

                m_Client.Referer = "http://www.erepublik.com/en";
                m_Client.Headers.Add("X-Requested-With", "XMLHttpRequest");
                m_Response = m_Client.DownloadString("http://www.erepublik.com/daily_tasks_reward");
                //ConsoleLog.WriteLine("Daily reward resp: " + m_Response);
                m_Client.Headers.Remove("X-Requested-With");

                if (String.IsNullOrEmpty(m_Response))
                {
                    ConsoleLog.WriteLine("Daily reward empty reply.");
                }
                else
                    if (m_Response.Contains("success"))
                    {
                        ConsoleLog.WriteLine("Daily reward collected.");
                    }
                    else
                        if (m_Response.Contains("error"))
                        {
                            ConsoleLog.WriteLine("Daily reward failed.");
                        }
                        else
                        {
                            ConsoleLog.WriteLine("Daily reward, фигня happened.");
                            //ConsoleLog.WriteLine("RewardDaily" + Environment.NewLine + m_Response, "RewardLog.txt");
                        }
            }
            catch (System.Exception e)
            {
                m_Client.Headers.Remove("X-Requested-With");
                throw new Exception(e.Message);
            }
        }

        public void Shout(string sShout)
        {
            m_Client.Headers.Add("X-Requested-With", "XMLHttpRequest");
            m_Client.Referer = "http://www.erepublik.com/en";
            string PostData = "ajax=true&_token=" + ms_Token2 + "&write_shout=" + System.Web.HttpUtility.UrlEncode(sShout);
            m_Response = m_Client.UploadString("http://www.erepublik.com/shoutbox/write", PostData);
            m_Client.Headers.Remove("X-Requested-With");
            //ConsoleLog.WriteLine("Shout" + Environment.NewLine + m_Response, "RewardLog.txt");
        }

        public void LookAlerts()
        {
            m_Client.Referer = "http://www.erepublik.com/en";
            m_Response = m_Client.DownloadString("http://www.erepublik.com/en/messages/alerts/1");
            //ConsoleLog.WriteLine("LookAlerts" + Environment.NewLine + m_Response, "RewardLog.txt");
        }

        public void ReadRandomTopArticle()
        {
            //throw new NotImplementedException();
        }

        public void ReportTicket(int iItemId, int iType, string sLang, string sItemName)
        {
            //Для iType
            //         1 Offensive ad
            //         2 Misleading ad
            //         3 Illegal link in ad
            //         4 Spam ad
            //         5 Other ad
            //         6 Vulgarity
            //         7 Insults
            //         8 Racism
            //         9 Pornography
            //         10 Spam
            //         11 External advertising
            //         12 Illegal links
            //         13 Trolling
            //         14 Flaming
            //         15 Accusations of not respecting the eRepublik Laws
            //         16 Illegal public debates
            //         17 Unlawful company picture
            //         18 Unlawful company name
            //         19 Unlawful company discussion area
            //         20 Unlawful party picture
            //         21 Unlawful party name
            //         22 Unlawful party discussion area
            //         23 Unlawful newspaper picture
            //         24 Unlawful newspaper name
            //         25 Illegal shout(s)
            //         26 Unlawful citizen/organization picture
            //         27 Unlawful citizen/organization name
            //         28 Unlawful citizen/organization shout(s)
            //         29 Multiple accounts usage

            //для sLang
            //         en English
            //         fr Français
            //         de Deutsch
            //         hu Magyar
            //         it Italiano
            //         pt Portugues
            //         ro Româna
            //         ru Русский
            //         es Español
            //         sv Svenska
            //         pl Polish
            //         gr Ελληνικά
            //         hr Hrvatska
            //         bg Български
            //         sr Српски
            //         tr Türkçe
            //         zz Other

            //для sItemName
            //         articles
            //         ads
            //         article_comments
            //         shouts
            //         citizens
            //         newspapers
            //         organizations

            m_Client.Referer = "http://www.erepublik.com/en";
            string PostData = "_token=" + ms_Token2 + "&report_reason=" + iType.ToString() + "&content_lang=" + sLang + "&report=Report";
            m_Response = m_Client.UploadString("http://www.erepublik.com/en/tickets/report/" + iItemId.ToString() + "/" + sItemName, PostData);
        }

        public bool BuyMoney(
            int iOfferId,   /*= 4756840; номер предложения*/
            double dAmount, /*= 0.01; сколько покупаем*/
            int iSellCurr,  /*= 1; валюта к продаже*/
            int iSellerId,  /*= 4001750; кто продает*/
            int iBuyCur     /*= 62; валюта к покупке*/
        )
        {
            bool result = false;

            //стучаться так
            //"http://www.erepublik.com/en/exchange/listOffers?select_page=select&buy_currency_history_id=0&sell_currency_history_id=0&account_type=citizen-&action_path=listOffers&page=page=2&buy_currency=62&sell_currency=41"
            try
            {
                System.Random rnd = new System.Random();
                string postData =
                    "select_page=select" +
                    "&account_type=citizen" +
                    "&page=" + rnd.Next(1, 10).ToString() +
                    "&offer_id=" + iOfferId.ToString() +
                    "&amount_to_accept=" + dAmount.ToString().Replace(',', '.') +
                    "&sell_currency=" + iSellCurr.ToString() +
                    "&company_id=" + iSellerId.ToString() +
                    "&buy_currency=" + iBuyCur.ToString() +
                    "&exchange_rate=" + ((double)(rnd.NextDouble() * 10)).ToString().Replace(',', '.') +
                    "&initial_amount=" + ((double)(rnd.NextDouble() * 10)).ToString().Replace(',', '.') +
                    "&action_path=listOffers" +
                    "&_token=" + ms_Token2 +
                    "&buy_currency_history_id=0" +
                    "&sell_currency_history_id=0";
                m_Client.Headers.Add("X-Requested-With", "XMLHttpRequest");
                m_Client.Referer = "http://www.erepublik.com/en/exchange";
                m_Response = m_Client.UploadString("http://www.erepublik.com/en/exchange/acceptOffer", postData);
                //ConsoleLog.WriteLine(m_Response, "GoldDrainLog.txt");
                result = true;
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Cannot buy money: " + e.Message);
            }
            m_Client.Headers.Remove("X-Requested-With");
            return result;
        }

        public bool DonateGold(int iUser, double dSum)
        {
            return DonateMoney(iUser, dSum, 62);
        }

        public bool DonateMoney(int iUser, double dSum, int iCurrency)
        {
            bool WasCaptcha = false, WasSuccess;
            string srcPage = "http://economy.erepublik.com/en/citizen/donate/money/" + iUser.ToString();
            try
            {
                m_Response = CustomRequest(srcPage);
                //ConsoleLog.WriteLine(m_Response, "DonateMoney.txt");

                string token = CommonUtils.GetStringBetween(
                    m_Response,
                    "donate_form[_csrf_token]\" value=\"",
                    "\"");
                //ConsoleLog.WriteLine("DonateMoney ");
                //m_Client.Headers.Add("X-Requested-With", "XMLHttpRequest");
                m_Client.Referer = srcPage;
                string PostData =
                    System.Web.HttpUtility.UrlEncode("donate_form[amount]") + "=" + dSum.ToString() + "&" +
                    System.Web.HttpUtility.UrlEncode("donate_form[currencyId]") + "=" + iCurrency.ToString() + "&" +
                    System.Web.HttpUtility.UrlEncode("donate_form[_csrf_token]") + "=" + token;

                ConsoleLog.WriteLine(PostData);
                m_Response = m_Client.UploadString(srcPage, PostData);
                //ConsoleLog.WriteLine(m_Response, "DonateMoney.txt");

                if (m_Response.Contains("captcha") || m_Response.Contains("manual_challenge"))
                {
                    WasCaptcha = true;

                    m_Client.Headers.Remove("X-Requested-With");
                    m_Response = m_Client.DownloadString("http://economy.erepublik.com/en/donate/captcha/money");

                    while (m_Response.Contains("manual_challenge"))
                    {
                        ConsoleLog.WriteLine(m_AuthLogin + ": donate money-captcha");
                        var captcha = CaptchaProvider.GetResolvedCaptcha();
                        string sToken = CommonUtils.GetToken(m_Response);
                        PostData = 
                            System.Web.HttpUtility.UrlEncode("captcha_form[_csrf_token]") + "=" + sToken + "&" +
                            "recaptcha_challenge_field=" + captcha.ChallengeID + 
                            "&recaptcha_response_field=" + System.Web.HttpUtility.UrlEncode(captcha.CaptchaText) + 
                            "&commit=Continue";
                        m_Response = m_Client.UploadString("http://economy.erepublik.com/en/donate/captcha/money", PostData);
                    }
                }
            }
            catch (System.Exception e)
            {
                m_Client.Headers.Remove("X-Requested-With");
                ConsoleLog.WriteLine("DonateMoney error: " + e.Message);
                return false;
            }

            WasSuccess = m_Response.Contains("success");
            m_Client.Headers.Remove("X-Requested-With");

            if (!WasCaptcha && WasSuccess)
            {
                ConsoleLog.WriteLine("DonateMoney success!");
                return true;
            }
            else
            {
                ConsoleLog.WriteLine("DonateMoney failed.");
                return false;
            }
        }

        public bool DonateMoneyToCountry(string Country, double Sum, int Currency)
        {
            bool WasCaptcha = false, WasSuccess;
            string srcPage = "http://www.erepublik.com/en/country/donate/" + Country;
            string postPage = "http://www.erepublik.com/country/donate/money?permalink=" + Country;
            try
            {
                m_Response = CustomRequest(srcPage);
                ConsoleLog.WriteLine(m_Response, "DonateMoneyToCountry.txt");

                string token = CommonUtils.GetStringBetween(
                    m_Response,
                    "name=\"_token\" value=\"",
                    "\"");
                //ConsoleLog.WriteLine("DonateMoney ");
                m_Client.Headers.Add("X-Requested-With", "XMLHttpRequest");
                m_Client.Referer = srcPage;
                string PostData =
                    "donate_sum=" + Sum.ToString() + "&" +
                    "_token=" + token + "&" +
                    "currency=" + Currency.ToString() + "&" +
                    "donate_to_country=" + Country;

                ConsoleLog.WriteLine(PostData);
                m_Response = m_Client.UploadString(postPage, PostData);
                ConsoleLog.WriteLine(m_Response, "DonateMoneyToCountryResp.txt");

                if (m_Response.Contains("captcha") || m_Response.Contains("manual_challenge"))
                {
                    ConsoleLog.WriteLine("DonateMoneyToCountry CAPTCHA!!!");
                    return false;

                    WasCaptcha = true;

                    m_Client.Headers.Remove("X-Requested-With");
                    m_Response = m_Client.DownloadString("http://economy.erepublik.com/en/donate/captcha/money");

                    while (m_Response.Contains("manual_challenge"))
                    {
                        ConsoleLog.WriteLine(m_AuthLogin + ": donate money-captcha");
                        var captcha = CaptchaProvider.GetResolvedCaptcha();
                        string sToken = CommonUtils.GetToken(m_Response);
                        PostData =
                            System.Web.HttpUtility.UrlEncode("captcha_form[_csrf_token]") + "=" + sToken + "&" +
                            "recaptcha_challenge_field=" + captcha.ChallengeID +
                            "&recaptcha_response_field=" + System.Web.HttpUtility.UrlEncode(captcha.CaptchaText) +
                            "&commit=Continue";
                        m_Response = m_Client.UploadString("http://economy.erepublik.com/en/donate/captcha/money", PostData);
                    }
                }
            }
            catch (System.Exception e)
            {
                m_Client.Headers.Remove("X-Requested-With");
                ConsoleLog.WriteLine("DonateMoney error: " + e.Message);
                return false;
            }

            WasSuccess = m_Response.Contains("success");
            m_Client.Headers.Remove("X-Requested-With");

            if (!WasCaptcha && WasSuccess)
            {
                ConsoleLog.WriteLine("DonateMoney success!");
                return true;
            }
            else
            {
                ConsoleLog.WriteLine("DonateMoney failed.");
                return false;
            }
        }

        List<int> GetInventory()
        {

            List<int> lst = new List<int>();
            m_Response = m_Client.DownloadString("http://economy.erepublik.com/en/inventory");
            Match mc = Regex.Match(m_Response, "li id=\"i_([0-9.]{1,9})");
            while (mc.Success)
            {
                int iId = int.Parse(mc.Groups[1].Value);
                lst.Add(iId);
                mc = mc.NextMatch();
            }
            return lst;
        }

        bool DonateAllInventoryToUser(List<int> lst, int iUser)
        {
            if (lst.Count == 0)
                return false;
            m_Client.Referer = "http://economy.erepublik.com/en/citizen/donate/"+iUser.ToString();
            string PostData = "";
            foreach (int el in lst)
            {
                PostData += "&products[]=" + el.ToString();
            }
            PostData.Trim('&');
            m_Response = m_Client.UploadString("http://economy.erepublik.com/en/citizen/donate/" + iUser.ToString(), PostData);
            return true;
        }

        bool DonateInventoryToUsers(List<int> lst, List<Pair2<int,int>> lstUsers)
        {
            while (lstUsers.Count>0)
            {
                if (lst.Count == 0)
                    return true;
                m_Client.Referer = "http://economy.erepublik.com/en/citizen/donate/" + lstUsers[0].First.ToString();
                string PostData = "";
                int i = lstUsers[0].Second;
                int j = 0;
                while (j<i&&lst.Count>0)
                {
                    PostData += "&products[]=" + lst[0].ToString();
                    lst.RemoveAt(0);
                    j++;
                }
                PostData.Trim('&');
                m_Response = m_Client.UploadString("http://economy.erepublik.com/en/citizen/donate/" + lstUsers[0].First.ToString(), PostData);
                if (lst.Count > 0)
                    lstUsers.RemoveAt(0);
                else
                    lstUsers[0].Second -= j;
            }
            return true;
        }

        /// <summary>
        /// Посещаем "Land" если первый раз родились
        /// </summary>
        /// <param name="jpeg"></param>
        /// <param name="sPath"></param>
        public void VisitLand(uint citizenID)
        {
            CustomRequest("http://economy.erepublik.com/en/land/overview/" + citizenID.ToString());
        }

        /// <summary>
        /// Логаут, чтобы убрать из списка "онлайн"
        /// </summary>
        public void Logout()
        {
            try
            {
                CustomRequest("http://www.erepublik.com/en/logout");
                ConsoleLog.WriteLine("Logged out");
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLine("Error logging out: " + e.Message);
            }
        }

        public bool RestoreSelfFirm(uint citizenID)
        {
            try
            {
                ConsoleLog.WriteLine(m_AuthLogin + ": Restoring firm");
                string land = "http://economy.erepublik.com/en/land/overview/" + citizenID;
                m_Response = m_Client.DownloadString(land);
                string restoreToken = CommonUtils.GetToken2(m_Response);

                if (!m_Response.Contains("/en/company/restore"))
                {
                    ConsoleLog.WriteLine("Allready restored");
                    return true;
                }

                string restoreURL = "http://economy.erepublik.com/en/company/restore" +
                    CommonUtils.GetStringBetween(
                        m_Response,
                        "href=\"/en/company/restore",
                        "\"");

                ConsoleLog.WriteLine("Restore url: " + restoreURL);

                m_Client.Referer = land;
                string PostData = "_csrf_token=" + restoreToken;
                m_Client.UploadString(restoreURL, PostData);

                ConsoleLog.WriteLine("Something restored");

                return false;
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Restore error: " + e.Message);

                return false;
            }
        }

        public void VisitInventory()
        {
            CustomRequest("http://www.erepublik.com/en/economy/inventory");
            //ConsoleLog.WriteLine(m_Response, "InventoryLog.txt");
            System.Threading.Thread.Sleep(5000);
            ConsoleLog.WriteLine("Inventory visited - " + m_AuthLogin);
        }
    }
}


