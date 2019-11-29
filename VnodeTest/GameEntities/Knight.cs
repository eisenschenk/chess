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
            var returnValues = new List<ValueTuple<int, int>>();
            for (int index = -1; index < 2; index += 2)
            {
                returnValues.Add((PositionXY.X + index, PositionXY.Y - 2));
                returnValues.Add((PositionXY.X + index, PositionXY.Y + 2));
                returnValues.Add((PositionXY.X - 2, PositionXY.Y + index));
                returnValues.Add((PositionXY.X + 2, PositionXY.Y + index));
            }

            foreach ((int x, int y) item in returnValues.ToArray())
            {
                if (item.x < 0 || item.x > 7 || item.y < 0 || item.y > 7
                    || gameboard.Board[ConvertToOneD(item)].ContainsPiece == true
                    && gameboard.Board[ConvertToOneD(item)].Piece.Color == Color)
                    returnValues.Remove(item);
            }

            return ConvertToOneD(returnValues);
        }

        public override BasePiece Copy() => new Knight(Position, Color);
    }
}
