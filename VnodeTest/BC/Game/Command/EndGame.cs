using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.Game.Command
{
    public class EndGame : AggregateCommand<Game>
    {
        public string Moves { get; }

        public EndGame(AggregateID<Game> id, string moves) : base(id)
        {
            Moves = moves;
        }




    }
}
