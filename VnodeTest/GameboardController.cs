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
        private Game Game;
        private Gameboard Gameboard => Game != null ? Game.Gameboard : null;
        private int Gameroom;
        private VNode RefreshReference;
        private PieceColor PlayerColor;
        private IEngine Engine;
        private string Enginemove;
        public BasePiece Selected { get; set; }
        private BasePiece[] PromotionSelect = new BasePiece[4];


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
                Text("Player vs. AI Start", Styles.Btn & Styles.MX2, () => SelectGameMode(Gamemode.PvE)),
                Text("AI vs. AI Start", Styles.MX2 & Styles.Btn, () => SelectGameMode(Gamemode.EvE)),
                Row(
                    Text($"Player vs. Player Start/Enter {gameroomDisplay}", Styles.MX2 & Styles.Btn, () => SelectGameMode(Gamemode.PvP, Gameroom.ToString())),
                    Input(Gameroom, i => Gameroom = i, Styles.MX2, " Select Gameroom Nr.")
                ),
                GameRepository.Instance.Keys.Any()
                ? Div(
                    Text("Open Games: "),
                    Fragment(GameRepository.Instance.Keys.Select(key => Text(key.ToString())))
                )
                : null
            );
        }


        private Game GetFittingGame(Gamemode gamemode, string _key)
        {
            Game game;
            int key = int.Parse(_key);
            while (GameRepository.Instance.TryGetGame(key, out game))
            {
                if (!game.HasOpenSpots)
                    key++;
                else
                    break;
            }
            if (game == default)
                game = GameRepository.Instance.AddGame(key, gamemode, new Gameboard(TimeSpan.FromSeconds(30000000)));

            return game;
        }
        private void SelectGameMode(Gamemode gamemode, string index = "0")
        {
            Game = GetFittingGame(gamemode, index);
            //PvP
            if (gamemode == Gamemode.PvP)
            {
                if (!Game.HasWhitePlayer)
                {
                    PlayerColor = PieceColor.White;
                    Game.HasWhitePlayer = true;
                }
                else
                {
                    PlayerColor = PieceColor.Black;
                    Game.HasBlackPlayer = true;
                }
            }
            //EvE && PvE
            else if (gamemode == Gamemode.EvE || gamemode == Gamemode.PvE)
            {
                Engine = new EngineControl();
                Game.HasWhitePlayer = true;
                Game.HasBlackPlayer = true;
                PlayerColor = PieceColor.White;
                if (gamemode == Gamemode.EvE)
                    ThreadPool.QueueUserWorkItem(o =>
                    {
                        while (!Gameboard.GameOver)
                            Gameboard.TryEngineMove(Enginemove = Engine.GetEngineMove(Gameboard.GetFeNotation()), Game.PlayedByEngine);
                    });
            }
        }

        public VNode Render()
        {
            return RefreshReference = RenderGameBoard();
        }

        private VNode RenderGameBoard()
        {
            if (Gameboard == default)
                return RenderGameModeSelection();
            if (Gameboard.IsPromotable && PlayerColor != Gameboard.CurrentPlayerColor)
                return RenderPromotionSelection();
            return RenderBoard();

        }

        //remember Enumerable range
        private VNode RenderBoard()
        {
            VNode board;
            int rowSize = 8;
            if (PlayerColor == PieceColor.White)
                board = Fragment(Enumerable.Range(0, 8)
                          .Select(row => Row(Gameboard.Board
                          .Skip(rowSize * row)
                          .Take(rowSize)
                          .Select((p, col) => RenderTile(p, col, row)))));
            else
                board = Fragment(Enumerable.Range(0, 8).Reverse()
                        .Select(row => Row(Gameboard.Board
                        .Skip(rowSize * row)
                        .Take(rowSize)
                        .Select((p, col) => RenderTile(p, col, row))
                        .Reverse())));
            return Div(
                board,
                Game.PlayedByEngine.B == true || Game.PlayedByEngine.W == true ? Text($"EngineMove: {Enginemove}") : null,
                Text($"Time remaining White: {Gameboard.WhiteClock:hh\\:mm\\:ss}"),
                Text($"Time remaining Black: {Gameboard.BlackClock:hh\\:mm\\:ss}"),
                Text($"Gameroom: {Game.ID}"),
                Gameboard.GameOver ? RenderGameOver() : null
            );
        }

        //remember onclick
        private VNode RenderTile(BasePiece piece, int col, int row)
        {
            var target = Gameboard.ConvertTo1D((col, row));
            Style lastMove = null;
            if (Gameboard.Lastmove.start != null
                && (target == Gameboard.Lastmove.start.Position && !Gameboard.IsPromotable || target == Gameboard.Lastmove.target && !Gameboard.IsPromotable))
                lastMove = Styles.BCred;
            return Div(
                 GetBaseStyle(target) & (Gameboard.Board[target] != null && Gameboard.Board[target] == Selected ? Styles.Selected : GetBorderStyle(target)) & lastMove,
                () => Select(target),
                piece != null
                ? Text(piece.Sprite, Styles.FontSize3)
                : null
            );
        }

        private Style GetBaseStyle(int position)
        {
            return TileColor(position) switch
            {
                PieceColor.Black => Styles.TileBlack,
                PieceColor.White => Styles.TileWhite,
                _ => Styles.TCwhite
            };
        }

        private Style GetBorderStyle(int position)
        {
            return TileColor(position) switch
            {
                PieceColor.Black => Styles.BorderBlack,
                PieceColor.White => Styles.BorderWhite,
                _ => Styles.TCwhite
            };
        }

        private PieceColor TileColor(int position)
        {
            //Boolsche algebra ^ = XOR
            var rowEven = (position / 8) % 2 == 0;
            if (rowEven)
                return (position % 2) switch
                {
                    0 => PieceColor.Black,
                    1 => PieceColor.White,
                    _ => PieceColor.Zero
                };
            return (position % 2) switch
            {
                0 => PieceColor.White,
                1 => PieceColor.Black,
                _ => PieceColor.Zero
            };
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
            Selected = Gameboard.Board[Gameboard.Lastmove.target];
            PromotionSelect[0] = new Rook(0, Selected.Color);
            PromotionSelect[1] = new Knight(1, Selected.Color);
            PromotionSelect[2] = new Bishop(2, Selected.Color);
            PromotionSelect[3] = new Queen(3, Selected.Color);
            return Div(
                Styles.M2,
                Text($"Select Piece you want the Pawn to be promoted to.", Styles.FontSize1p5),
                Row(Fragment(PromotionSelect.Select(x => RenderTile(x, x.PositionXY.X, x.PositionXY.Y))))
            );
        }

        private void Select(int target)
        {
            //eigener Select für Promotion bzw.RenderTile mit Onclick param?
            if (Gameboard.IsPromotable &&  PlayerColor != Gameboard.CurrentPlayerColor)
            {
                Selected = Gameboard.Board[Gameboard.Lastmove.target];
                Gameboard.Board[Selected.Position] = PromotionSelect[target];
                Gameboard.Board[Selected.Position].Position = Selected.Position;
                Selected = null;
                Gameboard.IsPromotable = false;
                return;
            }
            if (Gameboard.CurrentPlayerColor == PieceColor.White && Game.PlayedByEngine.W == false && PlayerColor == PieceColor.White
                || Gameboard.CurrentPlayerColor == PieceColor.Black && Game.PlayedByEngine.B == false && PlayerColor == PieceColor.Black)
            {

                if (Selected == null && Gameboard.Board[target] != null && Gameboard.Board[target].Color == Gameboard.CurrentPlayerColor)
                    Selected = Gameboard.Board[target];
                else if (Selected == Gameboard.Board[target])
                    Selected = null;
                else if (Selected != null)
                    if (Gameboard.TryMove(Selected, target))
                    {
                        Selected = null;
                        ThreadPool.QueueUserWorkItem(o =>
                        {
                            if (Game.PlayedByEngine.B && Gameboard.CurrentPlayerColor == PieceColor.Black)
                                Gameboard.TryEngineMove(Enginemove = Engine.GetEngineMove(Gameboard.GetFeNotation()));
                            else if (Game.PlayedByEngine.W && Gameboard.CurrentPlayerColor == PieceColor.White)
                                Gameboard.TryEngineMove(Enginemove = Engine.GetEngineMove(Gameboard.GetFeNotation()));
                        });
                    }
            }
        }

    }
}
