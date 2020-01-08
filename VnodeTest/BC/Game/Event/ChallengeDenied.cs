using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACL.ES;

namespace VnodeTest.BC.Game.Event
{
    public class ChallengeDenied : AggregateEvent<Game>
    {
        public ChallengeDenied(AggregateID<Game> id) : base(id)
        {
        }

    }
}
