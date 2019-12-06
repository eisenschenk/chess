﻿using ACL.UI.React;
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
        public Tile Selected { get; set; }


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


        private Game GetFittingGame(Gamemode gamemode, string _key, (bool, bool) engineControlled = default)
        {
            Game game;
            int key = int.Parse(_key);
            while (GameRepository.Instance.TryGetGame(key, out game))
            {
                if ((gamemode == Gamemode.PvP && !game.HasOpenSpots)
                    || ((gamemode == Gamemode.EvE || gamemode == Gamemode.PvE) && game.HasOpenSpots))
                    key++;
                else
                    break;
            }
            if (game == default)
                game = GameRepository.Instance.AddGame(key, gamemode, new Gameboard(TimeSpan.FromSeconds(30000000), engineControlled));

            return game;
        }
        private void SelectGameMode(Gamemode gamemode, string index = "0")
        {
            //PvP
            if (gamemode == Gamemode.PvP)
            {
                Game = GetFittingGame(gamemode, index);
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
                if (gamemode == Gamemode.EvE)
                    Game = GetFittingGame(gamemode, index, (true, true));
                else
                    Game = GetFittingGame(gamemode, index, (false, true));
                Game.HasWhitePlayer = true;
                Game.HasBlackPlayer = true;
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
            if (Gameboard.IsPromotable)
                return RenderPromotionSelection();

            if (PlayerColor == PieceColor.White)
                return Div(
                     Fragment(Enumerable.Range(0, 8)
                         .Select(rowx => Row(Gameboard.Board.Where(x => x.Position / 8 == rowx)
                         .Select(t => RenderTile(t))))),
                    Gameboard.PlayedByEngine.B == true || Gameboard.PlayedByEngine.W == true ? Text($"EngineMove: {Gameboard.EngineMove}") : null,
                    Text($"Time remaining White: {Gameboard.WhiteClock:hh\\:mm\\:ss}"),
                    Text($"Time remaining Black: {Gameboard.BlackClock:hh\\:mm\\:ss}"),
                    Text($"Gameroom: {Game.ID}")
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
                     Text($"Gameroom: {Game.ID}")
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
                 tile.Style & (tile == Selected ? Styles.Selected : tile.BorderStyle) & lastMove,
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
            promotionSelect[0] = (new Tile(new Rook(0, Selected.Piece.Color), 0));
            promotionSelect[1] = (new Tile(new Knight(1, Selected.Piece.Color), 1));
            promotionSelect[2] = (new Tile(new Bishop(2, Selected.Piece.Color), 2));
            promotionSelect[3] = (new Tile(new Queen(3, Selected.Piece.Color), 3));
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
                if (Gameboard.IsPromotable == true)
                {
                    Selected = Gameboard.Lastmove.target;
                    Gameboard.Board[Selected.Position].Piece = target.Piece;
                    Gameboard.Board[Selected.Position].Piece.Position = Selected.Position;
                    Selected = null;
                    Gameboard.IsPromotable = false;
                    return;
                }
                if (Selected == null && target.ContainsPiece && target.Piece.Color == Gameboard.CurrentPlayerColor)
                    Selected = target;
                else if (Selected == target)
                    Selected = null;
                else if (Selected != null)
                    if (Gameboard.TryMove(Selected, target))
                        Selected = null;
            }
        }

    }
}
