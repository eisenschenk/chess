using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    class Queen : BasePiece
    {
        public Queen(int position, PieceColor color) : base(position, color)
        {
            Value = PieceValue.Queen;
        }

        public override List<int> GetValidMovements(Gameboard gameboard)
        {
            var output = GetDiagonals(gameboard).Concat(GetStraightLines(gameboard));
            return output.ToList();
        }

        public override BasePiece Copy() => new Queen(Position, Color);
    }
}
