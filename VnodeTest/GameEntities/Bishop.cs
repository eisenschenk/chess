using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    class Bishop : BasePiece
    {
        public Bishop(int position, PieceColor color) : base(position, color)
        {
            Value = PieceValue.Bishop;
        }

        public override List<int> GetValidMovements(Gameboard gameboard)
        {
            return GetDiagonals(gameboard);
        }

        public override BasePiece Copy() => new Bishop(this.Position, this.Color);
    }
}
