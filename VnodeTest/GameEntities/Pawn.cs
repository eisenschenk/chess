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
            StartPosition = Position;
        }

        //TODO Pawn hits diagonal, not straight, implement block in case sth in front and enable kill via diagonals
        public override List<int> GetValidMovements(Gameboard gameboard)
        {
            if (StartPosition > 15)
            {
                if (StartPosition == Position)
                    return  GetStraightLines(gameboard, 2).Where(x => x < Position).ToList();
                return GetStraightLines(gameboard,1).Where(x => x < Position).ToList();
            }
            if (StartPosition == Position)
                return GetStraightLines(gameboard,2).Where(x => x > Position).ToList();
            return GetStraightLines(gameboard,1).Where(x => x > Position).ToList();
        }
    }
}
