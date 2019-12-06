using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    //Ienumerables statt Listen für alles#
    // protected statt public für vererbungen
    public abstract class BasePiece
    {
        public PieceColor Color { get; set; }
        public PieceValue Value { get; set; }
        public string Sprite => GetSprite();
        public (int X, int Y) PositionXY => (Position % 8, Position / 8);
        public int StartPosition { get; set; } //von color abh. 
        public bool HasMoved { get; set; }
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

        public IEnumerable<int> ConvertToOneD(IEnumerable<ValueTuple<int, int>> valuesXY)
        {
            return valuesXY.Select(ConvertToOneD);
        }

        public int ConvertToOneD(ValueTuple<int, int> valueXY)
        {
            return valueXY.Item1 + valueXY.Item2 * 8;
        }

        public IEnumerable<int> GetDiagonals(Gameboard gameboard, int distance = 7)
        {
            for (int directionX = -1; directionX < 2; directionX += 2)
                for (int directionY = -1; directionY < 2; directionY += 2)
                    foreach (var move in ConvertToOneD(GetPotentialMoves((directionX, directionY), gameboard, distance)))
                        yield return move;
        }

        public IEnumerable<int> GetStraightLines(Gameboard gameboard, int distance = 7)
        {
            var result = Enumerable.Empty<(int, int)>();
            for (int i = -1; i < 2; i += 2)
                result = result
                    .Concat(GetPotentialMoves((i, 0), gameboard, distance))
                    .Concat(GetPotentialMoves((0, i), gameboard, distance));
            return ConvertToOneD(result);
        }
        //distance -> -1 
        private IEnumerable<(int X, int Y)> GetPotentialMoves((int X, int Y) direction, Gameboard gameboard, int distance = 7)
        {
            var currentTarget = (X: PositionXY.X + direction.X, Y: PositionXY.Y + direction.Y);

            while (distance > 0
                && currentTarget.X < 8
                && currentTarget.X >= 0
                && currentTarget.Y < 8
                && currentTarget.Y >= 0)
            {
                var notNull = gameboard.Board[ConvertToOneD(currentTarget)] != null;
                var currentTargetColor = gameboard.Board[ConvertToOneD(currentTarget)]?.Color;
                if (notNull && currentTargetColor == Color)
                    yield break;

                yield return currentTarget;

                if (notNull && currentTargetColor != Color)
                    yield break;

                currentTarget.X += direction.X;
                currentTarget.Y += direction.Y;
                distance--;
            }
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

        public IEnumerable<int> GetValidMovements(Gameboard gameboard)
        {
            return GetPotentialMovements(gameboard).Where(m =>
            {
                var futureGameBoard = HypotheticalMove(gameboard, m);
                var kingSameColorPosition = futureGameBoard.Board
                    .Where(t => t != null && t.Color == Color && t is King)
                    .Single().Position;
                var enemyPieces = futureGameBoard.Board.Where(x => x != null && x.Color != Color);

                return !enemyPieces.SelectMany(t => t.GetPotentialMovements(futureGameBoard)).Contains(kingSameColorPosition);
            });
        }

        public Gameboard HypotheticalMove(Gameboard gameboard, int target)
        {
            var futureGameBoard = gameboard.Copy();
            futureGameBoard.Board[target] = Copy();
            futureGameBoard.Board[Position] = null;
            futureGameBoard.Board[target].Position = futureGameBoard.Board[target].Position;
            return futureGameBoard;
        }

        public abstract BasePiece Copy();

        protected abstract IEnumerable<int> GetPotentialMovements(Gameboard gameboard);
    }
}
