namespace eRepConsoleManagementSystem
{
    partial class DailyForm
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
            this.GroupcomboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.IndustrycomboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.IdtextBox = new System.Windows.Forms.TextBox();
            this.Start_button = new System.Windows.Forms.Button();
            this.chkLightMode = new System.Windows.Forms.CheckBox();
            this.chkResignBeforeWork = new System.Windows.Forms.CheckBox();
            this.FeedcomboBox = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(61, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Группа:";
            // 
            // GroupcomboBox
            // 
            this.GroupcomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GroupcomboBox.FormattingEnabled = true;
            this.GroupcomboBox.Location = new System.Drawing.Point(116, 6);
            this.GroupcomboBox.Margin = new System.Windows.Forms.Padding(2);
            this.GroupcomboBox.Name = "GroupcomboBox";
            this.GroupcomboBox.Size = new System.Drawing.Size(172, 21);
            this.GroupcomboBox.Sorted = true;
            this.GroupcomboBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(54, 62);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Отрасль:";
            // 
            // IndustrycomboBox
            // 
            this.IndustrycomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.IndustrycomboBox.FormattingEnabled = true;
            this.IndustrycomboBox.Items.AddRange(new object[] {
            "All",
            "Producer",
            "Marketing Manager",
            "Project Manager",
            "Carpenter",
            "Builder",
            "Architect",
            "Engineer",
            "Mechanic",
            "Fitter",
            "Technician",
            "Harvester"});
            this.IndustrycomboBox.Location = new System.Drawing.Point(116, 59);
            this.IndustrycomboBox.Margin = new System.Windows.Forms.Padding(2);
            this.IndustrycomboBox.Name = "IndustrycomboBox";
            this.IndustrycomboBox.Size = new System.Drawing.Size(172, 21);
            this.IndustrycomboBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 36);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Id вакансии (0-все):";
            // 
            // IdtextBox
            // 
            this.IdtextBox.Location = new System.Drawing.Point(116, 33);
            this.IdtextBox.Margin = new System.Windows.Forms.Padding(2);
            this.IdtextBox.Name = "IdtextBox";
            this.IdtextBox.Size = new System.Drawing.Size(172, 20);
            this.IdtextBox.TabIndex = 2;
            this.IdtextBox.Text = "0";
            // 
            // Start_button
            // 
            this.Start_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Start_button.Location = new System.Drawing.Point(9, 195);
            this.Start_button.Margin = new System.Windows.Forms.Padding(2);
            this.Start_button.Name = "Start_button";
            this.Start_button.Size = new System.Drawing.Size(336, 37);
            this.Start_button.TabIndex = 8;
            this.Start_button.Text = "Пуск";
            this.Start_button.UseVisualStyleBackColor = true;
            this.Start_button.Click += new System.EventHandler(this.Start_button_Click);
            // 
            // chkLightMode
            // 
            this.chkLightMode.AutoSize = true;
            this.chkLightMode.Location = new System.Drawing.Point(116, 90);
            this.chkLightMode.Name = "chkLightMode";
            this.chkLightMode.Size = new System.Drawing.Size(187, 17);
            this.chkLightMode.TabIndex = 9;
            this.chkLightMode.Text = "Только сбор инфы (mode = light)";
            this.chkLightMode.UseVisualStyleBackColor = true;
            // 
            // chkResignBeforeWork
            // 
            this.chkResignBeforeWork.AutoSize = true;
            this.chkResignBeforeWork.Location = new System.Drawing.Point(116, 113);
            this.chkResignBeforeWork.Name = "chkResignBeforeWork";
            this.chkResignBeforeWork.Size = new System.Drawing.Size(170, 17);
            this.chkResignBeforeWork.TabIndex = 10;
            this.chkResignBeforeWork.Text = "Увольняться перед работой";
            this.chkResignBeforeWork.UseVisualStyleBackColor = true;
            // 
            // FeedcomboBox
            // 
            this.FeedcomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FeedcomboBox.FormattingEnabled = true;
            this.FeedcomboBox.Items.AddRange(new object[] {
            "Не есть",
            "До работы",
            "После работы"});
            this.FeedcomboBox.Location = new System.Drawing.Point(116, 160);
            this.FeedcomboBox.Margin = new System.Windows.Forms.Padding(2);
            this.FeedcomboBox.Name = "FeedcomboBox";
            this.FeedcomboBox.Size = new System.Drawing.Size(92, 21);
            this.FeedcomboBox.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(77, 163);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Еда:";
            // 
            // DailyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 240);
            this.Controls.Add(this.FeedcomboBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.chkResignBeforeWork);
            this.Controls.Add(this.chkLightMode);
            this.Controls.Add(this.Start_button);
            this.Controls.Add(this.IdtextBox);
            this.Controls.Add(this.IndustrycomboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.GroupcomboBox);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DailyForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Ежедневная работа";
            this.Load += new System.EventHandler(this.DailyForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox GroupcomboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox IndustrycomboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox IdtextBox;
        private System.Windows.Forms.Button Start_button;
        private System.Windows.Forms.CheckBox chkLightMode;
        private System.Windows.Forms.CheckBox chkResignBeforeWork;
        private System.Windows.Forms.ComboBox FeedcomboBox;
        private System.Windows.Forms.Label label8;
    }
}