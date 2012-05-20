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
    public partial class PartyForm : Form
    {
        public PartyForm()
        {
            InitializeComponent();
        }

        Hashtable hsgrps;
        private void PartyForm_Load(object sender, EventArgs e)
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
			CmbElectionType.SelectedIndex = 0;
        }

        private void Joinbutton_Click(object sender, EventArgs e)
        {
            string sGroup = GroupcomboBox.Text;
            string  sPartyId = PartytextBox.Text;
            string sCandidate = CandidatetextBox.Text;
            string sElecType = CmbElectionType.Text;
            string sElectId = ElectionIdtextBox.Text;
            string[] args = new string[] { "election", "join", sGroup, sPartyId, sElecType, sElectId, sCandidate };
            Elections.Worker(args);
        }

        private void Leavebutton_Click(object sender, EventArgs e)
        {
            string sGroup = GroupcomboBox.Text;
            string sPartyId = PartytextBox.Text;
            string sCandidate = CandidatetextBox.Text;
			string sElecType = CmbElectionType.Text;
            string sElectId = ElectionIdtextBox.Text;
            string[] args = new string[] { "election", "leave", sGroup, sPartyId, sElecType, sElectId, sCandidate };
            Elections.Worker(args);
        }

        private void Votebutton_Click(object sender, EventArgs e)
        {
            string sGroup = GroupcomboBox.Text;
            string sPartyId = PartytextBox.Text;
            string sCandidate = CandidatetextBox.Text;
            string sElecType = CmbElectionType.Text;
            string sElectId = ElectionIdtextBox.Text;
            string[] args = new string[] { "election", "vote", sGroup, sPartyId, sElecType, sElectId, sCandidate };
            Elections.Worker(args);
        }

    }
}
