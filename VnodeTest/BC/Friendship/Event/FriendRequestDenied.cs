using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.Friendship.Event
{
    public class FriendRequestDenied : AggregateEvent<Friendship>
    {
        public FriendRequestDenied(AggregateID<Friendship> id) : base(id)
        {
        }
    }
}

