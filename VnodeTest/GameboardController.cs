using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ACL.UI.React.DOM;

namespace VnodeTest
{
    public class GameboardController
    {
        public GameEntities.Gameboard GameBoard;
        private GameEntities.Tile Selected;
        private bool Promotion;
        private bool GameOver;
        private GameEntities.PieceColor CurrentPlayerColor = GameEntities.PieceColor.White;

        public GameboardController()
        {
            GameBoard = new GameEntities.Gameboard();
        }

        public VNode Render()
        {
            if (GameOver)
                return RenderGameOver();
            return Div(
                Promotion ? RenderPromotionSelection() : RenderBoard(GameBoard)
            );
        }
        //remember Enumerable range
        private VNode RenderBoard(GameEntities.Gameboard board)
        {
            return Fragment(Enumerable.Range(0, 8)
                .Select(rowx => Row(board.Board.Where(x => x.Position / 8 == rowx)
                    .Select(t => RenderTile(t)))));
        }

        //remember onclick
        private VNode RenderTile(GameEntities.Tile tile)
        {
            return Div(
                tile.Style & (tile == Selected ? Styles.Selected : tile.BorderStyle),
                () => Select(tile),
                tile.ContainsPiece ? Text(tile.Piece.Sprite, Styles.FontSize3) : null
            );
        }

        private VNode RenderGameOver()
        {
            string winner;
            switch (CurrentPlayerColor)
            {
                case GameEntities.PieceColor.Black: winner = "White"; break;
                case GameEntities.PieceColor.White: winner = "Black"; break;
                case GameEntities.PieceColor.Zero: winner = "error"; break;
                default: winner = "error"; break;
            }
            return Text($"Gameover! {winner} won");
        }

        private VNode RenderPromotionSelection()
        {
            List<GameEntities.Tile> promotionSelect = new List<GameEntities.Tile>();
            promotionSelect.Add(new GameEntities.Tile(new GameEntities.Rook(0, Selected.Piece.Color), 0));
            promotionSelect.Add(new GameEntities.Tile(new GameEntities.Knight(1, Selected.Piece.Color), 1));
            promotionSelect.Add(new GameEntities.Tile(new GameEntities.Bishop(2, Selected.Piece.Color), 2));
            promotionSelect.Add(new GameEntities.Tile(new GameEntities.Queen(3, Selected.Piece.Color), 3));
            return Div(
                Styles.M2,
                Text($"Select Piece you want the Pawn to be promoted to.", Styles.FontSize1p5),
                Row(Fragment(promotionSelect.Select(x => RenderTile(x))))
            );
        }

        private void Select(GameEntities.Tile target)
        {
            // eigener Select für Promotion bzw. RenderTile mit Onclick param
            if (Promotion == true)
            {
                GameBoard.Board[Selected.Position].Piece = target.Piece;
                GameBoard.Board[Selected.Position].Piece.Position = Selected.Position;
                Selected = null;
                Promotion = false;
                return;
            }
            if (Selected == null && target.ContainsPiece /*&& target.Piece.Color == CurrentPlayerColor*/)
                Selected = target;
            else if (Selected == target)
                Selected = null;
            else if (Selected != null)
                if (TryMove(Selected, target))
                {
                    TryEnablePromotion(target);
                    ChangeCurrentPlayer();
                    CheckForGameOver();
                }
        }

        private bool TryCastling(GameEntities.Tile start, GameEntities.Tile target)
        {
            //some enemy piece might be standing next to the king, rooks potentialmoves dont prevent castling ...
            if (target.ContainsPiece)
                if (target.Piece is GameEntities.Rook && start.Piece is GameEntities.King
                && !target.Piece.HasMoved && !start.Piece.HasMoved
                && start.Piece.Color == target.Piece.Color
                && (target.Piece.GetStraightLines(GameBoard).Contains(start.Piece.Position + 1)
                    || target.Piece.GetStraightLines(GameBoard).Contains(start.Piece.Position - 1)))
                {
                    int direction = 1;
                    if (start.Piece.Position > target.Piece.Position)
                        direction *= -1;

                    GameBoard.Board[start.Position + 2 * direction].Piece = start.Piece;
                    GameBoard.Board[start.Position + 2 * direction].Piece.Position = start.Position + 2 * direction;
                    GameBoard.Board[start.Position + direction].Piece = target.Piece;
                    GameBoard.Board[start.Position + direction].Piece.Position = start.Position + direction;

                    GameBoard.Board[start.Position].Piece = null;
                    GameBoard.Board[target.Position].Piece = null;
                    Selected = null;
                    return true;
                }
            return false;
        }

        private bool TryMove(GameEntities.Tile start, GameEntities.Tile target)
        {
            if (!TryCastling(start, target))
            {
                if (!start.Piece.GetValidMovements(GameBoard).Contains(target.Position) || OwnKingIsCheckedAfterMove(start, target))
                    return false;
                MovePiece(start, target);
                Selected = null;
                return true;
            }
            return true;
        }

        private void CheckForGameOver()
        {
            foreach (GameEntities.Tile tile in GameBoard.Board.Where(t => t.ContainsPiece && t.Piece.Color == CurrentPlayerColor))
                foreach (int potentialmove in tile.Piece.GetValidMovements(GameBoard))
                    if (!OwnKingIsCheckedAfterMove(tile, GameBoard.Board[potentialmove]))
                        return;
            GameOver = true;
        }

        private void MovePiece(GameEntities.Tile start, GameEntities.Tile target)
        {
            target.Piece = start.Piece;
            target.Piece.Position = target.Position;
            start.Piece = null;
        }

        private void ChangeCurrentPlayer()
        {
            if (CurrentPlayerColor == GameEntities.PieceColor.White)
                CurrentPlayerColor = GameEntities.PieceColor.Black;
            else
                CurrentPlayerColor = GameEntities.PieceColor.White;
        }

        private void TryEnablePromotion(GameEntities.Tile tile)
        {
            if (tile.Piece is GameEntities.Pawn && (tile.Piece.Position > 55 || tile.Piece.Position < 7))
            {
                Selected = tile;
                Promotion = true;
            }
        }

        private bool OwnKingIsCheckedAfterMove(GameEntities.Tile source, GameEntities.Tile targetTile)
        {
            GameEntities.Tile start = source.Copy();
            GameEntities.Tile target = targetTile.Copy();
            var futureGameBoard = GameBoard.Copy();
            futureGameBoard.Board[target.Position].Piece = start.Piece;
            futureGameBoard.Board[target.Position].Piece.Position = target.Position;
            futureGameBoard.Board[start.Position].Piece = null;
            var kingSameColorPosition = futureGameBoard.Board
                .Where(t => t.ContainsPiece && t.Piece.Color == start.Piece.Color && t.Piece is GameEntities.King)
                .First().Piece.Position;
            var enemyPieces = futureGameBoard.Board.Where(x => x.ContainsPiece && x.Piece.Color != start.Piece.Color);

            if (enemyPieces.SelectMany(t => t.Piece.GetValidMovements(futureGameBoard)).Contains(kingSameColorPosition))
                return true;
            return false;
        }

    }
}
