using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.Friendship.Event
{
    public class FriendshipRequested : AggregateEvent<Friendship>
    {
        public AggregateID<Account.Account> Sender { get; }
        public AggregateID<Account.Account> Receiver { get; }

        public FriendshipRequested(AggregateID<Friendship> id, AggregateID<Account.Account> sender, AggregateID<Account.Account> receiver) : base(id)
        {
            Sender = sender;
            Receiver = receiver;
        }
    }
    
}
