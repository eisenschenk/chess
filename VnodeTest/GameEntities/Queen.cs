using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    class Queen: BasePiece
    {
        public Queen(int position, PieceColor color) : base(position,color){}

        public override List<ValueTuple<int, int>> GetValidMovements()
        {
            return GetDiagonals().Concat(GetStraightLines()).ToList();
        }
    }
}
