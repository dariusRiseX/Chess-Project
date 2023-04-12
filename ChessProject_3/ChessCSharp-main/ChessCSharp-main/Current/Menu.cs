using System;
using System.Windows.Forms;

namespace Chess
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }
        private void Menu_Load(object sender, EventArgs e)
        {

        }

        private void TwoPlayer_Click(object sender, EventArgs e)
        {
            var TwoPlayer = new GameWindow(false);
            TwoPlayer.Show();
        }

        private void SinglePlayer_Click(object sender, EventArgs e)
        {
            var SinglePlayer = new GameWindow(true);
            SinglePlayer.Show();
        }

        private void OptionsButton_Click(object sender, EventArgs e)
        {
            var OptionsWindow = new Options();
            OptionsWindow.Show();
        }
    }
}
