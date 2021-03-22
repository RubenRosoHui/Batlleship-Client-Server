using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BattleshipServer2
{
    public partial class FormSGame : Form
    {
        delegate void SetTextCallback(string text);
        //Listens for connections from TCP network clients.
        TcpListener listener;
        //Provides client connections for TCP network services.
        TcpClient client;
        //Provides  data flow for network access.
        NetworkStream ns;
        Thread t = null, t2= null;

        string box;
        string ship1 = "", ship2 = "", ship3 = "";
        int size;
        bool GameEnd;
        public FormSGame()
        {
            InitializeComponent();
            //Listens on the specified port
            listener = new TcpListener(4545);
            //Starts listening for incoming connection requests.
            listener.Start();
            //Accepts a pending connection request
            client = listener.AcceptTcpClient();
            //NetworkStream object used to send and receive data.
            ns = client.GetStream();

            //creates a new thread and runs it
            t = new Thread(Fun);
            t2 = new Thread(GameEnded);
            t.Start();
            t2.Start();
        }
        public void Fun()
        {
            byte[] bytes = new byte[1024];
            
            int bytesRead = ns.Read(bytes, 0, bytes.Length);
            this.SetText(Encoding.ASCII.GetString(bytes, 0, bytesRead));
            
        }
        private void SetText(string text)
        {
            if (this.InvokeRequired)
            {
                SetTextCallback del = new SetTextCallback(SetText);
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
                var texts = Regex.Split(text," ");
                this.box = texts[0];
                this.Text = texts[1]+"'s Server Side";
                Board();
                Ships();
            }

        }
        private string randomShips(string ships1, string ships2, string ships3) 
        {
            string ship;
            Random rnd = new Random();
            List<string> coords = new List<string>()
            {
                "A1","A2","A3","B1","B2","B3","C1","C2","C3"
            };
            if (size == 16) 
            {
                string[] size4 = new string[] { "A4","B4","C4", "D1", "D2", "D3", "D4"};
                coords.AddRange(size4);
            }
            else if (size == 25)
            {
                string[] size5 = new string[] { "A4", "A5", "B4", "B5", "C4", "C5", "D1", "D2", "D3", "D4", "D5", "E1", "E2", "E3", "E4", "E5"};
                coords.AddRange(size5);
            }
            if (coords.Contains(ships1)){coords.Remove(ships1);}
            if (coords.Contains(ships2)) { coords.Remove(ships2); }
            if (coords.Contains(ships3)) { coords.Remove(ships3); }
            
            ship = coords[rnd.Next(coords.Count)];
            return ship;
        }
        private void Ships() 
        {
            ship1 = randomShips(ship1,ship2,ship3);
            ship2 = randomShips(ship1, ship2, ship3);
            ship3 = randomShips(ship1, ship2, ship3);
            ServerShips ships = new ServerShips(ship1,ship2,ship3);
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ns, ships);
        }

        private void GameEnded()
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
                        GameEnd = serverShips.Endgame;
                        if (GameEnd == true) 
                        {
                            Application.Exit();
                            t.Abort();
                            t2.Abort();
                        }

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
        private void Board()
        {
            if (box == "3x3")
            {
                //Computer board 
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
                size = 9;
            }
            else if (box == "4x4")
            {
                //Computer board 
                btnSA5.Hide();
                btnSB5.Hide();
                btnSC5.Hide();
                btnSD5.Hide();
                btnSE1.Hide();
                btnSE2.Hide();
                btnSE3.Hide();
                btnSE4.Hide();
                btnSE5.Hide();
                size = 16;
            }
            else 
            {
                size = 25;
            }
        }
    }
}