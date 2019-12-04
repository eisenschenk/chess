using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VnodeTest.GameEntities;
using static ACL.UI.React.DOM;

namespace VnodeTest
{
    public class GameboardController
    {
        private Gameboard Gameboard;
        private VNode RefreshReference;

        public GameboardController()
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                while (true)
                {
                    Thread.Sleep(100);
                    Gameboard?.UpdateClocks();
                    RefreshReference?.Refresh();
                }
            });
        }

        private VNode RenderGameModeSelection()
        {
            return Div(
                Button("Player vs. Player (Offline)", () => GameModeSelection((false, false))),
                Button("Player vs. AI (Offline)", () => GameModeSelection((false, true))),
                Button("AI vs. AI (Offline)", () => GameModeSelection((true, true))),
                Button("Player vs. Player (Online)", () => GameModeSelection((false, false), false))
            );
        }

        private void GameModeSelection((bool W, bool B) engineControlled, bool online = false)
        {
            if (!GameRepository.Instance.TryGetBoard(0, out var board))
                GameRepository.Instance.AddBoard(0, board = new Gameboard(TimeSpan.FromSeconds(3000000)));
            Gameboard = board;
            Gameboard.PlayedByEngine = engineControlled;
        }

        public VNode Render()
        {
            return RefreshReference = RenderBoard();
        }

        //remember Enumerable range
        private VNode RenderBoard()
        {
            if (Gameboard == default)
                return RenderGameModeSelection();
            if (Gameboard.GameOver)
                return RenderGameOver();
            if (Gameboard.Promotion)
                return RenderPromotionSelection();

            //VNode Board()
            //{
            //    if (!RenderRemote)
            //        return Fragment(Enumerable.Range(0, 8)
            //         .Select(rowx => Row(board.Board.Where(x => x.Position / 8 == rowx)
            //         .Select(t => RenderTile(t)))));
            //    return Fragment(Enumerable.Range(0, 8)
            //       .Select(rowx => Row(board.Board.Where(x => x.Position / 8 == rowx)
            //       .OrderByDescending(x => x.Position / 8)
            //       .Select(t => RenderTile(t)))));
            //}
            return Div(
                 Fragment(Enumerable.Range(0, 8)
                     .Select(rowx => Row(Gameboard.Board.Where(x => x.Position / 8 == rowx)
                     .Select(t => RenderTile(t))))),
                Text($"EngineMove: {Gameboard.EngineMove}"),
                Text($"Time remaining White: {Gameboard.WhiteClock}"),
                Text($"Time remaining Black: {Gameboard.BlackClock}")
                );
        }

        //remember onclick
        private VNode RenderTile(GameEntities.Tile tile)
        {
            Style lastMove = null;
            if (Gameboard.Lastmove.start != null
                && (tile.Position == Gameboard.Lastmove.start.Position || tile.Position == Gameboard.Lastmove.target.Position))
                lastMove = Styles.BCred;
            return Div(
                 tile.Style & (tile == Gameboard.Selected ? Styles.Selected : tile.BorderStyle) & lastMove,
                () => Select(tile),
                tile.ContainsPiece ? Text(tile.Piece.Sprite, Styles.FontSize3) : null
            );
        }

        private VNode RenderGameOver()
        {
            string winner;
            switch (Gameboard.Winner)
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
            promotionSelect[0] = (new GameEntities.Tile(new GameEntities.Rook(0, Gameboard.Selected.Piece.Color), 0));
            promotionSelect[1] = (new GameEntities.Tile(new GameEntities.Knight(1, Gameboard.Selected.Piece.Color), 1));
            promotionSelect[2] = (new GameEntities.Tile(new GameEntities.Bishop(2, Gameboard.Selected.Piece.Color), 2));
            promotionSelect[3] = (new GameEntities.Tile(new GameEntities.Queen(3, Gameboard.Selected.Piece.Color), 3));
            return Div(
                Styles.M2,
                Text($"Select Piece you want the Pawn to be promoted to.", Styles.FontSize1p5),
                Row(Fragment(promotionSelect.Select(x => RenderTile(x))))
            );
        }

        private void Select(GameEntities.Tile target)
        {
            if (Gameboard.CurrentPlayerColor == GameEntities.PieceColor.White && Gameboard.PlayedByEngine.W == false
                || Gameboard.CurrentPlayerColor == GameEntities.PieceColor.Black && Gameboard.PlayedByEngine.B == false)
            {

                // eigener Select für Promotion bzw. RenderTile mit Onclick param
                if (Gameboard.Promotion == true)
                {
                    Gameboard.Board[Gameboard.Selected.Position].Piece = target.Piece;
                    Gameboard.Board[Gameboard.Selected.Position].Piece.Position = Gameboard.Selected.Position;
                    Gameboard.Selected = null;
                    Gameboard.Promotion = false;
                    return;
                }
                if (Gameboard.Selected == null && target.ContainsPiece && target.Piece.Color == Gameboard.CurrentPlayerColor)
                    Gameboard.Selected = target;
                else if (Gameboard.Selected == target)
                    Gameboard.Selected = null;
                else if (Gameboard.Selected != null)
                    Gameboard.TryMove(Gameboard.Selected, target);
            }
        }

    }
}
