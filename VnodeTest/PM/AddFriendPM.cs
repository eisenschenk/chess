using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACL.ES;
using ACL.MQ;
using VnodeTest.BC.Account;

namespace VnodeTest.PM
{
    public class AddFriendPM
    {
        public static void PMAddFriend(AggregateID<Account> id, AggregateID<Account> friendID)
        {
            MessageBus.Instance.Send(new BC.Account.Command.AcceptFriendRequest(id, friendID));
            Account.Commands.AddFriend(id, friendID);
            Account.Commands.AddFriend(friendID, id);
        }



    }
}
