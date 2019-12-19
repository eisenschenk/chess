using ACL.ES;
using ACL.MQ;
using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.BC.Account.Event;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.Account.Account>;
using static ACL.UI.React.DOM;

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
            Dict.Add(@event.ID, new AccountEntry(@event.ID, @event.Username, @event.Password, @event.Timestamp, new List<AccountID>()));
        }
        private void On(AccountLoggedIn @event)
        {
            Dict[@event.ID].LoggedIn = true;
        }
        private void On(FriendAdded @event)
        {
            Dict[@event.ID].Friends.Add(@event.FriendID);
            if (@event.FriendID != default)
                Dict[@event.FriendID].Friends.Add(@event.ID);
        }
        private void On(FriendDeleted @event)
        {
            Dict[@event.ID].Friends.Remove(@event.FriendID);
            if (@event.FriendID != default)
                Dict[@event.FriendID].Friends.Remove(@event.ID);
        }
    }

    public class AccountEntry : ISearchable
    {
        public AccountID ID { get; }
        public string Username { get; }
        public string Password { get; }
        public DateTimeOffset CreatedAt { get; }
        public bool LoggedIn { get; set; }
        public List<AccountID> Friends { get; }

        public AccountEntry(AccountID id, string username, string password, DateTimeOffset createdAt, List<AccountID> friends)
        {
            ID = id;
            Username = username;
            Password = password;
            CreatedAt = createdAt;
            Friends = friends;
        }

        VNode ISearchable.Render()
        {
            return Text(Username);
        }
        bool ISearchable.IsMatch(string searchquery)
        {
            return Username.Contains(searchquery);
        }
    }
}
