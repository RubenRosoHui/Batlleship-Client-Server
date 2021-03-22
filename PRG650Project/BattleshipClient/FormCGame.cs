using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using BattleshipServer2;

namespace BattleshipClient
{
    public partial class FormCGame : Form
    {
        //create delegates for setting text in the form
        delegate void SetTextForm(string text);
        delegate void SetTextShips(string text);
        
        //Creating variables to be used for Client to Server program 
        string hostName = "localhost";
        TcpClient client;
        NetworkStream ns;
        private int portNum = 4545;
        Thread t = null, t2 = null;

        //variables used for the game
        public string box;
        public string Username;
        int shipsLeft=3,ServerShipsLeft=3,size;
        string ship1, ship2, ship3;
        string ServerShip1, ServerShip2, ServerShip3;
        string ServerMove;
        //used for the possible coordinates
        public static List<string> coords = new List<string>()
            {
                "A1","A2","A3","B1","B2","B3","C1","C2","C3"
            };
        public FormCGame()
        {
            InitializeComponent();
            //setting the link between the client and server
            client = new TcpClient(hostName, portNum);
            ns = client.GetStream();
            byte[] bytes = new byte[1024];

            //creating and starting threads
            t = new Thread(new ThreadStart(ServersShips));
            t2 = new Thread(new ThreadStart(Ships));
            t.Start();
            t2.Start();
        }
        
        //Used to show the board according to user input
        public void Board() 
        {
            if (box == "3x3")
            {
                //User board hides ones for a 3x3 game board
                btnA4.Hide();
                btnA5.Hide();
                btnB4.Hide();
                btnB5.Hide();
                btnC4.Hide();
                btnC5.Hide();
                btnD1.Hide();
                btnD2.Hide();
                btnD3.Hide();
                btnD4.Hide();
                btnD5.Hide();
                btnE1.Hide();
                btnE2.Hide();
                btnE3.Hide();
                btnE4.Hide();
                btnE5.Hide();
                //Computer board hides ones for a 3x3 game board
                btnSA4.Hide();
                btnSA5.Hide();
                btnSB4.Hide();
                btnSB5.Hide();
                btnSC4.Hide();
                btnSC5.Hide();
                btnSD1.Hide();
                btnSD2.Hide();
                btnSD3.Hide();
                btnSD4.Hide();
                btnSD5.Hide();
                btnSE1.Hide();
                btnSE2.Hide();
                btnSE3.Hide();
                btnSE4.Hide();
                btnSE5.Hide();
                //set the number of possible coordinates to 9
                size = 9;
            }
            else if (box == "4x4")
            {
                //User board hides ones for a 4x4 game board
                btnA5.Hide();
                btnB5.Hide();
                btnC5.Hide();
                btnD5.Hide();
                btnE1.Hide();
                btnE2.Hide();
                btnE3.Hide();
                btnE4.Hide();
                btnE5.Hide();
                //Computer board hides ones for a 4x4 game board
                btnSA5.Hide();
                btnSB5.Hide();
                btnSC5.Hide();
                btnSD5.Hide();
                btnSE1.Hide();
                btnSE2.Hide();
                btnSE3.Hide();
                btnSE4.Hide();
                btnSE5.Hide();
                //set the number of possible coordinates to 16
                size = 16;
            }
            else 
            {
                //set the number of possible coordinates to 25
                size = 25;
            }
            if (size == 16)
            {
                //add possible coordinates for the game board
                string[] size4 = new string[] { "A4", "B4", "C4", "D1", "D2", "D3", "D4" };
                coords.AddRange(size4);
            }
            else if (size == 25)
            {
                //add possible coordinates for the game board
                string[] size5 = new string[] { "A4", "A5", "B4", "B5", "C4", "C5", "D1", "D2", "D3", "D4", "D5", "E1", "E2", "E3", "E4", "E5" };
                coords.AddRange(size5);
            }
        }
        
        //Used to get user input of ship location and set up the game for the User
        private void Ships()
        {
            SetText(Username);
            sendBox();

            //open a new form for input of the ship Coordinates
            using (var Ships = new Ships())
            {
                Ships.ShowDialog();
                //set the Client's ships 
                ship1 = Ships.ship1;
                ship2 = Ships.ship2;
                ship3 = Ships.ship3;
            }
            //Prints out the Ship coordinates of the client
            string ships = ship1 + " " + ship2 + " " + ship3;
            SetShips(ships);
        }
        
        //send the size of the game to the Server
        private void sendBox()
        {
            //send the box size and the username to the Server
            string s = box + " " + Username;
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            ns.Write(bytes, 0, bytes.Length);
        }
        
        //Get and set the ships from the server
        private void ServersShips()
        {
            byte[] bytes = new byte[1024];
            while (true)
            {
                try
                {
                    if (ns.DataAvailable)
                    {
                        IFormatter formatter = new BinaryFormatter();
                        ServerShips serverShips = (ServerShips)formatter.Deserialize(ns);
                        ServerShip1 = serverShips.returnShip1;
                        ServerShip2 = serverShips.returnShip2;
                        ServerShip3 = serverShips.returnShip3;
                    }
                    else
                    {
                        continue;
                    }
                }
                catch (System.IO.IOException ex)
                {
                    break;
                    throw ex;
                }
            }
        }
        
        //Set the User's ships coordinates to a label
        private void SetShips(string text)
        {
            if (this.lblTest.InvokeRequired)
            {
                SetTextShips del1 = new SetTextShips(SetShips);
                try
                {
                    this.Invoke(del1, new object[] { text });
                }
                catch (System.ObjectDisposedException)
                {
                    //this.Close();
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.Close();
                    });
                }
            }
            else
            {
                lblTest.Text = "Coordinates: " + text;
            }
        }
        
        //Set client's name on the form
        private void SetText(string text)
        {
            if (this.InvokeRequired)
            {
                SetTextForm del = new SetTextForm(SetText);
                try
                {
                    this.Invoke(del, new object[] { text });
                }
                catch (System.ObjectDisposedException)
                {
                    //this.Close();
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.Close();
                    });
                }
            }
            else
            {
                lblUser.Text = text;
                Text = $"{text}'s Battleship Game";
                Board();
            }
        }
        
        //ai choose random coordinate for next move
        private string randomShips()
        {
            string ship;
            Random rnd = new Random();
            ship = coords[rnd.Next(coords.Count)];
            if (coords.Contains(ship)) {coords.Remove(ship);}
            return ship;
        }
       
        //enemy check 
        private void check() 
        {
            //create a list of the button controls on the form
            List<Control> buttonControls = new List<Control>();
            foreach (var ctrl in this.Controls)
            {
                //check if the control is a button
                if (ctrl.GetType() == typeof(System.Windows.Forms.Button))
                {
                    buttonControls.Add(ctrl as Control);
                }
            }

            string compareValue = $"btn{txtEnemyMove.Text}";
            //check for the selected button 
            Control selectedButton = buttonControls.Find(b => b.Name == compareValue);
            if (selectedButton != null &&(txtEnemyMove.Text == ship1 || txtEnemyMove.Text == ship2 || txtEnemyMove.Text == ship3))
            {
                //change the color to when hit
                if (selectedButton.BackColor != Color.Red)
                {
                    selectedButton.BackColor = Color.Red;
                    selectedButton.Text = "Ship Hit";
                    lblServerCheck.Text = "Hit";
                    shipsLeft--;
                }
            }
            //change the color when miss
            else 
            {
                selectedButton.BackColor = Color.Green;
                selectedButton.Text = "Miss";
                lblServerCheck.Text = "Miss";
            }         
        }
        //User check
        private void CheckUserMove() 
        {
            //create a list of the button controls on the form
            List<Control> buttonControls = new List<Control>();
            foreach (var ctrl in this.Controls)
            {
                //check if the control is a button
                if (ctrl.GetType() == typeof(System.Windows.Forms.Button))
                {
                    buttonControls.Add(ctrl as Control);
                }
            }
            string compareValue = $"btnS{txtMove.Text}";
            //check for the selected button 
            Control selectedButton = buttonControls.Find(b => b.Name == compareValue);
            if (selectedButton != null && (txtMove.Text == ServerShip1 || txtMove.Text == ServerShip2 || txtMove.Text == ServerShip3))
            {
                //change the color to when hit
                if (selectedButton.BackColor != Color.Red)
                {
                    selectedButton.BackColor = Color.Red;
                    selectedButton.Text = "Ship Hit";
                    lblClientCheck.Text = "Hit";
                    ServerShipsLeft--;
                }
            }
            //change the color when miss
            else
            {
                selectedButton.BackColor = Color.Green;
                selectedButton.Text = "Miss";
                lblClientCheck.Text = "Miss";
            }
        }
        
        //End of the game
        private void EndGame() 
        {

            ServerShips ships = new ServerShips(true);
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ns, ships);
        }
       
        private void btnA1_Click(object sender, EventArgs e)
        {
            txtMove.Text = "A1";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if(ServerShipsLeft ==0 | shipsLeft ==0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK,MessageBoxIcon.None);}
                t.Abort();
                EndGame();
                Application.Exit();
            }

        }
        
        private void btnA2_Click(object sender, EventArgs e)
        {
            txtMove.Text = "A2";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if(ServerShipsLeft ==0 | shipsLeft ==0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }
        
        private void btnA3_Click(object sender, EventArgs e)
        {
            txtMove.Text = "A3";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }

        private void btnA4_Click(object sender, EventArgs e)
        {
            txtMove.Text = "A4";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }

        private void btnA5_Click(object sender, EventArgs e)
        {
            txtMove.Text = "A5";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }

        private void btnB1_Click(object sender, EventArgs e)
        {
            txtMove.Text = "B1";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }

        private void btnB2_Click(object sender, EventArgs e)
        {
            txtMove.Text = "B2";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }

        private void btnB3_Click(object sender, EventArgs e)
        {
            txtMove.Text = "B3";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }

        private void btnB4_Click(object sender, EventArgs e)
        {
            txtMove.Text = "B4";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }

        private void btnB5_Click(object sender, EventArgs e)
        {
            txtMove.Text = "B5";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }

        private void btnC1_Click(object sender, EventArgs e)
        {
            txtMove.Text = "C1";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }

        private void btnC2_Click(object sender, EventArgs e)
        {
            txtMove.Text = "C2";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }

        private void btnC3_Click(object sender, EventArgs e)
        {
            txtMove.Text = "C3";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }

        private void btnC4_Click(object sender, EventArgs e)
        {
            txtMove.Text = "C4";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }

        private void btnC5_Click(object sender, EventArgs e)
        {
            txtMove.Text = "C5";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }

        private void btnD1_Click(object sender, EventArgs e)
        {
            txtMove.Text = "D1";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }

        private void btnD2_Click(object sender, EventArgs e)
        {
            txtMove.Text = "D2";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }

        private void btnD3_Click(object sender, EventArgs e)
        {
            txtMove.Text = "D3";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }

        private void btnD4_Click(object sender, EventArgs e)
        {
            txtMove.Text = "D4";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }

        private void btnD5_Click(object sender, EventArgs e)
        {
            txtMove.Text = "D5";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }

        private void btnE1_Click(object sender, EventArgs e)
        {
            txtMove.Text = "E1";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }

        private void btnE2_Click(object sender, EventArgs e)
        {
            txtMove.Text = "E2";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }

        private void btnE3_Click(object sender, EventArgs e)
        {
            txtMove.Text = "E3";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }

        private void btnE4_Click(object sender, EventArgs e)
        {
            txtMove.Text = "E4";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }

        private void btnE5_Click(object sender, EventArgs e)
        {
            txtMove.Text = "E5";
            CheckUserMove();
            ServerMove = randomShips();
            txtEnemyMove.Text = ServerMove;
            check();
            //what happens when the game ends
            if (ServerShipsLeft == 0 | shipsLeft == 0)
            {
                if (ServerShipsLeft == 0) { MessageBox.Show("You Won", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                else if (shipsLeft == 0) { MessageBox.Show("You Lost", "Game End", MessageBoxButtons.OK, MessageBoxIcon.None); }
                t.Abort();
                EndGame();
                Application.Exit();
            }
        }
    }
}