using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ACL.UI.React.DOM;

namespace VnodeTest
{
    public class GameboardController
    {
        public GameEntities.Gameboard GameBoard;
        public VNode RefreshReference;


        public GameboardController()
        {
            GameBoard = new GameEntities.Gameboard();
            ThreadPool.QueueUserWorkItem(o =>
            {
                while (true)
                    RefreshReference?.Refresh(TimeSpan.FromSeconds(1));
            });
        }


        public VNode Render()
        {
            if (GameBoard.GameOver)
                return RenderGameOver();
            return RefreshReference = Div(
                GameBoard.Promotion ? RenderPromotionSelection() : RenderBoard(GameBoard)
            );
        }
        //remember Enumerable range
        private VNode RenderBoard(GameEntities.Gameboard board)
        {
            return Div(
                Fragment(Enumerable.Range(0, 8)
                    .Select(rowx => Row(board.Board.Where(x => x.Position / 8 == rowx)
                    .Select(t => RenderTile(t))))),
                Text($"EngineMove: {GameBoard.EngineMove}"),
                Text($"Time remaining White: {GameBoard.GameClockWhite.ToString()}"),
                Text($"Time remaining Black: {GameBoard.GameClockBlack.ToString()}")
                );
        }

        //remember onclick
        private VNode RenderTile(GameEntities.Tile tile)
        {
            Style lastMove = null;
            if (GameBoard.Lastmove.start != null
                && (tile.Position == GameBoard.Lastmove.start.Position || tile.Position == GameBoard.Lastmove.target.Position))
                lastMove = Styles.BCred;
            return Div(
                 tile.Style & (tile == GameBoard.Selected ? Styles.Selected : tile.BorderStyle) & lastMove,
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
            if (GameBoard.CurrentPlayerColor == GameEntities.PieceColor.White && GameBoard.PlayedByEngine.W == false
                || GameBoard.CurrentPlayerColor == GameEntities.PieceColor.Black && GameBoard.PlayedByEngine.B == false)
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
                if (GameBoard.Selected == null && target.ContainsPiece && target.Piece.Color == GameBoard.CurrentPlayerColor)
                    GameBoard.Selected = target;
                else if (GameBoard.Selected == target)
                    GameBoard.Selected = null;
                else if (GameBoard.Selected != null)
                    if (GameBoard.TryMove(GameBoard.Selected, target))
                        ThreadPool.QueueUserWorkItem(o =>
                        {
                            if (GameBoard.PlayedByEngine.B && GameBoard.CurrentPlayerColor == GameEntities.PieceColor.Black)
                                GameBoard.TryEngineMove();
                            if (GameBoard.PlayedByEngine.W && GameBoard.CurrentPlayerColor == GameEntities.PieceColor.White)
                                GameBoard.TryEngineMove();
                        });

            }
        }

    }
}
