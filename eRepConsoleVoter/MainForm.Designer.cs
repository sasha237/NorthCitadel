namespace eRepConsoleVoter
{
    partial class MainForm
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
            this.Filelabel = new System.Windows.Forms.Label();
            this.FiletextBox = new System.Windows.Forms.TextBox();
            this.Filebutton = new System.Windows.Forms.Button();
            this.Loginlabel = new System.Windows.Forms.Label();
            this.TopictextBox = new System.Windows.Forms.TextBox();
            this.Closebutton = new System.Windows.Forms.Button();
            this.SpamgroupBox = new System.Windows.Forms.GroupBox();
            this.Gobutton = new System.Windows.Forms.Button();
            this.SpamgroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // Filelabel
            // 
            this.Filelabel.AutoSize = true;
            this.Filelabel.Location = new System.Drawing.Point(23, 24);
            this.Filelabel.Name = "Filelabel";
            this.Filelabel.Size = new System.Drawing.Size(86, 17);
            this.Filelabel.TabIndex = 0;
            this.Filelabel.Text = "Файл базы:";
            // 
            // FiletextBox
            // 
            this.FiletextBox.BackColor = System.Drawing.SystemColors.Window;
            this.FiletextBox.Location = new System.Drawing.Point(115, 21);
            this.FiletextBox.Name = "FiletextBox";
            this.FiletextBox.ReadOnly = true;
            this.FiletextBox.Size = new System.Drawing.Size(172, 22);
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
            this.Loginlabel.Location = new System.Drawing.Point(6, 54);
            this.Loginlabel.Name = "Loginlabel";
            this.Loginlabel.Size = new System.Drawing.Size(103, 17);
            this.Loginlabel.TabIndex = 0;
            this.Loginlabel.Text = "Номер статьи:";
            // 
            // TopictextBox
            // 
            this.TopictextBox.Location = new System.Drawing.Point(115, 49);
            this.TopictextBox.Name = "TopictextBox";
            this.TopictextBox.Size = new System.Drawing.Size(313, 22);
            this.TopictextBox.TabIndex = 2;
            // 
            // Closebutton
            // 
            this.Closebutton.Location = new System.Drawing.Point(373, 160);
            this.Closebutton.Name = "Closebutton";
            this.Closebutton.Size = new System.Drawing.Size(74, 28);
            this.Closebutton.TabIndex = 0;
            this.Closebutton.Text = "Закрыть";
            this.Closebutton.UseVisualStyleBackColor = true;
            this.Closebutton.Click += new System.EventHandler(this.Closebutton_Click);
            // 
            // SpamgroupBox
            // 
            this.SpamgroupBox.Controls.Add(this.Gobutton);
            this.SpamgroupBox.Controls.Add(this.Filelabel);
            this.SpamgroupBox.Controls.Add(this.FiletextBox);
            this.SpamgroupBox.Controls.Add(this.TopictextBox);
            this.SpamgroupBox.Controls.Add(this.Loginlabel);
            this.SpamgroupBox.Controls.Add(this.Filebutton);
            this.SpamgroupBox.Location = new System.Drawing.Point(10, 12);
            this.SpamgroupBox.Name = "SpamgroupBox";
            this.SpamgroupBox.Size = new System.Drawing.Size(437, 143);
            this.SpamgroupBox.TabIndex = 4;
            this.SpamgroupBox.TabStop = false;
            this.SpamgroupBox.Text = " Параметры ";
            // 
            // Gobutton
            // 
            this.Gobutton.Location = new System.Drawing.Point(6, 74);
            this.Gobutton.Name = "Gobutton";
            this.Gobutton.Size = new System.Drawing.Size(422, 63);
            this.Gobutton.TabIndex = 3;
            this.Gobutton.Text = "Поехали";
            this.Gobutton.UseVisualStyleBackColor = true;
            this.Gobutton.Click += new System.EventHandler(this.Gobutton_Click_1);
            // 
            // VoteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 200);
            this.Controls.Add(this.SpamgroupBox);
            this.Controls.Add(this.Closebutton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VoteForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Автоматическая воталка";
            this.SpamgroupBox.ResumeLayout(false);
            this.SpamgroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label Filelabel;
        private System.Windows.Forms.TextBox FiletextBox;
        private System.Windows.Forms.Button Filebutton;
        private System.Windows.Forms.Label Loginlabel;
        private System.Windows.Forms.TextBox TopictextBox;
        private System.Windows.Forms.Button Closebutton;
        private System.Windows.Forms.GroupBox SpamgroupBox;
        private System.Windows.Forms.Button Gobutton;
    }
}

