using ACL.ES;
using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.BC.Account;
using VnodeTest.BC.Friendship;
using VnodeTest.BC.Game;
using static ACL.UI.React.DOM;

namespace VnodeTest
{
    public class FriendshipController
    {
        public AccountEntry AccountEntry { get; }
        public AccountProjection AccountProjection { get; }
        public GameProjection GameProjection { get; }
        public FriendshipProjection FriendshipProjection { get; }
        private RenderMode Rendermode = RenderMode.Overview;

        public FriendshipController(AccountEntry accountEntry, AccountProjection accountProjection, BC.Game.GameProjection gameProjection, FriendshipProjection friendshipProjection)
        {
            AccountEntry = accountEntry;
            AccountProjection = accountProjection;
            GameProjection = gameProjection;
            FriendshipProjection = friendshipProjection;
        }

        public VNode Render()
        {
            var friendAccounts = FriendshipProjection.Friendships
                .Where(x => x.Accepted == true && x.FriendBID == AccountEntry.ID || x.FriendAID == AccountEntry.ID);
            var _friendAccountsA = friendAccounts.Where(x => x.FriendAID != AccountEntry.ID).Select(x => x.FriendAID);
            var _friendAccountsB = friendAccounts.Where(x => x.FriendBID != AccountEntry.ID).Select(c => c.FriendBID);
            var friends = _friendAccountsA.Concat(_friendAccountsB).Select(f => AccountProjection.Accounts.Where(g => g.ID == f).SingleOrDefault());

            return Rendermode switch
            {
                RenderMode.Overview => RenderOverview(friends),
                RenderMode.AddFriend => RenderAddFriend(),
                RenderMode.DeleteFriend => RenderDeleteFriend(friends),
                RenderMode.PendingRequests => RenderReceiveRequests(friends),
                RenderMode.PlayFriend => RenderChallengeFriend(),
                _ => null,
            };
        }
        private VNode RenderOverview(IEnumerable<AccountEntry> friendAccounts)
        {
            var friendRequests = FriendshipProjection.Friendships.Where(x => x.FriendBID == AccountEntry.ID && x.Requested == true && x.Accepted == default);
            return Div(
                Text($"Pending Friendrequests({friendRequests.Count()})", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.PendingRequests),
                Text("Add Friend", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.AddFriend),
                Text("Remove Friend", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.DeleteFriend),
                friendAccounts.Any() ? Fragment(friendAccounts.Select(f => Text($"{f.Username}", !f.LoggedIn ? Styles.TCblack : Styles.TCgreen))) : Text("you got no friends ;(")
            );
        }

        private VNode RenderReceiveRequests(IEnumerable<AccountEntry> friendAccounts)
        {
            AggregateID<Account> FriendID(FriendshipEntry p)
            {
                if (p.FriendAID != AccountEntry.ID)
                    return p.FriendAID;
                return p.FriendBID;
            }
            var friendRequests = FriendshipProjection.Friendships.Where(x => x.FriendBID == AccountEntry.ID && x.Requested == true && x.Accepted == false);
            return Div(
                Fragment(friendRequests.Select(p => Row(
                        Text(friendAccounts.Count().ToString()),
                        Text("accept", Styles.Btn & Styles.MP4, () => Friendship.Commands.AcceptFriendRequest(AggregateID<Friendship>.Create(), AccountEntry.ID, FriendID(p))),
                        Text("deny", Styles.Btn & Styles.MP4, () => Friendship.Commands.DenyFriendRequest(AggregateID<Friendship>.Create(), AccountEntry.ID, FriendID(p)))
                ))),
                Text("back", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.Overview)
            );
        }

        private VNode RenderChallengeFriend()
        {
            return Div(
                Text("Accept", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.PlayFriend),
                Text("back", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.Overview)
            );
        }

        private VNode RenderAddFriend()
        {
            return Div(
                SearchbarComponent<AccountEntry>.Render(AccountProjection.Accounts, a => Friendship.Commands.RequestFriend(AggregateID<Friendship>.Create(), AccountEntry.ID, a.ID)),
                Text("back", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.Overview)
            );
        }

        private VNode RenderDeleteFriend(IEnumerable<AccountEntry> friends)
        {
            return Div(
                 SearchbarComponent<AccountEntry>.Render(friends, a => Friendship.Commands.AbortFriend(AggregateID<Friendship>.Create(), AccountEntry.ID, a.ID)),
                 Text("back", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.Overview)
            );
        }

        enum RenderMode
        {
            Overview,
            PendingRequests,
            ReceivedRequests,
            AddFriend,
            PlayFriend,
            DeleteFriend,

        }
    }
}
