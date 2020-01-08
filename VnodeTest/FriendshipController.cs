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
            var _friends = FriendshipProjection.GetFriends(AccountEntry.ID);
            var friends = AccountProjection.Accounts.Where(a => _friends.Contains(a.ID));

            return Rendermode switch
            {
                RenderMode.Overview => RenderOverview(friends),
                RenderMode.AddFriend => RenderAddFriend(),
                RenderMode.DeleteFriend => RenderDeleteFriend(friends),
                RenderMode.PendingRequests => RenderReceiveRequests(),
                RenderMode.PlayFriend => RenderChallengeFriend(),
                _ => null,
            };
        }
        private VNode RenderOverview(IEnumerable<AccountEntry> friendAccounts)
        {
            return Div(
                Text($"Pending Friendrequests({FriendshipProjection.GetFriendrequests(AccountEntry.ID).Count()})", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.PendingRequests),
                Text("Add Friend", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.AddFriend),
                Text("Remove Friend", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.DeleteFriend),
                friendAccounts.Any() ? Fragment(friendAccounts.Select(f => Text($"{f.Username}", !f.LoggedIn ? Styles.TCblack : Styles.TCgreen))) : Text("you got no friends ;(")
            );
        }

        private VNode RenderReceiveRequests()
        {
            return Div(
                Fragment(FriendshipProjection.GetFriendshipRequests(AccountEntry.ID).Select(p => Row(
                        Text(AccountProjection.Accounts.Where(x => x.ID == p.FriendAID).FirstOrDefault().Username),
                        Text("accept", Styles.Btn & Styles.MP4, () => Friendship.Commands.AcceptFriendRequest(p.ID, AccountEntry.ID, p.FriendAID)),
                        Text("deny", Styles.Btn & Styles.MP4, () => Friendship.Commands.DenyFriendRequest(p.ID))
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
                 SearchbarComponent<AccountEntry>.Render(friends, a =>
                 Friendship.Commands.AbortFriend(FriendshipProjection.GetFriendshipEntry(AccountEntry.ID, a.ID).ID)),
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
