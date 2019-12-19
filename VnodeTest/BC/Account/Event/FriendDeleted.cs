using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACL.ES;

namespace VnodeTest.BC.Account.Event
{
    public class FriendDeleted : AggregateEvent<Account>
    {
        public AggregateID<Account> FriendID;
        public FriendDeleted(AggregateID<Account> id, AggregateID<Account> friendID) : base(id)
        {
            FriendID = friendID;
        }
    }
}
