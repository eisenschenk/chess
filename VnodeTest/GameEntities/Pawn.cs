using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    class Pawn : BasePiece
    {
        private ValueTuple<int, int> StartPosition;
        public Pawn(int positionX, int positionY, PieceColor color) : base(positionX, positionY, color)
        {
            Value = PieceValue.Pawn;
            StartPosition = Position;
        }

        public override bool NotBlocked(ValueTuple<int, int> target, Gameboard gameboard)
        {

        }

        public override int[] GetValidMovements()
        {
            //if (Position == StartPosition)
            //{
            //    if (StartPosition < 16)
            //        return new int[] { Position + 8, Position + 16 };
            //    return new int[] { Position - 8, Position - 16 };
            //}
            //if (Position < 56 && Position > 7)
            //    if (Position < StartPosition)
            //        return new int[] { Position - 8 };
            //    else return new int[] { Position + 8 };
            //return null;
        }
    }
}
