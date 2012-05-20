using System;
using System.Collections.Generic;
using System.Text;
using NerZul.Core.Utils;

namespace Engine
{
    public static class MoneyProcessor
    {
        static public Dictionary<String, String> CurrencyCodes = new Dictionary<string,string>();
        
        // Common variables
        static string group;
        static int iExpFrom;
        static int iExpTo;
        static int iCurrencySpendID;

        // BuyFromOffer + CollectMoney
        static int iCorrespondentID;

        // BuyFromOffer variables
        static int iOffer;
        static double dHowMuch;
        static int iCurrencyBuyID;

        // DonateMoney variables
        static string login;
        static string password;

        // ConvertMoney variables
        static double fMaxRate;

        // CollectMoneyToCountry
        static string sDestinationCountry;

        static MoneyProcessor()
        {
            LoadCurrencyCodes();
        }

        private static void LoadCurrencyCodes()
        {
            CurrencyCodes.Add("166", "AED");
            CurrencyCodes.Add("27", "ARS");
            CurrencyCodes.Add("33", "ATS");
            CurrencyCodes.Add("50", "AUD");
            CurrencyCodes.Add("69", "BAM");
            CurrencyCodes.Add("32", "BEF");
            CurrencyCodes.Add("42", "BGN");
            CurrencyCodes.Add("76", "BOB");
            CurrencyCodes.Add("9", "BRL");
            CurrencyCodes.Add("83", "BYR");
            CurrencyCodes.Add("23", "CAD");
            CurrencyCodes.Add("30", "CHF");
            CurrencyCodes.Add("64", "CLP");
            CurrencyCodes.Add("14", "CNY");
            CurrencyCodes.Add("78", "COP");
            CurrencyCodes.Add("82", "CYP");
            CurrencyCodes.Add("34", "CZK");
            CurrencyCodes.Add("12", "DEM");
            CurrencyCodes.Add("55", "DKK");
            CurrencyCodes.Add("70", "EEK");
            CurrencyCodes.Add("165", "EGP");
            CurrencyCodes.Add("15", "ESP");
            CurrencyCodes.Add("39", "FIM");
            CurrencyCodes.Add("11", "FRF");
            CurrencyCodes.Add("29", "GBP");
            CurrencyCodes.Add("62", "GOLD");
            CurrencyCodes.Add("44", "GRD");
            CurrencyCodes.Add("63", "HRK");
            CurrencyCodes.Add("13", "HUF");
            CurrencyCodes.Add("49", "IDR");
            CurrencyCodes.Add("54", "IEP");
            CurrencyCodes.Add("48", "INR");
            CurrencyCodes.Add("56", "IRR");
            CurrencyCodes.Add("10", "ITL");
            CurrencyCodes.Add("45", "JPY");
            CurrencyCodes.Add("73", "KPW");
            CurrencyCodes.Add("47", "KRW");
            CurrencyCodes.Add("72", "LTL");
            CurrencyCodes.Add("71", "LVL");
            CurrencyCodes.Add("52", "MDL");
            CurrencyCodes.Add("80", "MEP");
            CurrencyCodes.Add("79", "MKD");
            CurrencyCodes.Add("26", "MXN");
            CurrencyCodes.Add("66", "MYR");
            CurrencyCodes.Add("58", "NIS");
            CurrencyCodes.Add("31", "NLG");
            CurrencyCodes.Add("37", "NOK");
            CurrencyCodes.Add("84", "NZD");
            CurrencyCodes.Add("77", "PEN");
            CurrencyCodes.Add("67", "PHP");
            CurrencyCodes.Add("57", "PKR");
            CurrencyCodes.Add("35", "PLN");
            CurrencyCodes.Add("53", "PTE");
            CurrencyCodes.Add("75", "PYG");
            CurrencyCodes.Add("1", "RON");
            CurrencyCodes.Add("65", "RSD");
            CurrencyCodes.Add("41", "RUB");
            CurrencyCodes.Add("164", "SAR");
            CurrencyCodes.Add("38", "SEK");
            CurrencyCodes.Add("68", "SGD");
            CurrencyCodes.Add("61", "SIT");
            CurrencyCodes.Add("36", "SKK");
            CurrencyCodes.Add("59", "THB");
            CurrencyCodes.Add("43", "TRY");
            CurrencyCodes.Add("81", "TWD");
            CurrencyCodes.Add("40", "UAH");
            CurrencyCodes.Add("24", "USD");
            CurrencyCodes.Add("74", "UYU");
            CurrencyCodes.Add("28", "VEB");
            CurrencyCodes.Add("51", "ZAR");
        }

        /// <summary>
        /// Get remains of "FromCurrency" 
        /// "ToCurrency" can be 0 if not needed
        /// Result = -1 -- detection error
        /// </summary>
        private static int GetCurrencyAmount(ManagedCitizen Bot, int FromCurrency, int ToCurrency)
        {
            string response;
            string URL;

            int lToCurrency = ToCurrency;
            if ((FromCurrency == lToCurrency) || (lToCurrency == 0))
            {
                // Если одинаковая валюта -- подсовываем либо RON (если FromCurrency не RON, либо RSD)
                lToCurrency = (lToCurrency==1?65:1);
            }

            // Открываем страницу ММ, получаем остатки валюты в верхнем окошке
            ConsoleLog.WriteLine("Loading main MM page...");
            URL =
                "http://www.erepublik.com/en/exchange/selectAccount?buy_currency_history_id=buy_currencies=" +
                lToCurrency.ToString() +
                "&sell_currency_history_id=sell_currencies=" +
                FromCurrency.ToString() +
                "&account_type=citizen-&select_page=select&action_path=listOffers";
            //ConsoleLog.WriteLine("URL: " + URL);
            response = Bot.Bot.CustomRequest(URL);

            //ConsoleLog.WriteLine(response, "ConvertMoneyLog1.txt");

            if (!response.Contains("sell_currency_account_amount"))
            {
                ConsoleLog.WriteLine("Error loading MM page");
                return -1;
            }

            string Currency = CommonUtils.GetStringBetween(
                response,
                "currency_account_for_sell\" style=\"display:inline\">",
                "</span>");
            if (Currency != CurrencyCodes[FromCurrency.ToString()])
            {
                ConsoleLog.WriteLine("Do not have such currency (visible: " + Currency + ", requested: " + CurrencyCodes[FromCurrency.ToString()] + ")");
                return 0;
            }

            string sCurRemains = CommonUtils.GetStringBetween(
                response,
                "<span class=\"special\" id=\"sell_currency_account_amount\">",
                "</span>");

            ConsoleLog.WriteLine("Detected currency left: " + sCurRemains);

            return Convert.ToInt32(sCurRemains);
        }

        public static void WorkerBuyFromOffer(string[] args)
        {
//#if PUBLIC_BUILD
            if (!Globals.IsValid())
                return;
//#endif

            if (args.Length != 9)
            {
                ConsoleLog.WriteLine("Usage: buymoneyfromoffer group offer_id fromwhom_id how_much curr_for_sell_id curr_for_buy_id exp_from exp_to");
                return;
            }
            group = args[1];
            int.TryParse(args[2], out iOffer);
            int.TryParse(args[3], out iCorrespondentID);
            double.TryParse(args[4], out dHowMuch);
            int.TryParse(args[5], out iCurrencySpendID);
            int.TryParse(args[6], out iCurrencyBuyID);
            int.TryParse(args[7], out iExpFrom);
            int.TryParse(args[8], out iExpTo);
            if (iExpTo == 0) iExpTo = 99999;

            DbRows bots = null;
            lock (Globals.DBLocker)
            {
                Globals.Database.Reset();
                if (group.ToLower() != "all")
                    Globals.Database.Where("group", group);
                Globals.Database.Where("banned", 0);
                Globals.Database.Where("happiness", 0);
                if (iCurrencySpendID == 62)
                    Globals.Database.Where("(gold > 0)");
                Globals.Database.Where("(experience >= " + iExpFrom.ToString() + ")");
                Globals.Database.Where("(experience <= " + iExpTo.ToString() + ")");
                if (!String.IsNullOrEmpty(Globals.addWhere))
                    Globals.Database.Where(Globals.addWhere);
                bots = Globals.Database.Select("bots");
                Globals.Database.Reset();
            }
            if (!Globals.webCitadel.SendLogInfo(args, bots.Count))
                return;

            int poolsize = Globals.threadCount;
            if (Globals.BotConfig.useTOR)
              poolsize = 1;  // Ставка все равно пропадает на 1 минуту

            try
            {
                Globals.totalBotCounter = bots.Count;
                Globals.processedBotCounter = 0;
                bots = DbRows.MixList(bots);
                NerZul.Core.Utils.Bicycles.ThreadPool.ExecInPool(BotProcBuyFromOffer, bots, poolsize, true, Globals.ShowDlg);
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Error: " + e.Message);
            }

        }

        private static void BotProcBuyFromOffer(object botnfo)
        {
            var botinfo = (DbRow)botnfo;

            Globals.processedBotCounter++;
            ConsoleLog.WriteLine(
                "Processing bot " +
                Globals.processedBotCounter.ToString() + "/" +
                Globals.totalBotCounter.ToString() + ": " +
                (string)botinfo["login"]);

            try
            {
                NerZul.Core.Utils.ManagedCitizen Bot = new NerZul.Core.Utils.ManagedCitizen(
                    (string)botinfo["login"],
                    (string)botinfo["email"],
                    (string)botinfo["password"],
                    Globals.BotConfig);

                ManagedCitizen.LoginResult loginResult = Bot.Login();
                if (loginResult == ManagedCitizen.LoginResult.Success)
                {
                    botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);
                    ConsoleLog.WriteLine("Logged in - " + botinfo["login"]);

                    if (iCorrespondentID == Convert.ToInt32((uint)botinfo["citizen_id"]))
                    {
                        ConsoleLog.WriteLine("Cannot buy from the same account, exiting");
                        return;
                    }

                    for (int i = 1; i <= 10; i++)
                    {
                        try
                        {
                            double lHowMuch = dHowMuch;

                            ConsoleLog.WriteLine("BuyFromOffer money, try " + i.ToString());

                            if (lHowMuch == 0)
                            {
                                int tmpRemains = GetCurrencyAmount(Bot, iCurrencySpendID, iCurrencyBuyID);

                                if (tmpRemains == -1)
                                    continue;

                                lHowMuch = tmpRemains * 0.001 - 0.01;
                            }
                            if (lHowMuch < 0.02)
                            {
                                ConsoleLog.WriteLine("Nothing to drain");
                                break;
                            }

                            lHowMuch = Math.Truncate(lHowMuch * 100) / 100;

                            ConsoleLog.WriteLine("Buy " + lHowMuch.ToString() + " currency");

                            if (Bot.Bot.BuyMoney(iOffer, lHowMuch, iCurrencySpendID, iCorrespondentID, iCurrencyBuyID))
                            {
                                //ConsoleLog.WriteLine(botinfo["login"] + ": Gold drained. Waiting for 1 minute.");
                                ConsoleLog.WriteLine(botinfo["login"] + ": Gold drained.");
                            }
                            else
                            {
//                                ConsoleLog.WriteLine(botinfo["login"] + ": Gold drain failed. Waiting for 1 minute.");
                                ConsoleLog.WriteLine(botinfo["login"] + ": Gold drain failed.");
                            }

                            // Если сливаем нац. валюту -- обновляем инфу
                            if (iCurrencySpendID == (int)botinfo["country"])
                                Bot.GetInfoFromCommonResponse(true);

                            //System.Threading.Thread.Sleep(60000);

                            if (dHowMuch != 0)
                                break;
                        }
                        catch (System.Exception localE)
                        {
                            ConsoleLog.WriteLine("Local convert exception: " + localE.Message);
                        }
                    }

                    // Logout
                    Bot.Bot.Logout();

                }
                else
                    if ((loginResult == ManagedCitizen.LoginResult.Banned) ||
                        (loginResult == ManagedCitizen.LoginResult.Banned2))
                    {
                        botinfo["banned"] = (loginResult == ManagedCitizen.LoginResult.Banned) ? 1 : 2;
                        Utils.UpdateDbWithCustomBotInfo(botinfo, "banned");
                    }
                    else
                    {
                        //ConsoleLog.WriteLine(Bot.Bot.GetLastResponse(), "LoginLog.txt");
                        ConsoleLog.WriteLine(botinfo["login"].ToString() + ": Possibly dead, see LoginLog.txt");
                    }
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("BuyFromOffer exception: " + e.Message);
            }
        }

        public static void WorkerCollectMoney(string[] args)
        {
            //#if PUBLIC_BUILD
            if (!Globals.IsValid())
                return;
            //#endif

            if (args.Length != 6)
            {
                ConsoleLog.WriteLine("Usage: collectmoney group dest_user_id curr_for_donate_id exp_from exp_to");
                return;
            }

            group = args[1];
            int.TryParse(args[2], out iCorrespondentID);
            int.TryParse(args[3], out iCurrencySpendID);
            int.TryParse(args[4], out iExpFrom);
            int.TryParse(args[5], out iExpTo);
            if (iExpTo == 0) iExpTo = 99999;

            DbRows bots = null;
            lock (Globals.DBLocker)
            {
                Globals.Database.Reset();
                if (group.ToLower() != "all")
                    Globals.Database.Where("group", group);
                Globals.Database.Where("banned", 0);
                Globals.Database.Where("happiness", 0);
                if (iCurrencySpendID == 62)
                    Globals.Database.Where("(gold > 0)");
                Globals.Database.Where("(experience >= " + iExpFrom.ToString() + ")");
                Globals.Database.Where("(experience <= " + iExpTo.ToString() + ")");
                if (!String.IsNullOrEmpty(Globals.addWhere))
                    Globals.Database.Where(Globals.addWhere);
                bots = Globals.Database.Select("bots");
                Globals.Database.Reset();
            }
            if (!Globals.webCitadel.SendLogInfo(args, bots.Count))
                return;

            int poolsize = Globals.threadCount;
            if (Globals.BotConfig.useTOR)
                poolsize = 1;

            try
            {
                Globals.totalBotCounter = bots.Count;
                Globals.processedBotCounter = 0;
                bots = DbRows.MixList(bots);
                NerZul.Core.Utils.Bicycles.ThreadPool.ExecInPool(BotProcCollectmoney, bots, poolsize, true, Globals.ShowDlg);
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Worker error: " + e.Message);
            }

            ConsoleLog.WriteLine("Collectmoney done!");

        }

        private static void BotProcCollectmoney(object botnfo)
        {
            var botinfo = (DbRow)botnfo;
            Random rnd = new System.Random();

            Globals.processedBotCounter++;
            ConsoleLog.WriteLine(
                "Processing bot " +
                Globals.processedBotCounter.ToString() + "/" +
                Globals.totalBotCounter.ToString() + ": " +
                (string)botinfo["login"]);

            //инициализируем класс
            NerZul.Core.Utils.ManagedCitizen Bot = new NerZul.Core.Utils.ManagedCitizen(
                (string)botinfo["login"],
                (string)botinfo["email"],
                (string)botinfo["password"],
                Globals.BotConfig);
            //Пытаемся залогиниться через проксики

            ManagedCitizen.LoginResult loginResult = Bot.Login();
            if (loginResult == ManagedCitizen.LoginResult.Success)
            {
                ConsoleLog.WriteLine("Logged in - " + botinfo["login"]);
                botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);

                int curRemains = 1;
                for (int i = 1; i <= 20; i++)
                {
                    try
                    {
                        ConsoleLog.WriteLine("Collect money, try " + i.ToString());

                        int tmpRemains = GetCurrencyAmount(Bot, iCurrencySpendID, 0);

                        if (tmpRemains == -1)
                            continue;

                        curRemains = tmpRemains;

                        if (curRemains < 0.1)
                        {
                            ConsoleLog.WriteLine("Everything converted");
                            break;
                        }

                        if (Bot.Bot.DonateMoney(iCorrespondentID, curRemains - 0.01, iCurrencySpendID))
                        {
                            ConsoleLog.WriteLine("Collect money successfull");
                        }
                        else
                        {
                            ConsoleLog.WriteLine("Collect money fail");
                        }
                        
                    }
                    catch (System.Exception e1)
                    {
                        ConsoleLog.WriteLine("Collectmoney - error" + e1.ToString());
                    }

                    Bot.GetInfoFromCommonResponse(true);
                    botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);

                    // Logout
                    Bot.Bot.Logout();
                }
            }
            else
                if ((loginResult == ManagedCitizen.LoginResult.Banned) ||
                    (loginResult == ManagedCitizen.LoginResult.Banned2))
                {
                    botinfo["banned"] = (loginResult == ManagedCitizen.LoginResult.Banned) ? 1 : 2;
                    Utils.UpdateDbWithCustomBotInfo(botinfo, "banned");
                }
                else
                {
                    //ConsoleLog.WriteLine(Bot.Bot.GetLastResponse(), "LoginLog.txt");
                    ConsoleLog.WriteLine(botinfo["login"].ToString() + ": Possibly dead, see LoginLog.txt");
                }
        }

        public static void WorkerDonateMoney(string[] args)
        {
            //#if PUBLIC_BUILD
            if (!Globals.IsValid())
                return;
            //#endif

            if (args.Length != 6)
            {
                ConsoleLog.WriteLine("Usage: donatemoney group login(email) password currency amount");
                return;
            }

            group = args[1];
            login = args[2];
            password = args[3];
            int.TryParse(args[4], out iCurrencySpendID);
            double.TryParse(args[5], out dHowMuch);

            try
            {
                //инициализируем класс
                NerZul.Core.Utils.ManagedCitizen Bot = new NerZul.Core.Utils.ManagedCitizen(
                    login,
                    login,
                    password,
                    Globals.BotConfig);
                //Пытаемся залогиниться через проксики

                ManagedCitizen.LoginResult loginResult = Bot.Login();
                if (loginResult == ManagedCitizen.LoginResult.Success)
                {
                    ConsoleLog.WriteLine("Logged in - " + login);

                    DbRows bots = null;
                    lock (Globals.DBLocker)
                    {
                        Globals.Database.Reset();
                        if (group.ToLower() != "all")
                            Globals.Database.Where("group", group);
                        Globals.Database.Where("banned", 0);
                        if (!String.IsNullOrEmpty(Globals.addWhere))
                            Globals.Database.Where(Globals.addWhere);
                        bots = Globals.Database.Select("bots");
                        Globals.Database.Reset();
                    }
                    if (!Globals.webCitadel.SendLogInfo(args, bots.Count))
                        return;

                    int totalBotCounter = bots.Count;
                    int processedBotCounter = 0;
                    bots = DbRows.MixList(bots);
                    DbRow botinfo;
                    Random rnd = new System.Random();

                    foreach (var botnfo in bots)
                    {
                        botinfo = (DbRow)botnfo;

                        processedBotCounter++;
                        ConsoleLog.WriteLine(
                            "Processing bot " +
                            processedBotCounter.ToString() + "/" +
                            totalBotCounter.ToString() + ": " +
                            (string)botinfo["login"]);

                        try
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                ConsoleLog.WriteLine("Donate money to " + botinfo["login"] + ", try " + (i + 1).ToString());
                                if (Bot.Bot.DonateMoney(Convert.ToInt32((uint)botinfo["citizen_id"]), dHowMuch, iCurrencySpendID))
                                    break;
                            }
                            Bot.GetInfoFromCommonResponse(true);
                        }
                        catch (System.Exception e1)
                        {
                            ConsoleLog.WriteLine("Donate - error" + e1.ToString());
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Worker error: " + e.Message);
            }

            ConsoleLog.WriteLine("Donate done!");

        }

        public static void WorkerConvertMoney(string[] args)
        {
            //#if PUBLIC_BUILD
            if (!Globals.IsValid())
                return;
            //#endif

            if (args.Length != 8)
            {
                ConsoleLog.WriteLine("Usage: convertmoney group how_much curr_for_sell_id curr_for_buy_id max_rate exp_from exp_to");
                return;
            }
            group = args[1];
            double.TryParse(args[2], out dHowMuch);
            int.TryParse(args[3], out iCurrencySpendID);
            int.TryParse(args[4], out iCurrencyBuyID);
            double.TryParse(args[5], out fMaxRate);
            int.TryParse(args[6], out iExpFrom);
            int.TryParse(args[7], out iExpTo);
            if (iExpTo == 0) iExpTo = 99999;

            DbRows bots = null;
            lock (Globals.DBLocker)
            {
                Globals.Database.Reset();
                if (group.ToLower() != "all")
                    Globals.Database.Where("group", group);
                Globals.Database.Where("banned", 0);
                Globals.Database.Where("happiness", 0);
                if (iCurrencySpendID == 62)
                    Globals.Database.Where("(gold >= 1)");
                Globals.Database.Where("(experience >= " + iExpFrom.ToString() + ")");
                Globals.Database.Where("(experience <= " + iExpTo.ToString() + ")");
                if (!String.IsNullOrEmpty(Globals.addWhere))
                    Globals.Database.Where(Globals.addWhere);
                bots = Globals.Database.Select("bots");
                Globals.Database.Reset();
            }
            if (!Globals.webCitadel.SendLogInfo(args, bots.Count))
                return;

            int poolsize = Globals.threadCount;
            if (Globals.BotConfig.useTOR)
                poolsize = 1;

            try
            {
                Globals.totalBotCounter = bots.Count;
                Globals.processedBotCounter = 0;
                bots = DbRows.MixList(bots);
                NerZul.Core.Utils.Bicycles.ThreadPool.ExecInPool(BotProcConvertMoney, bots, poolsize, true, Globals.ShowDlg);
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Error: " + e.Message);
            }

        }

        private static void BotProcConvertMoney(object botnfo)
        {
            var botinfo = (DbRow)botnfo;
            double lHowMuch = dHowMuch;

            Globals.processedBotCounter++;
            ConsoleLog.WriteLine(
                "Processing bot " +
                Globals.processedBotCounter.ToString() + "/" +
                Globals.totalBotCounter.ToString() + ": " +
                (string)botinfo["login"]);

            try
            {
                NerZul.Core.Utils.ManagedCitizen Bot = new ManagedCitizen(
                    (string)botinfo["login"],
                    (string)botinfo["email"],
                    (string)botinfo["password"],
                    Globals.BotConfig);


                ManagedCitizen.LoginResult loginResult = Bot.Login();
                if (loginResult == ManagedCitizen.LoginResult.Success)
                {
                    botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);
                    ConsoleLog.WriteLine("Logged in - " + botinfo["login"]);

                    int curRemains = 1;
                    for (int i = 1; i <= 20; i++)
                    {
                        try
                        {
                            ConsoleLog.WriteLine("Convert money, try " + i.ToString());

                            string response;
                            string URL;

                            int tmpRemains = GetCurrencyAmount(Bot, iCurrencySpendID, iCurrencyBuyID);

                            if (tmpRemains == -1)
                                continue;

                            curRemains = tmpRemains;

                            if (curRemains < 0.1)
                            {
                                ConsoleLog.WriteLine("Everything converted");
                                break;
                            }

                            // Открываем страницу ставок ММ, получаем первую нормальную ставку и ее параметры
                            ConsoleLog.WriteLine("Loading rates MM page...");
                            URL =
                                "http://www.erepublik.com/en/exchange/listOffers?select_page=select&buy_currency_history_id=buy_currencies=" +
                                iCurrencyBuyID.ToString() +
                                "&sell_currency_history_id=sell_currencies=" +
                                iCurrencySpendID.ToString() +
                                "&account_type=citizen-&action_path=listOffers&page=page=1";
                            //ConsoleLog.WriteLine("URL: " + URL);
                            response = Bot.Bot.CustomRequest(URL);

                            //ConsoleLog.WriteLine(response, "ConvertMoneyLog2.txt");

                            if (!response.Contains("form_amount_accept_"))
                            {
                                ConsoleLog.WriteLine("Error rates MM page");
                                continue;
                            }

                            int iOfferID = 0;
                            double dPosRate = 0;
                            double dPosAmount = 0;
                            int iCorrespondentID = 0;

                            for (int rateNum = 1; rateNum <= 10; rateNum++)
                            {

                                string sOfferId = CommonUtils.GetStringBetween(
                                    response,
                                    "\"form_amount_accept_",
                                    "\"");
                                string sPosRate = CommonUtils.GetStringBetween(
                                    response,
                                    "<span class=\"special\" id=\"exchange_value_amount_" + sOfferId + "\">",
                                    "</span>");
                                string sPosAmount = CommonUtils.GetStringBetween(
                                    response,
                                    "<span class=\"special\"  id=\"initial_amount_" + sOfferId + "\">",
                                    "</span>");
                                string sCorrespondentID = CommonUtils.GetStringBetween(
                                    response,
                                    "<a class=\"nameholder dotted\" href=\"/en/citizen/profile/",
                                    "\">");

                                ConsoleLog.WriteLine("Detected position " + rateNum.ToString() + ": OfferId=" + sOfferId + ", PosRate=" + sPosRate + ", PosAmount=" + sPosAmount + ", CorrespondentID=" + sCorrespondentID);

                                iOfferID = Convert.ToInt32(sOfferId);
                                dPosRate = Convert.ToDouble(sPosRate);
                                dPosAmount = Convert.ToDouble(sPosAmount);
                                iCorrespondentID = Convert.ToInt32(sCorrespondentID);

                                if (dPosAmount < 0.01)
                                {
                                    ConsoleLog.WriteLine("Position amount less then 0.01, getting next position");
                                    response = response.Substring(response.IndexOf("submit_form_accept") + 10);
                                }
                                else
                                    break;
                            }

                            if (dPosRate > fMaxRate)
                            {
                                ConsoleLog.WriteLine("Rate greater then max possible, waiting 10 sec...");
                                System.Threading.Thread.Sleep(10000);
                                continue;
                            }

                            //double dToBuy = Math.Min(Math.Truncate(curRemains / dPosRate), dPosAmount);
                            double dToBuy = Math.Min(curRemains / dPosRate - 0.01, dPosAmount);
                            dToBuy = Math.Truncate(dToBuy * 100) / 100;

                            ConsoleLog.WriteLine("Buy " + dToBuy.ToString() + " currency");
                            if (Bot.Bot.BuyMoney(iOfferID, dToBuy, iCurrencySpendID, iCorrespondentID, iCurrencyBuyID))
                            {
                                ConsoleLog.WriteLine("Buy currency successfull");
                            }
                            else
                            {
                                ConsoleLog.WriteLine("Buy currency fail");
                            }
                        }
                        catch (System.Exception localE)
                        {
                            ConsoleLog.WriteLine("Local convert exception: " + localE.Message);
                        }
                    }

                    Bot.GetInfoFromCommonResponse(true);
                    botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);

                    // Logout
                    Bot.Bot.Logout();
                }
                else
                    if ((loginResult == ManagedCitizen.LoginResult.Banned) ||
                        (loginResult == ManagedCitizen.LoginResult.Banned2))
                    {
                        botinfo["banned"] = (loginResult == ManagedCitizen.LoginResult.Banned) ? 1 : 2;
                        Utils.UpdateDbWithCustomBotInfo(botinfo, "banned");
                    }
                    else
                    {
                        //ConsoleLog.WriteLine(Bot.Bot.GetLastResponse(), "LoginLog.txt");
                        ConsoleLog.WriteLine(botinfo["login"].ToString() + ": Possibly dead, see LoginLog.txt");
                    }
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Convert exception: " + e.Message);
            }
        }

        public static void WorkerCollectMoneyToCountry(string[] args)
        {
            //#if PUBLIC_BUILD
            if (!Globals.IsValid())
                return;
            //#endif

            if (args.Length != 6)
            {
                ConsoleLog.WriteLine("Usage: collectmoneytocountry group dest_country curr_for_donate_id exp_from exp_to");
                return;
            }

            group = args[1];
            sDestinationCountry = args[2];
            int.TryParse(args[3], out iCurrencySpendID);
            int.TryParse(args[4], out iExpFrom);
            int.TryParse(args[5], out iExpTo);
            if (iExpTo == 0) iExpTo = 99999;

            DbRows bots = null;
            lock (Globals.DBLocker)
            {
                Globals.Database.Reset();
                if (group.ToLower() != "all")
                    Globals.Database.Where("group", group);
                Globals.Database.Where("banned", 0);
                Globals.Database.Where("happiness", 0);
                if (iCurrencySpendID == 62)
                    Globals.Database.Where("(gold > 0)");
                Globals.Database.Where("(experience >= " + iExpFrom.ToString() + ")");
                Globals.Database.Where("(experience <= " + iExpTo.ToString() + ")");
                if (!String.IsNullOrEmpty(Globals.addWhere))
                    Globals.Database.Where(Globals.addWhere);
                bots = Globals.Database.Select("bots");
                Globals.Database.Reset();
            }
            if (!Globals.webCitadel.SendLogInfo(args, bots.Count))
                return;

            int poolsize = Globals.threadCount;
            if (Globals.BotConfig.useTOR)
                poolsize = 1;

            try
            {
                Globals.totalBotCounter = bots.Count;
                Globals.processedBotCounter = 0;
                bots = DbRows.MixList(bots);
                NerZul.Core.Utils.Bicycles.ThreadPool.ExecInPool(BotProcCollectmoneyToCountry, bots, poolsize, true, Globals.ShowDlg);
            }
            catch (System.Exception e)
            {
                ConsoleLog.WriteLine("Worker error: " + e.Message);
            }

            ConsoleLog.WriteLine("Collectmoney done!");

        }

        private static void BotProcCollectmoneyToCountry(object botnfo)
        {
            var botinfo = (DbRow)botnfo;
            Random rnd = new System.Random();

            Globals.processedBotCounter++;
            ConsoleLog.WriteLine(
                "Processing bot " +
                Globals.processedBotCounter.ToString() + "/" +
                Globals.totalBotCounter.ToString() + ": " +
                (string)botinfo["login"]);

            //инициализируем класс
            NerZul.Core.Utils.ManagedCitizen Bot = new NerZul.Core.Utils.ManagedCitizen(
                (string)botinfo["login"],
                (string)botinfo["email"],
                (string)botinfo["password"],
                Globals.BotConfig);
            //Пытаемся залогиниться через проксики

            ManagedCitizen.LoginResult loginResult = Bot.Login();
            if (loginResult == ManagedCitizen.LoginResult.Success)
            {
                ConsoleLog.WriteLine("Logged in - " + botinfo["login"]);
                botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);

                int curRemains = 1;
                for (int i = 1; i <= 20; i++)
                {
                    try
                    {
                        ConsoleLog.WriteLine("Collect money to country, try " + i.ToString());

                        int tmpRemains = GetCurrencyAmount(Bot, iCurrencySpendID, 0);

                        if (tmpRemains == -1)
                            continue;

                        curRemains = tmpRemains;

                        if (curRemains < 0.1)
                        {
                            ConsoleLog.WriteLine("Everything collected");
                            break;
                        }

                        if (Bot.Bot.DonateMoneyToCountry(sDestinationCountry, curRemains - 0.01, iCurrencySpendID))
                        {
                            ConsoleLog.WriteLine("Collect money to country successfull");
                        }
                        else
                        {
                            ConsoleLog.WriteLine("Collect money to country fail");
                        }

                    }
                    catch (System.Exception e1)
                    {
                        ConsoleLog.WriteLine("Collect money to country - error" + e1.ToString());
                    }

                    Bot.GetInfoFromCommonResponse(true);
                    botinfo = Utils.TryToUpdateDbWithBasicInfo(botinfo, Bot);

                    // Logout
                    Bot.Bot.Logout();
                }
            }
            else
                if ((loginResult == ManagedCitizen.LoginResult.Banned) ||
                    (loginResult == ManagedCitizen.LoginResult.Banned2))
                {
                    botinfo["banned"] = (loginResult == ManagedCitizen.LoginResult.Banned) ? 1 : 2;
                    Utils.UpdateDbWithCustomBotInfo(botinfo, "banned");
                }
                else
                {
                    //ConsoleLog.WriteLine(Bot.Bot.GetLastResponse(), "LoginLog.txt");
                    ConsoleLog.WriteLine(botinfo["login"].ToString() + ": Possibly dead, see LoginLog.txt");
                }
        }


    }
}
