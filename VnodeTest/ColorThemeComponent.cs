using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ACL.UI.React.DOM;


namespace VnodeTest
{
    class ColorThemeComponent
    {
        public static VNode Render()
        {
            return new ComponentNode<ColorThemeComponent>(state =>
            Button("Theme", () => state.ChangeTheme())
            );
        }
        private void ChangeTheme()
        {

        }
    }
}
