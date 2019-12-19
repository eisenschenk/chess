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
        public AccountEntry AccountEntry { get; private set; }
        private bool PlayNormal;
        private bool PlayFriend;
        private AccountProjection AccountProjection;
        private RenderMode Rendermode = RenderMode.Overview;
        public bool GameStarted => PlayNormal || PlayFriend;
        public UserController(AccountEntry accountEntry, AccountProjection accountProjection)
        {
            AccountEntry = accountEntry;
            AccountProjection = accountProjection;
        }


        public VNode Render()
        {
            var friendAccounts = AccountProjection.Accounts.Where(a => AccountEntry.Friends.Contains(a.ID));

            return Rendermode switch
            {
                RenderMode.Overview => RenderOverview(friendAccounts),
                RenderMode.AddFriend => RenderAddFriend(),
                RenderMode.DeleteFriend => RenderDeleteFriend(friendAccounts),
                _ => null,
            };
        }
        private VNode RenderOverview(IEnumerable<AccountEntry> friendAccounts)
        {
            return Div(
                Text("Add Friend", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.AddFriend),
                Text("Remove Friend", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.DeleteFriend),
                //Text("back", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.Overview),
                friendAccounts.Any() ? Fragment(friendAccounts.Select(f => Text($"{f.Username}"))) : Text("you got no friends ;(")
            );

        }


        private VNode RenderAddFriend()
        {
            return Div(
                SearchbarComponent<AccountEntry>.Render(AccountProjection.Accounts, a => Account.Commands.AddFriend(AccountEntry.ID, a.ID)),
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
            AddFriend,
            DeleteFriend,

        }
    }
}
