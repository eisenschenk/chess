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
        public ValueTuple<int, int> Position { get; set; }
        public List<ValueTuple<int, int>> ValidMoves { get; set; }

        public BasePiece(int positionX, int positionY, PieceColor color)
        {
            Position = new ValueTuple<int, int>(positionX, positionY);
            Color = color;
            ValidMoves = GetValidMovements();
        }

        public bool MovingIsValid(ValueTuple<int, int> target, Gameboard gameboard)
        {
            if (ValidMoves.Contains(target) && NotBlocked(target, gameboard))
                return true;
            return false;
        }

        public bool IsInBounds(ValueTuple<int, int> target)
        {
            if (target.Item1 < 8 && target.Item1 >= 0 && target.Item2 < 8 && target.Item2 >= 0)
                return true;
            return false;
        }

        public List<ValueTuple<int, int>> GetDiagonals()
        {
            List<ValueTuple<int, int>> GetDiagonal(ValueTuple<int, int> direction)
            {
                List<ValueTuple<int, int>> diagonal = new List<(int, int)>();
                var currentyCheckedPosition = Position;
                while (currentyCheckedPosition.Item1 < 8 && currentyCheckedPosition.Item2 < 8)
                {
                    diagonal.Add(currentyCheckedPosition);
                    currentyCheckedPosition.Item1 += direction.Item1;
                    currentyCheckedPosition.Item2 += direction.Item2;
                }
                return diagonal;
            }
            var diagonals = new List<ValueTuple<int, int>>();
            for (int directionX = -1; directionX < 2; directionX += 2)
                for (int directionY = -1; directionY < 2; directionY += 2)
                    diagonals.Concat(GetDiagonal((directionX, directionY)));
            return diagonals;
        }

        public abstract List<ValueTuple<int, int>> GetValidMovements();

        public abstract bool NotBlocked(ValueTuple<int, int> target, Gameboard gameboard);
    }
}
