using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Engine;
using System.Collections;

namespace eRepConsoleManagementSystem
{
    public partial class ReportForm : Form
    {
        public ReportForm()
        {
            InitializeComponent();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

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

        Hashtable hsgrps;
        Hashtable hsType;
        Hashtable hsLang;
        Hashtable hsItem;
        private void ReportForm_Load(object sender, EventArgs e)
        {
            DbRows grps = Globals.Database.Select("bots", "`group`");
            BindingSource bs = null; new BindingSource();
            hsgrps = new Hashtable();
            hsType = new Hashtable();
            hsLang = new Hashtable();
            hsItem = new Hashtable();
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
            bs = new BindingSource();
            bs.DataSource = hsgrps;
            GroupcomboBox.DataSource = bs;
            GroupcomboBox.DisplayMember = "Key";
            Globals.Database.Reset();
            GroupcomboBox.Text = "all";

//             hsType.Add(1, "Offensive ad");
//             hsType.Add(2, "Misleading ad");
//             hsType.Add(3, "Illegal link in ad");
//             hsType.Add(4, "Spam ad");
//             hsType.Add(5, "Other ad");
            hsType.Add(6, "Vulgarity");
            hsType.Add(7, "Insults");
            hsType.Add(8, "Racism");
            hsType.Add(9, "Pornography");
            hsType.Add(10, "Spam");
            hsType.Add(11, "External advertising");
//             hsType.Add(12, "Illegal links");
//             hsType.Add(13, "Trolling");
//             hsType.Add(14, "Flaming");
//             hsType.Add(15, "Accusations of not respecting the eRepublik Laws");
            hsType.Add(16, "Illegal public debates");
//             hsType.Add(17, "Unlawful company picture");
//             hsType.Add(18, "Unlawful company name");
//             hsType.Add(19, "Unlawful company discussion area");
//             hsType.Add(20, "Unlawful party picture");
//             hsType.Add(21, "Unlawful party name");
//             hsType.Add(22, "Unlawful party discussion area");
//             hsType.Add(23, "Unlawful newspaper picture");
//             hsType.Add(24, "Unlawful newspaper name");
//             hsType.Add(25, "Illegal shout(s)");
//             hsType.Add(26, "Unlawful citizen/organization picture");
//             hsType.Add(27, "Unlawful citizen/organization name");
//             hsType.Add(28, "Unlawful citizen/organization shout(s)");
//             hsType.Add(29, "Multiple accounts usage");

            bs = new BindingSource();
            bs.DataSource = hsType;
            TypecomboBox.DataSource = bs;
            TypecomboBox.DisplayMember = "Value";
            TypecomboBox.Text = "Offensive ad";

            hsLang.Add("en", "English");
            hsLang.Add("fr", "Français");
            hsLang.Add("de", "Deutsch");
            hsLang.Add("hu", "Magyar");
            hsLang.Add("it", "Italiano");
            hsLang.Add("pt", "Portugues");
            hsLang.Add("ro", "Româna");
            hsLang.Add("ru", "Русский");
            hsLang.Add("es", "Español");
            hsLang.Add("sv", "Svenska");
            hsLang.Add("pl", "Polish");
            hsLang.Add("gr", "Ελληνικά");
            hsLang.Add("hr", "Hrvatska");
            hsLang.Add("bg", "Български");
            hsLang.Add("sr", "Српски");
            hsLang.Add("tr", "Türkçe");
            hsLang.Add("zz", "Other");

            bs = new BindingSource();
            bs.DataSource = hsLang;
            LangcomboBox.DataSource = bs;
            LangcomboBox.DisplayMember = "Value";
            LangcomboBox.Text = "English";

            hsItem.Add(1, "articles");
            hsItem.Add(2, "ads");
            hsItem.Add(3, "article_comments");
            hsItem.Add(4, "shouts");
            hsItem.Add(5, "citizens");
            hsItem.Add(6, "newspapers");
            hsItem.Add(7, "organizations");

            bs = new BindingSource();
            bs.DataSource = hsItem;
            ItemcomboBox.DataSource = bs;
            ItemcomboBox.DisplayMember = "Value";
            ItemcomboBox.Text = "articles";


        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Gobutton_Click(object sender, EventArgs e)
        {
            DictionaryEntry ppType = (DictionaryEntry)TypecomboBox.SelectedValue;
            DictionaryEntry ppLang = (DictionaryEntry)LangcomboBox.SelectedValue;
            DictionaryEntry ppItem = (DictionaryEntry)ItemcomboBox.SelectedValue;
            string sGroup = GroupcomboBox.Text;
            string sId = IdtextBox.Text;
            string sType = ppType.Key.ToString();
            string sLang = ppLang.Key.ToString();
            string sItem = ppItem.Key.ToString();
            string[] args = new string[] { "report", sGroup, sId, sType, sLang, sItem};
            ReportAll.Worker(args);
        }
    }
}
