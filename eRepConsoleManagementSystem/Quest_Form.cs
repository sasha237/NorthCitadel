using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Engine;

namespace eRepConsoleManagementSystem
{
    public partial class Quest_Form : Form
    {
        public Quest_Form()
        {
            InitializeComponent();
        }

        private void Startbutton_Click(object sender, EventArgs e)
        {
            string[] args = new string[] { 
                "collectquests", 
                GroupcomboBox.Text,
				CBAll.Checked ? "1" : "0",
				cbBuyFood.Checked ? "1" : "0",
				cbBuyWeapons.Checked ? "1" : "0"
            };
            Engine.CollectQuests.Worker(args);
        }

        Hashtable hsgrps;

        private void Quest_Form_Load(object sender, EventArgs e)
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
