using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.Game.Command
{
    public class SaveGame : AggregateCommand<Game>
    {
        public string Moves { get; }

        public SaveGame(AggregateID<Game> id, string moves) : base(id)
        {
            Moves = moves;
        }




    }
}
