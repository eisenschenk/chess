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

        //private void On(AccountRegistered @event)
        //{
        //    Dict.Add(@event.ID, new AccountEntry(@event.ID, @event.Username, @event.Password, @event.Timestamp, new List<AccountID>()));
        //}
       

    }

    public class FriendshipEntry //: ISearchable
    {
        public FriendshipID ID { get; }
        public AggregateID<Account.Account> FriendAID;
        public AggregateID<Account.Account> FriendBID;
       
        //public List<AccountID> PendingFriendRequests { get; } = new List<AccountID>();
        //public List<AccountID> ReceivedFriendRequests { get; } = new List<AccountID>();
       

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