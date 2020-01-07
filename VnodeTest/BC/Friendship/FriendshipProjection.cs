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
        }
        //private void On(FriendAdded @event)
        //{
        //    Dict[@event.ID].Friends.Add(@event.FriendID);
        //}
        private void On(FriendshipAborted @event)
        {
            Dict[@event.ID] = default;
        }
        //private void On(FriendDeleted @event)
        //{
        //    Dict[@event.ID].Friends.Remove(@event.FriendID);
        //}
        private void On(FriendshipRequested @event)
        {
            Dict[@event.ID].Requested = true;
        }
        private void On(FriendRequestDenied @event)
        {
            Dict[@event.ID].Accepted = false;
            Dict[@event.ID].Requested = false;
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

        //VNode ISearchable.Render()
        //{
        //    return Text(Username);
        //}
        //bool ISearchable.IsMatch(string searchquery)
        //{
        //    return Username.Contains(searchquery);
        //}
    }
}