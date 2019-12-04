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
        public VNode Render() => GameboardController.Render();

        private GameboardController _GameboardController;
        private GameboardController GameboardController =>
        _GameboardController ??
        (_GameboardController = ((Application)Application.Instance).AppContext.CreateGameboardController());
    }

}
