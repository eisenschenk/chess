using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    class King : BasePiece
    {
        public King(int position, PieceColor color) : base(position, color)
        {
            Value = PieceValue.King;
        }



        public override List<int> GetValidMovements(Gameboard gameboard)
        {
            return GetDiagonals(gameboard, 1).Concat(GetStraightLines(gameboard, 1)).ToList();
        }
        public override BasePiece Copy() => new King(Position, Color);
    }
}
