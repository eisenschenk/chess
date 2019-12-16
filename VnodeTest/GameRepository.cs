using System;
using System.Collections.Generic;
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

        private readonly Dictionary<int, Game> Store = new Dictionary<int, Game>();



        public IEnumerable<int> Keys => Store.Keys;

        public Game AddGame(int key, Gamemode mode, Gameboard board)
        {
            var game = new Game(key, mode, board);
            Store[key] = game;
            return game;
        }

        public void UpdateGame(int key, Gameboard board)
        {
            Store[key].Gameboard = board;
        }

        public bool TryGetGame(int key, out Game game) => Store.TryGetValue(key, out game);
    }
    class Game
    {
        public int ID { get; }
        public Gamemode Gamemode { get; }
        public bool HasBlackPlayer { get; set; }
        public bool HasWhitePlayer { get; set; }
        public bool HasOpenSpots => !HasBlackPlayer || !HasWhitePlayer;
        public Gameboard Gameboard { get; set; }
        public (bool W, bool B) PlayedByEngine { get; set; }

        public Game(int id, Gamemode gamemode, Gameboard gameboard)
        {
            ID = id;
            Gamemode = gamemode;
            Gameboard = gameboard;
            PlayedByEngine = gamemode switch
            {
                Gamemode.PvP => (false, false),
                Gamemode.PvE => (false, true),
                Gamemode.EvE => (true, true),
                _ => throw new Exception("error game switch")
            };
        }
    }
}
