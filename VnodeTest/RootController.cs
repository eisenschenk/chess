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
        public VNode Render() => SomeDataController.Render();

        private SomeDataController _SomeDataController;
        private SomeDataController SomeDataController =>
        _SomeDataController ??
        (_SomeDataController = ((Application)Application.Instance).AppContext.CreateSomeDataController());
    }

}
