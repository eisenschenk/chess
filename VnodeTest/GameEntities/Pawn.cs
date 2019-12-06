using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    class Pawn : BasePiece
    {

        public Pawn(int position, PieceColor color) : base(position, color)
        {
            Value = PieceValue.Pawn;
        }

        protected override IEnumerable<int> GetPotentialMovements(Gameboard gameboard)
        {
            int possibleMove = (StartPosition == Position) ? 2 : 1;
            Func<int, bool> enemyPiece = position => gameboard.Board[position] != null && gameboard.Board[position].Color != Color;
            if (Color == PieceColor.White)
            {
                //Position - 7 hack to prevent movement to the left/right
                var returnValues = GetStraightLines(gameboard, possibleMove).Where(x => x < Position - 7 && gameboard.Board[x] == null);
                return returnValues.Concat(GetDiagonals(gameboard, 1).Where(x => x < Position && enemyPiece(x) || x == gameboard.EnPassantTarget));
            }
            else
            {
                //Position + 7 hack to prevent movement to the left/right
                var returnValues = GetStraightLines(gameboard, possibleMove).Where(x => x > Position + 7 && gameboard.Board[x] == null);
                return returnValues.Concat(GetDiagonals(gameboard, 1).Where(x => x > Position && enemyPiece(x) || x == gameboard.EnPassantTarget));
            }
        }

        public override BasePiece Copy() => new Pawn(Position, Color);
    }
}
