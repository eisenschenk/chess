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

        //TODO: Promotion
        public override List<int> GetValidMovements(Gameboard gameboard)
        {
            List<int> returnValues;
            if (StartPosition > 15)
            {
                if (StartPosition == Position)
                    returnValues = GetStraightLines(gameboard, 2).Where(x => x < Position && gameboard.Board[x].ContainsPiece == false).ToList();
                else
                    returnValues = GetStraightLines(gameboard, 1).Where(x => x < Position && gameboard.Board[x].ContainsPiece == false).ToList();

                returnValues.AddRange(GetDiagonals(gameboard, 1)
                    .Where(x => x < Position && gameboard.Board[x].ContainsPiece == true && gameboard.Board[x].Piece.Color != Color).ToList());
            }
            else
            {
                if (StartPosition == Position)
                    returnValues = GetStraightLines(gameboard, 2).Where(x => x > Position && gameboard.Board[x].ContainsPiece == false).ToList();
                else
                    returnValues = GetStraightLines(gameboard, 1).Where(x => x > Position && gameboard.Board[x].ContainsPiece == false).ToList();

                returnValues.AddRange(GetDiagonals(gameboard, 1)
                    .Where(x => x > Position && gameboard.Board[x].ContainsPiece == true && gameboard.Board[x].Piece.Color != Color).ToList());
            }
            return returnValues;
        }
    }
}
