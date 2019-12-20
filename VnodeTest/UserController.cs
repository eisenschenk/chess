using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.BC.Account;
using static ACL.UI.React.DOM;


namespace VnodeTest
{
    public class UserController
    {
        //TODO: autologgout of all users @programstart
        public AccountEntry AccountEntry { get; private set; }
        public BC.Game.GameProjection GameProjection { get; }

        private bool PlayNormal;
        private bool PlayFriend;
        private AccountProjection AccountProjection;
        private RenderMode Rendermode = RenderMode.Overview;
        public bool GameStarted => PlayNormal || PlayFriend;
        public UserController(AccountEntry accountEntry, AccountProjection accountProjection, BC.Game.GameProjection gameProjection)
        {
            AccountEntry = accountEntry;
            AccountProjection = accountProjection;
            GameProjection = gameProjection;
        }


        public VNode Render()
        {
            var friendAccounts = AccountProjection.Accounts.Where(a => AccountEntry.Friends.Contains(a.ID));

            return Rendermode switch
            {
                RenderMode.Overview => RenderOverview(friendAccounts),
                RenderMode.AddFriend => RenderAddFriend(),
                RenderMode.DeleteFriend => RenderDeleteFriend(friendAccounts),
                RenderMode.PendingRequests => RenderReceiveRequests(),
                RenderMode.PlayFriend => RenderChallengeFriend(),
                _ => null,
            };
        }
        private VNode RenderOverview(IEnumerable<AccountEntry> friendAccounts)
        {
            return Div(
                Text($"Pending Friendrequests({AccountEntry.ReceivedFriendRequests.Count})", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.PendingRequests),
                Text("Add Friend", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.AddFriend),
                Text("Remove Friend", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.DeleteFriend),
                friendAccounts.Any() ? Fragment(friendAccounts.Select(f => Text($"{f.Username}", !f.LoggedIn ? Styles.TCblack : Styles.TCgreen))) : Text("you got no friends ;(")
            );
        }

        private VNode RenderReceiveRequests()
        {
            var pendingRequests = AccountProjection.Accounts.Where(a => AccountEntry.ReceivedFriendRequests.Contains(a.ID));
            return Div(
                Fragment(pendingRequests.Select(p => Row(
                        Text(p.Username),
                        Text("accept", Styles.Btn & Styles.MP4, () => Account.Commands.AcceptFriendRequest(AccountEntry.ID, p.ID)),
                        Text("deny", Styles.Btn & Styles.MP4, () => Account.Commands.DenyFriendRequest(AccountEntry.ID, p.ID))
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
                SearchbarComponent<AccountEntry>.Render(AccountProjection.Accounts, a => Account.Commands.RequestFriend(AccountEntry.ID, a.ID)),
                Text("back", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.Overview)
            );
        }

        private VNode RenderDeleteFriend(IEnumerable<AccountEntry> friends)
        {
            return Div(
                 SearchbarComponent<AccountEntry>.Render(friends, a => Account.Commands.DeleteFriend(AccountEntry.ID, a.ID)),
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
