namespace ForumSpammer
{
    partial class ForumSpamForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.LogintextBox = new System.Windows.Forms.TextBox();
            this.PasswordtextBox = new System.Windows.Forms.TextBox();
            this.MessagetextBox = new System.Windows.Forms.TextBox();
            this.Spambutton = new System.Windows.Forms.Button();
            this.ForumPathtextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RadioButtonSMF = new System.Windows.Forms.RadioButton();
            this.RadioButtonphpBB = new System.Windows.Forms.RadioButton();
            this.RadioButtonIPB = new System.Windows.Forms.RadioButton();
            this.RadioButtonvBulletin = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.Filelabel = new System.Windows.Forms.Label();
            this.FiletextBox = new System.Windows.Forms.TextBox();
            this.Filebutton = new System.Windows.Forms.Button();
            this.Grapbutton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.TitletextBox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(89, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Логин:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(79, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Пароль:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 155);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(128, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "Текст сообщения:";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // LogintextBox
            // 
            this.LogintextBox.Location = new System.Drawing.Point(146, 68);
            this.LogintextBox.Name = "LogintextBox";
            this.LogintextBox.Size = new System.Drawing.Size(239, 22);
            this.LogintextBox.TabIndex = 3;
            // 
            // PasswordtextBox
            // 
            this.PasswordtextBox.Location = new System.Drawing.Point(146, 96);
            this.PasswordtextBox.Name = "PasswordtextBox";
            this.PasswordtextBox.Size = new System.Drawing.Size(239, 22);
            this.PasswordtextBox.TabIndex = 4;
            // 
            // MessagetextBox
            // 
            this.MessagetextBox.Location = new System.Drawing.Point(146, 152);
            this.MessagetextBox.Multiline = true;
            this.MessagetextBox.Name = "MessagetextBox";
            this.MessagetextBox.Size = new System.Drawing.Size(239, 129);
            this.MessagetextBox.TabIndex = 6;
            this.MessagetextBox.TextChanged += new System.EventHandler(this.MessagetextBox_TextChanged);
            // 
            // Spambutton
            // 
            this.Spambutton.Location = new System.Drawing.Point(336, 304);
            this.Spambutton.Name = "Spambutton";
            this.Spambutton.Size = new System.Drawing.Size(198, 82);
            this.Spambutton.TabIndex = 9;
            this.Spambutton.Text = "Начать рассылку";
            this.Spambutton.UseVisualStyleBackColor = true;
            this.Spambutton.Click += new System.EventHandler(this.Spambutton_Click);
            // 
            // ForumPathtextBox
            // 
            this.ForumPathtextBox.Location = new System.Drawing.Point(146, 40);
            this.ForumPathtextBox.Name = "ForumPathtextBox";
            this.ForumPathtextBox.Size = new System.Drawing.Size(388, 22);
            this.ForumPathtextBox.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(32, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(108, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "Путь к форуму:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.RadioButtonSMF);
            this.groupBox1.Controls.Add(this.RadioButtonphpBB);
            this.groupBox1.Controls.Add(this.RadioButtonIPB);
            this.groupBox1.Controls.Add(this.RadioButtonvBulletin);
            this.groupBox1.Location = new System.Drawing.Point(391, 68);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(143, 213);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " Движок форума: ";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // RadioButtonSMF
            // 
            this.RadioButtonSMF.AutoSize = true;
            this.RadioButtonSMF.Location = new System.Drawing.Point(6, 102);
            this.RadioButtonSMF.Name = "RadioButtonSMF";
            this.RadioButtonSMF.Size = new System.Drawing.Size(57, 21);
            this.RadioButtonSMF.TabIndex = 3;
            this.RadioButtonSMF.TabStop = true;
            this.RadioButtonSMF.Text = "SMF";
            this.RadioButtonSMF.UseVisualStyleBackColor = true;
            // 
            // RadioButtonphpBB
            // 
            this.RadioButtonphpBB.AutoSize = true;
            this.RadioButtonphpBB.Location = new System.Drawing.Point(6, 75);
            this.RadioButtonphpBB.Name = "RadioButtonphpBB";
            this.RadioButtonphpBB.Size = new System.Drawing.Size(71, 21);
            this.RadioButtonphpBB.TabIndex = 2;
            this.RadioButtonphpBB.TabStop = true;
            this.RadioButtonphpBB.Text = "phpBB";
            this.RadioButtonphpBB.UseVisualStyleBackColor = true;
            // 
            // RadioButtonIPB
            // 
            this.RadioButtonIPB.AutoSize = true;
            this.RadioButtonIPB.Location = new System.Drawing.Point(6, 48);
            this.RadioButtonIPB.Name = "RadioButtonIPB";
            this.RadioButtonIPB.Size = new System.Drawing.Size(50, 21);
            this.RadioButtonIPB.TabIndex = 1;
            this.RadioButtonIPB.TabStop = true;
            this.RadioButtonIPB.Text = "IPB";
            this.RadioButtonIPB.UseVisualStyleBackColor = true;
            // 
            // RadioButtonvBulletin
            // 
            this.RadioButtonvBulletin.AutoSize = true;
            this.RadioButtonvBulletin.Location = new System.Drawing.Point(6, 21);
            this.RadioButtonvBulletin.Name = "RadioButtonvBulletin";
            this.RadioButtonvBulletin.Size = new System.Drawing.Size(82, 21);
            this.RadioButtonvBulletin.TabIndex = 0;
            this.RadioButtonvBulletin.TabStop = true;
            this.RadioButtonvBulletin.Text = "vBulletin";
            this.RadioButtonvBulletin.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(148, 284);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(331, 17);
            this.label5.TabIndex = 7;
            this.label5.Text = "Для автоподстановки имени введи %username%";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // Filelabel
            // 
            this.Filelabel.AutoSize = true;
            this.Filelabel.Location = new System.Drawing.Point(54, 15);
            this.Filelabel.Name = "Filelabel";
            this.Filelabel.Size = new System.Drawing.Size(86, 17);
            this.Filelabel.TabIndex = 9;
            this.Filelabel.Text = "Файл базы:";
            this.Filelabel.Visible = false;
            // 
            // FiletextBox
            // 
            this.FiletextBox.BackColor = System.Drawing.SystemColors.Window;
            this.FiletextBox.Location = new System.Drawing.Point(146, 12);
            this.FiletextBox.Name = "FiletextBox";
            this.FiletextBox.ReadOnly = true;
            this.FiletextBox.Size = new System.Drawing.Size(239, 22);
            this.FiletextBox.TabIndex = 0;
            this.FiletextBox.Visible = false;
            // 
            // Filebutton
            // 
            this.Filebutton.Location = new System.Drawing.Point(391, 11);
            this.Filebutton.Name = "Filebutton";
            this.Filebutton.Size = new System.Drawing.Size(143, 24);
            this.Filebutton.TabIndex = 1;
            this.Filebutton.Text = "Выбрать";
            this.Filebutton.UseVisualStyleBackColor = true;
            this.Filebutton.Visible = false;
            this.Filebutton.Click += new System.EventHandler(this.Filebutton_Click);
            // 
            // Grapbutton
            // 
            this.Grapbutton.Location = new System.Drawing.Point(146, 304);
            this.Grapbutton.Name = "Grapbutton";
            this.Grapbutton.Size = new System.Drawing.Size(184, 82);
            this.Grapbutton.TabIndex = 8;
            this.Grapbutton.Text = "Собрать список пользователей";
            this.Grapbutton.UseVisualStyleBackColor = true;
            this.Grapbutton.Visible = false;
            this.Grapbutton.Click += new System.EventHandler(this.Grapbutton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 127);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(124, 17);
            this.label6.TabIndex = 0;
            this.label6.Text = "Тема сообщения:";
            // 
            // TitletextBox
            // 
            this.TitletextBox.Location = new System.Drawing.Point(146, 124);
            this.TitletextBox.Name = "TitletextBox";
            this.TitletextBox.Size = new System.Drawing.Size(239, 22);
            this.TitletextBox.TabIndex = 5;
            // 
            // ForumSpamForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 395);
            this.Controls.Add(this.Filelabel);
            this.Controls.Add(this.FiletextBox);
            this.Controls.Add(this.Filebutton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ForumPathtextBox);
            this.Controls.Add(this.Grapbutton);
            this.Controls.Add(this.Spambutton);
            this.Controls.Add(this.MessagetextBox);
            this.Controls.Add(this.TitletextBox);
            this.Controls.Add(this.PasswordtextBox);
            this.Controls.Add(this.LogintextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ForumSpamForm";
            this.Text = "Форма для спама по форуму";
            this.Load += new System.EventHandler(this.ForumSpamForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox LogintextBox;
        private System.Windows.Forms.TextBox PasswordtextBox;
        private System.Windows.Forms.TextBox MessagetextBox;
        private System.Windows.Forms.Button Spambutton;
        private System.Windows.Forms.TextBox ForumPathtextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton RadioButtonSMF;
        private System.Windows.Forms.RadioButton RadioButtonphpBB;
        private System.Windows.Forms.RadioButton RadioButtonIPB;
        private System.Windows.Forms.RadioButton RadioButtonvBulletin;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label Filelabel;
        private System.Windows.Forms.TextBox FiletextBox;
        private System.Windows.Forms.Button Filebutton;
        private System.Windows.Forms.Button Grapbutton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox TitletextBox;
    }
}