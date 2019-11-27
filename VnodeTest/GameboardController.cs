using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ACL.UI.React.DOM;

namespace VnodeTest
{
    //Game doesnt finish right now
    public class GameboardController
    {
        public GameEntities.Gameboard GameBoard;
        private VNode[] GameRows = new VNode[8];
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

        private VNode RenderBoard(GameEntities.Gameboard board)
        {
            for (int indexRow = 0; indexRow < 8; indexRow++)
                GameRows[indexRow] = Row(board.Board.Where(x => x.Position / 8 == indexRow).Select(t => RenderTile(t)));
            return Fragment(GameRows.Select(x => x));
        }

        private VNode RenderTile(GameEntities.Tile tile)
        {
            var div = Div(
                tile.Style & (tile == Selected ? Styles.Selected : tile.BorderStyle),
                tile.ContainsPiece ? Text(tile.Piece.Sprite, Styles.FontSize3) : null
            );
            div.OnClick = () => Select(tile);
            return div;
        }

        private VNode RenderGameOver()
        {
            return Div();
        }

        private void Select(GameEntities.Tile tile)
        {
            if (Promotion == true)
            {
                GameBoard.Board[Selected.Position].Piece = tile.Piece;
                GameBoard.Board[Selected.Position].Piece.Position = Selected.Position;
                Selected = null;
                Promotion = false;
                return;
            }
            if (Selected == null && tile.ContainsPiece && tile.Piece.Color == CurrentPlayerColor)
                Selected = tile;
            else if (Selected == tile)
                Selected = null;
            else if (Selected != null)
            {
                TryCastling(tile);
                TryMove(Selected, tile);
            }
        }

        private bool TryCastling(GameEntities.Tile target)
        {
            if (target.ContainsPiece)
                if (target.Piece is GameEntities.Rook && Selected.Piece is GameEntities.King
                && !target.Piece.HasMoved && !Selected.Piece.HasMoved
                && Selected.Piece.Color == target.Piece.Color
                && target.Piece.Position == target.Piece.StartPosition
                && Selected.Piece.Position == Selected.Piece.StartPosition
                && (target.Piece.GetStraightLines(GameBoard).Contains(Selected.Piece.Position + 1)
                || target.Piece.GetStraightLines(GameBoard).Contains(Selected.Piece.Position - 1)))
                {
                    int direction = 1;
                    if (Selected.Piece.Position > target.Piece.Position)
                        direction *= -1;

                    GameBoard.Board[Selected.Position + 2 * direction].Piece = Selected.Piece;
                    GameBoard.Board[Selected.Position + 2 * direction].Piece.Position = Selected.Position + 2 * direction;
                    GameBoard.Board[Selected.Position + direction].Piece = target.Piece;
                    GameBoard.Board[Selected.Position + direction].Piece.Position = Selected.Position + direction;

                    GameBoard.Board[Selected.Position].Piece = null;
                    GameBoard.Board[target.Position].Piece = null;
                    Selected = null;
                    return true;
                }
            return false;
        }

        private bool TryMove(GameEntities.Tile start, GameEntities.Tile target)
        {
            if (Selected != null)
            {
                if (!start.Piece.GetValidMovements(GameBoard).Contains(target.Position) || OwnKingIsCheckedAfterMove(start, target))
                    return false;
                target.Piece = start.Piece;
                target.Piece.Position = target.Position;
                start.Piece = null;
                Selected = null;
                TryEnablePromotion(target);
                if (CurrentPlayerColor == GameEntities.PieceColor.White)
                    CurrentPlayerColor = GameEntities.PieceColor.Black;
                else
                    CurrentPlayerColor = GameEntities.PieceColor.White;
                return true;
            }
            return false;
        }

        private void TryEnablePromotion(GameEntities.Tile tile)
        {
            if (tile.Piece is GameEntities.Pawn && (tile.Piece.Position > 55 || tile.Piece.Position < 7))
            {
                Selected = tile;
                Promotion = true;
            }
        }

        private bool OwnKingIsCheckedAfterMove(GameEntities.Tile s, GameEntities.Tile t)
        {
            GameEntities.Tile start = s.Copy();
            GameEntities.Tile target = t.Copy();
            var futureGameBoard = GameBoard.Copy();
            var potentialmoves = new List<int>();
            futureGameBoard.Board[target.Position].Piece = start.Piece;
            futureGameBoard.Board[target.Position].Piece.Position = target.Position;
            futureGameBoard.Board[start.Position].Piece = null;
            var kingSameColorPosition = futureGameBoard.Board
                .Where(t => t.ContainsPiece && t.Piece.Color == start.Piece.Color && t.Piece is GameEntities.King)
                .First().Piece.Position;
            var enemyPieces = futureGameBoard.Board.Where(x => x.ContainsPiece && x.Piece.Color != start.Piece.Color);
            foreach (GameEntities.Tile tile in enemyPieces)
                potentialmoves.AddRange(tile.Piece.GetValidMovements(futureGameBoard));
            if (potentialmoves.Contains(kingSameColorPosition))
                return true;
            return false;
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

    }
}
