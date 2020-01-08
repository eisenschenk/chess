using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.Game.Command
{
    public class DenyChallenge : AggregateCommand<Game>
    {
        public DenyChallenge(AggregateID<Game> id) : base(id)
        {
        }

    }
}
