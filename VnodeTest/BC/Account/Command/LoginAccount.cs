using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.Account.Command
{
    public class LoginAccount : AggregateCommand<Account>
    {
        public string Password { get; }

        public LoginAccount(AggregateID<Account> id, string password) : base(id)
        {
            Password = password;
        }
    }
}
