namespace eRepConsoleSpammer
{
    partial class SpamForm
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
            this.Partylabel = new System.Windows.Forms.Label();
            this.PartytextBox = new System.Windows.Forms.TextBox();
            this.Partybutton = new System.Windows.Forms.Button();
            this.Filelabel = new System.Windows.Forms.Label();
            this.FiletextBox = new System.Windows.Forms.TextBox();
            this.Filebutton = new System.Windows.Forms.Button();
            this.Loginlabel = new System.Windows.Forms.Label();
            this.LogintextBox = new System.Windows.Forms.TextBox();
            this.Gobutton = new System.Windows.Forms.Button();
            this.Passwordlabel = new System.Windows.Forms.Label();
            this.PasswordtextBox = new System.Windows.Forms.TextBox();
            this.Subjlabel = new System.Windows.Forms.Label();
            this.SubjtextBox = new System.Windows.Forms.TextBox();
            this.Textlabel = new System.Windows.Forms.Label();
            this.TexttextBox = new System.Windows.Forms.TextBox();
            this.Closebutton = new System.Windows.Forms.Button();
            this.PartygroupBox = new System.Windows.Forms.GroupBox();
            this.SpamgroupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.UsertextBox = new System.Windows.Forms.TextBox();
            this.Userbutton = new System.Windows.Forms.Button();
            this.XMLbutton = new System.Windows.Forms.Button();
            this.XMLtextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.ParseXMLbutton = new System.Windows.Forms.Button();
            this.AutocaptchatextBox = new System.Windows.Forms.TextBox();
            this.AutocaptchacheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.RegiontextBox = new System.Windows.Forms.TextBox();
            this.Regionbutton = new System.Windows.Forms.Button();
            this.PartygroupBox.SuspendLayout();
            this.SpamgroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Partylabel
            // 
            this.Partylabel.AutoSize = true;
            this.Partylabel.Location = new System.Drawing.Point(6, 18);
            this.Partylabel.Name = "Partylabel";
            this.Partylabel.Size = new System.Drawing.Size(238, 17);
            this.Partylabel.TabIndex = 0;
            this.Partylabel.Text = "Обозначение партии (имя-номер):";
            // 
            // PartytextBox
            // 
            this.PartytextBox.Location = new System.Drawing.Point(90, 38);
            this.PartytextBox.Name = "PartytextBox";
            this.PartytextBox.Size = new System.Drawing.Size(197, 22);
            this.PartytextBox.TabIndex = 0;
            // 
            // Partybutton
            // 
            this.Partybutton.Location = new System.Drawing.Point(293, 38);
            this.Partybutton.Name = "Partybutton";
            this.Partybutton.Size = new System.Drawing.Size(135, 25);
            this.Partybutton.TabIndex = 1;
            this.Partybutton.Text = "Обновить состав";
            this.Partybutton.UseVisualStyleBackColor = true;
            this.Partybutton.Click += new System.EventHandler(this.Partybutton_Click);
            // 
            // Filelabel
            // 
            this.Filelabel.AutoSize = true;
            this.Filelabel.Location = new System.Drawing.Point(6, 24);
            this.Filelabel.Name = "Filelabel";
            this.Filelabel.Size = new System.Drawing.Size(86, 17);
            this.Filelabel.TabIndex = 0;
            this.Filelabel.Text = "Файл базы:";
            // 
            // FiletextBox
            // 
            this.FiletextBox.BackColor = System.Drawing.SystemColors.Window;
            this.FiletextBox.Location = new System.Drawing.Point(90, 21);
            this.FiletextBox.Name = "FiletextBox";
            this.FiletextBox.ReadOnly = true;
            this.FiletextBox.Size = new System.Drawing.Size(197, 22);
            this.FiletextBox.TabIndex = 0;
            // 
            // Filebutton
            // 
            this.Filebutton.Location = new System.Drawing.Point(293, 18);
            this.Filebutton.Name = "Filebutton";
            this.Filebutton.Size = new System.Drawing.Size(135, 24);
            this.Filebutton.TabIndex = 1;
            this.Filebutton.Text = "Выбрать";
            this.Filebutton.UseVisualStyleBackColor = true;
            this.Filebutton.Click += new System.EventHandler(this.Filebutton_Click);
            // 
            // Loginlabel
            // 
            this.Loginlabel.AutoSize = true;
            this.Loginlabel.Location = new System.Drawing.Point(41, 54);
            this.Loginlabel.Name = "Loginlabel";
            this.Loginlabel.Size = new System.Drawing.Size(51, 17);
            this.Loginlabel.TabIndex = 0;
            this.Loginlabel.Text = "Логин:";
            // 
            // LogintextBox
            // 
            this.LogintextBox.Location = new System.Drawing.Point(90, 49);
            this.LogintextBox.Name = "LogintextBox";
            this.LogintextBox.Size = new System.Drawing.Size(197, 22);
            this.LogintextBox.TabIndex = 2;
            // 
            // Gobutton
            // 
            this.Gobutton.Location = new System.Drawing.Point(293, 47);
            this.Gobutton.Name = "Gobutton";
            this.Gobutton.Size = new System.Drawing.Size(135, 52);
            this.Gobutton.TabIndex = 6;
            this.Gobutton.Text = "Поехали";
            this.Gobutton.UseVisualStyleBackColor = true;
            this.Gobutton.Click += new System.EventHandler(this.Gobutton_Click);
            // 
            // Passwordlabel
            // 
            this.Passwordlabel.AutoSize = true;
            this.Passwordlabel.Location = new System.Drawing.Point(31, 80);
            this.Passwordlabel.Name = "Passwordlabel";
            this.Passwordlabel.Size = new System.Drawing.Size(61, 17);
            this.Passwordlabel.TabIndex = 0;
            this.Passwordlabel.Text = "Пароль:";
            // 
            // PasswordtextBox
            // 
            this.PasswordtextBox.Location = new System.Drawing.Point(90, 77);
            this.PasswordtextBox.Name = "PasswordtextBox";
            this.PasswordtextBox.Size = new System.Drawing.Size(197, 22);
            this.PasswordtextBox.TabIndex = 3;
            // 
            // Subjlabel
            // 
            this.Subjlabel.AutoSize = true;
            this.Subjlabel.Location = new System.Drawing.Point(12, 102);
            this.Subjlabel.Name = "Subjlabel";
            this.Subjlabel.Size = new System.Drawing.Size(80, 17);
            this.Subjlabel.TabIndex = 0;
            this.Subjlabel.Text = "Заголовок:";
            // 
            // SubjtextBox
            // 
            this.SubjtextBox.Location = new System.Drawing.Point(6, 122);
            this.SubjtextBox.Name = "SubjtextBox";
            this.SubjtextBox.Size = new System.Drawing.Size(422, 22);
            this.SubjtextBox.TabIndex = 4;
            // 
            // Textlabel
            // 
            this.Textlabel.AutoSize = true;
            this.Textlabel.Location = new System.Drawing.Point(12, 147);
            this.Textlabel.Name = "Textlabel";
            this.Textlabel.Size = new System.Drawing.Size(125, 17);
            this.Textlabel.TabIndex = 0;
            this.Textlabel.Text = " Текст рассылки: ";
            // 
            // TexttextBox
            // 
            this.TexttextBox.Location = new System.Drawing.Point(6, 167);
            this.TexttextBox.Multiline = true;
            this.TexttextBox.Name = "TexttextBox";
            this.TexttextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TexttextBox.Size = new System.Drawing.Size(422, 142);
            this.TexttextBox.TabIndex = 5;
            // 
            // Closebutton
            // 
            this.Closebutton.Location = new System.Drawing.Point(375, 619);
            this.Closebutton.Name = "Closebutton";
            this.Closebutton.Size = new System.Drawing.Size(74, 28);
            this.Closebutton.TabIndex = 0;
            this.Closebutton.Text = "Закрыть";
            this.Closebutton.UseVisualStyleBackColor = true;
            this.Closebutton.Click += new System.EventHandler(this.Closebutton_Click);
            // 
            // PartygroupBox
            // 
            this.PartygroupBox.Controls.Add(this.Partylabel);
            this.PartygroupBox.Controls.Add(this.PartytextBox);
            this.PartygroupBox.Controls.Add(this.Partybutton);
            this.PartygroupBox.Location = new System.Drawing.Point(12, 12);
            this.PartygroupBox.Name = "PartygroupBox";
            this.PartygroupBox.Size = new System.Drawing.Size(437, 70);
            this.PartygroupBox.TabIndex = 3;
            this.PartygroupBox.TabStop = false;
            this.PartygroupBox.Text = " Партия ";
            // 
            // SpamgroupBox
            // 
            this.SpamgroupBox.Controls.Add(this.Filelabel);
            this.SpamgroupBox.Controls.Add(this.FiletextBox);
            this.SpamgroupBox.Controls.Add(this.LogintextBox);
            this.SpamgroupBox.Controls.Add(this.TexttextBox);
            this.SpamgroupBox.Controls.Add(this.Gobutton);
            this.SpamgroupBox.Controls.Add(this.Textlabel);
            this.SpamgroupBox.Controls.Add(this.Loginlabel);
            this.SpamgroupBox.Controls.Add(this.SubjtextBox);
            this.SpamgroupBox.Controls.Add(this.Filebutton);
            this.SpamgroupBox.Controls.Add(this.Subjlabel);
            this.SpamgroupBox.Controls.Add(this.PasswordtextBox);
            this.SpamgroupBox.Controls.Add(this.Passwordlabel);
            this.SpamgroupBox.Location = new System.Drawing.Point(14, 298);
            this.SpamgroupBox.Name = "SpamgroupBox";
            this.SpamgroupBox.Size = new System.Drawing.Size(437, 315);
            this.SpamgroupBox.TabIndex = 4;
            this.SpamgroupBox.TabStop = false;
            this.SpamgroupBox.Text = " Рассылка по партии ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 625);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(331, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Для автоподстановки имени введи %username%";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.UsertextBox);
            this.groupBox1.Controls.Add(this.Userbutton);
            this.groupBox1.Location = new System.Drawing.Point(12, 164);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(439, 70);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Пользователь";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Id пользователя";
            // 
            // UsertextBox
            // 
            this.UsertextBox.Location = new System.Drawing.Point(90, 38);
            this.UsertextBox.Name = "UsertextBox";
            this.UsertextBox.Size = new System.Drawing.Size(197, 22);
            this.UsertextBox.TabIndex = 0;
            // 
            // Userbutton
            // 
            this.Userbutton.Location = new System.Drawing.Point(293, 38);
            this.Userbutton.Name = "Userbutton";
            this.Userbutton.Size = new System.Drawing.Size(135, 25);
            this.Userbutton.TabIndex = 1;
            this.Userbutton.Text = "Обновить состав";
            this.Userbutton.UseVisualStyleBackColor = true;
            this.Userbutton.Click += new System.EventHandler(this.Userbutton_Click);
            // 
            // XMLbutton
            // 
            this.XMLbutton.Location = new System.Drawing.Point(298, 16);
            this.XMLbutton.Name = "XMLbutton";
            this.XMLbutton.Size = new System.Drawing.Size(42, 24);
            this.XMLbutton.TabIndex = 1;
            this.XMLbutton.Text = "...";
            this.XMLbutton.UseVisualStyleBackColor = true;
            this.XMLbutton.Click += new System.EventHandler(this.XMLbutton_Click);
            // 
            // XMLtextBox
            // 
            this.XMLtextBox.BackColor = System.Drawing.SystemColors.Window;
            this.XMLtextBox.Location = new System.Drawing.Point(95, 16);
            this.XMLtextBox.Name = "XMLtextBox";
            this.XMLtextBox.ReadOnly = true;
            this.XMLtextBox.Size = new System.Drawing.Size(197, 22);
            this.XMLtextBox.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "Файл XML:";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.XMLtextBox);
            this.groupBox3.Controls.Add(this.ParseXMLbutton);
            this.groupBox3.Controls.Add(this.XMLbutton);
            this.groupBox3.Location = new System.Drawing.Point(14, 240);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(437, 54);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Разобрать XML";
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // ParseXMLbutton
            // 
            this.ParseXMLbutton.Location = new System.Drawing.Point(346, 16);
            this.ParseXMLbutton.Name = "ParseXMLbutton";
            this.ParseXMLbutton.Size = new System.Drawing.Size(82, 24);
            this.ParseXMLbutton.TabIndex = 1;
            this.ParseXMLbutton.Text = "Разобрать";
            this.ParseXMLbutton.UseVisualStyleBackColor = true;
            this.ParseXMLbutton.Click += new System.EventHandler(this.ParseXMLbutton_Click);
            // 
            // AutocaptchatextBox
            // 
            this.AutocaptchatextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AutocaptchatextBox.Enabled = false;
            this.AutocaptchatextBox.Location = new System.Drawing.Point(123, 652);
            this.AutocaptchatextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.AutocaptchatextBox.Name = "AutocaptchatextBox";
            this.AutocaptchatextBox.Size = new System.Drawing.Size(199, 22);
            this.AutocaptchatextBox.TabIndex = 15;
            // 
            // AutocaptchacheckBox
            // 
            this.AutocaptchacheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AutocaptchacheckBox.AutoSize = true;
            this.AutocaptchacheckBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.AutocaptchacheckBox.Location = new System.Drawing.Point(12, 655);
            this.AutocaptchacheckBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.AutocaptchacheckBox.Name = "AutocaptchacheckBox";
            this.AutocaptchacheckBox.Size = new System.Drawing.Size(100, 21);
            this.AutocaptchacheckBox.TabIndex = 14;
            this.AutocaptchacheckBox.Text = "Автокапча";
            this.AutocaptchacheckBox.UseVisualStyleBackColor = true;
            this.AutocaptchacheckBox.CheckedChanged += new System.EventHandler(this.AutocaptchacheckBox_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.RegiontextBox);
            this.groupBox2.Controls.Add(this.Regionbutton);
            this.groupBox2.Location = new System.Drawing.Point(12, 88);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(437, 70);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Регион";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 17);
            this.label4.TabIndex = 0;
            this.label4.Text = "Номер региона:";
            // 
            // RegiontextBox
            // 
            this.RegiontextBox.Location = new System.Drawing.Point(90, 38);
            this.RegiontextBox.Name = "RegiontextBox";
            this.RegiontextBox.Size = new System.Drawing.Size(197, 22);
            this.RegiontextBox.TabIndex = 0;
            // 
            // Regionbutton
            // 
            this.Regionbutton.Location = new System.Drawing.Point(293, 38);
            this.Regionbutton.Name = "Regionbutton";
            this.Regionbutton.Size = new System.Drawing.Size(135, 25);
            this.Regionbutton.TabIndex = 1;
            this.Regionbutton.Text = "Обновить состав";
            this.Regionbutton.UseVisualStyleBackColor = true;
            this.Regionbutton.Click += new System.EventHandler(this.Regionbutton_Click);
            // 
            // SpamForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 685);
            this.Controls.Add(this.AutocaptchatextBox);
            this.Controls.Add(this.AutocaptchacheckBox);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SpamgroupBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.PartygroupBox);
            this.Controls.Add(this.Closebutton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SpamForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Рассылка по партии";
            this.Load += new System.EventHandler(this.SpamForm_Load);
            this.PartygroupBox.ResumeLayout(false);
            this.PartygroupBox.PerformLayout();
            this.SpamgroupBox.ResumeLayout(false);
            this.SpamgroupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Partylabel;
        private System.Windows.Forms.TextBox PartytextBox;
        private System.Windows.Forms.Button Partybutton;
        private System.Windows.Forms.Label Filelabel;
        private System.Windows.Forms.TextBox FiletextBox;
        private System.Windows.Forms.Button Filebutton;
        private System.Windows.Forms.Label Loginlabel;
        private System.Windows.Forms.TextBox LogintextBox;
        private System.Windows.Forms.Button Gobutton;
        private System.Windows.Forms.Label Passwordlabel;
        private System.Windows.Forms.TextBox PasswordtextBox;
        private System.Windows.Forms.Label Subjlabel;
        private System.Windows.Forms.TextBox SubjtextBox;
        private System.Windows.Forms.Label Textlabel;
        private System.Windows.Forms.TextBox TexttextBox;
        private System.Windows.Forms.Button Closebutton;
        private System.Windows.Forms.GroupBox PartygroupBox;
        private System.Windows.Forms.GroupBox SpamgroupBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox UsertextBox;
        private System.Windows.Forms.Button Userbutton;
        private System.Windows.Forms.Button XMLbutton;
        private System.Windows.Forms.TextBox XMLtextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button ParseXMLbutton;
        private System.Windows.Forms.TextBox AutocaptchatextBox;
        private System.Windows.Forms.CheckBox AutocaptchacheckBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox RegiontextBox;
        private System.Windows.Forms.Button Regionbutton;
    }
}

