using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Engine;
using System.Collections;
using NerZul.Core.Utils;

namespace eRepConsoleManagementSystem
{
    public partial class FeedForm : Form
    {
        public FeedForm()
        {
            InitializeComponent();
        }

        private void HealthtrackBar_Scroll(object sender, EventArgs e)
        {
            UpdateItems();
        }
        void UpdateItems()
        {
            Healthlabel.Text = HealthtrackBar.Value.ToString();
        }

        private void Startbutton_Click(object sender, EventArgs e)
        {
            int iVal = 0;
            if (int.TryParse(Healthlabel.Text, out iVal) == false || iVal.ToString() != Healthlabel.Text)
            {
                MessageBox.Show("Неверно задан уровень здоровья!");
                return;
            }

            string[] args = new string[] { 
                "feed", 
                GroupcomboBox.Text, 
                Healthlabel.Text,
                HealthtextBox.Text,
                LesstextBox.Text,
                chkJustEat.Checked.ToString(),
                FastFoodcheckBox.Checked.ToString(),
                HungryFirstcheckBox.Checked.ToString()
            };
            Engine.Feed.Worker(args);
        }

        Hashtable hsgrps;
        private void FeedForm_Load(object sender, EventArgs e)
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
        }
    }
}
