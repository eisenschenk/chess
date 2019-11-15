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

        //TODO Pawn hits diagonal, not straight, implement block in case sth in front and enable kill via diagonals
        public override List<int> GetValidMovements(Gameboard gameboard)
        {
            List<ValueTuple<int, int>> output = new List<(int, int)>();
            if (StartPosition.Item2 > 1)
            {
                if (StartPosition == PositionXY)
                    return ConvertToOneD( GetStraightLines(gameboard, 2).Where(x => x.Item2 > 1).ToList());
                return ConvertToOneD(GetStraightLines(gameboard,1).Where(x => x.Item2 > 1).ToList());
            }
            if (StartPosition == PositionXY)
                return ConvertToOneD(GetStraightLines(gameboard,2).Where(x => x.Item2 < 64).ToList());
            return ConvertToOneD(GetStraightLines(gameboard,1).Where(x => x.Item2 < 64).ToList());
        }
    }
}
