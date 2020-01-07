using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACL.ES;
using VnodeTest.BC.Account;

namespace VnodeTest.BC.Game.Event
{
    public class GamesResetted : AggregateEvent<Game>
    {
        public GamesResetted(AggregateID<Game> id) : base(id)
        {
        }
    }
}
