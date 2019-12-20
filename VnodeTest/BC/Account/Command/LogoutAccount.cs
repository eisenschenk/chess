using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.Account.Command
{
    public class LogoutAccount : AggregateCommand<Account>
    {
        public LogoutAccount(AggregateID<Account> id) : base(id)
        {
        }
    }
}
