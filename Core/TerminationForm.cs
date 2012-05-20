using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NerZul.Core.Utils.Bicycles
{
    public partial class TerminationForm : Form
    {
        public System.Threading.Thread[] pool;
        public TerminationForm()
        {
            InitializeComponent();
        }

        private void TerminationForm_Load(object sender, EventArgs e)
        {

        }

        private void TerminationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            int i;
            for (i = 0; i < pool.Length; i++)
            {
                if (pool[i] != null)
                    pool[i].Abort();
            }
        }

        private void Terminatebutton_Click(object sender, EventArgs e)
        {
            TerminationForm_FormClosing(null, null);
            Close();
        }
    }
}
