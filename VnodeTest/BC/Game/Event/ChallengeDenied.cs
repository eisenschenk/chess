using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACL.ES;

namespace VnodeTest.BC.Game.Event
{
    public class ChallengeDenied : AggregateEvent<Game>
    {
        public AggregateID<Account.Account> AccountID { get; }
        public AggregateID<Account.Account> FriendID { get; }
        public ChallengeDenied(AggregateID<Game> id, AggregateID<Account.Account> accountID, AggregateID<Account.Account> friendID) : base(id)
        {
            AccountID = accountID;
            FriendID = friendID;
        }

    }
}
