namespace Chess
{
    partial class GameWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameWindow));
            WhiteTimeDisplay = new System.Windows.Forms.Label();
            BlackTimeDisplay = new System.Windows.Forms.Label();
            this.P1Name = new System.Windows.Forms.Label();
            this.P2Name = new System.Windows.Forms.Label();
            this.MoveStackDisplay = new System.Windows.Forms.ListView();
            this.WhiteMoves = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.BlackMoves = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ResignButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // WhiteTimeDisplay
            // 
           WhiteTimeDisplay.AutoSize = true;
           WhiteTimeDisplay.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
           WhiteTimeDisplay.Location = new System.Drawing.Point(580, 55);
           WhiteTimeDisplay.Name = "WhiteTimeDisplay";
           WhiteTimeDisplay.Size = new System.Drawing.Size(22, 31);
           WhiteTimeDisplay.TabIndex = 0;
           WhiteTimeDisplay.Text = ".";
            // 
            // BlackTimeDisplay
            // 
            BlackTimeDisplay.AutoSize = true;
            BlackTimeDisplay.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            BlackTimeDisplay.Location = new System.Drawing.Point(580, 755);
            BlackTimeDisplay.Name = "BlackTimeDisplay";
            BlackTimeDisplay.Size = new System.Drawing.Size(22, 31);
            BlackTimeDisplay.TabIndex = 1;
            BlackTimeDisplay.Text = ".";
            // 
            // P1Name
            // 
            this.P1Name.AutoSize = true;
            this.P1Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.P1Name.Location = new System.Drawing.Point(20, 755);
            this.P1Name.Name = "P1Name";
            this.P1Name.Size = new System.Drawing.Size(113, 31);
            this.P1Name.TabIndex = 4;
            this.P1Name.Text = "Player 1";
            // 
            // P2Name
            // 
            this.P2Name.AutoSize = true;
            this.P2Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.P2Name.Location = new System.Drawing.Point(20, 55);
            this.P2Name.Name = "P2Name";
            this.P2Name.Size = new System.Drawing.Size(113, 31);
            this.P2Name.TabIndex = 5;
            this.P2Name.Text = "Player 2";
            // 
            // MoveStackDisplay
            // 
            this.MoveStackDisplay.BackColor = System.Drawing.SystemColors.MenuBar;
            this.MoveStackDisplay.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.WhiteMoves,
            this.BlackMoves});
            this.MoveStackDisplay.HideSelection = false;
            this.MoveStackDisplay.Location = new System.Drawing.Point(672, 100);
            this.MoveStackDisplay.Name = "MoveStackDisplay";
            this.MoveStackDisplay.Size = new System.Drawing.Size(300, 640);
            this.MoveStackDisplay.TabIndex = 6;
            this.MoveStackDisplay.UseCompatibleStateImageBehavior = false;
            this.MoveStackDisplay.View = System.Windows.Forms.View.Details;
            // 
            // WhiteMoves
            // 
            this.WhiteMoves.Text = "White Moves";
            this.WhiteMoves.Width = 150;
            // 
            // BlackMoves
            // 
            this.BlackMoves.Text = "Black Moves";
            this.BlackMoves.Width = 150;
            // 
            // ResignButton
            // 
            this.ResignButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ResignButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResignButton.ForeColor = System.Drawing.Color.Black;
            this.ResignButton.Location = new System.Drawing.Point(784, 753);
            this.ResignButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ResignButton.Name = "ResignButton";
            this.ResignButton.Size = new System.Drawing.Size(102, 48);
            this.ResignButton.TabIndex = 7;
            this.ResignButton.Text = "Resign";
            this.ResignButton.UseVisualStyleBackColor = false;
            this.ResignButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // GameWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(983, 853);
            this.Controls.Add(this.ResignButton);
            this.Controls.Add(this.MoveStackDisplay);
            this.Controls.Add(this.P2Name);
            this.Controls.Add(this.P1Name);
            this.Controls.Add(BlackTimeDisplay);
            this.Controls.Add(WhiteTimeDisplay);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(999, 899);
            this.MinimumSize = new System.Drawing.Size(999, 834);
            this.Name = "GameWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chess";
            this.Load += new System.EventHandler(this.GameWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public static System.Windows.Forms.Label WhiteTimeDisplay;
        public static System.Windows.Forms.Label BlackTimeDisplay;
        private System.Windows.Forms.Label P1Name;
        private System.Windows.Forms.Label P2Name;
        private System.Windows.Forms.ListView MoveStackDisplay;
        private System.Windows.Forms.ColumnHeader WhiteMoves;
        private System.Windows.Forms.ColumnHeader BlackMoves;
        private System.Windows.Forms.Button ResignButton;
    }
}