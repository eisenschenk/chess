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
    // check & checkmate & win for previous moves display 
    public class Gameboard
    {
        public BasePiece[] Board { get; set; } = new BasePiece[64];
        public int EnPassantTarget { get; set; } = -1;

        public BasePiece this[int x, int y]
        {
            get => Board[y * 8 + x];
            set => Board[y * 8 + x] = value;
        }
        public Gameboard()
        {
            PutPiecesInStartingPosition();
        }

        private Gameboard(IEnumerable<BasePiece> collection, int enpassanttarget)
        {
            Board = collection.ToArray();
            EnPassantTarget = enpassanttarget;
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

        public Gameboard Copy() => new Gameboard(Board.ToArray(), EnPassantTarget);

        public bool TryCastling(BasePiece start, int target, out Gameboard gameboard, Game game)
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
                    MovePiece(start, start.Position + 2 * direction, game);
                    if (direction > 0)
                        MovePieceInternal(Board[3 * direction + startPosition], startPosition + direction);
                    else
                        MovePieceInternal(Board[4 * direction + startPosition], startPosition + direction);
                    gameboard = this.Copy();
                    return true;
                }
            }
            gameboard = this.Copy();
            return false;
        }

        public bool TryMove(BasePiece start, int target, out Gameboard gameboard, Game game, (bool, bool) engineControlled = default)
        {
            if (TryCastling(start, target, out gameboard, game))
                return true;

            if (!start.GetValidMovements(this).Contains(target))
                return false;

            if (Board[target] != null || start is Pawn)
                game.HalfMoveCounter = 0;
            else
                game.HalfMoveCounter++;
            MovePiece(start, target, game, engineControlled);
            gameboard = this.Copy();
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

        public void MovePieceInternal(BasePiece start, int target)
        {
            Board[target] = start.Copy();
            Board[start.Position] = null;
            Board[target].Position = target;
        }

        private void MovePiece(BasePiece start, int target, Game game = null, (bool, bool) engineControlled = default)
        {
            if (start is Pawn)
            {
                var enPassant = EnPassantTarget;
                EnPassantTarget = -1;
                if (target == enPassant)
                    Board[game.Lastmove.target] = null;
                else if (Math.Abs(start.PositionXY.Y - ConvertTo2D(target).Y) == 2)
                    EnPassantTarget = start.PositionXY.X + (start.Color == PieceColor.Black ? (start.PositionXY.Y + 1) * 8 : (start.PositionXY.Y - 1) * 8);
            }
            game.Lastmove = (start.Copy(), target);
            MovePieceInternal(start, target);
            game.ActionsAfterMoveSuccess(this.Copy().Board[target], game, engineControlled);
        }

        private bool TryAnCastling(string notation, IEnumerable<BasePiece> _pieces, Game game)
        {

            if (notation.Contains("0-0") || notation.Contains("O-O"))
            {
                var king = _pieces.Where(k => k is King).Single();
                // implement +
                if (notation == "0-0" || notation == "O-O")
                    game.TryMove(king, ConvertTo1D((king.PositionXY.X + 2, king.PositionXY.Y)));
                else
                    game.TryMove(king, ConvertTo1D((king.PositionXY.X - 2, king.PositionXY.Y)));
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



        private bool TryAnPromotion(Match match, IEnumerable<BasePiece> _pieces, int destination, Game game)
        {
            if (match.Groups["promotion"].Value != "")
                if (game.TryMove(_pieces.Single(), destination))
                {
                    Board[destination] = match.Groups["promotion"].Value switch
                    {
                        "=Q" => new Queen(destination, game.InverseColor()),
                        "=R" => new Rook(destination, game.InverseColor()),
                        "=N" => new Knight(destination, game.InverseColor()),
                        "=B" => new Bishop(destination, game.InverseColor()),
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

        private bool TryEndGame(string notation, Game game)
        {
            if (notation.Contains("1-0") || notation.Contains("0-1") || notation.Contains("½–½"))
            {
                game.Winner = notation switch
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

        public bool TryAlgebraicNotaionToMyNotaion(string notation, Game game)
        {
            var match = Regex.Match(notation, "^(?<piece>[KQRNB]?)(?<sourceX>[a-h]?)(?<sourceY>[1-8]?)(?<captures>x?)(?<destination>[a-h][1-8])(?<promotion>(=[QRNB])?)(?<check>\\+?)(?<mate>#?)$",
                    RegexOptions.Singleline & RegexOptions.ExplicitCapture);

            //_pieces where !=empty && correct color
            var _pieces = Board.Where(p => p != null && p.Color == game.CurrentPlayerColor);

            if (TryEndGame(notation, game))
                return true;
            if (TryAnCastling(notation, _pieces, game))
                return true;

            var destination = ConvertTo1D((ParseStringXToInt(match.Groups["destination"].Value), ParseStringYToInt(match.Groups["destination"].Value)));
            _pieces = GetCorrectPieceType(match, _pieces);
            _pieces = GetCorrectSourceX(match, _pieces);
            _pieces = GetCorrectSourceY(match, _pieces);
            _pieces = GetCorrectDestination(destination, _pieces);
            var pieces = _pieces.ToArray();

            if (TryAnPromotion(match, pieces, destination, game))
                return true;

            if (game.TryMove(pieces.Single(), destination))
                return true;
            else
                throw new Exception("error in TryMove() in AN");
        }

        public bool CheckMateDetection(Gameboard gameboard, PieceColor color)
        {
            foreach (BasePiece piece in gameboard.Board.Where(t => t != null && t.Color == color))
                if (piece.GetValidMovements(gameboard).Any())
                    return false;
            return true;
        }

        public string ParseToAN(BasePiece start, int target, Gameboard gameboard)
        {
            var targetXY = ConvertTo2D(target);
            //castling
            if (start is King && Math.Abs(start.PositionXY.X - targetXY.X) == 2)
            {
                if (start.PositionXY.X > targetXY.X)
                    return "O-O-O";
                return "O-O";
            }
            var _pieces = gameboard.Board.Where(p => p != null && p.Color == start.Color);

            //check & checkmate
            var kingDifferentColorPosition = gameboard.Board.Where(t => t != null && t.Color != start.Color && t is King).Single().Position;
            var check = !_pieces.SelectMany(t => t.GetValidMovements(gameboard)).Contains(kingDifferentColorPosition) ? string.Empty : "+";
            check = CheckMateDetection(gameboard, start.Color) ? "#" : check;

            //Correct piece type
            _pieces = _pieces.Where(s => s.Sprite == start.Sprite);

            //destination
            _pieces = _pieces.Where(d => d.GetValidMovements(gameboard).Contains(target));
            var pieceLetter = GetCorrectSpriteAbreviation(start.Value, start.Color);
            var promotionLetter = start is Pawn && (targetXY.Y == 0 || targetXY.Y == 7) ? GetCorrectSpriteAbreviation(_pieces.First().Value, start.Color) : string.Empty;
            var capturePiece = gameboard.Board[target] == null ? string.Empty : "x";

            //xy-position the same
            var ypieces = _pieces.Where(n => n.PositionXY.Y == start.PositionXY.Y).ToArray();
            var xpieces = _pieces.Where(m => m.PositionXY.X == start.PositionXY.X).ToArray();
            string sourceX = ParseIntToString(start.Position)[0].ToString();
            string sourceY = ParseIntToString(start.Position)[1].ToString();
            string source;
            if (xpieces.Length == 1 && ypieces.Length == 1)
                source = string.Empty;
            else if (ypieces.Length > 1 && xpieces.Length == 1)
                source = sourceX;
            else if (xpieces.Length > 1 && ypieces.Length == 1)
                source = sourceY;
            else
                source = sourceX + sourceY;

            string targetX = ParseIntToString(target)[0].ToString();
            string targetY = ParseIntToString(target)[1].ToString();

            //game end


            return $"{pieceLetter}{source}{capturePiece}{targetX}{targetY}{promotionLetter}{check}";
        }

        private string GetCorrectSpriteAbreviation(PieceValue value, PieceColor color)
        {
            if (color == PieceColor.White)
                return value switch
                {
                    PieceValue.King => "K",
                    PieceValue.Queen => "Q",
                    PieceValue.Rook => "R",
                    PieceValue.Bishop => "B",
                    PieceValue.Knight => "N",
                    PieceValue.Pawn => "",
                    _ => throw new Exception("error in GetCorrectSpriteAbreviation White")
                };
            return value switch
            {
                PieceValue.King => "k",
                PieceValue.Queen => "q",
                PieceValue.Rook => "r",
                PieceValue.Bishop => "b",
                PieceValue.Knight => "n",
                PieceValue.Pawn => "",
                _ => throw new Exception("error in GetCorrectSpriteAbreviation White")
            };
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

        public static string ParseIntToString(int index)
        {
            var x = index % 8;
            var y = index / 8;
            var yOut = (8 - y).ToString();
            var xOut = (char)('a' + x);
            return xOut + yOut;
        }

        public static int ParseStringYToInt(string input)
        {
            var c = input[input.Length - 1];
            if (c < '1' || c > '8')
                throw new Exception("out of bounds Y");
            return 55 - c + 1;
        }
    }
}

