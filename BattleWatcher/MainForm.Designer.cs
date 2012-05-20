namespace BattleWatcher
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
            this.Gobutton = new System.Windows.Forms.Button();
            this.IdtextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SectextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Gobutton
            // 
            this.Gobutton.Location = new System.Drawing.Point(11, 72);
            this.Gobutton.Name = "Gobutton";
            this.Gobutton.Size = new System.Drawing.Size(169, 36);
            this.Gobutton.TabIndex = 2;
            this.Gobutton.Text = "Go";
            this.Gobutton.UseVisualStyleBackColor = true;
            this.Gobutton.Click += new System.EventHandler(this.Gobutton_Click);
            // 
            // IdtextBox
            // 
            this.IdtextBox.Location = new System.Drawing.Point(81, 12);
            this.IdtextBox.Name = "IdtextBox";
            this.IdtextBox.Size = new System.Drawing.Size(100, 22);
            this.IdtextBox.TabIndex = 0;
            this.IdtextBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Battle Id:";
            // 
            // SectextBox
            // 
            this.SectextBox.Location = new System.Drawing.Point(114, 40);
            this.SectextBox.Name = "SectextBox";
            this.SectextBox.Size = new System.Drawing.Size(30, 22);
            this.SectextBox.TabIndex = 1;
            this.SectextBox.Text = "5";
            this.SectextBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Refresh each:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(151, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "sec";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(192, 120);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SectextBox);
            this.Controls.Add(this.IdtextBox);
            this.Controls.Add(this.Gobutton);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Gobutton;
        private System.Windows.Forms.TextBox IdtextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox SectextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}