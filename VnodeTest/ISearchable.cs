using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest
{
    public interface ISearchable
    {
        VNode Render();
        bool IsMatch(string searchquery);
    }
}
