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

        public override int[] GetValidMovements()
        {
            List<int> returnValues = new List<int>();
            for (int index = 0; index < 64; index++)
                if (Position - index % 9 == 0 || Position - index % 7 == 0)
                    returnValues.Add(index);
            return returnValues.ToArray();
        }
    }
}
