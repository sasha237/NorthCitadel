using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using NerZul.Core;
using System.Text.RegularExpressions;
using NerZul.Core.Network;
using System.Globalization;

namespace BattleWatcher
{
    public partial class MainForm : Form
    {
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
        public MainForm()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Gobutton_Click(object sender, EventArgs e)
        {
            string sBattelId = IdtextBox.Text;
            double dSec = 0;
            
            if (sBattelId.Length == 0)
            {
                MessageBox.Show("Wrong number");
                return;
            }

            if(!double.TryParse(SectextBox.Text, out dSec))
            {
                MessageBox.Show("Wrong time");
                return;
            }
            
            SortedDictionary<int, Pair2<int, double>> sdMatr = new SortedDictionary<int, Pair2<int, double>>();
            SortedDictionary<int, int> sdFight = new SortedDictionary<int, int>();
            SortedDictionary<int, int> sdOldFight = new SortedDictionary<int, int>();
            HttpClient httpClient = new HttpClient();
            int i=1;
            double dDomination = 0;
            double dOldDomination = 0;
            while (true)
            {
                Console.WriteLine("Step " + i.ToString());
                int iSumm = 0;
                int iA = 0;
                int iD = 0;

                string Response = "";
                try
                {
                    Response = httpClient.DownloadString("http://www.erepublik.com/en/military/battle-log/" + sBattelId);
                }
                catch (System.Exception e1)
                {
                    MessageBox.Show("Wrong number");
                    return;
                }
                bool bFlag = true;
                string sAttackers = "\"attackers\":";
                string sDefenders = "\"defenders\":";
                int iPos;
                int iAttk = Response.IndexOf(sAttackers);
                int iDef = Response.IndexOf(sDefenders);
                int j = 0;
                dDomination = double.Parse(Regex.Match(Response, "domination\":([0-9.]{1,9})").Groups[1].Value, CultureInfo.GetCultureInfo("en-US").NumberFormat)/100.0;
                Match mcD = Regex.Match(Response, "damage\":\"([0-9.]{1,9})\"");
                Match mcI = Regex.Match(Response, "id\":\"([0-9.]{1,9})\"");
                while (mcD.Success && mcI.Success)
                {
                    int iDam = int.Parse(mcD.Groups[1].Value);
                    int iId = int.Parse(mcI.Groups[1].Value);
                    if (sdOldFight.ContainsKey(iId))
                    {
                        j++;
                        mcD = mcD.NextMatch();
                        mcI = mcI.NextMatch();
                        continue;
                    }
                    iSumm += iDam;
                    if (mcD.Groups[1].Index > iAttk && mcD.Groups[1].Index < iDef)
                        iA += iDam;
                    else
                        iD += iDam;
                    sdFight.Add(iId, iDam);

                    mcD = mcD.NextMatch();
                    mcI = mcI.NextMatch();
                }
                if (j>25)
                {
                    bFlag = false;
                }
                
                if (bFlag)
                {
                    i++;
                    if (dOldDomination != 0)
                    {
                        if (dOldDomination != dDomination)
                        {
                            double dVal1 = Math.Abs(dDomination * dOldDomination / (dDomination - dOldDomination) * (iA / dDomination - iSumm));
                            double dVal2 = Math.Abs(dDomination * dOldDomination / (dDomination - dOldDomination) * (iD / dDomination - iSumm));
                            double dVal = dVal1 + dVal2;
                            dVal /= 100.0;
                            Console.WriteLine("Current                  1% = " + ((int)(dVal)).ToString() + " dmg");
                            sdMatr.Add(i, new Pair2<int, double>(Math.Abs(iSumm), Math.Abs(dVal)));
                        }
                        if (sdMatr.Count!=0)
                        {
                            double dSum = 0;
                            double dSumlast10 = 10;
                            double dCount = 0;
                            double dWithoutPeaks = 0;
                            double dWithoutPeakslast10 = 0;
                            double dCount10 = 0;
                            foreach (var el in sdMatr)
                            {
                                Pair2<int, double> pp = el.Value as Pair2<int, double>;
                                dCount++;
                                dSum += pp.Second;
                                if (dCount + 10 > sdMatr.Count)
                                {
                                    dSumlast10 += pp.Second;
                                    dCount10++;
                                }
                            }
                            dSum /= dCount;
                            Console.WriteLine("Average                  1% = " + ((int)(dSum)).ToString() + " dmg");
                            if (dCount > 10)
                            {
                                dSumlast10 /= dCount10;
                                Console.WriteLine("Average last 10          1% = " + ((int)(dSumlast10)).ToString() + " dmg");
                            }
                            dCount = 0;
                            dCount10 = 0;
                            foreach (var el in sdMatr)
                            {
                                Pair2<int, double> pp = el.Value as Pair2<int, double>;
                                if (pp.Second > dSum * 3)
                                    continue;
                                dCount++;
                                dWithoutPeaks += pp.Second;
                                if (dCount + 10 > sdMatr.Count)
                                {
                                    dWithoutPeakslast10 += pp.Second;
                                    dCount10++;
                                }
                            }
                            dWithoutPeaks /= dCount;
                            Console.WriteLine("Average no peaks         1% = " + ((int)(dWithoutPeaks)).ToString() + " dmg");
                            if (dCount > 10)
                            {
                                dWithoutPeakslast10 /= dCount10;
                                Console.WriteLine("Average no peaks last 10 1% = " + ((int)(dWithoutPeakslast10)).ToString() + " dmg");
                            }
                        }
                    }
                    sdOldFight.Clear();
                    foreach(var ff in sdFight)
                    {
                        sdOldFight.Add(ff.Key, ff.Value);
                    }
                    sdFight.Clear();

                    dOldDomination = dDomination;


                }
                else
                {
                    sdFight.Clear();
                    Console.WriteLine(j.ToString()+" fights are same! Trying again!");
                }
                System.Threading.Thread.Sleep((int)(dSec*1000));

            }
//             double dSum = 0;
//             double dCount = 1;
//             foreach (var el in sdMatr)
//             {
//                 Pair2<int, double> pp = el.Value as Pair2<int, double>;
//                 dCount++;
//                 dSum += pp.Second;
//             }
//             dSum /= dCount;
// 
//             MessageBox.Show("Total probably 1% = "+dSum.ToString()+" dmg");
        }
    }
}
