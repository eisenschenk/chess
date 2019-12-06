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
        public BasePiece[] Board { get; set; } = new BasePiece[64];
        public bool IsPromotable { get; set; }
        public bool GameOver => Winner.HasValue;
        public PieceColor? Winner { get; set; }
        public PieceColor CurrentPlayerColor { get; set; } = PieceColor.White;
        public int EnPassantTarget { get; set; } = -1;
        public (BasePiece start, int target) Lastmove { get; set; }
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

        public BasePiece this[int x, int y]
        {
            get => Board[y * 8 + x];
            set => Board[y * 8 + x] = value;
        }
        public Gameboard(TimeSpan playerClockTime, (bool, bool) playedByEngine = default, IEngine engine = default)
        {
            PutPiecesInStartingPosition();
            LastClockUpdate = DateTime.Now;
            WhiteClock = playerClockTime;
            BlackClock = playerClockTime;
        }

        private Gameboard(IEnumerable<BasePiece> collection)
        {
            Board = collection.ToArray();
        }

        public void TryEngineMove(string engineMove, (bool, bool) engineControlled = default)
        {
            //EngineMove = Engine.GetEngineMove(GetFeNotation());
            var _engineMove = GetCoordinates(engineMove);
            TryMove(Board[_engineMove.start], _engineMove.target, engineControlled);
            if (engineMove.Length >= 5)
                Board[_engineMove.target] = engineMove[4] switch
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


            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)
                {
                    var piece = this[x, y];

                    if (x == 0 && y >= 1)
                    {
                        if (emptyCount != 0)
                        {
                            output += emptyCount.ToString();
                            emptyCount = 0;
                        }
                        output += "/";
                    }
                    if (piece == null)
                        emptyCount++;

                    if (piece != null)
                    {
                        if (emptyCount != 0)
                            output += emptyCount.ToString();
                        emptyCount = 0;
                        output += piece.Value switch
                        {
                            PieceValue.King => piece.Color == PieceColor.White ? "K" : "k",
                            PieceValue.Queen => piece.Color == PieceColor.White ? "Q" : "q",
                            PieceValue.Bishop => piece.Color == PieceColor.White ? "B" : "b",
                            PieceValue.Knight => piece.Color == PieceColor.White ? "N" : "n",
                            PieceValue.Rook => piece.Color == PieceColor.White ? "R" : "r",
                            PieceValue.Pawn => piece.Color == PieceColor.White ? "P" : "p",
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
                if (Board[rook] != null && !Board[rook].HasMoved
                    && Board[king] != null && !Board[king].HasMoved)
                    if (Board[king].Color == PieceColor.White)
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
                Board[pawns] = new Pawn(pawns, PieceColor.Black);
            Board[0] = new Rook(0, PieceColor.Black);
            Board[1] = new Knight(1, PieceColor.Black);
            Board[2] = new Bishop(2, PieceColor.Black);
            Board[3] = new Queen(3, PieceColor.Black);
            Board[4] = new King(4, PieceColor.Black);
            Board[5] = new Bishop(5, PieceColor.Black);
            Board[6] = new Knight(6, PieceColor.Black);
            Board[7] = new Rook(7, PieceColor.Black);

            for (int pawns = 48; pawns < 56; pawns++)
                Board[pawns] = new Pawn(pawns, PieceColor.White);
            Board[56] = new Rook(56, PieceColor.White);
            Board[57] = new Knight(57, PieceColor.White);
            Board[58] = new Bishop(58, PieceColor.White);
            Board[59] = new Queen(59, PieceColor.White);
            Board[60] = new King(60, PieceColor.White);
            Board[61] = new Bishop(61, PieceColor.White);
            Board[62] = new Knight(62, PieceColor.White);
            Board[63] = new Rook(63, PieceColor.White);
        }

        public Gameboard Copy() => new Gameboard(Board.Select(t => t?.Copy()));

        public bool TryCastling(BasePiece start, int target)
        {
            if (start is King)
            {
                if (Math.Abs(ConvertTo2D(target).X - start.PositionXY.X) == 2)
                {
                    //direction hack => target either left or right rook
                    int direction = 1;
                    if (start.Position > target)
                        direction *= -1;
                    //moving king&rook
                    var startPosition = start.Position;
                    MovePiece(start, start.Position + 2 * direction);
                    if (direction > 0)
                        MovePieceInternal(Board[3 * direction + startPosition], startPosition + direction);
                    else
                        MovePieceInternal(Board[4 * direction + startPosition], startPosition + direction);
                    return true;
                }
            }
            return false;
        }

        public bool TryMove(BasePiece start, int target, (bool, bool) engineControlled = default)
        {
            if (TryCastling(start, target))
                return true;

            if (!start.GetValidMovements(this).Contains(target))
                return false;

            if (Board[target] != null || start is Pawn)
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

        private void ActionsAfterMoveSuccess(BasePiece target, (bool, bool) engineControlled = default)
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
            if (HalfMoveCounter >= 50)
            {
                Winner = PieceColor.Zero;
                return true;
            }
            foreach (BasePiece piece in Board.Where(t => t != null && t.Color == CurrentPlayerColor))
                if (piece.GetValidMovements(this).Any())
                    return false;
            return true;
        }


        public bool CheckDetection(PieceColor color)
        {
            var king = Board.Where(p => p != null && p.Color == color && p is King).Single();
            var enemyMoves = Board.Where(p => p != null && p.Color != color).SelectMany(m => m.GetValidMovements(this));
            if (enemyMoves.Contains(king.Position))
                return true;
            return false;
        }

        private void MovePieceInternal(BasePiece start, int target)
        {
            Board[target] = start;
            Board[start.Position] = null;
            Board[target].Position = target;
        }
        private void MovePiece(BasePiece start, int target, (bool, bool) engineControlled = default)
        {
            if (start is Pawn)
            {
                var enPassant = EnPassantTarget;
                EnPassantTarget = -1;
                if (target == enPassant)
                    Board[Lastmove.target] = null;
                else if (Math.Abs(start.PositionXY.Y - ConvertTo2D(target).Y) == 2)
                    EnPassantTarget = start.PositionXY.X + (start.Color == PieceColor.Black ? (start.PositionXY.Y + 1) * 8 : (start.PositionXY.Y - 1) * 8);
            }
            Lastmove = (start.Copy(), target);
            MovePieceInternal(start, target);
            ActionsAfterMoveSuccess(Board[target], engineControlled);
        }

        public void TryEnablePromotion(BasePiece piece, (bool W, bool B) engineControlled = default)
        {
            if (piece is Pawn && (piece.Position > 55 || piece.Position < 7))
            {
                if ((CurrentPlayerColor == PieceColor.White && !engineControlled.W) || (CurrentPlayerColor == PieceColor.Black && !engineControlled.B))
                    IsPromotable = true;
            }
        }

        private bool TryAnCastling(string notation, IEnumerable<BasePiece> _pieces)
        {

            if (notation.Contains("0-0") || notation.Contains("O-O"))
            {
                var king = _pieces.Where(k => k is King).Single();
                // implement +
                if (notation == "0-0" || notation == "O-O")
                    TryMove(king, ConvertTo1D((king.PositionXY.X + 2, king.PositionXY.Y)));
                else
                    TryMove(king, ConvertTo1D((king.PositionXY.X - 2, king.PositionXY.Y)));
                return true;
            }
            return false;
        }

        private IEnumerable<BasePiece> GetCorrectPieceType(Match match, IEnumerable<BasePiece> _pieces)
        {
            _pieces = _pieces.Where(v => (match.Groups["piece"].Value switch
            {
                "K" => v is King,
                "Q" => v is Queen,
                "R" => v is Rook,
                "N" => v is Knight,
                "B" => v is Bishop,
                _ => v is Pawn
            }));
            return _pieces;
        }

        //string input statt match
        private IEnumerable<BasePiece> GetCorrectSourceX(Match match, IEnumerable<BasePiece> _pieces)
        {
            _pieces = _pieces.Where(x =>
                match.Groups["sourceX"].Value != ""
                    ? x.PositionXY.X == ParseStringXToInt(match.Groups["sourceX"].Value)
                    : x != default);
            return _pieces;
        }

        private IEnumerable<BasePiece> GetCorrectSourceY(Match match, IEnumerable<BasePiece> _pieces)
        {
            _pieces = _pieces.Where(y =>
                match.Groups["sourceY"].Value != ""
                    ? y.PositionXY.Y == ParseStringYToInt(match.Groups["sourceY"].Value)
                    : y != default);
            return _pieces;
        }

        private PieceColor InverseColor()
        {
            if (CurrentPlayerColor == PieceColor.White)
                return PieceColor.Black;
            else return PieceColor.White;
        }

        private bool TryAnPromotion(Match match, IEnumerable<BasePiece> _pieces, int destination)
        {
            if (match.Groups["promotion"].Value != "")
                if (TryMove(_pieces.Single(), destination))
                {
                    Board[destination] = match.Groups["promotion"].Value switch
                    {
                        "=Q" => new Queen(destination, InverseColor()),
                        "=R" => new Rook(destination, InverseColor()),
                        "=N" => new Knight(destination, InverseColor()),
                        "=B" => new Bishop(destination, InverseColor()),
                        _ => throw new Exception("error in promotion")
                    };
                    return true;
                }
                else
                    throw new Exception("error in promotion");
            return false;
        }

        private IEnumerable<BasePiece> GetCorrectDestination(int destination, IEnumerable<BasePiece> _pieces)
        {
            _pieces = _pieces.Where(d => d.GetValidMovements(this).Contains(destination));
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
            var _pieces = Board.Where(p => p != null && p.Color == CurrentPlayerColor);

            if (TryEndGame(notation))
                return true;
            if (TryAnCastling(notation, _pieces))
                return true;

            var destination = ConvertTo1D((ParseStringXToInt(match.Groups["destination"].Value), ParseStringYToInt(match.Groups["destination"].Value)));
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

        public (int X, int Y) ConvertTo2D(int index)
        {
            return (index % 8, index / 8);
        }

        public int ConvertTo1D((int X, int Y) xy)
        {
            return (xy.X + xy.Y * 8);
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

