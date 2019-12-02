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

        protected override IEnumerable<int> GetPotentialMovements(Gameboard gameboard)
        {
            return GetDiagonals(gameboard).Concat(GetStraightLines(gameboard));
        }

        public override BasePiece Copy() => new Queen(Position, Color);
    }
}
