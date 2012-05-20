namespace eRepConsoleManagementSystem
{
    partial class FeedForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FeedForm));
            this.HealthtrackBar = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.Healthlabel = new System.Windows.Forms.Label();
            this.HealthtextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Startbutton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.GroupcomboBox = new System.Windows.Forms.ComboBox();
            this.chkJustEat = new System.Windows.Forms.CheckBox();
            this.LesstextBox = new System.Windows.Forms.TextBox();
            this.FastFoodcheckBox = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.HungryFirstcheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.HealthtrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // HealthtrackBar
            // 
            this.HealthtrackBar.LargeChange = 1;
            resources.ApplyResources(this.HealthtrackBar, "HealthtrackBar");
            this.HealthtrackBar.Maximum = 5;
            this.HealthtrackBar.Minimum = 1;
            this.HealthtrackBar.Name = "HealthtrackBar";
            this.HealthtrackBar.Value = 1;
            this.HealthtrackBar.Scroll += new System.EventHandler(this.HealthtrackBar_Scroll);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // Healthlabel
            // 
            resources.ApplyResources(this.Healthlabel, "Healthlabel");
            this.Healthlabel.Name = "Healthlabel";
            // 
            // HealthtextBox
            // 
            resources.ApplyResources(this.HealthtextBox, "HealthtextBox");
            this.HealthtextBox.Name = "HealthtextBox";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // Startbutton
            // 
            resources.ApplyResources(this.Startbutton, "Startbutton");
            this.Startbutton.Name = "Startbutton";
            this.Startbutton.UseVisualStyleBackColor = true;
            this.Startbutton.Click += new System.EventHandler(this.Startbutton_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // GroupcomboBox
            // 
            this.GroupcomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GroupcomboBox.FormattingEnabled = true;
            resources.ApplyResources(this.GroupcomboBox, "GroupcomboBox");
            this.GroupcomboBox.Name = "GroupcomboBox";
            this.GroupcomboBox.Sorted = true;
            // 
            // chkJustEat
            // 
            resources.ApplyResources(this.chkJustEat, "chkJustEat");
            this.chkJustEat.Name = "chkJustEat";
            this.chkJustEat.UseVisualStyleBackColor = true;
            // 
            // LesstextBox
            // 
            resources.ApplyResources(this.LesstextBox, "LesstextBox");
            this.LesstextBox.Name = "LesstextBox";
            // 
            // FastFoodcheckBox
            // 
            resources.ApplyResources(this.FastFoodcheckBox, "FastFoodcheckBox");
            this.FastFoodcheckBox.Name = "FastFoodcheckBox";
            this.FastFoodcheckBox.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // HungryFirstcheckBox
            // 
            resources.ApplyResources(this.HungryFirstcheckBox, "HungryFirstcheckBox");
            this.HungryFirstcheckBox.Name = "HungryFirstcheckBox";
            this.HungryFirstcheckBox.UseVisualStyleBackColor = true;
            // 
            // FeedForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.HungryFirstcheckBox);
            this.Controls.Add(this.FastFoodcheckBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.LesstextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkJustEat);
            this.Controls.Add(this.GroupcomboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Startbutton);
            this.Controls.Add(this.HealthtextBox);
            this.Controls.Add(this.Healthlabel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.HealthtrackBar);
            this.Name = "FeedForm";
            this.Load += new System.EventHandler(this.FeedForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.HealthtrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar HealthtrackBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label Healthlabel;
        private System.Windows.Forms.TextBox HealthtextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button Startbutton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox GroupcomboBox;
        private System.Windows.Forms.CheckBox chkJustEat;
        private System.Windows.Forms.TextBox LesstextBox;
        private System.Windows.Forms.CheckBox FastFoodcheckBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox HungryFirstcheckBox;
    }
}