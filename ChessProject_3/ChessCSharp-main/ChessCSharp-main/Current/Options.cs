using System;
using System.Windows.Forms;

namespace Chess
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        public void ApplyButton_Click(object sender, EventArgs e)
        {
            string Player1Name = POneNameInput.Text;
            string Player2Name = PTwoNameInput.Text;
            string FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

            int TimeControl = 120;

            if (NTCcheckbox.Checked)
            {
                TimeControl = -1;
            } 
            else
            {
                try // check if user input is valid
                {
                    TimeControl = Convert.ToInt32(TimeInput.Text);
                }
                catch
                {
                    TimeControl = 120; // set time to 2 mins each if input isnt valid
                }
            }

            if (SSNcheckbox.Checked)
            {
                GameWindow.ShowBoardSquareNumbers = true;
            }

            if (FenInput.Text != "")
            {
                FEN = FenInput.Text;
            }

            GameWindow.TimeControl = TimeControl;
            GameWindow.Player1Name = Player1Name;
            GameWindow.Player2Name = Player2Name;
            FenStringUtility.InputedPostion = FEN;

            this.Close();
        }
    }
}
