using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;

namespace NerZul.Network
{
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
        
        private WebProxy m_Proxy = null;
        private string m_Response = "", m_AuthLogin, m_AuthPassword;
        private HttpClient m_Client = new HttpClient();
        private string ms_Token = "";
        private string ms_Token2 = "";
        private int m_RegionID=0;
        private string ms_Proxy=null;
		public string Proxy
        {
            get { return ms_Proxy; }
            set {
				ms_Proxy = value;
				m_Proxy=new WebProxy("http://"+value+"/");
				m_Client.Proxy=m_Proxy;
			}
        }
        private static string mc_TokenScanString = "id=\"_token\" name=\"_token\" value=\"";
        
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
        public Bot(string UserName, string Password, string UserAgent)
        {
            m_AuthLogin = UserName;
            m_AuthPassword = Password;
            m_Client.UserAgent= UserAgent;
        }
        /// <summary>
        /// Свойство, определяющее то время, которое будет производиться ожидания ответа на запрос.
        /// Устанавливается в миллисекундах
        /// </summary>
        public int ConnectionTimeout
        {
            get { return m_Client.Timeout; }
            set { m_Client.Timeout = value; }
        }

        /// <summary>
        /// Выполняет процедуру авторизации в eRepublik. В это же время получает токены сессии.
        /// Для авторизации используются ранее запомненные параметры
        /// </summary>
        /// <returns></returns>
        public bool Login()
        {
            m_Response = m_Client.DownloadString("http://www.erepublik.com/en");
            m_Response = m_Response.Remove(0, m_Response.IndexOf(mc_TokenScanString) + mc_TokenScanString.Length);
            ms_Token = m_Response.Substring(0, m_Response.IndexOf("\""));
            string PostData = "_token=" + ms_Token +
            "&citizen_name=" + m_AuthLogin + "&citizen_password=" + m_AuthPassword;
            m_Response=m_Client.UploadString("http://www.erepublik.com/en/login", PostData);
            bool bOK= (m_Response.IndexOf("logout") != -1);
            if (!bOK) return false;
            ms_Token2 = m_Response.Remove(0, m_Response.IndexOf(mc_TokenScanString) + mc_TokenScanString.Length);
            ms_Token2 = ms_Token2.Substring(0,ms_Token2.IndexOf("\""));
            return true;
        }

        /// <summary>
        /// Получает список предложений работы
        /// </summary>
        /// <param name="Country">Страна. Если задать 0, то используется страна пребывания</param>
        /// <param name="Type">Отрасль. Допустимые значения - "all" ("a"), "land" ("l"),
        /// "manufacturing" ("m"), "constructions" ("c")</param>
        /// <param name="Skill">Уровень навыка. 0 - без фильтра. Нулевому скиллу соответствует 1.</param>
        /// <param name="Page">Страница с рынка вакансий, которую требуется загрузить.</param>
        /// <returns></returns>
        public List<OfferInfo> GetJobOffers(int Country, string Type,int Skill, int Page)
        {
            const string CompanyAffix = "href=\"/en/company/";
            const string QualityAffix = "<div class=\"qllevel\" style=\"width: ";
            const string LevelAfffix = "<span class=\"special\">";
            const string LevelSuffix = "</span>";
            const string SalaryAfffix1 = "<span class=\"special\">";
            const string SalarySuffix1 = "</span>";
            const string SalaryAfffix2 = "<sup>.";
            const string SalarySuffix2 = "</sup>";
            const string IDAffix="id=\"applybutton_";

            if (Type == "a") Type = "all";
            else if (Type == "l") Type = "land";
            else if (Type == "m") Type = "manufacturing";
            else if (Type == "c") Type = "constructions";
            string html=m_Response=m_Client.DownloadString("http://www.erepublik.com/en/human-resources/country-"
                +Country.ToString()+"-domain-"+Type+"-skill-"+Skill.ToString());
            int cPos = 0; int cPos2; string sVal;
            
            List<OfferInfo> list=new List<OfferInfo>();
            while ((cPos = html.IndexOf(CompanyAffix,cPos)) != -1)
            {
                OfferInfo offer=new OfferInfo();
                cPos += CompanyAffix.Length;
                //Вытаскиваем ID компании
                cPos2 = html.IndexOf('"',cPos);
                sVal = html.Substring(cPos, cPos2 - cPos);
                sVal = sVal.Substring(sVal.LastIndexOf('-') + 1);
                offer.Add("company", int.Parse(sVal));
                //Вытаскиваем звёздочки
                cPos = html.IndexOf(QualityAffix, cPos2);
                cPos += QualityAffix.Length;
                sVal = html[cPos].ToString();
                offer.Add("quality", int.Parse(sVal) / 2);
                //Вытаскиваем уровень
                cPos = html.IndexOf(LevelAfffix, cPos);
                cPos+=LevelAfffix.Length;
                cPos2=html.IndexOf(LevelSuffix,cPos);
                sVal=html.Substring(cPos,cPos2-cPos);
                offer.Add("level",int.Parse(sVal));
                //Вытаскиваем ЗП
                //Целая часть
                cPos = html.IndexOf(SalaryAfffix1, cPos);
                cPos+=SalaryAfffix1.Length;
                cPos2=html.IndexOf(SalarySuffix1,cPos);
                sVal=html.Substring(cPos,cPos2-cPos);
                //Дробная
                cPos = html.IndexOf(SalaryAfffix2, cPos);
                cPos+=SalaryAfffix2.Length;
                cPos2=html.IndexOf(SalarySuffix2,cPos);
                sVal+=html.Substring(cPos,cPos2-cPos);
                offer.Add("salary",int.Parse(sVal));
                //Вытаскиваем ID вакансии
                cPos = html.IndexOf(IDAffix, cPos);
                cPos += IDAffix.Length;
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
        {return GetJobOffers(Country,Type,0);}
        /// <summary>
        /// Получает список предложений работы
        /// </summary>
        /// <param name="Country">Страна</param>
        /// <returns></returns>
        public List<OfferInfo> GetJobOffers(int Country)
        {return GetJobOffers(Country,"all");}
        public List<OfferInfo> GetJobOffers()
        { return GetJobOffers(0); }
        #endregion
        /// <summary>
        /// Найм на работу по вакансии
        /// </summary>
        /// <param name="ID">Номер вакансии. Можно получить через GetJobOffers</param>
        /// <returns></returns>
        public bool Hire(int ID)
        {
            string PostData = "_token=" + ms_Token + "&job=" + ID.ToString();
            m_Client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            m_Response = m_Client.UploadString("http://www.erepublik.com/apply_for_job",PostData);
            if ((m_Response.IndexOf("url") == -1) || (m_Response.IndexOf("job_id") == -1)) return false;
            return true;
        }
        /// <summary>
        /// Попробовать поработать.
        /// </summary>
        /// <remarks>TODO: Допилить</remarks>
        /// <returns>Всегда false</returns>
        public bool Work()
        {
            m_Response=m_Client.DownloadString("http://www.erepublik.com/en/work");
            return false;
        }

        /// <summary>
        /// Запросы на работу. Множество их. Раньше было багоюзом, сейчас можно попробовать при
        /// загруженности сервера. Использовать без прокси
        /// </summary>
        /// <param name="cnt">Количество используемых потоков.</param>
        public void WorkFlood(int cnt)
        {
            m_Client.Flood("http://www.erepublik.com/en/work", cnt, "http://www.erepublik.com/en/company/moscow-airlines-206535");
        }

        public void FightFlood(int Battle)
        {
            m_Client.Flood("http://www.erepublik.com/en/battles/fight/" + Battle.ToString()+"/0", 10,
                "http://www.erepublik.com/en/battles/fight/" + Battle.ToString() + "/0", true, "_token=" + ms_Token2);
        }
        public void Fight(int Battle)
        {
            string PostData = "_token=" + ms_Token2;
            m_Client.Referer = "http://www.erepublik.com/en/battles/fight/" + Battle.ToString() + "/0";
            m_Client.UploadString(m_Client.Referer, PostData);

        }
        /// <summary>
        /// Обновление аватарки аккаунта
        /// </summary>
        /// <param name="jpeg">JPG-изображение с аватарой</param>
        /// <returns>true в случае успеха, false при несетевой ошибке</returns>
        public bool UploadAvatar(byte[] jpeg)
        {
            const string MailAffix="id=\"citizen_email\" value=\"";
            m_Response = m_Client.DownloadString("http://www.erepublik.com/en/citizen/edit/profile");
            int cPos = 0, cPos2 = 0; 
            //Вытаскиваем мыло
            cPos = m_Response.IndexOf(MailAffix);
            cPos += MailAffix.Length;
            cPos2 = m_Response.IndexOf('"', cPos);
            string Mail = m_Response.Substring(cPos, cPos2 - cPos);

            //Формируем тело POST-запроса
            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            string NameAffix = "--" + boundary + "\r\nContent-Disposition: form-data; name=\"";
            string NameSuffix = "\"\r\n\r\n";

            
            System.IO.MemoryStream postdata = new System.IO.MemoryStream();
            //Данные формы            
            string formdata = "";
            formdata += NameAffix + "_token" + NameSuffix + ms_Token + "\r\n";
            formdata += NameAffix + "citizen_email" + NameSuffix + Mail + "\r\n";
            formdata += NameAffix + "birth_date[]" + NameSuffix + "1984" + "\r\n";
            formdata += NameAffix + "birth_date[]" + NameSuffix + "2" + "\r\n";
            formdata += NameAffix + "birth_date[]" + NameSuffix + "5" + "\r\n";

            //Заголовок для файла
            formdata += NameAffix + "citizen_file\"; filename=\"avatar.jpg\"\r\n";
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
            System.IO.File.WriteAllBytes("log.txt", buffer);
            m_Client.Timeout = m_Client.Timeout * 10;
            try
            {
                m_Response = m_Client.UploadMultipartData("http://www.erepublik.com/en/citizen/edit/profile", buffer, boundary);
            }
            finally
            {
                m_Client.Timeout = m_Client.Timeout / 10;
            }
            return (m_Response.IndexOf("You have succesfully edited your profile") != 0);
        }

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
            const string IDAffix="name=\"amount_offer\" id=\"amount_";
            m_Response = m_Client.DownloadString("http://www.erepublik.com/en/market/country-" +
                Country.ToString() + "-industry-" + Type.ToString() + "-quality-" + Quality.ToString() +
                "-citizen_account-/1");
            int cPos = 0, cPos2 = 0;
            List<OfferInfo> list=new List<OfferInfo>();
            while ((cPos = m_Response.IndexOf(CompanyAffix,cPos)) != -1)
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
                sVal = m_Response.Substring(cPos,2);
                offer.Add("quality", int.Parse(sVal)/20);
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
            string RegionIDAffix="current_region_id\" value=\"";
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
            m_Client.Referer="http://www.erepublik.com/en/citizen/change-residence";
            string PostData = "_token=" + ms_Token + "&commit=Move";
            PostData += "&isPostBack=";
            PostData += "&country_list=" + Country.ToString();
            PostData += "&current_region_id=" + RegionID.ToString();

            PostData += "&region_list=" + Region.ToString();
            PostData += "&region_selected_id=";

            
            m_Client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            m_Response=m_Client.UploadString("http://www.erepublik.com/en/citizen/change-residence",PostData);
            return m_Response.Contains("You have successfully moved to");

        }
        /// <summary>
        /// Оживлялка. Используется если действительно надо оживить. Ранее использовалось для багоюза
        /// </summary>
        public void Revive()
        {
            m_Client.Referer = "http://www.erepublik.com/en";
            string PostData = "_token=" + ms_Token2;
            this.m_Response = m_Client.UploadString("http://www.erepublik.com/en/dead_revive", PostData);
            //m_Response = m_Response;
        }
        /// <summary>
        /// Голосует за статью
        /// </summary>
        /// <param name="ArticleID">ID статьи</param>
        public void VoteArticle(int ArticleID)
        {
            m_Client.Referer = "http://www.erepublik.com/en/article/a-" + ArticleID.ToString() + "/1/20";
            string PostData = "_token=" + ms_Token2 + "&article_id=" + ArticleID.ToString();
            m_Response = m_Client.UploadString("http://www.erepublik.com/vote-article", PostData);
        }

    }
}
