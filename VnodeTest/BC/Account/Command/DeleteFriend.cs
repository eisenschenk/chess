using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.Account.Command
{
    public class DeleteFriend : AggregateCommand<Account>
    {
        public AggregateID<Account> FriendID { get; }
        public DeleteFriend(AggregateID<Account> id, AggregateID<Account> friendID) : base(id)
        {
            FriendID = friendID;
        }

    }
}
