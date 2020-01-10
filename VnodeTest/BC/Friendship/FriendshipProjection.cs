using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendshipID = ACL.ES.AggregateID<VnodeTest.BC.Friendship.Friendship>;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.Account.Account>;
using static ACL.UI.React.DOM;
using ACL.UI.React;
using ACL.MQ;
using VnodeTest.BC.Friendship.Event;

namespace VnodeTest.BC.Friendship
{
    public class FriendshipProjection : Projection
    {
        private readonly Dictionary<FriendshipID, FriendshipEntry> Dict = new Dictionary<FriendshipID, FriendshipEntry>();

        public FriendshipEntry this[FriendshipID id] => Dict[id];
        public IEnumerable<FriendshipEntry> Friendships => Dict.Values;

        private readonly Dictionary<AccountID, Dictionary<FriendshipID, FriendshipEntry>> AccountFriendRequests = new Dictionary<AccountID, Dictionary<FriendshipID, FriendshipEntry>>();
        private readonly Dictionary<AccountID, Dictionary<FriendshipID, FriendshipEntry>> AccountFriends = new Dictionary<AccountID, Dictionary<FriendshipID, FriendshipEntry>>();

        public FriendshipProjection(IEventStore store, IMessageBus bus) : base(store, bus)
        {
        }
        private void On(FriendRequestAccepted @event)
        {
            var friendship = Dict[@event.ID];

            friendship.Accepted = true;
            friendship.Requested = false;

            DeleteAccountFriendRequest(friendship.Sender, @event.ID);
            DeleteAccountFriendRequest(friendship.Receiver, @event.ID);
            AddAccountFriend(friendship.Sender, friendship);
            AddAccountFriend(friendship.Receiver, friendship);
        }
        private void On(FriendshipAborted @event)
        {
            var friendship = Dict[@event.ID];

            Dict.Remove(@event.ID);

            DeleteAccountFriend(friendship.Sender, @event.ID);
            DeleteAccountFriend(friendship.Receiver, @event.ID);
        }
        private void On(FriendshipRequested @event)
        {
            var friendship = new FriendshipEntry(@event.ID, @event.Sender, @event.Receiver);

            Dict.Add(@event.ID, friendship);
            Dict[@event.ID].Requested = true;

            //AddAccountFriendRequest(@event.Sender, friendship);
            AddAccountFriendRequest(@event.Receiver, friendship);
        }
        private void On(FriendRequestDenied @event)
        {
            var friendship = Dict[@event.ID];

            Dict.Remove(@event.ID);

            //DeleteAccountFriendRequest(friendship.Sender, @event.ID);
            DeleteAccountFriendRequest(friendship.Receiver, @event.ID);
        }

        private void AddAccountFriendRequest(AccountID accountID, FriendshipEntry friendship)
        {
            if (!AccountFriendRequests.TryGetValue(accountID, out var friendships))
                AccountFriendRequests.Add(accountID, friendships = new Dictionary<FriendshipID, FriendshipEntry>());
            friendships.Add(friendship.ID, friendship);
        }

        private void DeleteAccountFriendRequest(AccountID accountID, FriendshipID friendshipID)
        {
            AccountFriendRequests[accountID].Remove(friendshipID);
        }

        private void AddAccountFriend(AccountID accountID, FriendshipEntry friendship)
        {
            if (!AccountFriends.TryGetValue(accountID, out var friendships))
                AccountFriends.Add(accountID, friendships = new Dictionary<FriendshipID, FriendshipEntry>());
            friendships.Add(friendship.ID, friendship);
        }

        private void DeleteAccountFriend(AccountID accountID, FriendshipID friendshipID)
        {
            AccountFriends[accountID].Remove(friendshipID);
        }

        public IEnumerable<(AccountID AccountID, FriendshipID FriendshipID)> GetFriends(AccountID accountID)
        {
            if (AccountFriends.TryGetValue(accountID, out _))
                return AccountFriends[accountID].Values.Select(f => (f.Sender == accountID ? f.Receiver : f.Sender, f.ID));
            return Enumerable.Empty<(AccountID AccountID, FriendshipID FriendshipID)>();
        }

        public IEnumerable<FriendshipEntry> GetFriendshipRequests(AccountID accountID)
        {
            if (AccountFriendRequests.TryGetValue(accountID, out _))
                return AccountFriendRequests[accountID].Values;
            return Enumerable.Empty<FriendshipEntry>();
        }

        public int GetFriendshipRequestCount(AccountID accountID)
        {
            if (AccountFriendRequests.TryGetValue(accountID, out _))
                return AccountFriendRequests[accountID].Count;
            return 0;
        }
    }

    public class FriendshipEntry
    {
        public FriendshipID ID { get; }
        public AccountID Sender;
        public AccountID Receiver;
        public bool Accepted;
        public bool Requested;

        public FriendshipEntry(FriendshipID id, AccountID friendA, AccountID friendB)
        {
            ID = id;
            Sender = friendA;
            Receiver = friendB;
        }

    }
}