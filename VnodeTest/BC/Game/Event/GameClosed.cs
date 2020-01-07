using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.Game.Event
{
    public class GameClosed : AggregateEvent<Game>
    {
        public int RepositoryID { get; }
        public GameClosed(AggregateID<Game> id) : base(id)
        {
        }
    }
}
