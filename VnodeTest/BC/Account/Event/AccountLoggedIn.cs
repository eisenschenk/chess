using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.Account.Event
{
    public class AccountLoggedIn : AggregateEvent<Account>
    {
        public AccountLoggedIn(AggregateID<Account> id) : base(id)
        {
        }
    }
}
