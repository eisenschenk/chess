using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.GameEntities;

namespace VnodeTest.BC.Game.Command
{
    public class CloseGame : AggregateCommand<Game>
    {
        public CloseGame(AggregateID<Game> id) : base(id)
        {
        }
    }
}
