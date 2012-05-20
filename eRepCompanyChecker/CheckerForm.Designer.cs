namespace eRepCompanyChecker
{
    partial class CheckerForm
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
            this.label4 = new System.Windows.Forms.Label();
            this.LogintextBox = new System.Windows.Forms.TextBox();
            this.PasswordtextBox = new System.Windows.Forms.TextBox();
            this.CompanyNumtextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.BotNumtextBox = new System.Windows.Forms.TextBox();
            this.SectextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.FireLastcheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Логин:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 35);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Пароль:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 53);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(136, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Название/номер фирмы:";
            // 
            // LogintextBox
            // 
            this.LogintextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.LogintextBox.Location = new System.Drawing.Point(91, 10);
            this.LogintextBox.Margin = new System.Windows.Forms.Padding(2);
            this.LogintextBox.Name = "LogintextBox";
            this.LogintextBox.Size = new System.Drawing.Size(139, 20);
            this.LogintextBox.TabIndex = 0;
            // 
            // PasswordtextBox
            // 
            this.PasswordtextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PasswordtextBox.Location = new System.Drawing.Point(91, 32);
            this.PasswordtextBox.Margin = new System.Windows.Forms.Padding(2);
            this.PasswordtextBox.Name = "PasswordtextBox";
            this.PasswordtextBox.PasswordChar = '*';
            this.PasswordtextBox.Size = new System.Drawing.Size(139, 20);
            this.PasswordtextBox.TabIndex = 1;
            // 
            // CompanyNumtextBox
            // 
            this.CompanyNumtextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CompanyNumtextBox.Location = new System.Drawing.Point(7, 69);
            this.CompanyNumtextBox.Margin = new System.Windows.Forms.Padding(2);
            this.CompanyNumtextBox.Name = "CompanyNumtextBox";
            this.CompanyNumtextBox.Size = new System.Drawing.Size(223, 20);
            this.CompanyNumtextBox.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(7, 162);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(222, 30);
            this.button1.TabIndex = 5;
            this.button1.Text = "Пуск";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 117);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(159, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Желаемое число работников:";
            // 
            // BotNumtextBox
            // 
            this.BotNumtextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.BotNumtextBox.Location = new System.Drawing.Point(166, 115);
            this.BotNumtextBox.Margin = new System.Windows.Forms.Padding(2);
            this.BotNumtextBox.Name = "BotNumtextBox";
            this.BotNumtextBox.Size = new System.Drawing.Size(64, 20);
            this.BotNumtextBox.TabIndex = 4;
            this.BotNumtextBox.Text = "9";
            // 
            // SectextBox
            // 
            this.SectextBox.Location = new System.Drawing.Point(99, 138);
            this.SectextBox.Margin = new System.Windows.Forms.Padding(2);
            this.SectextBox.Name = "SectextBox";
            this.SectextBox.Size = new System.Drawing.Size(42, 20);
            this.SectextBox.TabIndex = 4;
            this.SectextBox.Text = "30";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 140);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Проверять раз в";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(144, 140);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "секунд";
            // 
            // FireLastcheckBox
            // 
            this.FireLastcheckBox.AutoSize = true;
            this.FireLastcheckBox.Location = new System.Drawing.Point(7, 94);
            this.FireLastcheckBox.Name = "FireLastcheckBox";
            this.FireLastcheckBox.Size = new System.Drawing.Size(229, 17);
            this.FireLastcheckBox.TabIndex = 6;
            this.FireLastcheckBox.Text = "Увольнять только последнего в списке";
            this.FireLastcheckBox.UseVisualStyleBackColor = true;
            // 
            // CheckerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(235, 199);
            this.Controls.Add(this.FireLastcheckBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.SectextBox);
            this.Controls.Add(this.BotNumtextBox);
            this.Controls.Add(this.CompanyNumtextBox);
            this.Controls.Add(this.PasswordtextBox);
            this.Controls.Add(this.LogintextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "CheckerForm";
            this.Text = "Работа с фирмой";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox LogintextBox;
        private System.Windows.Forms.TextBox PasswordtextBox;
        private System.Windows.Forms.TextBox CompanyNumtextBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox BotNumtextBox;
        private System.Windows.Forms.TextBox SectextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox FireLastcheckBox;
    }
}