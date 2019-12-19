using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.BC.Account.Event;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.Account.Account>;

namespace VnodeTest.BC.Account
{
    public class AccountProjection : Projection
    {
        private readonly Dictionary<AccountID, AccountEntry> Dict = new Dictionary<AccountID, AccountEntry>();

        public AccountEntry this[AccountID id] => Dict[id];
        public IEnumerable<AccountEntry> Accounts => Dict.Values;


        public AccountProjection(IEventStore store, IMessageBus bus) : base(store, bus)
        {
        }

        private void On(AccountRegistered @event)
        {
            Dict.Add(@event.ID, new AccountEntry(@event.ID, @event.Username, @event.Password, @event.Timestamp));
        }
        private void On(AccountLoggedIn @event)
        {
            Dict[@event.ID].LoggedIn = true;
        }
    }

    public class AccountEntry
    {
        public AccountID ID { get; }
        public string Username { get; }
        public string Password { get; }
        public DateTimeOffset CreatedAt { get; }
        public bool LoggedIn { get; set; }

        public AccountEntry(AccountID id, string username, string password, DateTimeOffset createdAt)
        {
            ID = id;
            Username = username;
            Password = password;
            CreatedAt = createdAt;
        }
    }
}
