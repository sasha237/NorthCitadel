namespace eRepConsoleManagementSystem
{
    partial class FightForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FightForm));
            this.BattleIdtextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Gobutton = new System.Windows.Forms.Button();
            this.GroupcomboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.BuyItemtextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.CountryIdtextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.LeftHPtextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.DoNotChangeWeaponcheckBox = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.ShotLimittextBox = new System.Windows.Forms.TextBox();
            this.CyclicFight = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.ExpLimittextBox = new System.Windows.Forms.TextBox();
            this.chkBuyWeapon = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // BattleIdtextBox
            // 
            resources.ApplyResources(this.BattleIdtextBox, "BattleIdtextBox");
            this.BattleIdtextBox.Name = "BattleIdtextBox";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // Gobutton
            // 
            resources.ApplyResources(this.Gobutton, "Gobutton");
            this.Gobutton.Name = "Gobutton";
            this.Gobutton.UseVisualStyleBackColor = true;
            this.Gobutton.Click += new System.EventHandler(this.Gobutton_Click);
            // 
            // GroupcomboBox
            // 
            resources.ApplyResources(this.GroupcomboBox, "GroupcomboBox");
            this.GroupcomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GroupcomboBox.FormattingEnabled = true;
            this.GroupcomboBox.Name = "GroupcomboBox";
            this.GroupcomboBox.Sorted = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // BuyItemtextBox
            // 
            resources.ApplyResources(this.BuyItemtextBox, "BuyItemtextBox");
            this.BuyItemtextBox.Name = "BuyItemtextBox";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // CountryIdtextBox
            // 
            resources.ApplyResources(this.CountryIdtextBox, "CountryIdtextBox");
            this.CountryIdtextBox.Name = "CountryIdtextBox";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // LeftHPtextBox
            // 
            resources.ApplyResources(this.LeftHPtextBox, "LeftHPtextBox");
            this.LeftHPtextBox.Name = "LeftHPtextBox";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // DoNotChangeWeaponcheckBox
            // 
            resources.ApplyResources(this.DoNotChangeWeaponcheckBox, "DoNotChangeWeaponcheckBox");
            this.DoNotChangeWeaponcheckBox.Checked = true;
            this.DoNotChangeWeaponcheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DoNotChangeWeaponcheckBox.Name = "DoNotChangeWeaponcheckBox";
            this.DoNotChangeWeaponcheckBox.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // ShotLimittextBox
            // 
            resources.ApplyResources(this.ShotLimittextBox, "ShotLimittextBox");
            this.ShotLimittextBox.Name = "ShotLimittextBox";
            // 
            // CyclicFight
            // 
            resources.ApplyResources(this.CyclicFight, "CyclicFight");
            this.CyclicFight.Name = "CyclicFight";
            this.CyclicFight.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // ExpLimittextBox
            // 
            resources.ApplyResources(this.ExpLimittextBox, "ExpLimittextBox");
            this.ExpLimittextBox.Name = "ExpLimittextBox";
            // 
            // chkBuyWeapon
            // 
            resources.ApplyResources(this.chkBuyWeapon, "chkBuyWeapon");
            this.chkBuyWeapon.Name = "chkBuyWeapon";
            this.chkBuyWeapon.UseVisualStyleBackColor = true;
            // 
            // FightForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkBuyWeapon);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.ExpLimittextBox);
            this.Controls.Add(this.CyclicFight);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.ShotLimittextBox);
            this.Controls.Add(this.DoNotChangeWeaponcheckBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.LeftHPtextBox);
            this.Controls.Add(this.BuyItemtextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.CountryIdtextBox);
            this.Controls.Add(this.GroupcomboBox);
            this.Controls.Add(this.Gobutton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BattleIdtextBox);
            this.Name = "FightForm";
            this.Load += new System.EventHandler(this.WarForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox BattleIdtextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Gobutton;
        private System.Windows.Forms.ComboBox GroupcomboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox BuyItemtextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox CountryIdtextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox LeftHPtextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox DoNotChangeWeaponcheckBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox ShotLimittextBox;
        private System.Windows.Forms.CheckBox CyclicFight;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox ExpLimittextBox;
        private System.Windows.Forms.CheckBox chkBuyWeapon;
    }
}