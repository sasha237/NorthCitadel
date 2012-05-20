namespace PalBot {
	partial class MainForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			this.PaneCaptcha = new System.Windows.Forms.Panel();
			this.CaptchasCount = new System.Windows.Forms.Label();
			this.CaptchaText = new System.Windows.Forms.TextBox();
			this.CaptchaImage = new System.Windows.Forms.PictureBox();
			this.PBTotal = new System.Windows.Forms.ProgressBar();
			this.PBBot = new System.Windows.Forms.ProgressBar();
			this.LbStatus = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			this.PaneCaptcha.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.CaptchaImage)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.Dock = System.Windows.Forms.DockStyle.Top;
			label1.Location = new System.Drawing.Point(0, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(460, 13);
			label1.TabIndex = 1;
			label1.Text = "Общий прогресс";
			// 
			// label2
			// 
			label2.Dock = System.Windows.Forms.DockStyle.Top;
			label2.Location = new System.Drawing.Point(0, 36);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(460, 13);
			label2.TabIndex = 1;
			label2.Text = "Прогресс бота";
			// 
			// PaneCaptcha
			// 
			this.PaneCaptcha.Controls.Add(this.CaptchasCount);
			this.PaneCaptcha.Controls.Add(this.CaptchaText);
			this.PaneCaptcha.Controls.Add(this.CaptchaImage);
			this.PaneCaptcha.Dock = System.Windows.Forms.DockStyle.Top;
			this.PaneCaptcha.Location = new System.Drawing.Point(0, 101);
			this.PaneCaptcha.Name = "PaneCaptcha";
			this.PaneCaptcha.Size = new System.Drawing.Size(460, 83);
			this.PaneCaptcha.TabIndex = 3;
			// 
			// CaptchasCount
			// 
			this.CaptchasCount.Dock = System.Windows.Forms.DockStyle.Right;
			this.CaptchasCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.CaptchasCount.Location = new System.Drawing.Point(400, 0);
			this.CaptchasCount.Name = "CaptchasCount";
			this.CaptchasCount.Size = new System.Drawing.Size(60, 83);
			this.CaptchasCount.TabIndex = 2;
			this.CaptchasCount.Text = "0";
			this.CaptchasCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// CaptchaText
			// 
			this.CaptchaText.Location = new System.Drawing.Point(3, 56);
			this.CaptchaText.Name = "CaptchaText";
			this.CaptchaText.Size = new System.Drawing.Size(315, 20);
			this.CaptchaText.TabIndex = 1;
			this.CaptchaText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CaptchaText_KeyDown);
			// 
			// CaptchaImage
			// 
			this.CaptchaImage.Location = new System.Drawing.Point(3, 3);
			this.CaptchaImage.Name = "CaptchaImage";
			this.CaptchaImage.Size = new System.Drawing.Size(315, 50);
			this.CaptchaImage.TabIndex = 0;
			this.CaptchaImage.TabStop = false;
			// 
			// PBTotal
			// 
			this.PBTotal.Dock = System.Windows.Forms.DockStyle.Top;
			this.PBTotal.Location = new System.Drawing.Point(0, 13);
			this.PBTotal.Maximum = 10000;
			this.PBTotal.Name = "PBTotal";
			this.PBTotal.Size = new System.Drawing.Size(460, 23);
			this.PBTotal.TabIndex = 0;
			// 
			// PBBot
			// 
			this.PBBot.Dock = System.Windows.Forms.DockStyle.Top;
			this.PBBot.Location = new System.Drawing.Point(0, 49);
			this.PBBot.Maximum = 10000;
			this.PBBot.Name = "PBBot";
			this.PBBot.Size = new System.Drawing.Size(460, 23);
			this.PBBot.TabIndex = 0;
			// 
			// LbStatus
			// 
			this.LbStatus.Dock = System.Windows.Forms.DockStyle.Top;
			this.LbStatus.Location = new System.Drawing.Point(0, 72);
			this.LbStatus.Name = "LbStatus";
			this.LbStatus.Size = new System.Drawing.Size(460, 29);
			this.LbStatus.TabIndex = 2;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(460, 246);
			this.Controls.Add(this.PaneCaptcha);
			this.Controls.Add(this.LbStatus);
			this.Controls.Add(this.PBBot);
			this.Controls.Add(label2);
			this.Controls.Add(this.PBTotal);
			this.Controls.Add(label1);
			this.Name = "MainForm";
			this.Text = "Веселая ферма";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.PaneCaptcha.ResumeLayout(false);
			this.PaneCaptcha.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.CaptchaImage)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ProgressBar PBTotal;
		private System.Windows.Forms.ProgressBar PBBot;
		private System.Windows.Forms.Label LbStatus;
		private System.Windows.Forms.PictureBox CaptchaImage;
		private System.Windows.Forms.Panel PaneCaptcha;
		private System.Windows.Forms.TextBox CaptchaText;
		private System.Windows.Forms.Label CaptchasCount;

	}
}

