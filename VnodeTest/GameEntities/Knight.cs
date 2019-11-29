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
            for (int indexX = -1; indexX < 2; indexX += 2)
            {
                returnValues.Add((PositionXY.Item1 + indexX, PositionXY.Item2 - 2));
                returnValues.Add((PositionXY.Item1 + indexX, PositionXY.Item2 + 2));
            }
            for (int indexY = -1; indexY < 2; indexY += 2)
            {
                returnValues.Add((PositionXY.Item1 - 2, PositionXY.Item2 + indexY));
                returnValues.Add((PositionXY.Item1 + 2, PositionXY.Item2 + indexY));
            }
            foreach (ValueTuple<int, int> item in returnValues.ToArray())
            {
                if (item.Item1 < 0 || item.Item1 > 7 || item.Item2 < 0 || item.Item2 > 7 
                    || gameboard.Board[ConvertToOneD(item)].ContainsPiece == true 
                    && gameboard.Board[ConvertToOneD(item)].Piece.Color == Color)
                    returnValues.Remove(item);
            }

            return ConvertToOneD(returnValues);
        }

        public override BasePiece Copy() => new Knight(Position, Color);
    }
}
