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
        public string Sprite => GetSprite();
        public ValueTuple<int, int> PositionXY => (Position % 8, Position / 8);
        public int StartPosition { get; }
        public bool HasMoved { get; private set; }
        private int _Position;
        public int Position
        {
            get
            {
                return _Position;
            }
            set
            {
                if (_Position != StartPosition)
                    HasMoved = true;
                _Position = value;

            }
        }

        public BasePiece(int position, PieceColor color)
        {
            Position = position;
            StartPosition = position;
            Color = color;
        }

        public List<int> ConvertToOneD(List<ValueTuple<int, int>> valuesXY)
        {
            return valuesXY.Select(x => x.Item1 + x.Item2 * 8).ToList();
        }

        public int ConvertToOneD(ValueTuple<int, int> valueXY)
        {
            return valueXY.Item1 + valueXY.Item2 * 8;
        }

        public List<int> GetDiagonals(Gameboard gameboard, int distance = 7)
        {
            var diagonals = new List<ValueTuple<int, int>>();
            for (int directionX = -1; directionX < 2; directionX += 2)
                for (int directionY = -1; directionY < 2; directionY += 2)
                    diagonals.AddRange(GetPotentialMoves((directionX, directionY), gameboard, distance));
            return ConvertToOneD(diagonals);
        }

        public List<int> GetStraightLines(Gameboard gameboard, int distance = 7)
        {
            var straightLines = new List<ValueTuple<int, int>>();
            for (int directionX = -1; directionX < 2; directionX += 2)
                straightLines.AddRange(GetPotentialMoves((directionX, 0), gameboard, distance));
            for (int directionY = -1; directionY < 2; directionY += 2)
                straightLines.AddRange(GetPotentialMoves((0, directionY), gameboard, distance));
            return ConvertToOneD(straightLines);
        }

        private List<ValueTuple<int, int>> GetPotentialMoves(ValueTuple<int, int> direction, Gameboard gameboard, int distance = 7)
        {
            List<ValueTuple<int, int>> output = new List<(int, int)>();
            var currentTarget = (PositionXY.Item1 + direction.Item1, PositionXY.Item2 + direction.Item2);

            while (distance > 0
                && currentTarget.Item1 < 8
                && currentTarget.Item1 >= 0
                && currentTarget.Item2 < 8
                && currentTarget.Item2 >= 0)
            {
                var notNull = gameboard.Board[ConvertToOneD(currentTarget)].Piece != null;
                var currentTargetColor = gameboard.Board[ConvertToOneD(currentTarget)].Piece?.Color;
                if (notNull && currentTargetColor == Color)
                    return output;

                output.Add(currentTarget);

                if (notNull && currentTargetColor != Color)
                    return output;

                currentTarget.Item1 += direction.Item1;
                currentTarget.Item2 += direction.Item2;
                distance--;
            }
            return output;
        }

        private string GetSprite()
        {
            if (Color == PieceColor.White)
                return Value switch
                {
                    PieceValue.King => "\u2654",
                    PieceValue.Queen => "\u2655",
                    PieceValue.Rook => "\u2656",
                    PieceValue.Bishop => "\u2657",
                    PieceValue.Knight => "\u2658",
                    PieceValue.Pawn => "\u2659",
                    _ => ""
                };
            return Value switch
            {
                PieceValue.King => "\u265A",
                PieceValue.Queen => "\u265B",
                PieceValue.Rook => "\u265C",
                PieceValue.Bishop => "\u265D",
                PieceValue.Knight => "\u265E",
                PieceValue.Pawn => "\u265F",
                _ => ""
            };
        }

        public abstract BasePiece Copy();

        public abstract List<int> GetValidMovements(Gameboard gameboard);
    }
}
