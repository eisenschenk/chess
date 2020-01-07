using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACL.ES;

namespace VnodeTest.BC.Friendship.Event
{
    public class FriendDeleted : AggregateEvent<Friendship>
    {
        public AggregateID<Account.Account> FriendIDa { get; }
        public AggregateID<Account.Account> FriendIDb { get; }

        public FriendDeleted(AggregateID<Friendship> id, AggregateID<Account.Account> friendIDa, AggregateID<Account.Account> friendIDb) : base(id)
        {
            FriendIDa = friendIDa;
            FriendIDa = friendIDb;
        }
    }
}
