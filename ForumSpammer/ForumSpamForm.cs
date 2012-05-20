using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ForumSpammer
{
    public partial class ForumSpamForm : Form
    {
        public ForumSpamForm()
        {
            InitializeComponent();
        }

        private void Grapbutton_Click(object sender, EventArgs e)
        {

        }

        private void Spambutton_Click(object sender, EventArgs e)
        {
            IForumBot bt = new ForumBotIPB(ForumPathtextBox.Text);
            bt.WorkingCycle(LogintextBox.Text, PasswordtextBox.Text, TitletextBox.Text, MessagetextBox.Text);

        }

        private void Filebutton_Click(object sender, EventArgs e)
        {

        }

        private void ForumSpamForm_Load(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void MessagetextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
