using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    class Knight : BasePiece
    {
        public Knight(ValueTuple<int, int> position, PieceColor color) : base(position, color) { }

        public override List<(int, int)> GetValidMovements()
        {
            var returnValues = new List<ValueTuple<int, int>>();
            for (int indexX = -1; indexX < 2; indexX += 2)
            {
                returnValues.Add((indexX, -2));
                returnValues.Add((indexX, +2));
            }
            for (int indexY = -1; indexY < 2; indexY += 2)
            {
                returnValues.Add((-2, indexY));
                returnValues.Add((+2, indexY));
            }
            return returnValues;
        }
    }
}
