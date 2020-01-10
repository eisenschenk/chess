using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.Game.Event
{
    class GameEnded : AggregateEvent<Game>
    {
        public string Moves { get; }

        public GameEnded(AggregateID<Game> id, string moves) : base(id)
        {
            Moves = moves;
        }
    }
}
