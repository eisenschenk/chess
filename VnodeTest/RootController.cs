using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Text;
using VnodeTest.BC.Account;
using static ACL.UI.React.DOM;


namespace VnodeTest
{
    public class RootController
    {
        private readonly Session Session;
        public AccountEntry AccountEntry { get; set; }
        private Func<VNode> CurrentContent;

        private VNode RenderSideMenu()
        {
            return Div(
                Text("Account", Styles.Btn & Styles.MP4, () => CurrentContent = UserController.Render),
                Text("Play Game", Styles.Btn & Styles.MP4, () => CurrentContent = GameboardController.Render)
            );
        }

        public RootController(Session session)
        {
            Session = session;
        }

        //public VNode Render() => CurrentContent();
        public VNode Render() => AccountEntry == null ? LoginController.Render(this) : Row(RenderSideMenu(), CurrentContent?.Invoke());

        //GameboardController.LoggedIn ? GameboardController.Render() : LoginController.Render(GameboardController);

        private GameboardController _GameboardController;
        private GameboardController GameboardController =>
            _GameboardController ??= ((Application)Application.Instance).AppContext.CreateGameboardController();

        private LoginController _LoginController;
        private LoginController LoginController =>
            _LoginController ??= ((Application)Application.Instance).AppContext.CreateLoginController();

        private UserController _UserController;
        private UserController UserController =>
            _UserController ??= ((Application)Application.Instance).AppContext.CreateUserController(AccountEntry);
    }
}

