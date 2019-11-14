using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    class Bishop : BasePiece
    {
        public Bishop(ValueTuple<int, int> position, PieceColor color) : base(position, color)
        {
            Value = PieceValue.Bishop;
        }

        public override List<ValueTuple<int, int>> GetValidMovements()
        {
            return GetDiagonals();
        }
    }
}
