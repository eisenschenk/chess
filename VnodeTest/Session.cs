using ACL.UI.Manager;
using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Text;

namespace VnodeTest
{
    public class Session : ACL.UI.Manager.Session
    {
        private readonly RootController RootController;

        public Session(Application application, SessionStartupInfo info)
            : base(application, info)
        {
            RootController = new RootController(this);
        }

        protected override VNode Render() => RootController.Render();
    }

}
