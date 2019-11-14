using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    class King : BasePiece
    {
        public King(ValueTuple<int, int> position, PieceColor color) : base(position, color)
        {
            Value = PieceValue.King;
        }

        //public override bool NotBlocked(ValueTuple<int, int> target, Gameboard gameboard)
        //{
        //    if (Checkmate())
        //        return false;
        //    return true;
        //}

        //TODO: currently placeholder
        //private bool Checkmate()
        //{
        //    return false;
        //}

        public override List<ValueTuple<int, int>> GetValidMovements()
        {
            return GetDiagonals(1).Concat(GetStraightLines(1)).ToList();
        }
    }
}
