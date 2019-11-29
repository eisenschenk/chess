using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    public class Gameboard
    {
        public Tile[] Board { get; set; } = new Tile[64];
        public bool Promotion { get; set; }
        public bool GameOver { get; set; }
        public PieceColor Winner { get; set; }
        public PieceColor CurrentPlayerColor { get; set; } = PieceColor.White;
        public Tile Selected { get; set; }
        public int EnPassantTarget { get; set; }
        public (BasePiece start, Tile target) Lastmove { get; set; }
        public bool EnPassantPossible { get; set; }

        public Tile this[int x, int y]
        {
            get => Board[y * 8 + x];
            set => Board[y * 8 + x] = value;
        }
        public Gameboard()
        {
            for (int index = 0; index < 64; index++)
                Board[index] = new Tile(index);
            PutPiecesInStartingPosition();
        }

        public Gameboard(IEnumerable<Tile> collection)
        {
            Board = collection.ToArray();
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
            //some enemy piece might be standing next to the king, rooks potentialmoves dont prevent castling ...
            if (target.ContainsPiece)
                if (target.Piece is Rook && start.Piece is King
                && !target.Piece.HasMoved && !start.Piece.HasMoved
                && start.Piece.Color == target.Piece.Color
                && (target.Piece.GetStraightLines(this).Contains(start.Piece.Position + 1)
                    || target.Piece.GetStraightLines(this).Contains(start.Piece.Position - 1)))
                {
                    int direction = 1;
                    if (start.Piece.Position > target.Piece.Position)
                        direction *= -1;

                    Board[start.Position + 2 * direction].Piece = start.Piece;
                    Board[start.Position + 2 * direction].Piece.Position = start.Position + 2 * direction;
                    Board[start.Position + direction].Piece = target.Piece;
                    Board[start.Position + direction].Piece.Position = start.Position + direction;

                    Board[start.Position].Piece = null;
                    Board[target.Position].Piece = null;
                    Selected = null;
                    return true;
                }
            return false;
        }

        public void CheckForPossibleEnPassant(Tile start, Tile target)
        {

            if (start.Piece is Pawn && Math.Abs(start.PositionXY.Y - target.PositionXY.Y) == 2
                && HasAdjacentEnemyPawn(target))
                EnPassantPossible = true;
        }

        private bool HasAdjacentEnemyPawn(Tile target)
        {
            static bool IsInBounds(int X)
            {
                if (X < 8 && X >= 0)
                    return true;
                return false;
            }
            for (int directionX = -1; directionX < 2; directionX += 2)
                if (IsInBounds(target.PositionXY.X + directionX) && Board[target.PositionXY.X + directionX].ContainsPiece
                    && this[target.PositionXY.X + directionX, target.PositionXY.Y].Piece is Pawn)
                    return true;
            return false;
        }

        public bool TryMove(Tile start, Tile target)
        {
            if (!TryCastling(start, target))
            {

                if (!start.Piece.GetValidMovements(this).Contains(target.Position))
                    return false;
                CheckForPossibleEnPassant(start, target);
                MovePiece(start, target);
                Selected = null;
                TryEnablePromotion(target);
                ChangeCurrentPlayer();
                CheckForGameOver();
                return true;
            }
            return false;
        }

        public void CheckForGameOver()
        {
            foreach (Tile tile in Board.Where(t => t.ContainsPiece && t.Piece.Color == CurrentPlayerColor))
                if (tile.Piece.GetValidMovements(this).Any())
                    return;
            GameOver = true;
        }

        private void MovePiece(Tile start, Tile target)
        {
            if (start.Piece is Pawn)
            {
                if (EnPassantPossible && target.Position == EnPassantTarget)
                    Board[Lastmove.target.Position].Piece = null;
                else if (Math.Abs(start.PositionXY.Y - target.PositionXY.Y) == 2)
                    EnPassantTarget = start.PositionXY.X + (start.Piece.Color == PieceColor.Black ? (start.PositionXY.Y + 1) * 8 : (start.PositionXY.Y - 1) * 8);
            }
            Lastmove = (start.Piece.Copy(), target.Copy());
            target.Piece = start.Piece;
            target.Piece.Position = target.Position;
            start.Piece = null;
        }

        public void ChangeCurrentPlayer()
        {
            if (CurrentPlayerColor == PieceColor.White)
                CurrentPlayerColor = PieceColor.Black;
            else
                CurrentPlayerColor = PieceColor.White;
        }

        public void TryEnablePromotion(Tile tile)
        {
            if (tile.Piece is Pawn && (tile.Piece.Position > 55 || tile.Piece.Position < 7))
            {
                Selected = tile;
                Promotion = true;
            }
        }

        private bool TryAnCastling(string notation, IEnumerable<Tile> _pieces)
        {

            if (notation.Contains("0-0") || notation.Contains("O-O"))
            {
                var king = _pieces.Where(k => k.Piece is King).Single();
                if (notation == "0-0" || notation == "O-O")
                    TryCastling(king, this[king.Piece.PositionXY.X + 3, king.Piece.PositionXY.Y]);
                else
                    TryCastling(king, this[king.Piece.PositionXY.X - 4, king.Piece.PositionXY.Y]);
                ChangeCurrentPlayer();
                CheckForGameOver();
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
                        _ => default
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

        private bool TryEndGame(string notation, IEnumerable<Tile> _pieces)
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
                GameOver = true;
                return true;
            }
            return false;

        }

        public bool TryANtoMyN(string notation)
        {
            var match = Regex.Match(notation, "^(?<piece>[KQRNB]?)(?<sourceX>[a-h]?)(?<sourceY>[1-8]?)(?<captures>x?)(?<destination>[a-h][1-8])(?<promotion>(=[QRNB])?)(?<check>\\+?)(?<mate>#?)$",
                    RegexOptions.Singleline & RegexOptions.ExplicitCapture);

            //_pieces where !=empty && correct color
            var _pieces = Board.Where(p => p.ContainsPiece && p.Piece.Color == CurrentPlayerColor);
            if (TryEndGame(notation, _pieces))
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


        private int ParseStringXToInt(string input)
        {
            var c = input[0];
            if (c < 'a' || c > 'h')
                throw new Exception("out of bounds X");
            return c - 'a';
        }
        private int ParseStringYToInt(string input)
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

