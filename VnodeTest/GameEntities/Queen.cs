using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    class Queen: BasePiece
    {
        public Queen(int position, PieceColor color) : base(position,color)
        {
            Value = PieceValue.Queen;
        }

        public override List<int> GetValidMovements(Gameboard gameboard)
        {
            return ConvertToOneD(GetDiagonals(gameboard).Concat(GetStraightLines(gameboard)).ToList());
        }
    }
}
