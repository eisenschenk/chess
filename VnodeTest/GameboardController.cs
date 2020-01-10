using ACL.ES;
using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VnodeTest.BC.Account;
using VnodeTest.GameEntities;
using static ACL.UI.React.DOM;

namespace VnodeTest
{
    public class GameboardController
    {
        public Game Game;
        private Gameboard Gameboard => Game != null ? Game.Gameboard : null;
        private int Gameroom;
        private VNode RefreshReference;
        private PieceColor PlayerColor;
        private IEngine Engine;
        private string Enginemove;
        public BasePiece Selected { get; set; }
        public BC.Friendship.FriendshipProjection FriendshipProjection { get; }

        private BasePiece[] PromotionSelect = new BasePiece[4];
        private (Gameboard Board, (BasePiece start, int target) LastMove) SelectedPreviousMove;
        private bool Pause;
        private readonly BC.Account.AccountProjection AccountProjection;
        private readonly BC.Game.GameProjection GameProjection;
        private Gamemode Gamemode;// = Gamemode.PvF;
        public Rendermode RenderMode;
        private AccountEntry AccountEntry;
        private AggregateID<BC.Game.Game> GameID;
        public GameboardController(AccountProjection accountProjection, AccountEntry accountEntry, BC.Game.GameProjection gameProjection, BC.Friendship.FriendshipProjection friendshipProjection)
        {
            AccountProjection = accountProjection;
            AccountEntry = accountEntry;
            GameProjection = gameProjection;
            FriendshipProjection = friendshipProjection;
            ThreadPool.QueueUserWorkItem(o =>
            {
                while (true)
                {
                    while (Pause)
                        Thread.Sleep(100);
                    Thread.Sleep(100);
                    Game?.UpdateClocks();
                    RefreshReference?.Refresh();
                }
            });
        }

        private VNode RenderGameModeSelection()
        {
            var gameroomDisplay = Gameroom == default ? "Random Room" : $"Room {Gameroom}";


            return Div(
                Text("Player vs. AI Start", Styles.Btn & Styles.MP4, () => SelectGameMode(Gamemode.PvE)),
                Text("AI vs. AI Start", Styles.MP4 & Styles.Btn, () => SelectGameMode(Gamemode.EvE)),
                Text("Play vs. Friend", Styles.MP4 & Styles.Btn, () => RenderMode = Rendermode.PlayFriend)
            );
        }

        private void SelectGameMode(Gamemode gamemode, string index = "0")
        {
            GameID = AggregateID<BC.Game.Game>.Create();
            BC.Game.Game.Commands.OpenGame(GameID, gamemode);
            GameRepository.Instance.TryGetGame(GameID, out Game);
            BC.Game.Game.Commands.JoinGame(GameID, AccountEntry.ID);

            //EvE && PvE
            if (gamemode == Gamemode.EvE || gamemode == Gamemode.PvE)
            {
                Engine = new EngineControl();
                Game.HasWhitePlayer = true;
                Game.HasBlackPlayer = true;
                PlayerColor = PieceColor.White;
                if (gamemode == Gamemode.EvE)
                    ThreadPool.QueueUserWorkItem(o =>
                    {
                        while (!Game.GameOver)
                        {
                            while (Pause)
                                Thread.Sleep(100);
                            Game.TryEngineMove(Enginemove = Engine.GetEngineMove(Game.GetFeNotation()), Game.PlayedByEngine);
                        }
                    });
            }
        }

        private VNode RenderPreviousMoves()
        {
            void SelectForRender((Gameboard Board, (BasePiece start, int target) LastMove) move)
            {
                if (SelectedPreviousMove == move)
                    SelectedPreviousMove = (null, (null, 0));
                else
                    SelectedPreviousMove = move;
            }
            return Fragment(Game.Moves.Select(g =>
                Game.Moves.IndexOf(g) >= 1
                ? Text($"Show {Gameboard.ParseToAN(g.LastMove.start.Copy(), g.LastMove.target, Game.Moves[Game.Moves.IndexOf(g) - 1].Board.Copy())}",
                    SelectedPreviousMove == g ? Styles.SelectedBtn & Styles.MP4 : Styles.Btn & Styles.MP4,
                    () => SelectForRender(g))
                : null
            ));
        }

        public VNode Render()
        {
            return RefreshReference = RenderGameBoard();
        }

        public enum Rendermode
        {
            Gameboard,
            PlayFriend,
            ChallengeDenied,
            WaitingForChallenged
        }

        private VNode RenderGameBoard()
        {
            var challenges = GameProjection.Games.Where(x => x.Challenged == AccountEntry.ID);
            if (challenges.Any() && Game == default)
                return RenderChallenges(challenges);
            if (RenderMode == Rendermode.PlayFriend)
                return FriendChallenge();
            if (Gameboard == default)
                return RenderGameModeSelection();
            if (Game.IsPromotable && PlayerColor != Game.CurrentPlayerColor)
                return RenderPromotionSelection();
            if (RenderMode == Rendermode.WaitingForChallenged)
                return RenderWaitingRoom();
            if (RenderMode == Rendermode.ChallengeDenied)
                return RenderChallengeDenied();
            return RenderBoard();

        }

        private VNode RenderWaitingRoom()
        {
            var challenges = GameProjection.Games.Where(x => x.Challenger == AccountEntry.ID);
            var game = GameProjection.Games.Where(x => x.PlayerWhite == AccountEntry.ID).FirstOrDefault();
            if (game != default)
            {
                GameRepository.Instance.TryGetGame(game.ID, out Game);
                PlayerColor = PieceColor.White;
                Game.HasWhitePlayer = true;
                RenderMode = Rendermode.Gameboard;
            }
            return Div(
                Fragment(challenges.Select(c =>
                    Div(
                        Game.HasOpenSpots && c.Created.AddSeconds(c.Timer) > DateTime.Now ? Text($"Waiting for Friend: {c.Timer - c.Elapsed.Seconds}") : null,
                        Text("Abort Challenge!", Styles.AbortBtn & Styles.MP4, () => BC.Game.Game.Commands.DenyChallenge(c.ID))
                    )
                )),
                Text("back", Styles.Btn & Styles.MP4, () => { Game = null; RenderMode = Rendermode.Gameboard; })
            );

        }

        private VNode RenderChallengeDenied()
        {
            return Text("Challenge denied!", Styles.AbortBtn & Styles.MP4, () => RenderMode = Rendermode.Gameboard);
        }

        private VNode RenderChallenges(IEnumerable<BC.Game.GameEntry> challenges)
        {
            return Div(
                Fragment(challenges.Select(c =>
                      Row(
                          Text(AccountProjection[c.Challenger].Username),
                          Text("Accept", Styles.Btn & Styles.MP4, () =>
                            {
                                BC.Game.Game.Commands.JoinGame(c.ID, c.Challenger);
                                BC.Game.Game.Commands.JoinGame(c.ID, c.Challenged);
                                BC.Game.Game.Commands.AcceptChallenge(c.ID, c.Challenger, c.Challenged);
                                GameID = c.ID;
                                GameRepository.Instance.TryGetGame(GameID, out Game);
                                PlayerColor = PieceColor.Black;
                                Game.HasBlackPlayer = true;
                            }),
                          Text("Deny", Styles.Btn & Styles.MP4, () => BC.Game.Game.Commands.DenyChallenge(c.ID))
                      )
                )));
        }

        private VNode FriendChallenge()
        {
            var friends = FriendshipProjection.GetFriends(AccountEntry.ID).Select(id => AccountProjection[id]);
            return Div(
                Fragment(friends.Select(f =>
                        Row(
                            Text(f.Username),
                            Text("Challenge", Styles.Btn & Styles.MP4, () => ChallengeFriend(f))
                        )
                )),
                Text("back", Styles.Btn & Styles.MP4, () => RenderMode = Rendermode.Gameboard)
            );
        }

        private void ChallengeFriend(AccountEntry accountEntry)
        {
            GameID = AggregateID<BC.Game.Game>.Create();
            BC.Game.Game.Commands.OpenGame(GameID, Gamemode.PvF);
            GameRepository.Instance.TryGetGame(GameID, out Game);

            BC.Game.Game.Commands.RequestChallenge(GameID, AccountEntry.ID, accountEntry.ID);
            PlayerColor = PieceColor.White;
            Game.HasWhitePlayer = true;
            RenderMode = Rendermode.WaitingForChallenged;
        }


        private VNode GetBoardVNode(Gameboard gameboard, (BasePiece start, int target) lastmove)
        {
            int rowSize = 8;
            if (PlayerColor == PieceColor.White)
                return Fragment(Enumerable.Range(0, 8)
                          .Select(row => Row(gameboard.Board
                          .Skip(rowSize * row)
                          .Take(rowSize)
                          .Select((p, col) => RenderTile(p, col, row, lastmove)))));
            else
                return Fragment(Enumerable.Range(0, 8).Reverse()
                        .Select(row => Row(gameboard.Board
                        .Skip(rowSize * row)
                        .Take(rowSize)
                        .Select((p, col) => RenderTile(p, col, row, lastmove))
                        .Reverse())));
        }

        //remember Enumerable range
        private VNode RenderBoard()
        {
            VNode board = GetBoardVNode(Gameboard, Game.Lastmove);

            return Div(
                !Game.Winner.HasValue
                    ? Text("Surrender", Styles.AbortBtn & Styles.MP4, () =>
                        {
                            if (PlayerColor == PieceColor.Black)
                                Game.Winner = PieceColor.White;
                            else
                                Game.Winner = PieceColor.Black;
                            BC.Game.Game.Commands.CloseGame(GameID);
                            BC.Game.Game.Commands.SaveGame(GameID, Allmoves());
                        })
                    : Text("Close Game", Styles.AbortBtn & Styles.MP4, () =>
                    {
                        Game = default;
                        RenderMode = Rendermode.Gameboard;
                    }),
                Row(
                    Div(SelectedPreviousMove.Board != null ? GetBoardVNode(SelectedPreviousMove.Board, SelectedPreviousMove.LastMove) : board),
                    Div(Text("Pause", Styles.AbortBtn & Styles.MP4, PauseGame), RenderPreviousMoves())
                ),
                Game.PlayedByEngine.B == true || Game.PlayedByEngine.W == true ? Text($"EngineMove: {Enginemove}") : null,
                Text($"Time remaining White: {Game.WhiteClock:hh\\:mm\\:ss}"),
                Text($"Time remaining Black: {Game.BlackClock:hh\\:mm\\:ss}"),
                Text($"Gameroom: {Game.ID}"),
                Game.GameOver ? RenderGameOver() : null
            );
        }
        //TODO: look at copy
        private string Allmoves()
        {
            StringBuilder allmoves = new StringBuilder();
            foreach ((Gameboard Board, (BasePiece start, int target) Lastmove) entry in Game.Moves.Where(g => Game.Moves.IndexOf(g) >= 1))
            {
                allmoves.Append(Game.Gameboard.ParseToAN(entry.Lastmove.start, entry.Lastmove.target, Game.Moves[Game.Moves.IndexOf(entry) - 1].Board));
                allmoves.Append(".");
            }
            return allmoves.ToString();
        }

        private void PauseGame()
        {
            Pause = !Pause;
        }

        //remember onclick
        private VNode RenderTile(BasePiece piece, int col, int row, (BasePiece start, int target) lastmove = default)
        {
            var target = Gameboard.ConvertTo1D((col, row));
            Style lastMove = null;
            if (lastmove.start != null
                && (target == lastmove.start.Position && !Game.IsPromotable || target == lastmove.target && !Game.IsPromotable))
                lastMove = Styles.BCred;
            if (SelectedPreviousMove.Board == null)
                return Div(
                     GetBaseStyle(target) & (Gameboard.Board[target] != null && Gameboard.Board[target] == Selected ? Styles.Selected : GetBorderStyle(target)) & lastMove,
                    () => Select(target),
                    piece != null
                    ? Text(piece.Sprite, Styles.FontSize3)
                    : null
                );
            else
                return Div(
                     GetBaseStyle(target) & GetBorderStyle(target) & lastMove,
                    piece != null ? Text(piece.Sprite, Styles.FontSize3) : null
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
            switch (Game.Winner)
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
            Selected = Gameboard.Board[Game.Lastmove.target];
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
            if (Game.IsPromotable && PlayerColor != Game.CurrentPlayerColor)
            {
                Selected = Gameboard.Board[Game.Lastmove.target];
                Gameboard.Board[Selected.Position] = PromotionSelect[target];
                Gameboard.Board[Selected.Position].Position = Selected.Position;
                Selected = null;
                Game.IsPromotable = false;
                return;
            }
            if (Game.CurrentPlayerColor == PieceColor.White && Game.PlayedByEngine.W == false && PlayerColor == PieceColor.White
                || Game.CurrentPlayerColor == PieceColor.Black && Game.PlayedByEngine.B == false && PlayerColor == PieceColor.Black)
            {

                if (Selected == null && Gameboard.Board[target] != null && Gameboard.Board[target].Color == Game.CurrentPlayerColor)
                    Selected = Gameboard.Board[target];
                else if (Selected == Gameboard.Board[target])
                    Selected = null;
                else if (Selected != null)
                {
                    if (Game.TryMove(Selected, target))
                    {
                        Selected = null;
                        ThreadPool.QueueUserWorkItem(o =>
                        {
                            if (Game.PlayedByEngine.B && Game.CurrentPlayerColor == PieceColor.Black)
                                Game.TryEngineMove(Enginemove = Engine.GetEngineMove(Game.GetFeNotation()));
                            else if (Game.PlayedByEngine.W && Game.CurrentPlayerColor == PieceColor.White)
                                Game.TryEngineMove(Enginemove = Engine.GetEngineMove(Game.GetFeNotation()));
                        });
                    }
                }
            }
        }

    }
}
