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
    public partial class GoldForm : Form
    {
        public GoldForm()
        {
            InitializeComponent();
        }
        Hashtable hsgrps;
        private void GoldForm_Load(object sender, EventArgs e)
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

        private void Gobutton_Click(object sender, EventArgs e)
        {
            string[] args = new string[] { "buymoneyfromoffer",
                GroupcomboBox.Text,
                OfferNumtextBox.Text,
                FromWhotextBox.Text,
                HowMuchtextBox.Text,
                CurrencyforSelltextBox.Text,
                CurrencyForBuytextBox.Text,
                ExpFromtextBox.Text,
                ExpTotextBox.Text
            };
            MoneyProcessor.WorkerBuyFromOffer(args);
        }

        private void Showerbutton_Click(object sender, EventArgs e)
        {
            string[] args = new string[] { "collectmoney",
                GroupcomboBox.Text,
                MamonIdtextBox.Text,
                MammonCurr.Text,
                MamonExpFrom.Text,
                MamonExpTo.Text
            };
            MoneyProcessor.WorkerCollectMoney(args);
        }

        private void GoDonate_Click(object sender, EventArgs e)
        {
            string[] args = new string[] { "donatemoney",
                GroupcomboBox.Text,
                DonateLogin.Text,
                DonatePassword.Text,
                DonateCurrency.Text,
                DonateAmount.Text
            };
            MoneyProcessor.WorkerDonateMoney(args);
        }
    }
}
