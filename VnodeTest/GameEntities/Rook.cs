using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    class Rook : BasePiece
    {
        public Rook(int position, PieceColor color) : base(position, color)
        {
            Value = PieceValue.Rook;
        }

        public override int[] GetValidMovements()
        {
            List<int> returnValues = new List<int>();
            for (int index = 0; index < 64; index++)
            {
                if (Position - index % 8 == 0)
                    returnValues.Add(index);

            }
        }
    }
}
