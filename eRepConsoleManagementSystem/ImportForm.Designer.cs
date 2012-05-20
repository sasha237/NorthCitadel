namespace eRepConsoleManagementSystem
{
    partial class ImportForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportForm));
			this.label1 = new System.Windows.Forms.Label();
			this.FilePathtextBox = new System.Windows.Forms.TextBox();
			this.Selectbutton = new System.Windows.Forms.Button();
			this.StartButton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.GrouptextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AccessibleDescription = null;
			this.label1.AccessibleName = null;
			resources.ApplyResources(this.label1, "label1");
			this.label1.Font = null;
			this.label1.Name = "label1";
			// 
			// FilePathtextBox
			// 
			this.FilePathtextBox.AccessibleDescription = null;
			this.FilePathtextBox.AccessibleName = null;
			resources.ApplyResources(this.FilePathtextBox, "FilePathtextBox");
			this.FilePathtextBox.BackColor = System.Drawing.SystemColors.Window;
			this.FilePathtextBox.BackgroundImage = null;
			this.FilePathtextBox.Font = null;
			this.FilePathtextBox.Name = "FilePathtextBox";
			this.FilePathtextBox.ReadOnly = true;
			// 
			// Selectbutton
			// 
			this.Selectbutton.AccessibleDescription = null;
			this.Selectbutton.AccessibleName = null;
			resources.ApplyResources(this.Selectbutton, "Selectbutton");
			this.Selectbutton.BackgroundImage = null;
			this.Selectbutton.Font = null;
			this.Selectbutton.Name = "Selectbutton";
			this.Selectbutton.UseVisualStyleBackColor = true;
			this.Selectbutton.Click += new System.EventHandler(this.Selectbutton_Click);
			// 
			// StartButton
			// 
			this.StartButton.AccessibleDescription = null;
			this.StartButton.AccessibleName = null;
			resources.ApplyResources(this.StartButton, "StartButton");
			this.StartButton.BackgroundImage = null;
			this.StartButton.Font = null;
			this.StartButton.Name = "StartButton";
			this.StartButton.UseVisualStyleBackColor = true;
			this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
			// 
			// label2
			// 
			this.label2.AccessibleDescription = null;
			this.label2.AccessibleName = null;
			resources.ApplyResources(this.label2, "label2");
			this.label2.Font = null;
			this.label2.Name = "label2";
			// 
			// GrouptextBox
			// 
			this.GrouptextBox.AccessibleDescription = null;
			this.GrouptextBox.AccessibleName = null;
			resources.ApplyResources(this.GrouptextBox, "GrouptextBox");
			this.GrouptextBox.BackgroundImage = null;
			this.GrouptextBox.Font = null;
			this.GrouptextBox.Name = "GrouptextBox";
			// 
			// ImportForm
			// 
			this.AccessibleDescription = null;
			this.AccessibleName = null;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = null;
			this.Controls.Add(this.GrouptextBox);
			this.Controls.Add(this.StartButton);
			this.Controls.Add(this.Selectbutton);
			this.Controls.Add(this.FilePathtextBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Font = null;
			this.Icon = null;
			this.Name = "ImportForm";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox FilePathtextBox;
        private System.Windows.Forms.Button Selectbutton;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox GrouptextBox;
    }
}