using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.Account.Event
{
    public class FriendshipAborted : AggregateEvent<Account>
    {
        public AggregateID<Account> FriendID { get; }
        public FriendshipAborted(AggregateID<Account> id, AggregateID<Account> friendID) : base(id)
        {
            FriendID = friendID;
        }
    }
}
