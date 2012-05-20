using System;
using System.Collections.Generic;
using System.Text;
using SimpleDOMParserCSharp;
using System.Xml;
using System.Runtime.CompilerServices;

namespace eRepResourseDetector
{
    [assembly: SuppressIldasmAttribute()]
    class Program
    {
        static Dictionary<string, string> pResnames = null;
        static void Main(string[] args)
        {
            SimpleDOMParser dp = null;
            SimpleElement elCountries = null;
            while (true)
            {
                try
                {
                    dp = new SimpleDOMParser();
                    elCountries = dp.parse(new XmlTextReader("http://api.erepublik.com/v2/feeds/countries"));
                    Console.WriteLine("Countries");
                    break;
                }
                catch (System.Exception e)
                {
                    Console.WriteLine("Countries again");
                }
            }
            pResnames = new Dictionary<string, string>();
            Dictionary<string, Dictionary<string, string>> ssCountryRegions = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, Dictionary<string, int>> ssRegionsResources = new Dictionary<string, Dictionary<string, int>>();
            Dictionary<string, Dictionary<string, int>> ssCountryResources = new Dictionary<string, Dictionary<string, int>>();
            foreach (SimpleElement elCountry in elCountries.ChildElements)
            {
                string sCountyName = "";
                foreach (SimpleElement elCountryEls in elCountry.ChildElements)
                {
                    if (elCountryEls.TagName == "name")
                    {
                        sCountyName = elCountryEls.Text;
                    }
                    if (elCountryEls.TagName == "regions")
                    {
                        Dictionary<string, string> ssRegions = new Dictionary<string, string>();
                        foreach (SimpleElement elRegion in elCountryEls.ChildElements)
                        {
                            string sRegId = "";
                            string sRegName = "";

                            foreach (SimpleElement elRegionEl in elRegion.ChildElements)
                            {
                                if (elRegionEl.TagName == "name")
                                {
                                    sRegName = elRegionEl.Text;
                                }
                                if (elRegionEl.TagName == "id")
                                {
                                    sRegId = elRegionEl.Text;
                                }
                                if (sRegId != "" && sRegName != "")
                                {
                                    ssRegions.Add(sRegName, sRegId);
                                    break;
                                }
                            }
                        }
                        ssCountryRegions.Add(sCountyName, ssRegions);
                    }
                }
            }
            foreach(var elem in ssCountryRegions)
            {
                Console.WriteLine(elem.Key.ToString());
                Dictionary<string, int> ssRegionsRes1 = new Dictionary<string, int>();
                foreach (var regElem in elem.Value)
                {
                    Dictionary<string, int> ssRegionsRes2 = new Dictionary<string, int>();
                    SimpleDOMParser dp1 = null;
                    SimpleElement elRegions = null;
                    while (true)
                    {
                        try
                        {
                            dp1 = new SimpleDOMParser();
                            Console.WriteLine(regElem.Key.ToString());
                            elRegions = dp1.parse(new XmlTextReader("http://api.erepublik.com/v2/feeds/regions/" + regElem.Value.ToString()));
                            break;
                        }
                        catch (System.Exception e)
                        {
                            Console.WriteLine("Region again");
                        }
                    }
                    foreach (SimpleElement elRegion in elRegions.ChildElements)
                    {
                        if (elRegion.TagName == "raw-materials")
                        {
                            string sVal = "";
                            foreach (SimpleElement elMat in elRegion.ChildElements)
                            {
                                if (elMat.TagName == "name")
                                {
                                    string sBuf;
                                    int iVal = 0;
                                    sVal = elMat.Text;
                                    ssRegionsRes2.Add(sVal, 1);
                                    if (ssRegionsRes1.TryGetValue(sVal, out iVal))
                                        ssRegionsRes1[sVal]++;
                                    else
                                        ssRegionsRes1.Add(sVal, 1);

                                    if (!pResnames.TryGetValue(sVal, out sBuf))
                                        pResnames.Add(sVal,sVal);
                                    break;
                                }
                            }
                            ssRegionsResources.Add(regElem.Key, ssRegionsRes2);
                            break;
                        }
                    }
                }
                ssCountryResources.Add(elem.Key, ssRegionsRes1);
            }
            System.IO.File.WriteAllText("countries.csv", GetRes(ssCountryResources));
            System.IO.File.WriteAllText("regions.csv", GetRes(ssRegionsResources));
            System.IO.File.WriteAllText("countryregions.csv", GetCountriesRegionsRes(ssCountryRegions, ssRegionsResources));
        }
        static string GetCountriesRegionsRes(Dictionary<string, Dictionary<string, string>> ssCountryRegions, Dictionary<string, Dictionary<string, int>> ssRegionsResources)
        {
            StringBuilder sbMain = new StringBuilder();
            sbMain.Append("country;region;");
            foreach (var s in pResnames)
            {
                sbMain.Append(s.Key.ToString() + ";");
            }
            sbMain.AppendLine();
            foreach (var elCountry in ssCountryRegions)
            {
                foreach (var elRegion in elCountry.Value)
                {
                    if (ssRegionsResources.ContainsKey(elRegion.Key))
                    {
                        sbMain.Append(elCountry.Key.ToString() + ";" + elRegion.Key.ToString() + ";");
                        foreach (var s in pResnames)
                        {
                            if (ssRegionsResources[elRegion.Key].ContainsKey(s.Key))
                            {
                                sbMain.Append(ssRegionsResources[elRegion.Key][s.Key].ToString() + ";");
                            }
                            else
                                sbMain.Append("0;");
                        }
                        sbMain.AppendLine();
                    }
                }
            }
            return (string)sbMain.ToString();
        }
        static string GetRes(Dictionary<string, Dictionary<string, int>> ssRess)
        {
            StringBuilder sbMain = new StringBuilder();
            sbMain.Append("name;");
            foreach (var s in pResnames)
            {
                sbMain.Append(s.Key.ToString()+";");
            }
            sbMain.AppendLine();
            foreach (var sRegs in ssRess)
            {
                sbMain.Append(sRegs.Key+";");
                foreach (var s in pResnames)
                {
                    if (sRegs.Value.ContainsKey(s.Key))
                    {
                        sbMain.Append(sRegs.Value[s.Key].ToString()+";");
                    }
                    else
                        sbMain.Append("0;");
                }
                sbMain.AppendLine();
            }
            return (string)sbMain.ToString();
        }
    }
}
