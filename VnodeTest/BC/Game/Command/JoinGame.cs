using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.Game.Command
{
    public class JoinGame : AggregateCommand<Game>
    {
        public AggregateID<Account.Account> AccountID { get; }

        public JoinGame(AggregateID<Game> id, AggregateID<Account.Account> accountID) : base(id)
        {
            AccountID = accountID;
        }
    }
}
