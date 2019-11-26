using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ACL.UI.React.DOM;

namespace VnodeTest
{
    //TODO turn change between black and white, maybe show currently viable color to make next move
    //TODO implement Castling
    //Game doesnt finish right now
    public class GameboardController
    {
        public GameEntities.Gameboard GameBoard;
        private VNode[] GameRows = new VNode[8];
        private GameEntities.Tile Selected;
        private bool Promotion;
        public GameboardController()
        {
            GameBoard = new GameEntities.Gameboard();
        }

        public VNode Render()
        {
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
            if (Selected == null && tile.ContainsPiece)
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
            if (target.Piece is GameEntities.Rook && Selected.Piece is GameEntities.King
                && Selected.Piece.Color == target.Piece.Color
                && target.Piece.Position == target.Piece.StartPosition
                && Selected.Piece.Position == Selected.Piece.StartPosition
                && target.Piece.GetStraightLines(GameBoard).Contains(Selected.Piece.Position + 1)
                || target.Piece.GetStraightLines(GameBoard).Contains(Selected.Piece.Position - 1))
            {
                if (Selected.Piece.Position > target.Piece.Position)
                {
                    GameBoard.Board[Selected.Position - 2] = Selected;
                    GameBoard.Board[Selected.Position - 1] = target;
                    Selected = null;
                }
                GameBoard.Board[Selected.Position + 2] = Selected;
                GameBoard.Board[Selected.Position + 1] = target;
                Selected = null;

            }

            //public bool TryCastling(Gameboard gameboard, int currentTarget)
            //{
            //    var target = gameboard.Board[currentTarget].Piece;
            //    if ((target is King && this is Rook)
            //        || (target is Rook && this is King)
            //        && target.Position == target.StartPosition
            //        && Position == StartPosition)
            //    {

            //    }

            //}
        }

        private bool TryMove(GameEntities.Tile start, GameEntities.Tile target)
        {
            if (!start.Piece.GetValidMovements(GameBoard).Contains(target.Position) || OwnKingIsCheckedAfterMove(start, target))
                return false;
            target.Piece = start.Piece;
            target.Piece.Position = target.Position;
            start.Piece = null;
            Selected = null;
            TryEnablePromotion(target);
            return true;
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
