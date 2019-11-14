using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    class Bishop : BasePiece
    {
        public Bishop(int positionX, int positionY, PieceColor color) : base(positionX, positionY, color)
        {
            Value = PieceValue.Bishop;
        }

        public override List<ValueTuple<int, int>> GetValidMovements()
        {
            return GetDiagonals();
        }

        public override bool NotBlocked(ValueTuple<int, int> target, Gameboard gameboard)
        {

        }
    }
}
