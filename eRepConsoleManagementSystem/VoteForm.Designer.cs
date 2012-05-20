namespace eRepConsoleManagementSystem
{
    partial class VoteForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VoteForm));
			this.Filelabel = new System.Windows.Forms.Label();
			this.Loginlabel = new System.Windows.Forms.Label();
			this.TopictextBox = new System.Windows.Forms.TextBox();
			this.SpamgroupBox = new System.Windows.Forms.GroupBox();
			this.GroupcomboBox = new System.Windows.Forms.ComboBox();
			this.Gobutton = new System.Windows.Forms.Button();
			this.SpamgroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// Filelabel
			// 
			this.Filelabel.AccessibleDescription = null;
			this.Filelabel.AccessibleName = null;
			resources.ApplyResources(this.Filelabel, "Filelabel");
			this.Filelabel.Font = null;
			this.Filelabel.Name = "Filelabel";
			// 
			// Loginlabel
			// 
			this.Loginlabel.AccessibleDescription = null;
			this.Loginlabel.AccessibleName = null;
			resources.ApplyResources(this.Loginlabel, "Loginlabel");
			this.Loginlabel.Font = null;
			this.Loginlabel.Name = "Loginlabel";
			// 
			// TopictextBox
			// 
			this.TopictextBox.AccessibleDescription = null;
			this.TopictextBox.AccessibleName = null;
			resources.ApplyResources(this.TopictextBox, "TopictextBox");
			this.TopictextBox.BackgroundImage = null;
			this.TopictextBox.Font = null;
			this.TopictextBox.Name = "TopictextBox";
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
			this.SpamgroupBox.Controls.Add(this.TopictextBox);
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
			this.Gobutton.Click += new System.EventHandler(this.Gobutton_Click_1);
			// 
			// VoteForm
			// 
			this.AccessibleDescription = null;
			this.AccessibleName = null;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = null;
			this.Controls.Add(this.SpamgroupBox);
			this.Font = null;
			this.Icon = null;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "VoteForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Load += new System.EventHandler(this.VoteForm_Load);
			this.SpamgroupBox.ResumeLayout(false);
			this.SpamgroupBox.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label Filelabel;
        private System.Windows.Forms.Label Loginlabel;
        private System.Windows.Forms.TextBox TopictextBox;
        private System.Windows.Forms.GroupBox SpamgroupBox;
        private System.Windows.Forms.Button Gobutton;
        private System.Windows.Forms.ComboBox GroupcomboBox;
    }
}

