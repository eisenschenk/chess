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
        public Tile[] Board = new Tile[64];
        public bool Promotion;
        public bool GameOver;
        public PieceColor Winner;
        public PieceColor CurrentPlayerColor = PieceColor.White;
        public Tile Selected;
        //public (int X, int Y) PositionXY
        //{
        //    get
        //    {
        //        return (X: Board[])
        //    }
        //}
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
                Board[pawns].Piece = new Pawn(pawns, PieceColor.White);
            Board[0].Piece = new Rook(0, PieceColor.White);
            Board[1].Piece = new Knight(1, PieceColor.White);
            Board[2].Piece = new Bishop(2, PieceColor.White);
            Board[3].Piece = new Queen(3, PieceColor.White);
            Board[4].Piece = new King(4, PieceColor.White);
            Board[5].Piece = new Bishop(5, PieceColor.White);
            Board[6].Piece = new Knight(6, PieceColor.White);
            Board[7].Piece = new Rook(7, PieceColor.White);

            for (int pawns = 48; pawns < 56; pawns++)
                Board[pawns].Piece = new Pawn(pawns, PieceColor.Black);
            Board[56].Piece = new Rook(56, PieceColor.Black);
            Board[57].Piece = new Knight(57, PieceColor.Black);
            Board[58].Piece = new Bishop(58, PieceColor.Black);
            Board[59].Piece = new Queen(59, PieceColor.Black);
            Board[60].Piece = new King(60, PieceColor.Black);
            Board[61].Piece = new Bishop(61, PieceColor.Black);
            Board[62].Piece = new Knight(62, PieceColor.Black);
            Board[63].Piece = new Rook(63, PieceColor.Black);
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

        public bool TryMove(Tile start, Tile target)
        {
            if (!start.Piece.GetValidMovements(this).Contains(target.Position) || OwnKingIsCheckedAfterMove(start, target))
                return false;
            MovePiece(start, target);
            Selected = null;

            return true;
        }

        public void CheckForGameOver()
        {
            foreach (Tile tile in Board.Where(t => t.ContainsPiece && t.Piece.Color == CurrentPlayerColor))
                foreach (int potentialmove in tile.Piece.GetValidMovements(this))
                    if (!OwnKingIsCheckedAfterMove(tile, Board[potentialmove]))
                        return;
            GameOver = true;
        }

        private void MovePiece(Tile start, Tile target)
        {
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

        private bool OwnKingIsCheckedAfterMove(Tile source, Tile targetTile)
        {
            Tile start = source.Copy();
            Tile target = targetTile.Copy();
            var futureGameBoard = this.Copy();
            futureGameBoard.Board[target.Position].Piece = start.Piece;
            futureGameBoard.Board[target.Position].Piece.Position = target.Position;
            futureGameBoard.Board[start.Position].Piece = null;
            var kingSameColorPosition = futureGameBoard.Board
                .Where(t => t.ContainsPiece && t.Piece.Color == start.Piece.Color && t.Piece is King)
                .First().Piece.Position;
            var enemyPieces = futureGameBoard.Board.Where(x => x.ContainsPiece && x.Piece.Color != start.Piece.Color);

            if (enemyPieces.SelectMany(t => t.Piece.GetValidMovements(futureGameBoard)).Contains(kingSameColorPosition))
                return true;
            return false;
        }

        private bool TryAnCastling(Match match, IEnumerable<Tile> _pieces)
        {

            if (match.Value.Contains("0-0"))
            {
                var king = _pieces.Where(k => k.Piece is King).Single();
                if (match.Value == "0-0")
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
                _ => v.Piece is BasePiece
            }));
            return _pieces;
        }

        private IEnumerable<Tile> GetCorrectSourceX(Match match, IEnumerable<Tile> _pieces)
        {
            _pieces = _pieces.Where(x =>
            match.Groups["sourceX"].Value != null
            ? x.Piece.PositionXY.X == ParseStringXToInt(match.Groups["sourceX"].Value)
            : x.ContainsPiece);
            return _pieces;
        }

        private IEnumerable<Tile> GetCorrectSourceY(Match match, IEnumerable<Tile> _pieces)
        {
            _pieces = _pieces.Where(y =>
            match.Groups["sourceY"].Value != null
            ? y.Piece.PositionXY.Y == ParseStringYToInt(match.Groups["sourceY"].Value)
            : y.ContainsPiece);
            return _pieces;
        }

        private bool TryAnPromotion(Match match, IEnumerable<Tile> _pieces, Tile destination)
        {
            if (match.Groups["promotion"].Value != null)
                if (TryMove(_pieces.Single(), destination))
                {

                    Board[destination.Position].Piece = match.Groups["promotion"].Value switch
                    {
                        "=Q" => new Queen(destination.Position, CurrentPlayerColor),
                        "=R" => new Rook(destination.Position, CurrentPlayerColor),
                        "=N" => new Knight(destination.Position, CurrentPlayerColor),
                        "=B" => new Bishop(destination.Position, CurrentPlayerColor),
                        _ => default
                    };
                    ChangeCurrentPlayer();
                    CheckForGameOver();
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

        private bool TryEndGame(Match match, IEnumerable<Tile> _pieces)
        {
            if (match.Value.Contains("1-0") || match.Value.Contains("0-1") || match.Value.Contains("½–½"))
            {
                Winner = match.Value switch
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
            var destination = this[ParseStringXToInt(match.Groups["destination"].Value), ParseStringYToInt(match.Groups["destination"].Value)];

            //_pieces where !=empty && correct color
            var _pieces = Board.Where(p => p.ContainsPiece && p.Piece.Color == CurrentPlayerColor);
            if (TryEndGame(match, _pieces))
                return true;
            if (TryAnCastling(match, _pieces))
                return true;

            _pieces = GetCorrectPieceType(match, _pieces);
            _pieces = GetCorrectSourceX(match, _pieces);
            _pieces = GetCorrectSourceY(match, _pieces);
            _pieces = GetCorrectDestination(destination, _pieces);

            if (TryAnPromotion(match, _pieces, destination))
                return true;

            if (TryMove(_pieces.Single(), destination))
            {
                ChangeCurrentPlayer();
                CheckForGameOver();
                return true;
            }
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
            var c = input[1];
            if (c < '1' || c > '8')
                throw new Exception("out of bounds Y");
            return c - '1';
        }

    }
}

