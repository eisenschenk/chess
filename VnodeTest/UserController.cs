using ACL.ES;
using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.BC.Account;
using VnodeTest.BC.Friendship;
using static ACL.UI.React.DOM;


namespace VnodeTest
{
    public class UserController
    {
        //TODO: autologgout of all users @programstart
        public AccountEntry AccountEntry { get; private set; }
        public BC.Game.GameProjection GameProjection { get; }
        public FriendshipProjection FriendshipProjection { get; }

        private bool PlayNormal;
        private bool PlayFriend;
        private AccountProjection AccountProjection;
        public bool GameStarted => PlayNormal || PlayFriend;
        public UserController(AccountEntry accountEntry, AccountProjection accountProjection, BC.Game.GameProjection gameProjection, FriendshipProjection friendshipProjection)
        {
            AccountEntry = accountEntry;
            AccountProjection = accountProjection;
            GameProjection = gameProjection;
            FriendshipProjection = friendshipProjection;
        }


       public VNode Render()
        {
            return Div();
        }
    }
}
