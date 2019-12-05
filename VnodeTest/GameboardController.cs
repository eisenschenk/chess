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
        private PieceColor PlayerColor;
        private string Gameroom;
       

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
            var gameroomDisplay = Gameroom == default ? "Random Room" : $"Room {Gameroom}";
            return Div(
               Text("Player vs. AI Start", Styles.Btn & Styles.MX2, () => GameModeSelection(Gamemode.PvE)),
               Text("AI vs. AI Start", Styles.MX2 & Styles.Btn, () => GameModeSelection(Gamemode.EvE)),
               Row(
                   Text($"Player vs. Player Start/Enter {gameroomDisplay}", Styles.MX2 & Styles.Btn, () => GameModeSelection(Gamemode.PvP, Gameroom)),
                   Input(Gameroom, i => Gameroom = i, Styles.MX2, "Gameroom Nr.")
                )
            );
        }

        private Gameboard GetFittingBoard(int index, bool condition)
        {
            while (condition)
            {
                index++;
                if (index != 0)
                {
                    GameRepository.Instance.AddBoard(index, new Gameboard(TimeSpan.FromSeconds(3000000)));
                    break;
                }
            }
            GameRepository.Instance.TryGetBoard(index, out var board);
            Gameroom = index.ToString();

            return board;
        }

        private void GameModeSelection(Gamemode gamemode, string index = "0")
        {
            Int32.TryParse(index, out int _index);
            //human vs human
            if (gamemode == Gamemode.PvP)
            {
                if (!GameRepository.Instance.TryGetBoard(_index, out var board))
                    GameRepository.Instance.AddBoard(_index, board = new Gameboard(TimeSpan.FromSeconds(3000000)));
                else
                    board = GetFittingBoard(_index, GameRepository.Instance.TryGetBoard(_index, out var _board) && _board.HasPlayerBlack && _board.HasPlayerWhite);
                Gameboard = board;
                if (!Gameboard.HasPlayerWhite)
                {
                    PlayerColor = PieceColor.White;
                    Gameboard.HasPlayerWhite = true;
                }
                else
                {
                    PlayerColor = PieceColor.Black;
                    Gameboard.HasPlayerBlack = true;
                }
            }

            //ai vs ai
            else if (gamemode == Gamemode.EvE)
            {
                if (!GameRepository.Instance.TryGetBoard(_index, out var board))
                    GameRepository.Instance.AddBoard(_index, board = new Gameboard(TimeSpan.FromSeconds(3000000)));
                else
                    board = GetFittingBoard(_index, GameRepository.Instance.TryGetBoard(_index, out var _board) && (_board.HasPlayerWhite || _board.HasPlayerBlack));
                Gameboard = board;
                Gameboard.HasPlayerWhite = true;
                Gameboard.HasPlayerBlack = true;
                Gameboard.PlayedByEngine = (true, true);
                PlayerColor = PieceColor.White;
                Gameboard.TryEngineMove();
            }

            //human vs ai
            else if (gamemode == Gamemode.PvE)
            {
                if (!GameRepository.Instance.TryGetBoard(_index, out var board))
                    GameRepository.Instance.AddBoard(_index, board = new Gameboard(TimeSpan.FromSeconds(3000000)));
                else
                    board = GetFittingBoard(_index, GameRepository.Instance.TryGetBoard(_index, out var _board) && (_board.HasPlayerWhite || _board.HasPlayerBlack));
                Gameboard = board;
                Gameboard.HasPlayerWhite = true;
                Gameboard.HasPlayerBlack = true;
                Gameboard.PlayedByEngine = (false, true);
                PlayerColor = PieceColor.White;
            }
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

            if (PlayerColor == PieceColor.White)
                return Div(
                     Fragment(Enumerable.Range(0, 8)
                         .Select(rowx => Row(Gameboard.Board.Where(x => x.Position / 8 == rowx)
                         .Select(t => RenderTile(t))))),
                    Gameboard.PlayedByEngine.B == true || Gameboard.PlayedByEngine.W == true ? Text($"EngineMove: {Gameboard.EngineMove}") : null,
                    Text($"Time remaining White: {Gameboard.WhiteClock:hh\\:mm\\:ss}"),
                    Text($"Time remaining Black: {Gameboard.BlackClock:hh\\:mm\\:ss}"),
                    Text($"Gameroom: {Gameroom}")
                    );
            else
            {
                return Div(
                    Fragment(Enumerable.Range(0, 8).Reverse()
                        .Select(rowx => Row(Gameboard.Board.Where(x => x.Position / 8 == rowx)
                        .OrderByDescending(t => t.PositionXY.X)
                        .Select(t => RenderTile(t))))),
                    Gameboard.PlayedByEngine.B == true || Gameboard.PlayedByEngine.W == true ? Text($"EngineMove: {Gameboard.EngineMove}") : null,
                    Text($"Time remaining White: {Gameboard.WhiteClock:hh\\:mm\\:ss}"),
                    Text($"Time remaining Black: {Gameboard.BlackClock:hh\\:mm\\:ss}"),
                     Text($"Gameroom: {Gameroom}")
                   );
            }
        }

        //remember onclick
        private VNode RenderTile(Tile tile)
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
                case PieceColor.Black: winner = "Black won"; break;
                case PieceColor.White: winner = "White won"; break;
                case PieceColor.Zero: winner = "Draw"; break;
                default: winner = "error"; break;
            }
            return Text($"Gameover! {winner}");
        }

        private VNode RenderPromotionSelection()
        {
            Tile[] promotionSelect = new GameEntities.Tile[4];
            promotionSelect[0] = (new Tile(new Rook(0, Gameboard.Selected.Piece.Color), 0));
            promotionSelect[1] = (new Tile(new Knight(1, Gameboard.Selected.Piece.Color), 1));
            promotionSelect[2] = (new Tile(new Bishop(2, Gameboard.Selected.Piece.Color), 2));
            promotionSelect[3] = (new Tile(new Queen(3, Gameboard.Selected.Piece.Color), 3));
            return Div(
                Styles.M2,
                Text($"Select Piece you want the Pawn to be promoted to.", Styles.FontSize1p5),
                Row(Fragment(promotionSelect.Select(x => RenderTile(x))))
            );
        }

        private void Select(Tile target)
        {
            if (Gameboard.CurrentPlayerColor == PieceColor.White && Gameboard.PlayedByEngine.W == false && PlayerColor == PieceColor.White
                || Gameboard.CurrentPlayerColor == PieceColor.Black && Gameboard.PlayedByEngine.B == false && PlayerColor == PieceColor.Black)
            {

                // eigener Select für Promotion bzw. RenderTile mit Onclick param?
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
