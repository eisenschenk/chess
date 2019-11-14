using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    class Pawn : BasePiece
    {
        private int StartPosition;
        public Pawn(int position, PieceColor color) : base(position, color)
        {
            Value = PieceValue.Pawn;
            StartPosition = position;
        }

        public override int[] GetValidMovements()
        {
            if (Position == StartPosition)
            {
                if (StartPosition < 16)
                    return new int[] { Position + 8, Position + 16 };
                return new int[] { Position - 8, Position - 16 };
            }
            if (Position < 56 && Position > 7)
                if (Position < StartPosition)
                    return new int[] { Position - 8 };
            return new int[] { Position + 8 };
        }
    }
}
