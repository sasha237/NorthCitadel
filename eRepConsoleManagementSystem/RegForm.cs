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
    public partial class RegForm : Form
    {
        public XmlTextReader textReader;
        public Hashtable siCountries;
        public Hashtable siRegions;
        public Hashtable siCountriesRegions;
        public RegForm()
        {
            InitializeComponent();
        }

        private void Startbutton_Click(object sender, EventArgs e)
        {
            if(!GmailradioButton.Checked&&!DomainradioButton.Checked&&!MaillistradioButton.Checked)
            {
                MessageBox.Show("Не выбран метод регистрации!");
                return;
            }
            string sFirstArg = "";
            if (ActivatecheckBox.Checked)
            {
                sFirstArg = "regactbots";
            }
            else
                sFirstArg = "regbots";


            string sLogin = LogintextBox.Text;
            string sPassword = PasswordtextBox.Text;
            string sServer = ServertextBox.Text;
            if (ActivatecheckBox.Checked)
            {
                if (sLogin.Length == 0)
                {
                    MessageBox.Show("Не задан логин!");
                    return;
                }
                if (sPassword.Length == 0)
                {
                    MessageBox.Show("Не задан пароль!");
                    return;
                }

                if (sServer.Length == 0)
                {
                    MessageBox.Show("Не задан сервер!");
                    return;
                }
            }


            string sType = "";
            if (GmailradioButton.Checked)
            {
                sType = "gmail";
            }
            if (DomainradioButton.Checked)
            {
                sType = "domain";
            }
            if (MaillistradioButton.Checked)
            {
                sType = "maillist";
            }

            string sMail = "";
            if (GmailradioButton.Checked)
            {
                sMail = MailtextBox.Text;
            }
            if (DomainradioButton.Checked)
            {
                sMail = DomaintextBox.Text;
            }
            if (MaillistradioButton.Checked)
            {
                sMail = FilePathtextBox.Text;
            }
            
            if (sMail.Length==0)
            {
                MessageBox.Show("Не задан адрес электронной почты!");
                return;
            }
            int iBots = 0;
            if(int.TryParse(BotstextBox.Text,out iBots)==false)
            {
                MessageBox.Show("Неверно задано количество ботов!");
                return;
            }
            int iThreads = 0;
            if (int.TryParse(ThreadstextBox.Text, out iThreads) == false)
            {
                MessageBox.Show("Неверно задано количество потоков!");
                return;
            }
            string sGroup = GrouptextBox.Text;
            if (sGroup.Length == 0)
            {
                MessageBox.Show("Неверно задана группа!");
                return;
            }
            string sCountry = CountrycomboBox.Text;
            if (sCountry.Length == 0)
            {
                MessageBox.Show("Неверно задана страна!");
                return;
            }
            string sRegion = RegioncomboBox.Text;
            if (sRegion.Length == 0)
            {
                MessageBox.Show("Неверно задан регион!");
                return;
            }
            string sCId = siCountries[sCountry] as string;
            string sRId = siRegions[sRegion] as string;
            string sNickGen = NickGencomboBox.Text;
            if (sNickGen.Length == 0)
            {
                MessageBox.Show("Неверно задан тип генерации ников!");
                return;
            }

            string[] args1 = new string[] { 
                sFirstArg, 
                sType,
                sMail, 
                BotstextBox.Text, 
                ThreadstextBox.Text, 
                sCId, 
                sRId, 
                sGroup, 
                sNickGen,
                DelaytextBox.Text
            };

            string[] args2 = new string[] { 
                sFirstArg, 
                sType,
                sMail, 
                BotstextBox.Text, 
                ThreadstextBox.Text, 
                sCId, 
                sRId, 
                sGroup, 
                sNickGen,
                DelaytextBox.Text,
                ServertextBox.Text,
                LogintextBox.Text,
                PasswordtextBox.Text
            };
            if (!ActivatecheckBox.Checked)
                RegBots.Worker(args1);
            else
                RegBots.Worker(args2);
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

        private void RegForm_Load(object sender, EventArgs e)
        {
            NickGencomboBox.SelectedIndex = 1;

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
                        if (sCountryId!=""&&sCountryName!="")
                        {
                            if(!siCountries.Contains(sCountryName))
                                siCountries.Add(sCountryName, sCountryId);
                        }
                        if (siBufRegions.Count!=0&&sCountryName!="")
                        {
                            if (!siCountriesRegions.Contains(sCountryName))
                                siCountriesRegions.Add(sCountryName, siBufRegions);
                        }
                    }
                } 
                BindingSource bs = new BindingSource();
                bs.DataSource = siCountries;
                CountrycomboBox.DataSource = bs;
                CountrycomboBox.DisplayMember = "Key";
                CountrycomboBox_SelectedIndexChanged(null, null);
            }
            catch (System.Exception e1)
            {
                ConsoleLog.WriteLine(e1.ToString());
            }



        }

        private void RegioncheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CountrycomboBox_SelectedIndexChanged(sender, e);
        }

        private void RegioncomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ActivatecheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool bFlag = ActivatecheckBox.Checked;
            ServertextBox.Enabled = bFlag;
            LogintextBox.Enabled = bFlag;
            PasswordtextBox.Enabled = bFlag;
        }

        private void Selectbutton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofn = new OpenFileDialog();
            ofn.Filter = "All files (*.*)|*.*";
            ofn.Title = "Выберите файл";
            if (ofn.ShowDialog() == DialogResult.OK)
            {
                FilePathtextBox.Text = ofn.FileName;
            }

        }
    }
}
