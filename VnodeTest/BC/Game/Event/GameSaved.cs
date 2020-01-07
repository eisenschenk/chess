using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.Game.Event
{
    class GameSaved : AggregateEvent<Game>
    {
        public int RepositoryID { get; }
        public string Moves { get; }

        public GameSaved(AggregateID<Game> id, string moves) : base(id)
        {
            Moves = moves;
        }
    }
}
