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
        public Pawn(int position, PieceColor color) : base(position, color)
        {
            Value = PieceValue.Pawn;
            StartPosition = PositionXY;
        }

        public override List<ValueTuple<int, int>> GetValidMovements()
        {
            List<ValueTuple<int, int>> output = new List<(int, int)>();
            if (StartPosition.Item2 > 1)
            {
                if (StartPosition == PositionXY)
                    return GetStraightLines(2).Where(x => x.Item2 > 1).ToList();
                return GetStraightLines(1).Where(x => x.Item2 > 1).ToList();
            }
            if (StartPosition == PositionXY)
                return GetStraightLines(2).Where(x => x.Item2 < 64).ToList();
            return GetStraightLines(1).Where(x => x.Item2 < 64).ToList();
        }
    }
}
