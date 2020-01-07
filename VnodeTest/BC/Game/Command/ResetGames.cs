using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.Game.Command
{
    public class ResetGames : AggregateCommand<Game>
    {
        public ResetGames(AggregateID<Game> id, DateTimeOffset timestamp = default) : base(id, timestamp)
        {
        }
    }
}
