using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.Friendship.Command
{
    public class DenyFriendRequest : AggregateCommand<Friendship>
    {

        public DenyFriendRequest(AggregateID<Friendship> id) : base(id)
        {
        }
    }
}
