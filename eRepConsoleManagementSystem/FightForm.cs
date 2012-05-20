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
    public partial class FightForm : Form
    {
        public FightForm()
        {
            InitializeComponent();
            toolTip1.SetToolTip(Gobutton, "Атаковать");

        }

        private void Gobutton_Click(object sender, EventArgs e)
        {
            string sGroup = GroupcomboBox.Text;
            string sBattleId = BattleIdtextBox.Text;
            string sBuyItem = chkBuyWeapon.Checked.ToString();
            string sBattleCountry = CountryIdtextBox.Text;
            string sLeftHP = LeftHPtextBox.Text;
            string[] args = new string[] { 
                "fight",
                sGroup,
                sBattleId,
                sBuyItem,
                sLeftHP,
                sBattleCountry,
                DoNotChangeWeaponcheckBox.Checked.ToString(),
                ShotLimittextBox.Text,
                CyclicFight.Checked.ToString(),
                ExpLimittextBox.Text
            };
            Engine.Fight.Worker(args);

        }

        Hashtable hsgrps;
        private void WarForm_Load(object sender, EventArgs e)
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
