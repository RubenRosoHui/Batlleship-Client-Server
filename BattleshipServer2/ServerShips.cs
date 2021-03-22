using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipServer2
{
    [Serializable]
    public class ServerShips
    {
        string ship1, ship2, ship3;
        bool end;
        public ServerShips(string Ship1,string Ship2,string Ship3) 
        {
            ship1 = Ship1;
            ship2 = Ship2;
            ship3 = Ship3;
        }
        public ServerShips(bool End) { end = End;}

        public bool Endgame { get { return end; } }
        public string returnShip1{get{return ship1;}}
        public string returnShip2{get{return ship2;}}
        public string returnShip3{get{return ship3;}}
    }
}
