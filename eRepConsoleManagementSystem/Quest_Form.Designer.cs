namespace eRepConsoleManagementSystem
{
    partial class Quest_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Quest_Form));
            this.GroupcomboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Startbutton = new System.Windows.Forms.Button();
            this.CBAll = new System.Windows.Forms.CheckBox();
            this.cbBuyWeapons = new System.Windows.Forms.CheckBox();
            this.cbBuyFood = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
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
            // Startbutton
            // 
            resources.ApplyResources(this.Startbutton, "Startbutton");
            this.Startbutton.Name = "Startbutton";
            this.Startbutton.UseVisualStyleBackColor = true;
            this.Startbutton.Click += new System.EventHandler(this.Startbutton_Click);
            // 
            // CBAll
            // 
            resources.ApplyResources(this.CBAll, "CBAll");
            this.CBAll.Name = "CBAll";
            this.CBAll.UseVisualStyleBackColor = true;
            // 
            // cbBuyWeapons
            // 
            resources.ApplyResources(this.cbBuyWeapons, "cbBuyWeapons");
            this.cbBuyWeapons.Name = "cbBuyWeapons";
            this.cbBuyWeapons.UseVisualStyleBackColor = true;
            // 
            // cbBuyFood
            // 
            resources.ApplyResources(this.cbBuyFood, "cbBuyFood");
            this.cbBuyFood.Name = "cbBuyFood";
            this.cbBuyFood.UseVisualStyleBackColor = true;
            // 
            // Quest_Form
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbBuyFood);
            this.Controls.Add(this.cbBuyWeapons);
            this.Controls.Add(this.CBAll);
            this.Controls.Add(this.GroupcomboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Startbutton);
            this.Name = "Quest_Form";
            this.Load += new System.EventHandler(this.Quest_Form_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox GroupcomboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Startbutton;
		private System.Windows.Forms.CheckBox CBAll;
		private System.Windows.Forms.CheckBox cbBuyWeapons;
        private System.Windows.Forms.CheckBox cbBuyFood;
    }
}