using System;
using System.Collections.Generic;
using NerZul.Core.Utils;
using System.Threading;

namespace eRepCompanyChecker
{
    public static class Trader
    {
        public static void SellGoods(string[] args)
        {
            //ConsoleLog.WriteLine("args.Length = " + args.Length.ToString() + "; args = " + args.ToString());
            if (args.Length != 9)
            {
                ConsoleLog.WriteLine("Usage: sellgoods login password country industry quality amount price delay");
                return;
            }

            string sLogin = args[1];
            string sPassword = args[2];
            string sCountry = args[3];
            string sIndustry = args[4];
            string sQuality = args[5];
            int amount = Convert.ToInt32(args[6]);
            string sPrice = args[7];
            int delay = Convert.ToInt32(args[8]);
            TraderBot bt = new TraderBot(sLogin, sLogin, sPassword, "Mozilla//4.0 (compatible; MSIE 7.0; Windows NT 6.0)", "", 0);
            bt.HttpClient.SetProxy(null, null);
            bool loggedIn = false;

            while (true)
            {
                if (!loggedIn)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            if (!loggedIn)
                            {
                                ConsoleLog.WriteLine("Trying to login (" + (i + 1).ToString() + ")...");
                                if (bt.Login())
                                {
                                    ConsoleLog.WriteLine("Logged in!");
                                    loggedIn = true;
                                    break;
                                }
                                else
                                {
                                    ConsoleLog.WriteLine("Login failed!");
                                    System.Threading.Thread.Sleep(1000);
                                    continue;
                                }
                            }
                        }
                        catch (System.Exception e)
                        {
                            ConsoleLog.WriteLine("SellGoods login error: " + e.Message);
                            ConsoleLog.WriteLine(bt.GetLastResponse(), "Responses.txt");
                        }
                    }

                    if (!loggedIn)
                    {
                        ConsoleLog.WriteLine("SellGoods login error: unable to login.");
                        break;
                    }
                }

                int currentOffer = bt.FindGoodOffer();
                ConsoleLog.WriteLine("Current offer amount left: " + currentOffer.ToString());

                int remains = bt.GetRemains(sIndustry, sQuality, false);
                ConsoleLog.WriteLine("Current remains: " + remains.ToString());

                if (remains == 0)
                {
                    ConsoleLog.WriteLine("Everything sold out.");
                    break;
                }

                int sellQty = Math.Min(amount - currentOffer, remains);

                if (sellQty > 0)
                {
                    Thread.Sleep(1000);
                    if (bt.SetOnSale(sCountry, sIndustry, sQuality, sellQty.ToString(), sPrice, false))
                    {
                        ConsoleLog.WriteLine("Offer of " + sellQty.ToString() + " items posted.");
                    }
                    else
                    {
                        ConsoleLog.WriteLine("Offer of " + sellQty.ToString() + " items failed.");
                    };
                }

                ConsoleLog.WriteLine("Wait " + delay.ToString() + " seconds for next check...");
                Thread.Sleep(delay * 1000);
            }
        }

        public static void BuyGoods(string[] args)
        {
            //ConsoleLog.WriteLine("args.Length = " + args.Length.ToString() + "; args = " + args.ToString());
            if (args.Length != 8)
            {
                ConsoleLog.WriteLine("Usage: buygoods login password country industry quality price delay");
                return;
            }

            string sLogin = args[1];
            string sPassword = args[2];
            int country = Convert.ToInt32(args[3]);
            int industry = Convert.ToInt32(args[4]);
            int quality = Convert.ToInt32(args[5]);
            double price = Convert.ToDouble(args[6]);
            double delay = Convert.ToDouble(args[7]);
            TraderBot bt = new TraderBot(sLogin, sLogin, sPassword, "Mozilla//4.0 (compatible; MSIE 7.0; Windows NT 6.0)", "", 0);
            bt.HttpClient.SetProxy(null, null);
            bool loggedIn = false;

            while (true)
            {
                if (!loggedIn)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            if (!loggedIn)
                            {
                                ConsoleLog.WriteLine("Trying to login (" + (i + 1).ToString() + ")...");
                                if (bt.Login())
                                {
                                    ConsoleLog.WriteLine("Logged in!");
                                    loggedIn = true;
                                    break;
                                }
                                else
                                {
                                    ConsoleLog.WriteLine("Login failed!");
                                    System.Threading.Thread.Sleep(1000);
                                    continue;
                                }
                            }
                        }
                        catch (System.Exception e)
                        {
                            ConsoleLog.WriteLine("BuyGoods login error: " + e.Message);
                            ConsoleLog.WriteLine(bt.GetLastResponse(), "Responses.txt");
                        }
                    }

                    if (!loggedIn)
                    {
                        ConsoleLog.WriteLine("BuyGoods login error: unable to login.");
                        break;
                    }
                }

                string sOffer;
                int amount;
                double foundPrice = bt.GetMinPrice(country, industry, quality, true, out sOffer, out amount);

                if (foundPrice <= price)
                {
                    //amount = 1;
                    bt.BuyItem(country, industry, amount, quality, 0, false);
                }
                else
                {
                    ConsoleLog.WriteLine("Price above the limit, skipping.");
                }
                
                ConsoleLog.WriteLine("Wait " + delay.ToString() + " seconds for next check...");
                Thread.Sleep((int)(delay * 1000));
            }
        }
    }
}
