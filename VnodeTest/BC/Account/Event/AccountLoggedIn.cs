﻿using ACL.ES;
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

        public AccountLoggedIn(AggregateID<Account> id, string username, string password) : base(id)
        {
            Username = username;
            Password = password;
        }
    }
}
