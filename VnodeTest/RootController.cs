using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Text;

namespace VnodeTest
{
    public class RootController
    {
        private readonly Session Session;
        private Func<VNode> CurrentContent;

        public RootController(Session session)
        {
            Session = session;
        }

        //public VNode Render() => CurrentContent();
        public VNode Render() => GameboardController.LoggedIn ? GameboardController.Render() : LoginController.Render(GameboardController);

        private GameboardController _GameboardController;
        private GameboardController GameboardController =>
            _GameboardController ??= ((Application)Application.Instance).AppContext.CreateGameboardController();

        private LoginController _LoginController;
        private LoginController LoginController =>
            _LoginController ??= ((Application)Application.Instance).AppContext.CreateLoginController();
    }

}
