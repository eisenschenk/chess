using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendshipID = ACL.ES.AggregateID<VnodeTest.BC.Friendship.Friendship>;
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


        public FriendshipProjection(IEventStore store, IMessageBus bus) : base(store, bus)
        {
        }

        private void On(FriendRequestAccepted @event)
        {
            Dict[@event.ID].Accepted = true;
            Dict[@event.ID].Requested = false;
        }
        private void On(FriendshipAborted @event)
        {
            Dict.Remove(@event.ID);
        }
        private void On(FriendshipRequested @event)
        {
            Dict.Add(@event.ID, new FriendshipEntry(@event.ID, @event.FriendIDa, @event.FriendIDb));
            Dict[@event.ID].Requested = true;
        }
        private void On(FriendRequestDenied @event)
        {
            Dict.Remove(@event.ID);
        }


        public IEnumerable<AggregateID<Account.Account>> GetFriendrequests(AggregateID<Account.Account> accountID)
        {
            return Friendships.Where(f => f.FriendBID == accountID && f.Accepted == false && f.Requested == true).Select(s => s.FriendAID);
        }

        public IEnumerable<AggregateID<Account.Account>> GetFriends(AggregateID<Account.Account> accountID)
        {
            var _friendships = Friendships.Where(f => f.Accepted == true && (f.FriendAID == accountID || f.FriendBID == accountID));
            var _someFriendsA = _friendships.Where(x => x.FriendAID != accountID).Select(f => f.FriendAID);
            var _someFriendsB = _friendships.Where(x => x.FriendBID != accountID).Select(f => f.FriendBID);
            return _someFriendsA.Concat(_someFriendsB);
        }

        public IEnumerable<FriendshipEntry> GetFriendshipRequests(AggregateID<Account.Account> accountID)
        {
            return Friendships.Where(x => x.Requested == true && x.FriendBID == accountID && x.Accepted == false);
        }

        public FriendshipEntry GetFriendshipEntry(AggregateID<Account.Account> accountIDa, AggregateID<Account.Account> accountIDb)
        {
            return Friendships
                .Where(e => e.Accepted == true && ((e.FriendAID == accountIDa && e.FriendBID == accountIDb) || e.FriendAID == accountIDb && e.FriendBID == accountIDa))
                .SingleOrDefault();
        }
    }

    public class FriendshipEntry //: ISearchable
    {
        public FriendshipID ID { get; }
        public AggregateID<Account.Account> FriendAID;
        public AggregateID<Account.Account> FriendBID;
        public bool Accepted;
        public bool Requested;
        //public List<Account.Account> PendingFriendRequests { get; } = new List<Account.Account>();
        //public List<Account.Account> ReceivedFriendRequests { get; } = new List<Account.Account>();


        public FriendshipEntry(FriendshipID id, AggregateID<Account.Account> friendA, AggregateID<Account.Account> friendB)
        {
            ID = id;
            FriendAID = friendA;
            FriendBID = friendB;
        }

    }
}