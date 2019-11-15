using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static ACL.UI.React.DOM;

namespace VnodeTest
{
    public class SomeDataController
    {
        GameboardController GameboardController = new GameboardController();

        public VNode Render()
        {
           return GameboardController.Render();
        }

    }
}
