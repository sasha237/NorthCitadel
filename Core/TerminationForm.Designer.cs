namespace NerZul.Core.Utils.Bicycles
{
    partial class TerminationForm
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
            this.Terminatebutton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Terminatebutton
            // 
            this.Terminatebutton.Location = new System.Drawing.Point(12, 12);
            this.Terminatebutton.Name = "Terminatebutton";
            this.Terminatebutton.Size = new System.Drawing.Size(302, 70);
            this.Terminatebutton.TabIndex = 0;
            this.Terminatebutton.Text = "Прервать";
            this.Terminatebutton.UseVisualStyleBackColor = true;
            this.Terminatebutton.Click += new System.EventHandler(this.Terminatebutton_Click);
            // 
            // TerminationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(326, 94);
            this.Controls.Add(this.Terminatebutton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TerminationForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Прервать";
            this.Load += new System.EventHandler(this.TerminationForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TerminationForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Terminatebutton;
    }
}