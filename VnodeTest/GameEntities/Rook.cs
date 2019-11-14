using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    class Rook : BasePiece
    {
        public Rook(int positionX, int positionY, PieceColor color) : base(positionX, positionY, color)
        {
            Value = PieceValue.Rook;
        }

        public override bool NotBlocked(ValueTuple<int,int> target, Gameboard gameboard)
        {

        }

        //TODO
        public override int[] GetValidMovements()
        {
            //List<int> returnValues = new List<int>();
            //for (int index = 0; index < 64; index++)
            //{
            //    if (Position - index % 8 == 0)
            //        returnValues.Add(index);

            //}
        }
    }
}
