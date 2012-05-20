using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using NerZul;
using Engine;
using System.Collections;

namespace eRepConsoleManagementSystem
{
    public partial class VoteForm : Form
    {

        public VoteForm()
        {
            InitializeComponent();
        }
        private void Gobutton_Click_1(object sender, EventArgs e)
        {
            string sGroup, sLink;
            sGroup = GroupcomboBox.Text;
            sLink = TopictextBox.Text;
            int iLink = 0;
            if (sLink.Length == 0)
            {
                MessageBox.Show("Отсуствует номер на статьи!");
                return;
            }
            iLink = int.Parse(sLink);
            if (iLink.ToString() != sLink)
            {
                MessageBox.Show("Неверно задан номер статьи!");
                return;
            }

            string[] args = new string[] { "votearticle", sGroup, sLink };
            Engine.VoteArticle.Worker(args);

            MessageBox.Show("Готово!");

        }

        Hashtable hsgrps;
        private void VoteForm_Load(object sender, EventArgs e)
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
