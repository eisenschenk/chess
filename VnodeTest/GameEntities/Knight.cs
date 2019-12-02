using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    class Knight : BasePiece
    {
        public Knight(int position, PieceColor color) : base(position, color)
        {
            Value = PieceValue.Knight;
        }

        protected override IEnumerable<int> GetPotentialMovements(Gameboard gameboard)
        {
            var returnValues = new List<(int X, int Y)>(8);
            for (int index = -1; index < 2; index += 2)
            {
                returnValues.Add((PositionXY.X + index, PositionXY.Y - 2));
                returnValues.Add((PositionXY.X + index, PositionXY.Y + 2));
                returnValues.Add((PositionXY.X - 2, PositionXY.Y + index));
                returnValues.Add((PositionXY.X + 2, PositionXY.Y + index));
            }
            return returnValues.Where(p => p.X >= 0 && p.X < 8 && p.Y >= 0 && p.Y < 8
            && (gameboard.Board[ConvertToOneD(p)].ContainsPiece == false
            || gameboard.Board[ConvertToOneD(p)].ContainsPiece == true && gameboard.Board[ConvertToOneD(p)].Piece.Color != Color))
                .Select(t => ConvertToOneD(t));
        }

        public override BasePiece Copy() => new Knight(Position, Color);
    }
}
