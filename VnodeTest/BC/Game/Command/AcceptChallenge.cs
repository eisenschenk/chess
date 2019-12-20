using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.Game.Command
{
    public class AcceptChallenge : AggregateCommand<Game>
    {
        public AggregateID<Account.Account> AccountID { get; }
        public AggregateID<Account.Account> FriendID { get; }
        public AcceptChallenge(AggregateID<Game> id, AggregateID<Account.Account> accountID, AggregateID<Account.Account> friendID) : base(id)
        {
            AccountID = accountID;
            FriendID = friendID;
        }

    }
}
