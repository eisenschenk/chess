using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    public class Gameboard
    {
        public Tile[] Board = new Tile[64];

        public Gameboard()
        {
            for (int index = 0; index < 64; index++)
                    Board[index] = new Tile(index);
        }
    }
}
