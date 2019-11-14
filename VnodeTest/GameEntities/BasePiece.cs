using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    public abstract class BasePiece
    {
        public PieceColor Color { get; set; }
        public PieceValue Value { get; set; }
        public string Sprite { get; }
        public int Position { get; set; }
        public ValueTuple<int, int> PositionXY => (Position % 8, Position / 8);
        private List<ValueTuple<int, int>> ValidMovesXY { get; set; }
        public List<int> ValidMoves => ConvertToOneD(ValidMovesXY);

        public BasePiece(int position, PieceColor color)
        {
            Position = position;
            Color = color;
            ValidMovesXY = GetValidMovements();
        }

        private List<int> ConvertToOneD(List<ValueTuple<int, int>> valuesXY)
        {
            return valuesXY.Select(x => x.Item1 % 8 + x.Item2 / 8).ToList();
        }

        public List<ValueTuple<int, int>> GetPotentialMovementLine(ValueTuple<int, int> direction, int distance = 8)
        {
            List<ValueTuple<int, int>> output = new List<(int, int)>();
            var currentyCheckedPosition = PositionXY;
            while (distance > 0 && currentyCheckedPosition.Item1 < 8 && currentyCheckedPosition.Item2 < 8)
            {
                if (PositionXY != currentyCheckedPosition)
                    output.Add(currentyCheckedPosition);
                currentyCheckedPosition.Item1 += direction.Item1;
                currentyCheckedPosition.Item2 += direction.Item2;
                distance--;
            }
            return output;
        }

        public List<ValueTuple<int, int>> GetDiagonals(int distance = 8)
        {
            var diagonals = new List<ValueTuple<int, int>>();
            for (int directionX = -1; directionX < 2; directionX += 2)
                for (int directionY = -1; directionY < 2; directionY += 2)
                    diagonals.Concat(GetPotentialMovementLine((directionX, directionY), distance));
            return diagonals;
        }

        public List<ValueTuple<int, int>> GetStraightLines(int distance = 8)
        {
            var straightLines = new List<ValueTuple<int, int>>();
            for (int directionX = -1; directionX < 2; directionX += 2)
                straightLines.Concat(GetPotentialMovementLine((directionX, 0)));
            for (int directionY = -1; directionY < 2; directionY += 2)
                straightLines.Concat(GetPotentialMovementLine((0, directionY), distance));
            return straightLines;
        }

        public abstract List<ValueTuple<int, int>> GetValidMovements();

        //public bool MovingIsValid(ValueTuple<int, int> target, Gameboard gameboard)
        //{
        //    if (ValidMoves.Contains(target) && NotBlocked(target, gameboard))
        //        return true;
        //    return false;
        //}

        //public bool IsInBounds(ValueTuple<int, int> target)
        //{
        //    if (target.Item1 < 8 && target.Item1 >= 0 && target.Item2 < 8 && target.Item2 >= 0)
        //        return true;
        //    return false;
        //}


        //public abstract bool NotBlocked(ValueTuple<int, int> target, Gameboard gameboard);
    }
}
