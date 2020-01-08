using ACL.ES;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.GameEntities;

namespace VnodeTest
{
    class GameRepository
    {
        private static readonly object InstanceLock = new object();
        private static GameRepository _Instance;
        public static GameRepository Instance
        {
            get
            {
                if (_Instance == default)
                    lock (InstanceLock)
                        if (_Instance == default)
                            _Instance = new GameRepository();
                return _Instance;
            }
        }

        private GameRepository() { }

        private readonly Dictionary<AggregateID<BC.Game.Game>, Game> Store = new Dictionary<AggregateID<BC.Game.Game>, Game>();

        public IEnumerable<AggregateID<BC.Game.Game>> Keys => Store.Keys;

        public Game AddGame(AggregateID<BC.Game.Game> key, Gamemode mode, Gameboard board)
        {
            var game = new Game(key, mode, board, TimeSpan.FromSeconds(30000000));
            Store[key] = game;
            return game;
        }

        public bool TryGetGame(AggregateID<BC.Game.Game> key, out Game game) => Store.TryGetValue(key, out game);
    }
    public class Game
    {
        public AggregateID<BC.Game.Game> ID { get; }
        public Gamemode Gamemode { get; }
        public bool HasBlackPlayer { get; set; }

        public bool HasWhitePlayer { get; set; }
        public bool HasOpenSpots => !HasBlackPlayer || !HasWhitePlayer;
        public bool IsEmpty => !HasBlackPlayer && !HasWhitePlayer;
        public Gameboard Gameboard { get; private set; }
        public (bool W, bool B) PlayedByEngine { get; set; }
        public bool IsPromotable { get; set; }
        public bool GameOver => Winner.HasValue;
        public PieceColor? Winner { get; set; }
        public PieceColor CurrentPlayerColor { get; set; } = PieceColor.White;
        public (BasePiece start, int target) Lastmove { get; set; }
        public int MoveCounter { get; set; } = 1;
        public int HalfMoveCounter { get; set; }
        private TimeSpan _WhiteClock;
        public TimeSpan WhiteClock { get => _WhiteClock; private set => _WhiteClock = value; }

        private TimeSpan _BlackClock;
        public TimeSpan BlackClock { get => _BlackClock; private set => _BlackClock = value; }
        private DateTime LastClockUpdate;
        public readonly List<(Gameboard Board, (BasePiece start, int target) LastMove)> Moves = new List<(Gameboard Board, (BasePiece start, int target) LastMove)>();



        public Game(AggregateID<BC.Game.Game> id, Gamemode gamemode, Gameboard gameboard, TimeSpan playerClockTime)
        {
            ID = id;
            Gamemode = gamemode;
            Gameboard = gameboard;
            Moves.Add((gameboard, (null, 0)));
            LastClockUpdate = DateTime.Now;
            WhiteClock = playerClockTime;
            BlackClock = playerClockTime;
            PlayedByEngine = gamemode switch
            {
                Gamemode.PvP => (false, false),
                Gamemode.PvF => (false, false),
                Gamemode.PvE => (false, true),
                Gamemode.EvE => (true, true),
                _ => throw new Exception("error game switch")
            };
        }

        private static (int start, int target) GetCoordinates(string input)
        {
            var startX = Gameboard.ParseStringXToInt(input[0].ToString());
            var startY = Gameboard.ParseStringYToInt(input[1].ToString());
            var targetX = Gameboard.ParseStringXToInt(input[2].ToString());
            var targetY = Gameboard.ParseStringYToInt(input[3].ToString());
            return (startX + startY * 8, targetX + targetY * 8);
        }

        //TODO naming
        public void TryEngineMove(string engineMove, (bool, bool) engineControlled = default)
        {
            var _engineMove = GetCoordinates(engineMove);
            var newBoard = Gameboard.Copy();
            if (newBoard.TryMove(newBoard.Board[_engineMove.start], _engineMove.target, out var newboard, this, engineControlled))
            {
                newBoard = newboard;
                if (engineMove.Length >= 5)
                    newBoard.Board[_engineMove.target] = engineMove[4] switch
                    {
                        'q' => new Queen(_engineMove.target, CurrentPlayerColor),
                        'n' => new Knight(_engineMove.target, CurrentPlayerColor),
                        'b' => new Bishop(_engineMove.target, CurrentPlayerColor),
                        'r' => new Rook(_engineMove.target, CurrentPlayerColor),
                        _ => default
                    };
                Moves.Add((newBoard, Lastmove));
                Gameboard = newBoard;
            }
        }

        public bool TryMove(BasePiece start, int target, (bool, bool) engineControlled = default)
        {
            var newBoard = Gameboard.Copy();
            if (newBoard.TryMove(start, target, out newBoard, this, engineControlled))
            {
                Moves.Add((newBoard, Lastmove));
                Gameboard = newBoard;
                return true;
            }
            return false;
        }
        public string GetFeNotation()
        {
            int emptyCount = 0;
            StringBuilder stringBuilder = new StringBuilder();

            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)
                {
                    var piece = Gameboard[x, y];

                    if (x == 0 && y >= 1)
                    {
                        if (emptyCount != 0)
                        {
                            stringBuilder.Append(emptyCount.ToString());
                            emptyCount = 0;
                        }
                        stringBuilder.Append("/");
                    }
                    if (piece == null)
                        emptyCount++;

                    if (piece != null)
                    {
                        if (emptyCount != 0)
                            stringBuilder.Append(emptyCount.ToString());
                        emptyCount = 0;
                        stringBuilder.Append(piece.Value switch
                        {
                            PieceValue.King => piece.Color == PieceColor.White ? "K" : "k",
                            PieceValue.Queen => piece.Color == PieceColor.White ? "Q" : "q",
                            PieceValue.Bishop => piece.Color == PieceColor.White ? "B" : "b",
                            PieceValue.Knight => piece.Color == PieceColor.White ? "N" : "n",
                            PieceValue.Rook => piece.Color == PieceColor.White ? "R" : "r",
                            PieceValue.Pawn => piece.Color == PieceColor.White ? "P" : "p",
                            _ => throw new Exception("error FEN piece.value switch")
                        });
                    }
                }
            stringBuilder.Append(CurrentPlayerColor == PieceColor.White ? " w " : " b ");
            stringBuilder.Append(GetPossibleCastles());
            stringBuilder.Append(Gameboard.EnPassantTarget == -1 ? $" {Gameboard.ParseIntToString(Gameboard.EnPassantTarget)} " : " - ");
            stringBuilder.Append($"{HalfMoveCounter} ");
            stringBuilder.Append($"{MoveCounter}");
            return stringBuilder.ToString();
        }

        private string GetPossibleCastles()
        {
            string CheckCastle(int king, int rook)
            {
                string _output = string.Empty;
                if (Gameboard.Board[rook] != null && !Gameboard.Board[rook].HasMoved
                    && Gameboard.Board[king] != null && !Gameboard.Board[king].HasMoved)
                    if (Gameboard.Board[king].Color == PieceColor.White)
                    {
                        if (king > rook)
                            return _output += "Q";
                        else
                            return _output += "K";
                    }
                    else
                    {
                        if (king > rook)
                            return _output += "q";
                        else
                            return _output += "k";
                    }
                return _output;
            }
            string output = CheckCastle(60, 63);
            output += CheckCastle(60, 56);
            output += CheckCastle(4, 7);
            return output += CheckCastle(4, 0);
        }

        private void TryEnablePromotion(BasePiece piece, (bool W, bool B) engineControlled = default)
        {
            if (piece is Pawn && (piece.Position > 55 || piece.Position < 7))
            {
                if ((CurrentPlayerColor == PieceColor.White && !engineControlled.W) || (CurrentPlayerColor == PieceColor.Black && !engineControlled.B))
                    IsPromotable = true;
            }
        }

        public void ActionsAfterMoveSuccess(BasePiece target, Game game = null, (bool, bool) engineControlled = default)
        {
            TryEnablePromotion(target, engineControlled);
            game?.UpdateClocks(changeCurrentPlayer: true);
            if (CurrentPlayerColor == PieceColor.White)
                MoveCounter++;
            if (CheckForGameOver())
                Winner = InverseColor();
        }

        public bool CheckForGameOver()
        {
            if (HalfMoveCounter >= 50)
            {
                Winner = PieceColor.Zero;
                return true;
            }
            return Gameboard.CheckMateDetection(Gameboard, CurrentPlayerColor);
        }

        public PieceColor InverseColor()
        {
            if (CurrentPlayerColor == PieceColor.White)
                return PieceColor.Black;
            else return PieceColor.White;
        }

        private readonly object UpdateClockLock = new object();

        public void UpdateClocks() => UpdateClocks(false);

        public void UpdateClocks(bool changeCurrentPlayer)
        {
            lock (UpdateClockLock)
            {
                var now = DateTime.Now;
                void updateColor(PieceColor color, ref TimeSpan clock)
                {
                    if (CurrentPlayerColor != color)
                        return;
                    clock -= now - LastClockUpdate;
                    if (clock <= TimeSpan.Zero)
                        Winner = color;
                }
                updateColor(PieceColor.Black, ref _BlackClock);
                updateColor(PieceColor.White, ref _WhiteClock);
                LastClockUpdate = now;
                if (changeCurrentPlayer)
                    CurrentPlayerColor = InverseColor();
            }
        }
    }
}
