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

        private readonly Dictionary<int, Gameboard> Store = new Dictionary<int, Gameboard>();


        public void AddBoard(int key, Gameboard board) => Store[key] = board;

        public bool TryGetBoard(int key, out Gameboard board) => Store.TryGetValue(key, out board);
    }
}
