namespace eRepConsoleManagementSystem
{
    partial class Comment_Form
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Comment_Form));
			this.IdtextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.MessagetextBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.GroupcomboBox = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.Gobutton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// IdtextBox
			// 
			resources.ApplyResources(this.IdtextBox, "IdtextBox");
			this.IdtextBox.Name = "IdtextBox";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// MessagetextBox
			// 
			resources.ApplyResources(this.MessagetextBox, "MessagetextBox");
			this.MessagetextBox.Name = "MessagetextBox";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// GroupcomboBox
			// 
			this.GroupcomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.GroupcomboBox.FormattingEnabled = true;
			resources.ApplyResources(this.GroupcomboBox, "GroupcomboBox");
			this.GroupcomboBox.Name = "GroupcomboBox";
			this.GroupcomboBox.Sorted = true;
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// Gobutton
			// 
			resources.ApplyResources(this.Gobutton, "Gobutton");
			this.Gobutton.Name = "Gobutton";
			this.Gobutton.UseVisualStyleBackColor = true;
			this.Gobutton.Click += new System.EventHandler(this.Gobutton_Click);
			// 
			// Comment_Form
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.Gobutton);
			this.Controls.Add(this.GroupcomboBox);
			this.Controls.Add(this.MessagetextBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.IdtextBox);
			this.Name = "Comment_Form";
			this.Load += new System.EventHandler(this.Comment_Form_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox IdtextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox MessagetextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox GroupcomboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Gobutton;
    }
}