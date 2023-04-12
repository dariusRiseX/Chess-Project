namespace Chess
{
    partial class Options
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Options));
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.POneNameInput = new System.Windows.Forms.TextBox();
            this.PTwoNameInput = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.FenInput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.TimeInput = new System.Windows.Forms.TextBox();
            this.ApplyButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.NTCcheckbox = new System.Windows.Forms.CheckBox();
            this.SSNcheckbox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackgroundImage = global::Chess.Properties.Resources.WKing;
            this.pictureBox2.InitialImage = global::Chess.Properties.Resources.BKing;
            this.pictureBox2.Location = new System.Drawing.Point(57, 99);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(66, 72);
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            // 
            // POneNameInput
            // 
            this.POneNameInput.Location = new System.Drawing.Point(155, 127);
            this.POneNameInput.Name = "POneNameInput";
            this.POneNameInput.Size = new System.Drawing.Size(100, 20);
            this.POneNameInput.TabIndex = 6;
            this.POneNameInput.Text = "Player 1";
            // 
            // PTwoNameInput
            // 
            this.PTwoNameInput.Location = new System.Drawing.Point(441, 127);
            this.PTwoNameInput.Name = "PTwoNameInput";
            this.PTwoNameInput.Size = new System.Drawing.Size(100, 20);
            this.PTwoNameInput.TabIndex = 7;
            this.PTwoNameInput.Text = "Player 2";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::Chess.Properties.Resources.BKing;
            this.pictureBox1.InitialImage = global::Chess.Properties.Resources.BKing;
            this.pictureBox1.Location = new System.Drawing.Point(345, 99);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(66, 72);
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // FenInput
            // 
            this.FenInput.Location = new System.Drawing.Point(155, 432);
            this.FenInput.Name = "FenInput";
            this.FenInput.Size = new System.Drawing.Size(365, 20);
            this.FenInput.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS Reference Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(211, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 24);
            this.label1.TabIndex = 10;
            this.label1.Text = "Player Names";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(83, 435);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "FEN Input:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(362, 301);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "TimeControl (s):\r\n";
            // 
            // TimeInput
            // 
            this.TimeInput.Location = new System.Drawing.Point(477, 298);
            this.TimeInput.Name = "TimeInput";
            this.TimeInput.Size = new System.Drawing.Size(43, 20);
            this.TimeInput.TabIndex = 13;
            this.TimeInput.Text = "120";
            // 
            // ApplyButton
            // 
            this.ApplyButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.ApplyButton.Location = new System.Drawing.Point(206, 494);
            this.ApplyButton.Name = "ApplyButton";
            this.ApplyButton.Size = new System.Drawing.Size(189, 55);
            this.ApplyButton.TabIndex = 15;
            this.ApplyButton.Text = "Apply And Close";
            this.ApplyButton.UseVisualStyleBackColor = false;
            this.ApplyButton.Click += new System.EventHandler(this.ApplyButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("MS Reference Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(211, 221);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(146, 24);
            this.label4.TabIndex = 16;
            this.label4.Text = "Time Control";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("MS Reference Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(226, 367);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(122, 24);
            this.label5.TabIndex = 17;
            this.label5.Text = "FEN String\r\n";
            // 
            // NTCcheckbox
            // 
            this.NTCcheckbox.AutoSize = true;
            this.NTCcheckbox.Location = new System.Drawing.Point(206, 298);
            this.NTCcheckbox.Name = "NTCcheckbox";
            this.NTCcheckbox.Size = new System.Drawing.Size(102, 17);
            this.NTCcheckbox.TabIndex = 19;
            this.NTCcheckbox.Text = "No Time Control";
            this.NTCcheckbox.UseVisualStyleBackColor = true;
            // 
            // SSNcheckbox
            // 
            this.SSNcheckbox.AutoSize = true;
            this.SSNcheckbox.Location = new System.Drawing.Point(43, 297);
            this.SSNcheckbox.Name = "SSNcheckbox";
            this.SSNcheckbox.Size = new System.Drawing.Size(135, 17);
            this.SSNcheckbox.TabIndex = 20;
            this.SSNcheckbox.Text = "Show Square Numbers\r\n";
            this.SSNcheckbox.UseVisualStyleBackColor = true;
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(584, 561);
            this.Controls.Add(this.SSNcheckbox);
            this.Controls.Add(this.NTCcheckbox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ApplyButton);
            this.Controls.Add(this.TimeInput);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FenInput);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.PTwoNameInput);
            this.Controls.Add(this.POneNameInput);
            this.Controls.Add(this.pictureBox2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Options";
            this.Text = "Options";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TextBox POneNameInput;
        private System.Windows.Forms.TextBox PTwoNameInput;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox FenInput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TimeInput;
        private System.Windows.Forms.Button ApplyButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox NTCcheckbox;
        private System.Windows.Forms.CheckBox SSNcheckbox;
    }
}