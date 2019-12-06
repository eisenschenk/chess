using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    public class Gameboard
    {
        public Tile[] Board { get; set; } = new Tile[64];
        public bool IsPromotable { get; set; }
        public bool GameOver => Winner.HasValue;
        public PieceColor? Winner { get; set; }
        public PieceColor CurrentPlayerColor { get; set; } = PieceColor.White;
        public int EnPassantTarget { get; set; } = -1;
        public (BasePiece start, Tile target) Lastmove { get; set; }
        public int MoveCounter { get; private set; } = 1;
        public int HalfMoveCounter { get; private set; }
        //public IEngine Engine { get; }
        // public (bool W, bool B) PlayedByEngine { get; }
        // public string EngineMove { get; private set; } // debug string

        private TimeSpan _WhiteClock;
        public TimeSpan WhiteClock { get => _WhiteClock; private set => _WhiteClock = value; }

        private TimeSpan _BlackClock;
        public TimeSpan BlackClock { get => _BlackClock; private set => _BlackClock = value; }
        private DateTime LastClockUpdate;

        public Tile this[int x, int y]
        {
            get => Board[y * 8 + x];
            set => Board[y * 8 + x] = value;
        }
        public Gameboard(TimeSpan playerClockTime, (bool, bool) playedByEngine = default, IEngine engine = default)
        {
            for (int index = 0; index < 64; index++)
                Board[index] = new Tile(index);
            PutPiecesInStartingPosition();
            LastClockUpdate = DateTime.Now;
            WhiteClock = playerClockTime;
            BlackClock = playerClockTime;
        }

        private Gameboard(IEnumerable<Tile> collection)
        {
            Board = collection.ToArray();
        }

        public void TryEngineMove(string engineMove, (bool, bool) engineControlled = default)
        {
            //EngineMove = Engine.GetEngineMove(GetFeNotation());
            var _engineMove = GetCoordinates(engineMove);
            TryMove(Board[_engineMove.start], Board[_engineMove.target], engineControlled);
            if (engineMove.Length >= 5)
                Board[_engineMove.target].Piece = engineMove[4] switch
                {
                    'q' => new Queen(_engineMove.target, CurrentPlayerColor),
                    'n' => new Knight(_engineMove.target, CurrentPlayerColor),
                    'b' => new Bishop(_engineMove.target, CurrentPlayerColor),
                    'r' => new Rook(_engineMove.target, CurrentPlayerColor),
                    _ => default
                };
        }

        private static (int start, int target) GetCoordinates(string input)
        {
            var startX = ParseStringXToInt(input[0].ToString());
            var startY = ParseStringYToInt(input[1].ToString());
            var targetX = ParseStringXToInt(input[2].ToString());
            var targetY = ParseStringYToInt(input[3].ToString());
            return (startX + startY * 8, targetX + targetY * 8);
        }

        public string GetFeNotation()
        {
            int emptyCount = 0;
            string output = string.Empty;
            foreach (Tile tile in Board)
            {
                if (tile.PositionXY.X % 8 == 0 && tile.PositionXY.Y >= 1)
                {
                    if (emptyCount != 0)
                    {
                        output += emptyCount.ToString();
                        emptyCount = 0;
                    }
                    output += "/";
                }
                if (!tile.ContainsPiece)
                    emptyCount++;

                if (tile.ContainsPiece)
                {
                    if (emptyCount != 0)
                        output += emptyCount.ToString();
                    emptyCount = 0;
                    output += tile.Piece.Value switch
                    {
                        PieceValue.King => tile.Piece.Color == PieceColor.White ? "K" : "k",
                        PieceValue.Queen => tile.Piece.Color == PieceColor.White ? "Q" : "q",
                        PieceValue.Bishop => tile.Piece.Color == PieceColor.White ? "B" : "b",
                        PieceValue.Knight => tile.Piece.Color == PieceColor.White ? "N" : "n",
                        PieceValue.Rook => tile.Piece.Color == PieceColor.White ? "R" : "r",
                        PieceValue.Pawn => tile.Piece.Color == PieceColor.White ? "P" : "p",
                        _ => throw new Exception("error FEN piece.value switch")
                    };
                }
            }
            output += CurrentPlayerColor == PieceColor.White ? " w " : " b ";
            output += GetPossibleCastles();
            output += EnPassantTarget == -1 ? $" {ParseIntToString(EnPassantTarget)} " : " - ";
            output += $"{HalfMoveCounter} ";
            output += $"{MoveCounter}";
            return output;
        }

        private string GetPossibleCastles()
        {
            string CheckCastle(int king, int rook)
            {
                string _output = string.Empty;
                if (Board[rook].ContainsPiece && !Board[rook].Piece.HasMoved
                    && Board[king].ContainsPiece && !Board[king].Piece.HasMoved)
                    if (Board[king].Piece.Color == PieceColor.White)
                    {
                        if (king > rook)
                            return _output += "Q";
                        else
                            return _output += "K";
                    }
                    else
                    {
                        if (king > rook)
                            return _output += "q";
                        else
                            return _output += "k";
                    }
                return _output;
            }
            string output = CheckCastle(60, 63);
            output += CheckCastle(60, 56);
            output += CheckCastle(4, 7);
            return output += CheckCastle(4, 0);
        }

        private void PutPiecesInStartingPosition()
        {
            for (int pawns = 8; pawns < 16; pawns++)
                Board[pawns].Piece = new Pawn(pawns, PieceColor.Black);
            Board[0].Piece = new Rook(0, PieceColor.Black);
            Board[1].Piece = new Knight(1, PieceColor.Black);
            Board[2].Piece = new Bishop(2, PieceColor.Black);
            Board[3].Piece = new Queen(3, PieceColor.Black);
            Board[4].Piece = new King(4, PieceColor.Black);
            Board[5].Piece = new Bishop(5, PieceColor.Black);
            Board[6].Piece = new Knight(6, PieceColor.Black);
            Board[7].Piece = new Rook(7, PieceColor.Black);

            for (int pawns = 48; pawns < 56; pawns++)
                Board[pawns].Piece = new Pawn(pawns, PieceColor.White);
            Board[56].Piece = new Rook(56, PieceColor.White);
            Board[57].Piece = new Knight(57, PieceColor.White);
            Board[58].Piece = new Bishop(58, PieceColor.White);
            Board[59].Piece = new Queen(59, PieceColor.White);
            Board[60].Piece = new King(60, PieceColor.White);
            Board[61].Piece = new Bishop(61, PieceColor.White);
            Board[62].Piece = new Knight(62, PieceColor.White);
            Board[63].Piece = new Rook(63, PieceColor.White);
        }

        public Gameboard Copy() => new Gameboard(Board.Select(t => t.Copy()));

        public bool TryCastling(Tile start, Tile target)
        {
            if (start.ContainsPiece && start.Piece is King)
            {
                if (Math.Abs(target.PositionXY.X - start.PositionXY.X) == 2)
                {
                    //direction hack => target either left or right rook
                    int direction = 1;
                    if (start.Piece.Position > target.Position)
                        direction *= -1;
                    //moving king&rook
                    MovePiece(start, Board[start.Position + 2 * direction]);
                    if (direction > 0)
                        MovePieceInternal(Board[3 * direction + start.Position], Board[start.Position + direction]);
                    else
                        MovePieceInternal(Board[4 * direction + start.Position], Board[start.Position + direction]);
                    return true;
                }
            }
            return false;
        }

        public bool TryMove(Tile start, Tile target, (bool, bool) engineControlled = default)
        {
            if (TryCastling(start, target))
                return true;

            if (!start.Piece.GetValidMovements(this).Contains(target.Position))
                return false;

            if (target.ContainsPiece || start.Piece is Pawn)
                HalfMoveCounter = 0;
            else
                HalfMoveCounter++;
            MovePiece(start, target, engineControlled);
            return true;
        }

        private readonly object UpdateClockLock = new object();

        public void UpdateClocks() => UpdateClocks(false);

        private void UpdateClocks(bool changeCurrentPlayer)
        {
            lock (UpdateClockLock)
            {
                var now = DateTime.Now;
                void updateColor(PieceColor color, ref TimeSpan clock)
                {
                    if (CurrentPlayerColor != color)
                        return;
                    clock -= now - LastClockUpdate;
                    if (clock <= TimeSpan.Zero)
                        Winner = color;
                }
                updateColor(PieceColor.Black, ref _BlackClock);
                updateColor(PieceColor.White, ref _WhiteClock);
                LastClockUpdate = now;
                if (changeCurrentPlayer)
                    CurrentPlayerColor = InverseColor();
            }
        }

        private void ActionsAfterMoveSuccess(Tile target, (bool, bool) engineControlled = default)
        {
            TryEnablePromotion(target, engineControlled);
            UpdateClocks(changeCurrentPlayer: true);
            if (CurrentPlayerColor == PieceColor.White)
                MoveCounter++;
            if (CheckForGameOver())
                Winner = InverseColor();
        }

        public bool CheckForGameOver()
        {
            if (HalfMoveCounter >= 100)
                return true;
            foreach (Tile tile in Board.Where(t => t.ContainsPiece && t.Piece.Color == CurrentPlayerColor))
                if (tile.Piece.GetValidMovements(this).Any())
                    return false;
            return true;
        }


        public bool CheckDetection(PieceColor color)
        {
            var king = Board.Where(p => p.ContainsPiece && p.Piece.Color == color && p.Piece is King).Single();
            var enemyMoves = Board.Where(p => p.ContainsPiece && p.Piece.Color != color).SelectMany(m => m.Piece.GetValidMovements(this));
            if (enemyMoves.Contains(king.Position))
                return true;
            return false;
        }

        private void MovePieceInternal(Tile start, Tile target)
        {
            target.Piece = start.Piece;
            target.Piece.Position = target.Position;
            start.Piece = null;
        }
        private void MovePiece(Tile start, Tile target, (bool, bool) engineControlled = default)
        {
            if (start.Piece is Pawn)
            {
                var enPassant = EnPassantTarget;
                EnPassantTarget = -1;
                if (target.Position == enPassant)
                    Board[Lastmove.target.Position].Piece = null;
                else if (Math.Abs(start.PositionXY.Y - target.PositionXY.Y) == 2)
                    EnPassantTarget = start.PositionXY.X + (start.Piece.Color == PieceColor.Black ? (start.PositionXY.Y + 1) * 8 : (start.PositionXY.Y - 1) * 8);
            }
            Lastmove = (start.Piece.Copy(), target.Copy());
            MovePieceInternal(start, target);
            ActionsAfterMoveSuccess(target, engineControlled);
        }

        public void TryEnablePromotion(Tile tile, (bool W, bool B) engineControlled = default)
        {
            if (tile.Piece is Pawn && (tile.Piece.Position > 55 || tile.Piece.Position < 7))
            {
                if ((CurrentPlayerColor == PieceColor.White && !engineControlled.W) || (CurrentPlayerColor == PieceColor.Black && !engineControlled.B))
                    IsPromotable = true;
            }
        }

        private bool TryAnCastling(string notation, IEnumerable<Tile> _pieces)
        {

            if (notation.Contains("0-0") || notation.Contains("O-O"))
            {
                var king = _pieces.Where(k => k.Piece is King).Single();
                // implement +
                if (notation == "0-0" || notation == "O-O")
                    TryMove(king, this[king.Piece.PositionXY.X + 2, king.Piece.PositionXY.Y]);
                else
                    TryMove(king, this[king.Piece.PositionXY.X - 2, king.Piece.PositionXY.Y]);
                return true;
            }
            return false;
        }

        private IEnumerable<Tile> GetCorrectPieceType(Match match, IEnumerable<Tile> _pieces)
        {
            _pieces = _pieces.Where(v => (match.Groups["piece"].Value switch
            {
                "K" => v.Piece is King,
                "Q" => v.Piece is Queen,
                "R" => v.Piece is Rook,
                "N" => v.Piece is Knight,
                "B" => v.Piece is Bishop,
                _ => v.Piece is Pawn
            }));
            return _pieces;
        }

        //string input statt match
        private IEnumerable<Tile> GetCorrectSourceX(Match match, IEnumerable<Tile> _pieces)
        {
            _pieces = _pieces.Where(x =>
                match.Groups["sourceX"].Value != ""
                    ? x.Piece.PositionXY.X == ParseStringXToInt(match.Groups["sourceX"].Value)
                    : x.ContainsPiece);
            return _pieces;
        }

        private IEnumerable<Tile> GetCorrectSourceY(Match match, IEnumerable<Tile> _pieces)
        {
            _pieces = _pieces.Where(y =>
                match.Groups["sourceY"].Value != ""
                    ? y.Piece.PositionXY.Y == ParseStringYToInt(match.Groups["sourceY"].Value)
                    : y.ContainsPiece);
            return _pieces;
        }

        private PieceColor InverseColor()
        {
            if (CurrentPlayerColor == PieceColor.White)
                return PieceColor.Black;
            else return PieceColor.White;
        }

        private bool TryAnPromotion(Match match, IEnumerable<Tile> _pieces, Tile destination)
        {
            if (match.Groups["promotion"].Value != "")
                if (TryMove(_pieces.Single(), destination))
                {
                    Board[destination.Position].Piece = match.Groups["promotion"].Value switch
                    {
                        "=Q" => new Queen(destination.Position, InverseColor()),
                        "=R" => new Rook(destination.Position, InverseColor()),
                        "=N" => new Knight(destination.Position, InverseColor()),
                        "=B" => new Bishop(destination.Position, InverseColor()),
                        _ => throw new Exception("error in promotion")
                    };
                    return true;
                }
                else
                    throw new Exception("error in promotion");
            return false;
        }

        private IEnumerable<Tile> GetCorrectDestination(Tile destination, IEnumerable<Tile> _pieces)
        {
            _pieces = _pieces.Where(d => d.Piece.GetValidMovements(this).Contains(destination.Position));
            return _pieces;
        }

        private bool TryEndGame(string notation)
        {
            if (notation.Contains("1-0") || notation.Contains("0-1") || notation.Contains("½–½"))
            {
                Winner = notation switch
                {
                    "1-0" => PieceColor.White,
                    "0-1" => PieceColor.Black,
                    "½–½" => PieceColor.Zero,
                    _ => default
                };
                return true;
            }
            return false;

        }

        public bool TryAlgebraicNotaionToMyNotaion(string notation)
        {
            var match = Regex.Match(notation, "^(?<piece>[KQRNB]?)(?<sourceX>[a-h]?)(?<sourceY>[1-8]?)(?<captures>x?)(?<destination>[a-h][1-8])(?<promotion>(=[QRNB])?)(?<check>\\+?)(?<mate>#?)$",
                    RegexOptions.Singleline & RegexOptions.ExplicitCapture);

            //_pieces where !=empty && correct color
            var _pieces = Board.Where(p => p.ContainsPiece && p.Piece.Color == CurrentPlayerColor);

            if (TryEndGame(notation))
                return true;
            if (TryAnCastling(notation, _pieces))
                return true;

            var destination = this[ParseStringXToInt(match.Groups["destination"].Value), ParseStringYToInt(match.Groups["destination"].Value)];
            _pieces = GetCorrectPieceType(match, _pieces);
            _pieces = GetCorrectSourceX(match, _pieces);
            _pieces = GetCorrectSourceY(match, _pieces);
            _pieces = GetCorrectDestination(destination, _pieces);
            var pieces = _pieces.ToArray();

            if (TryAnPromotion(match, pieces, destination))
                return true;

            if (TryMove(pieces.Single(), destination))
                return true;
            else
                throw new Exception("error in TryMove() in AN");
        }


        public static int ParseStringXToInt(string input)
        {
            var c = input[0];
            if (c < 'a' || c > 'h')
                throw new Exception("out of bounds X");
            return c - 'a';
        }

        private static string ParseIntToString(int index)
        {
            var x = index % 8;
            var y = index / 8;
            var yOut = y switch
            {
                0 => "8",
                1 => "7",
                2 => "6",
                3 => "5",
                4 => "4",
                5 => "3",
                6 => "2",
                7 => "1",
                _ => throw new Exception("error in int to string conversion y-value")
            };
            var xOut = ('a' + x).ToString();
            return xOut + yOut;
        }


        public static int ParseStringYToInt(string input)
        {
            var c = input[input.Length - 1];
            if (c < '1' || c > '8')
                throw new Exception("out of bounds Y");
            return c switch
            {
                '1' => 7,
                '2' => 6,
                '3' => 5,
                '4' => 4,
                '5' => 3,
                '6' => 2,
                '7' => 1,
                '8' => 0,
                _ => default
            };
        }

    }
}

