using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NerZul;
using Engine;
using System.Collections;

namespace eRepConsoleManagementSystem
{
    public partial class DailyForm : Form
    {
        public DailyForm()
        {
            InitializeComponent();
        }
        Hashtable hsgrps;
        private void DailyForm_Load(object sender, EventArgs e)
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
            IndustrycomboBox.Text = "all";
            FeedcomboBox.SelectedIndex = 0;
        }

       
        private void Start_button_Click(object sender, EventArgs e)
        {
            string sGroup = GroupcomboBox.Text;
            string sDefWork = IdtextBox.Text;
            string sInd = IndustrycomboBox.SelectedIndex.ToString();
            if (sInd == "0")
            {
                sInd = "all";
            }

            string[] args = new string[] { 
                "daily",
                sGroup,
                sDefWork,
                sInd,
                chkLightMode.Checked ? "light" : "normal",
                chkResignBeforeWork.Checked.ToString(),
                FeedcomboBox.SelectedIndex.ToString()
            };
            Engine.Daily.Worker(args);
        }
    }
}
