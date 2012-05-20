namespace eRepConsoleManagementSystem
{
    partial class Subscribe_Form
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Subscribe_Form));
			this.SpamgroupBox = new System.Windows.Forms.GroupBox();
			this.GroupcomboBox = new System.Windows.Forms.ComboBox();
			this.Gobutton = new System.Windows.Forms.Button();
			this.Filelabel = new System.Windows.Forms.Label();
			this.NewspapertextBox = new System.Windows.Forms.TextBox();
			this.Loginlabel = new System.Windows.Forms.Label();
			this.SpamgroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// SpamgroupBox
			// 
			this.SpamgroupBox.AccessibleDescription = null;
			this.SpamgroupBox.AccessibleName = null;
			resources.ApplyResources(this.SpamgroupBox, "SpamgroupBox");
			this.SpamgroupBox.BackgroundImage = null;
			this.SpamgroupBox.Controls.Add(this.GroupcomboBox);
			this.SpamgroupBox.Controls.Add(this.Gobutton);
			this.SpamgroupBox.Controls.Add(this.Filelabel);
			this.SpamgroupBox.Controls.Add(this.NewspapertextBox);
			this.SpamgroupBox.Controls.Add(this.Loginlabel);
			this.SpamgroupBox.Font = null;
			this.SpamgroupBox.Name = "SpamgroupBox";
			this.SpamgroupBox.TabStop = false;
			// 
			// GroupcomboBox
			// 
			this.GroupcomboBox.AccessibleDescription = null;
			this.GroupcomboBox.AccessibleName = null;
			resources.ApplyResources(this.GroupcomboBox, "GroupcomboBox");
			this.GroupcomboBox.BackgroundImage = null;
			this.GroupcomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.GroupcomboBox.Font = null;
			this.GroupcomboBox.FormattingEnabled = true;
			this.GroupcomboBox.Name = "GroupcomboBox";
			this.GroupcomboBox.Sorted = true;
			// 
			// Gobutton
			// 
			this.Gobutton.AccessibleDescription = null;
			this.Gobutton.AccessibleName = null;
			resources.ApplyResources(this.Gobutton, "Gobutton");
			this.Gobutton.BackgroundImage = null;
			this.Gobutton.Font = null;
			this.Gobutton.Name = "Gobutton";
			this.Gobutton.UseVisualStyleBackColor = true;
			this.Gobutton.Click += new System.EventHandler(this.Gobutton_Click);
			// 
			// Filelabel
			// 
			this.Filelabel.AccessibleDescription = null;
			this.Filelabel.AccessibleName = null;
			resources.ApplyResources(this.Filelabel, "Filelabel");
			this.Filelabel.Font = null;
			this.Filelabel.Name = "Filelabel";
			// 
			// NewspapertextBox
			// 
			this.NewspapertextBox.AccessibleDescription = null;
			this.NewspapertextBox.AccessibleName = null;
			resources.ApplyResources(this.NewspapertextBox, "NewspapertextBox");
			this.NewspapertextBox.BackgroundImage = null;
			this.NewspapertextBox.Font = null;
			this.NewspapertextBox.Name = "NewspapertextBox";
			// 
			// Loginlabel
			// 
			this.Loginlabel.AccessibleDescription = null;
			this.Loginlabel.AccessibleName = null;
			resources.ApplyResources(this.Loginlabel, "Loginlabel");
			this.Loginlabel.Font = null;
			this.Loginlabel.Name = "Loginlabel";
			// 
			// Subscribe_Form
			// 
			this.AccessibleDescription = null;
			this.AccessibleName = null;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = null;
			this.Controls.Add(this.SpamgroupBox);
			this.Font = null;
			this.Icon = null;
			this.Name = "Subscribe_Form";
			this.Load += new System.EventHandler(this.Subscribe_Form_Load);
			this.SpamgroupBox.ResumeLayout(false);
			this.SpamgroupBox.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox SpamgroupBox;
        private System.Windows.Forms.ComboBox GroupcomboBox;
        private System.Windows.Forms.Button Gobutton;
        private System.Windows.Forms.Label Filelabel;
        private System.Windows.Forms.TextBox NewspapertextBox;
        private System.Windows.Forms.Label Loginlabel;
    }
}