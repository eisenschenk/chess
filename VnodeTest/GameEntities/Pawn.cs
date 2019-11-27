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

        //TODO: Promotion
        public override List<int> GetValidMovements(Gameboard gameboard)
        {
            int possibleMove = (StartPosition == Position) ? 2 : 1;
            Func<int,bool> enemyPiece = position => gameboard.Board[position].ContainsPiece == true && gameboard.Board[position].Piece.Color != Color;
            List<int> returnValues;
            if (StartPosition > 15)
            {
                returnValues = GetStraightLines(gameboard, possibleMove).Where(x => x < Position - 7 && gameboard.Board[x].ContainsPiece == false).ToList();
                returnValues.AddRange(GetDiagonals(gameboard, 1).Where(x => x < Position && enemyPiece(x)).ToList());
            }
            else
            {
                returnValues = GetStraightLines(gameboard, possibleMove).Where(x => x > Position + 7 && gameboard.Board[x].ContainsPiece == false).ToList();
                returnValues.AddRange(GetDiagonals(gameboard, 1).Where(x => x > Position && enemyPiece(x)).ToList());
            }
            return returnValues;
        }

        public override BasePiece Copy() => new Pawn(Position, Color);
    }
}
