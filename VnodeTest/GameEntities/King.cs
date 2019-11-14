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

        public override int[] GetValidMovements()
        {

            return new int[] {
                Position - 1,
                Position - 7,
                Position - 8,
                Position - 9,
                Position + 1,
                Position + 7,
                Position + 8,
                Position + 9
            };
        }
    }
}
