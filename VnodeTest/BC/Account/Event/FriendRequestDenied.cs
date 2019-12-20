using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.Account.Event
{
    public class FriendRequestDenied : AggregateEvent<Account>
    {
        public AggregateID<Account> FriendID { get; }
        public FriendRequestDenied(AggregateID<Account> id, AggregateID<Account> friendID) : base(id)
        {
            FriendID = friendID;
        }
    }
}

