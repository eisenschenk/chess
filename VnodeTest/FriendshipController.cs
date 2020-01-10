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
            var friends = FriendshipProjection.GetFriends(AccountEntry.ID);

            return Rendermode switch
            {
                RenderMode.Overview => RenderOverview(friends.Select(t => AccountProjection[t.AccountID])),
                RenderMode.AddFriend => RenderAddFriend(),
                RenderMode.DeleteFriend => RenderDeleteFriend(friends.Select(t => new BefriendedAccountEntrySearchWrapper(AccountProjection[t.AccountID], t.FriendshipID))),
                RenderMode.PendingRequests => RenderReceiveRequests(),
                RenderMode.PlayFriend => RenderChallengeFriend(),
                _ => null,
            };
        }
        private VNode RenderOverview(IEnumerable<AccountEntry> friendAccounts)
        {
            return Div(
                Text($"Pending Friendrequests({FriendshipProjection.GetFriendRequestCount(AccountEntry.ID)})", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.PendingRequests),
                Text("Add Friend", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.AddFriend),
                Text("Remove Friend", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.DeleteFriend),
                friendAccounts.Any() ? Fragment(friendAccounts.Select(f => Text($"{f.Username}", !f.LoggedIn ? Styles.TCblack : Styles.TCgreen))) : Text("you got no friends ;(")
            );
        }

        private VNode RenderReceiveRequests()
        {
            return Div(
                Fragment(FriendshipProjection.GetFriendshipRequests(AccountEntry.ID).Select(p => Row(
                    Text(AccountProjection[p.Sender].Username),
                    Text("accept", Styles.Btn & Styles.MP4, () => Friendship.Commands.AcceptFriendRequest(p.ID)),
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
                SearchbarComponent<BefriendedAccountEntrySearchWrapper>.Render(AccountProjection.Accounts.Select(a => new BefriendedAccountEntrySearchWrapper(a, default)),
                    w => Friendship.Commands.RequestFriend(AggregateID<Friendship>.Create(), AccountEntry.ID, w.AccountEntry.ID)),
                Text("back", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.Overview)
            );
        }

        private VNode RenderDeleteFriend(IEnumerable<BefriendedAccountEntrySearchWrapper> friends)
        {
            return Div(
                SearchbarComponent<BefriendedAccountEntrySearchWrapper>.Render(friends, w =>
                    Friendship.Commands.AbortFriend(w.FriendshipID)),
                Text("back", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.Overview)
            );
        }

        private class BefriendedAccountEntrySearchWrapper : ISearchable
        {
            public AccountEntry AccountEntry { get; }
            public AggregateID<Friendship> FriendshipID { get; }

            public BefriendedAccountEntrySearchWrapper(AccountEntry accountEntry, AggregateID<Friendship> friendshipID)
            {
                AccountEntry = accountEntry;
                FriendshipID = friendshipID;
            }

            VNode ISearchable.Render()
            {
                return Text(AccountEntry.Username);
            }
            bool ISearchable.IsMatch(string searchquery)
            {
                return AccountEntry.Username.Contains(searchquery);
            }
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
