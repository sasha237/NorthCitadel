using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Web;
using NerZul.Core.Network;
using System.Xml;
using System.Collections;
using Engine;
using NerZul.Core.Utils;
using SimpleDOMParserCSharp;

namespace eRepConsoleManagementSystem
{
    public partial class FlyForm : Form
    {
        Hashtable hsgrps;
        public XmlTextReader textReader;
        public Hashtable siCountries;
        public Hashtable siRegions;
        public Hashtable siCountriesRegions;
        public FlyForm()
        {
            InitializeComponent();
        }

        private void RegioncheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CountrycomboBox_SelectedIndexChanged(sender, e);
        }

        private void GroupcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CountrycomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sCountry = CountrycomboBox.Text;
            if (!siCountriesRegions.Contains(sCountry))
                return;
            Hashtable regs = null;
            if (RegioncheckBox.Checked != true)
                regs = siCountriesRegions[sCountry] as Hashtable;
            else
                regs = siRegions;


            BindingSource bs = new BindingSource();
            bs.DataSource = regs;
            RegioncomboBox.DataSource = bs;
            RegioncomboBox.DisplayMember = "Key";
        }

        private void DistancetrackBar_Scroll(object sender, EventArgs e)
        {
            UpdateItems();
        }
        void UpdateItems()
        {
            Distancelabel.Text = DistancetrackBar.Value.ToString();
        }


        private void FlyForm_Load(object sender, EventArgs e)
        {

            DbRows grps = Globals.Database.Select("bots", "`group`");
            BindingSource bs = new BindingSource();
            hsgrps = new Hashtable();
            hsgrps.Add("all", grps.Count);
            for (int i = 0; i < grps.Count; ++i)
            {
                DbRow row = grps[i] as DbRow;
                string str = row["group"] as string;
                if (hsgrps.Contains(str))
                    hsgrps[str] = int.Parse(hsgrps[str].ToString()) + 1;
                else
                    hsgrps.Add(str, 1);

            }
            bs.DataSource = hsgrps;
            GroupcomboBox.DataSource = bs;
            GroupcomboBox.DisplayMember = "Key";
            Globals.Database.Reset();
            GroupcomboBox.Text = "all";

            try
            {
                ConsoleLog.WriteLine("Downloading List of Countries");
                string sCountryName = "";
                string sCountryId = "";
                string sRegId = "";
                string sRegName = "";
                Hashtable siBufRegions = new Hashtable();
                siCountries = new Hashtable();
                siRegions = new Hashtable();
                siCountriesRegions = new Hashtable();
                SimpleDOMParser dp = new SimpleDOMParser();
                SimpleElement elCountries = dp.parse(new XmlTextReader("http://api.erepublik.com/v2/feeds/countries"));
                foreach (SimpleElement elCountry in elCountries.ChildElements)
                {
                    sCountryName = "";
                    sCountryId = "";
                    siBufRegions = new Hashtable();
                    foreach (SimpleElement elCountryEls in elCountry.ChildElements)
                    {
                        if (elCountryEls.TagName == "name")
                        {
                            sCountryName = elCountryEls.Text;
                        }
                        if (elCountryEls.TagName == "id")
                        {
                            sCountryId = elCountryEls.Text;
                        }

                        if (elCountryEls.TagName == "regions")
                        {
                            foreach (SimpleElement elRegion in elCountryEls.ChildElements)
                            {
                                sRegId = "";
                                sRegName = "";
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
                                        siRegions.Add(sRegName, sRegId);
                                        siBufRegions.Add(sRegName, sRegId);
                                        break;
                                    }
                                }
                            }
                        }
                        if (sCountryId != "" && sCountryName != "")
                        {
                            if (!siCountries.Contains(sCountryName))
                                siCountries.Add(sCountryName, sCountryId);
                        }
                        if (siBufRegions.Count != 0 && sCountryName != "")
                        {
                            if (!siCountriesRegions.Contains(sCountryName))
                                siCountriesRegions.Add(sCountryName, siBufRegions);
                        }
                    }
                } 
            }
            catch (System.Exception e1)
            {
                ConsoleLog.WriteLine(e1.ToString());
            }


            bs = new BindingSource();
            bs.DataSource = siCountries;
            CountrycomboBox.DataSource = bs;
            CountrycomboBox.DisplayMember = "Key";
            CountrycomboBox_SelectedIndexChanged(null, null);
        }

        private void RegioncomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void Startbutton_Click(object sender, EventArgs e)
        {
            string sDist = (DistancetrackBar.Value).ToString();
            string sGroup = GroupcomboBox.Text;
            string sCountry = CountrycomboBox.Text;
            string sRegion = RegioncomboBox.Text;
            string sCId = siCountries[sCountry] as string;
            string sRId = siRegions[sRegion] as string;
            string[] args = new string[] { "fly", sGroup, sDist, sCId, sRId };
            Fly.Worker(args);
        }
    }
}
