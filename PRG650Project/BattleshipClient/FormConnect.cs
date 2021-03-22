using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleshipClient
{
    public partial class FormConnect : Form
    {
        public FormConnect()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (cmbBoard.Text == "3x3" | cmbBoard.Text == "4x4" | cmbBoard.Text == "5x5")
            {
                FormCGame game = new FormCGame();
                game.FormClosed += new FormClosedEventHandler(game_FormClosed);
                game.Username = txtName.Text;
                game.box = cmbBoard.Text;
                game.Show();
                this.Hide();
            }
        }
        private void game_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
        }
    }
}
