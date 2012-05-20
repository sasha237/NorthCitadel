using System;
using System.Collections.Generic;
using NerZul.Core.Utils;
using NerZul.Core.Network;

namespace eRepCompanyChecker
{
    using Google.GData.Client;
    using Google.GData.Extensions;
    using Google.GData.Spreadsheets;

    public static class Donater
    {
        private class MUs
        {
            public List<string> formMUs = new List<string>();

            public List<string> erepMUs = new List<string>();

            public string this[string formMU]
            {
                get
                {
                    int idx = formMUs.IndexOf(formMU);
                    if (idx != -1)
                        return erepMUs[idx];
                    else
                        return "";
                }
                set
                {
                    int idx = formMUs.IndexOf(formMU);
                    if (idx != -1)
                        erepMUs[idx] = value;
                }
            }
        }

        private class ItemStorage
        {
            public uint row;
            public bool viewedForDuplicate = false;

            public string time;

            public string login;
            public string personID;
            public int wellness;
            public int tanks;
            public string militaryUnit;

            public int doneTanks;
            public CellEntry doneTanksCell;
            public int doneFood;
            public CellEntry doneFoodCell;
            public int doneFoodQ;
            public CellEntry doneFoodQCell;

            public string comment;
            public CellEntry commentCell;

            public int tanksLimit;
            public CellEntry tanksLimitCell;
            public int foodLimit;
            public CellEntry foodLimitCell;
        }

        private static void LoadSettings(
            SpreadsheetsService sheetService,
            WorksheetEntry wsSettings,
            MUs MUList,
            List<string> blackList)
        {
            ConsoleLog.WriteLine("Loading settings...");

            #region Query cells
            AtomLink cellLink = wsSettings.Links.FindService(GDataSpreadsheetsNameTable.CellRel, null);

            CellQuery query = new CellQuery(cellLink.HRef.ToString());
            query.ReturnEmpty = ReturnEmptyCells.yes;
            query.MaximumColumn = 4;
            CellFeed feed = sheetService.Query(query);

            List<ItemStorage> lines = new List<ItemStorage>();
            #endregion

            #region Load cells
            lines.Clear();
            foreach (CellEntry curCell in feed.Entries)
            {
                if (curCell.Cell.Row < 2) continue;

                if (curCell.Cell.Column == 1)       // fomMU
                {
                    MUList.formMUs.Add(curCell.Cell.Value);
                }
                if (curCell.Cell.Column == 2)       // erepMU
                {
                    MUList.erepMUs.Add(curCell.Cell.Value);
                }
                if (curCell.Cell.Column == 4)       // BlackList
                {
                    blackList.Add(curCell.Cell.Value);
                }
            }
            ConsoleLog.WriteLine("Setting items loaded: " + lines.Count);
            #endregion

            ConsoleLog.WriteLine("Loaded MUs: ");
            foreach (var mu in MUList.formMUs)
            {
                if (!String.IsNullOrEmpty(mu))
                    ConsoleLog.WriteLine(mu + "=" + MUList[mu]);
            }

            ConsoleLog.WriteLine("Loaded blacklist: ");
            foreach (var mu in blackList)
            {
                if (!String.IsNullOrEmpty(mu))
                    ConsoleLog.WriteLine(mu);
            }
        }

        public static void GDocSupply(string[] args)
        {
            MUs MUList = new MUs();
            List<string> blackList = new List<string>();

            if (args.Length != 16)
            {
                ConsoleLog.WriteLine("Usage: armysupply eLogin ePassword ePin gLogin gPassword document_name sheet_name do_tanks do_food food_q period max_tanks max_health validation_type(none/MU/citizenship) do_init_block");
                ConsoleLog.WriteLine("Example: armysupply snab_org 123456 1111 xxx@gmail.com 54321 \"Army supply\" \"Sheet1\" true true 5 60 8 300 MU false");
                return;
            }

            string eLogin = args[1];
            string ePassword = args[2];
            string ePin = args[3];
            string gLogin = args[4];
            string gPassword = args[5];
            string sDocumentName = args[6];
            string sSheetName = args[7];
            bool bDoTanks = Convert.ToBoolean(args[8]);
            bool bDoFood = Convert.ToBoolean(args[9]);
            int iFoodQ = Convert.ToInt32(args[10]);
            int iPeriod = Convert.ToInt32(args[11]);
            int iMaxTanks = Convert.ToInt32(args[12]);
            int iMaxHP = Convert.ToInt32(args[13]);
            string sValidation = args[14];
            bool bDoInitBlock = Convert.ToBoolean(args[15]);

            string token = "";

            ConsoleLog.WriteLine("Loading document...");
            #region Open sheet
            SpreadsheetsService sheetService = new SpreadsheetsService("EArmy supply");
            sheetService.setUserCredentials(gLogin, gPassword);
            SpreadsheetQuery sheetQuery = new SpreadsheetQuery();
            SpreadsheetFeed sheetFeed = sheetService.Query(sheetQuery);

            SpreadsheetEntry sheetSnab = null;
            foreach (SpreadsheetEntry entry in sheetFeed.Entries)
            {
                if (entry.Title.Text == sDocumentName)
                {
                    sheetSnab = entry;
                    break;
                }
            }
            if (sheetSnab == null)
            {
                ConsoleLog.WriteLine("Document '" + sDocumentName + "' not found");
                return;
            }

            AtomLink sheetLink = sheetSnab.Links.FindService(GDataSpreadsheetsNameTable.WorksheetRel, null);

            WorksheetQuery wsQuery = new WorksheetQuery(sheetLink.HRef.ToString());
            WorksheetFeed wsFeed = sheetService.Query(wsQuery);

            WorksheetEntry wsSnab = null;
            WorksheetEntry wsSettings = null;
            foreach (WorksheetEntry ws in wsFeed.Entries)
            {
                if (ws.Title.Text == sSheetName)
                {
                    wsSnab = ws;
                }
                if (ws.Title.Text == "Settings")
                {
                    wsSettings = ws;
                }
            }
            if (wsSnab == null)
            {
                ConsoleLog.WriteLine("Page '" + sSheetName + "' not found");
                return;
            }
            if (wsSettings != null)
            {
                LoadSettings(sheetService, wsSettings, MUList, blackList);
            }
            #endregion

            #region Login to supplier
            DonaterBot bt = new DonaterBot(
                eLogin,
                eLogin,
                ePassword,
                ePin,
                "Mozilla//4.0 (compatible; MSIE 7.0; Windows NT 6.0)",
                Globals.BotConfig.AntiGateKey,
                Globals.BotConfig.precaptchaBufferSize);
            bt.HttpClient.SetProxy(null, null);
            bool loggedIn = false;
            int iTryToConnect = 0;
            System.Random rnd = new System.Random();
            #endregion

            while (true)
            {
                try
                {
                    ConsoleLog.WriteLine("Loading cells...");
                    #region Query cells
                    AtomLink cellLink = wsSnab.Links.FindService(GDataSpreadsheetsNameTable.CellRel, null);

                    CellQuery query = new CellQuery(cellLink.HRef.ToString());
                    query.ReturnEmpty = ReturnEmptyCells.yes;
                    query.MaximumColumn = 13;
                    CellFeed feed = sheetService.Query(query);

                    List<ItemStorage> lines = new List<ItemStorage>();
                    ItemStorage item = null;
                    #endregion

                    #region Load cells
                    lines.Clear();
                    foreach (CellEntry curCell in feed.Entries)
                    {
                        if (curCell.Cell.Row < 2) continue;

                        item = lines.Find(l => l.row == curCell.Cell.Row);
                        if (item == null)
                        {
                            item = new ItemStorage();
                            lines.Add(item);
                            item.row = curCell.Cell.Row;
                        }

                        if (curCell.Cell.Column == 1)       // time
                        {
                            item.time = curCell.Cell.Value;
                        }
                        if (curCell.Cell.Column == 2)       // login
                        {
                            item.login = curCell.Cell.Value;
                        }
                        if (curCell.Cell.Column == 3)       // link to profile
                        {
                            item.personID = curCell.Cell.Value;
                            try
                            {
                                item.personID = item.personID.Substring(item.personID.LastIndexOf('/') + 1);
                                Convert.ToInt64(item.personID);
                            }
                            catch
                            {
                                item.personID = "";
                            }
                            // Cut ID from link
                        }
                        if (curCell.Cell.Column == 4)       // wellness
                        {
                            try
                            {
                                item.wellness = Convert.ToInt32(curCell.Cell.Value);
                            }
                            catch
                            {
                                item.wellness = 0;
                            }
                        }
                        if (curCell.Cell.Column == 5)       // needed tanks
                        {
                            try
                            {
                                item.tanks = Convert.ToInt32(curCell.Cell.Value);
                            }
                            catch
                            {
                                item.tanks = 0;
                            }
                        }
                        if (curCell.Cell.Column == 6)       // military unit
                        {
                            item.militaryUnit = curCell.Cell.Value;
                        }
                        if (curCell.Cell.Column == 8)       // done tanks
                        {
                            item.doneTanksCell = curCell;
                            try
                            {
                                item.doneTanks = Convert.ToInt32(curCell.Cell.Value);
                            }
                            catch
                            {
                                item.wellness = 0;
                            }
                        }
                        if (curCell.Cell.Column == 9)       // done food q
                        {
                            try
                            {
                                item.doneFoodQ = Convert.ToInt32(curCell.Cell.Value);
                            }
                            catch
                            {
                                item.doneFoodQ = 0;
                            }
                            item.doneFoodQCell = curCell;
                        }
                        if (curCell.Cell.Column == 10)      // done food
                        {
                            item.doneFoodCell = curCell;
                            try
                            {
                                item.doneFood = Convert.ToInt32(curCell.Cell.Value);
                            }
                            catch
                            {
                                item.doneFood = 0;
                            }
                        }
                        if (curCell.Cell.Column == 11)       // autocomment cell
                        {
                            item.comment = curCell.Cell.Value;
                            item.commentCell = curCell;
                        }
                        if (curCell.Cell.Column == 12)       // tanks limit cell
                        {
                            try
                            {
                                if (!String.IsNullOrEmpty(curCell.Cell.Value))
                                    item.tanksLimit = Convert.ToInt32(curCell.Cell.Value);
                                else
                                    item.tanksLimit = -1;
                            }
                            catch
                            {
                                item.tanksLimit = -1;
                            }

                            item.tanksLimitCell = curCell;
                        }
                        if (curCell.Cell.Column == 13)       // HP limit cell
                        {
                            try
                            {
                                if (!String.IsNullOrEmpty(curCell.Cell.Value))
                                    item.foodLimit = Convert.ToInt32(curCell.Cell.Value);
                                else
                                    item.foodLimit = -1;
                            }
                            catch
                            {
                                item.foodLimit = -1;
                            }

                            item.foodLimitCell = curCell;
                        }
                    }
                    ConsoleLog.WriteLine("Items loaded: " + lines.Count);
                    #endregion

                    #region Parce cells
                    int row = 0;
                    List<ItemStorage> tmpList = new List<ItemStorage>(lines);
                    foreach (ItemStorage testItem in tmpList)
                    {
                        row++;

                        if (string.IsNullOrEmpty(testItem.time))
                        {
                            lines.Remove(testItem);
                            continue;
                        }

                        if (!string.IsNullOrEmpty(testItem.comment))
                        {
                            lines.Remove(testItem);
                            continue;
                        }

                        if (string.IsNullOrEmpty(testItem.personID))
                        {
                            lines.Remove(testItem);
                            testItem.commentCell.Cell.InputValue = "Bad profile";
                            testItem.commentCell.Update();
                            continue;
                        }

                        if (blackList.Contains(testItem.login))
                        {
                            lines.Remove(testItem);
                            testItem.commentCell.Cell.InputValue = "Blacklist";
                            testItem.commentCell.Update();
                            continue;
                        }

                        if ((sValidation.ToLower() == "mu") &&
                            (String.IsNullOrEmpty(MUList[testItem.militaryUnit])))
                        {
                            lines.Remove(testItem);
                            testItem.commentCell.Cell.InputValue = "MU not in list";
                            testItem.commentCell.Update();
                            continue;
                        }

                        if ((testItem.wellness == 0) && (testItem.tanks == 0))
                        {
                            lines.Remove(testItem);
                            testItem.commentCell.Cell.InputValue = "Bad wellness/tanks";
                            testItem.commentCell.Update();
                            continue;
                        }

                        //if (testItem.tanks > 30)
                        //{
                        //    lines.Remove(testItem);
                        //    testItem.commentCell.Cell.InputValue = "Bad tanks";
                        //    testItem.commentCell.Update();
                        //    continue;
                        //}

                        if (tmpList.Find(o => (o.personID == testItem.personID && o.viewedForDuplicate)) != null)
                        {
                            lines.Remove(testItem);
                            testItem.commentCell.Cell.InputValue = "Duplicate record";
                            testItem.commentCell.Update();
                            continue;
                        }

                        if (testItem.tanksLimit == -1)
                        {
                            if (bDoInitBlock)
                            {
                                testItem.tanksLimit = 0;
                            }
                            else
                            {
                                testItem.tanksLimit = iMaxTanks;
                            }
                            testItem.tanksLimitCell.Cell.InputValue = testItem.tanksLimit.ToString();
                            testItem.tanksLimitCell.Update();
                        }

                        if (testItem.foodLimit == -1)
                        {
                            if (bDoInitBlock)
                            {
                                testItem.foodLimit = 0;
                            }
                            else
                            {
                                testItem.foodLimit = iMaxHP;
                            }
                            testItem.foodLimitCell.Cell.InputValue = testItem.foodLimit.ToString();
                            testItem.foodLimitCell.Update();
                        }                        

                        testItem.viewedForDuplicate = true;

                        ConsoleLog.WriteLine(
                            row + ": " +
                            testItem.time + ";" +
                            testItem.login + ";" +
                            testItem.personID + ";" +
                            testItem.wellness + ";" +
                            testItem.tanks + ";" +
                            testItem.doneTanks + ";" +
                            testItem.doneFoodQ + ";" +
                            testItem.doneFood
                            );
                    }
                    #endregion

                    #region Log in
                    if (!loggedIn)
                    {
                        iTryToConnect++;
                        if (iTryToConnect > 10)
                            break;

                        ConsoleLog.WriteLine("Trying to login (" + (iTryToConnect).ToString() + ")...");
                        if (bt.Login())
                        {
                            ConsoleLog.WriteLine("Logged in!");
                            iTryToConnect = 0;
                            loggedIn = true;
                        }
                        else
                        {
                            ConsoleLog.WriteLine("Login failed!");
                            System.Threading.Thread.Sleep(1000);
                            continue;
                        }
                    }
                    #endregion

                    //Process donations
                    foreach (ItemStorage donateItem in lines)
                    {
                        ConsoleLog.WriteLine("Donating to: login=" + donateItem.login + ", id=" + donateItem.personID);

                        #region Calculate quantities
                        int foodQty;
                        if ((bDoFood) && (donateItem.wellness > 0) && (donateItem.doneFoodQ == 0))
                        {
                            foodQty = Convert.ToInt32(Math.Round((double)((double)Math.Min(donateItem.wellness, donateItem.foodLimit) / (iFoodQ * 2))));
                            foodQty = foodQty - donateItem.doneFood;
                        }
                        else
                        {
                            foodQty = 0;
                        }

                        int tankQty;

                        if (bDoTanks)
                        {
                            tankQty = Math.Min(donateItem.tanks, donateItem.tanksLimit);
                            tankQty = tankQty - donateItem.doneTanks;
                        }
                        else
                        {
                            tankQty = 0;
                        }

                        if ((foodQty <= 0) && (tankQty <= 0))
                        {
                            ConsoleLog.WriteLine("Nothing to donate");
                            continue;
                        }
                        #endregion

                        #region Open pages and validate
                        string srcPage = "http://www.erepublik.com/en/citizen/profile/" + donateItem.personID;
                        
                        bt.CustomRequest(srcPage);

                        if (!bt.GetLastResponse().Contains("career_tab_content"))
                        {
                            ConsoleLog.WriteLine("Profile page loading failed. Try to relogin...");
                            loggedIn = false;
                            break;
                        }

                        if (bt.GetLastResponse().IndexOf("alt=\"" + donateItem.login.Trim() + "\"", StringComparison.OrdinalIgnoreCase) == -1)
                        {
                            ConsoleLog.WriteLine("Donating login validation failed");
                            donateItem.commentCell.Cell.InputValue = "Profile != Login";
                            donateItem.commentCell.Update();
                            continue;
                        }
                        else
                        {
                            ConsoleLog.WriteLine("Login validated");
                        }

                        if ((sValidation.ToLower() == "mu") &&
                            (bt.GetLastResponse().IndexOf("alt=\"" + MUList[donateItem.militaryUnit.Trim()] + "\"", StringComparison.OrdinalIgnoreCase) == -1))
                        {
                            ConsoleLog.WriteLine("Donating MU validation failed (" + MUList[donateItem.militaryUnit.Trim()] + ")");
                            ConsoleLog.WriteLine(bt.GetLastResponse(), "Storage page");
                            donateItem.commentCell.Cell.InputValue = "Profile != MU";
                            donateItem.commentCell.Update();
                            continue;
                        }
                        else
                        {
                            ConsoleLog.WriteLine("MU validated");
                        }
                                                
                        srcPage = "http://economy.erepublik.com/en/citizen/donate/" + donateItem.personID;

                        bt.CustomRequest(srcPage);

                        if (bt.CheckPin(true))
                        {
                            bt.SubmitPin();
                        }

                        if (bt.CheckPin(true))
                        {
                            ConsoleLog.WriteLine("Pin validation failed");
                            break;
                        }

                        if (!bt.GetLastResponse().Contains("<th colspan=\"4\" valign=\"middle\">Your storage</th>"))
                        {
                            ConsoleLog.WriteLine("Donate page loading failed. Try to relogin...");
                            loggedIn = false;
                            break;
                        }

                        token = CommonUtils.GetStringBetween(
                            bt.GetLastResponse(),
                            "donate_form[_csrf_token]\" value=\"",
                            "\"");
                        #endregion

                        #region Donate food
                        if (foodQty > 0)
                        {
                            ConsoleLog.WriteLine("Donating food: " + foodQty + " (q" + iFoodQ + ")");

                            if (bt.DonateItem(foodQty.ToString(), Goods.Food.ToString(), iFoodQ.ToString(), token, srcPage))
                            //if (true)
                            {
                                ConsoleLog.WriteLine("Donating food success");
                                donateItem.doneFoodQCell.Cell.InputValue = iFoodQ.ToString();
                                donateItem.doneFoodQCell.Update();
                                donateItem.doneFoodCell.Cell.InputValue = (donateItem.doneFood + foodQty).ToString();
                                donateItem.doneFoodCell.Update();
                            }
                            else
                            {
                                ConsoleLog.WriteLine("Donating food failed");
                                //ConsoleLog.WriteLine(bt.GetLastResponse(), "DonateLog.txt");
                            }

                            ConsoleLog.WriteLine("Wait 5 sec...");
                            System.Threading.Thread.Sleep(5 * 1000);
                        }
                        else
                        {
                            ConsoleLog.WriteLine("No food needed");
                        }
                        #endregion

                        #region Donate tanks
                        if (tankQty > 0)
                        {
                            ConsoleLog.WriteLine("Donating tanks: " + tankQty);

                            if (bt.DonateItem(tankQty.ToString(), Goods.Weapon.ToString(), 5.ToString(), token, srcPage))
                            //if (true)
                            {
                                ConsoleLog.WriteLine("Donating tanks success");
                                donateItem.doneTanksCell.Cell.InputValue = (donateItem.doneTanks + tankQty).ToString();
                                donateItem.doneTanksCell.Update();
                            }
                            else
                            {
                                ConsoleLog.WriteLine("Donating tanks failed");
                                ConsoleLog.WriteLine(bt.GetLastResponse(), "DonateLog.txt");
                            }

                            ConsoleLog.WriteLine("Wait 5 sec...");
                            System.Threading.Thread.Sleep(5 * 1000);
                        }
                        else
                        {
                            ConsoleLog.WriteLine("No tanks needed");
                        }
                        #endregion
                    }
                }
                catch (System.Exception e)
                {
                    ConsoleLog.WriteLine("Donater error: " + e.Message);
                    ConsoleLog.WriteLine(bt.GetLastResponse(), "Responses.txt");
                }

                ConsoleLog.WriteLine("Waiting for next check");
                //break;
                System.Threading.Thread.Sleep(iPeriod * 1000);
            }

            ConsoleLog.WriteLine("It's OK :)");
        }
    }
}
