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
        public int RepositoryID { get; }
        public string Moves { get; }

        public SaveGame(AggregateID<Game> id, int repositoryID, string moves) : base(id)
        {
            RepositoryID = repositoryID;
            Moves = moves;
        }




    }
}
