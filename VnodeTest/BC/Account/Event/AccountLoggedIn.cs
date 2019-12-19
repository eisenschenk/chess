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
        public string Username { get; }
        public string Password { get; }
        public GameboardController GameboardController { get; }

        public AccountLoggedIn(AggregateID<Account> id, string username, string password, GameboardController gameboardController) : base(id)
        {
            Username = username;
            Password = password;
            GameboardController = gameboardController;
        }
    }
}
