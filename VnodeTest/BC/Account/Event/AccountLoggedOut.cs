using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACL.ES;

namespace VnodeTest.BC.Account.Event
{
    public class AccountLoggedOut : AggregateEvent<Account>
    {
        public AccountLoggedOut(AggregateID<Account> id) : base(id)
        {
        }
    }
}
