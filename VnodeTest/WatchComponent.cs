using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ACL.UI.React.DOM;

namespace VnodeTest
{
    class WatchComponent
    {
        public static VNode Render()
        {
            return new ComponentNode<WatchComponent>(state =>
                state.GetTime()
            );
        }
        private VNode GetTime()
        {
            return Text(DateTime.Now.ToString(), Styles.TCgreen & Styles.ML2 & Styles.Absolute & Styles.R10);
        }
    }
}
