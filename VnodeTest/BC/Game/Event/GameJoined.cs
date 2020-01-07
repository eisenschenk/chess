using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACL.ES;
using VnodeTest.BC.Account;

namespace VnodeTest.BC.Game.Event
{
    public class GameJoined : AggregateEvent<Game>
    {
        public AggregateID<Account.Account> AccountID { get; }
        public GameJoined(AggregateID<Game> id, AggregateID<Account.Account> accountID) : base(id)
        {
            AccountID = accountID;
        }

    }
}
