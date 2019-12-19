using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest
{
    static class VNodeExtensions
    {
        public static VNode WithOnclick(this VNode node, Action onclick)
        {
            node.OnClick = onclick;
            return node;
        }
    }
}
