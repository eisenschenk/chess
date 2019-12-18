using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.BC.Account;
using VnodeTest.GameEntities;

namespace VnodeTest.BC.Game.Event
{

    public class GameOpened : AggregateEvent<Game>
    {
        public Gamemode Gamemode { get; }
        public int RepositoryID { get; }
        public GameOpened(AggregateID<Game> id, Gamemode gamemode, int repositoryID) : base(id)
        {
            Gamemode = gamemode;
            RepositoryID = repositoryID;
        }
    }
}
