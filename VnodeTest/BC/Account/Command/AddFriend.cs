using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.Account.Command
{
    public class AddFriend : AggregateCommand<Account>
    {
        public AggregateID<Account> FriendID { get; }
        //private readonly AccountProjection AccountProjection;
        //public string Username => ID != null ? AccountProjection.Accounts.Where(a => a.ID == ID).First().Username : null;

        public AddFriend(AggregateID<Account> id, AggregateID<Account> friendID) : base(id)
        {
            FriendID = friendID;
        }

    }
}
