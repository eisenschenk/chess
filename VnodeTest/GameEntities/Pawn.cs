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
        public Pawn(ValueTuple<int, int> position, int positionY, PieceColor color) : base(position, color)
        {
            Value = PieceValue.Pawn;
            StartPosition = Position;
        }

        public override List<ValueTuple<int, int>> GetValidMovements()
        {
            List<ValueTuple<int, int>> output = new List<(int, int)>();
            if (StartPosition.Item2 > 1)
            {
                if (StartPosition == Position)
                    return GetStraightLines(2).Where(x => x.Item2 > 1).ToList();
                return GetStraightLines(1).Where(x => x.Item2 > 1).ToList();
            }
            if (StartPosition == Position)
                return GetStraightLines(2).Where(x => x.Item2 < 64).ToList();
            return GetStraightLines(1).Where(x => x.Item2 < 64).ToList();
        }
    }
}
