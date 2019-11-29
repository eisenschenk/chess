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



        public GameboardController()
        {
            GameBoard = new GameEntities.Gameboard();
        }

        public VNode Render()
        {
            if (GameBoard.GameOver)
                return RenderGameOver();
            return Div(
                GameBoard.Promotion ? RenderPromotionSelection() : RenderBoard(GameBoard)
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
                tile.Style & (tile == GameBoard.Selected ? Styles.Selected : tile.BorderStyle),
                () => Select(tile),
                tile.ContainsPiece ? Text(tile.Piece.Sprite, Styles.FontSize3) : null
            );
        }

        private VNode RenderGameOver()
        {
            string winner;
            switch (GameBoard.Winner)
            {
                case GameEntities.PieceColor.Black: winner = "Black won"; break;
                case GameEntities.PieceColor.White: winner = "White won"; break;
                case GameEntities.PieceColor.Zero: winner = "Draw"; break;
                default: winner = "error"; break;
            }
            return Text($"Gameover! {winner}");
        }

        private VNode RenderPromotionSelection()
        {
            GameEntities.Tile[] promotionSelect = new GameEntities.Tile[4];
            promotionSelect[0] = (new GameEntities.Tile(new GameEntities.Rook(0, GameBoard.Selected.Piece.Color), 0));
            promotionSelect[1] = (new GameEntities.Tile(new GameEntities.Knight(1, GameBoard.Selected.Piece.Color), 1));
            promotionSelect[2] = (new GameEntities.Tile(new GameEntities.Bishop(2, GameBoard.Selected.Piece.Color), 2));
            promotionSelect[3] = (new GameEntities.Tile(new GameEntities.Queen(3, GameBoard.Selected.Piece.Color), 3));
            return Div(
                Styles.M2,
                Text($"Select Piece you want the Pawn to be promoted to.", Styles.FontSize1p5),
                Row(Fragment(promotionSelect.Select(x => RenderTile(x))))
            );
        }

        private void Select(GameEntities.Tile target)
        {
            // eigener Select für Promotion bzw. RenderTile mit Onclick param
            if (GameBoard.Promotion == true)
            {
                GameBoard.Board[GameBoard.Selected.Position].Piece = target.Piece;
                GameBoard.Board[GameBoard.Selected.Position].Piece.Position = GameBoard.Selected.Position;
                GameBoard.Selected = null;
                GameBoard.Promotion = false;
                return;
            }
            if (GameBoard.Selected == null && target.ContainsPiece /*&& target.Piece.Color == CurrentPlayerColor*/)
                GameBoard.Selected = target;
            else if (GameBoard.Selected == target)
                GameBoard.Selected = null;
            else if (GameBoard.Selected != null)
                GameBoard.TryMove(GameBoard.Selected, target);

        }
    }
}
