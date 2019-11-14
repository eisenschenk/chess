using ACL.CSS;
using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Text;

namespace VnodeTest
{
    using static ACL.CSS.DOM;

    public class Styles : ACL.UI.React.DOM.DefaultStyles
    {
        static Styles() => RegisterStyles<Styles>();

        public Rule[] GetRules() => GetRulesOfType(this);

        public Rule[] SomeRules => RuleSet(
                         Def(BCblack,
                             BackgroundColor("black")
                             ),

                         Def(BCred,
                             BackgroundColor("red")
                             ),

                         Def(TCblack,
                             Color("black")
                             ),

                         Def(TCwhite,
                             Color("white")
                             ),

                         Def(TCgreen,
                             Color("green")
                             ),

                         Def(Dropdown,
                            Display("block"),
                            Position("absolute"),
                            BackgroundColor("grey"),
                            Width("fit-content"),
                            Height("auto"),
                            Padding("12px 16px"),
                            ZIndex("1")
                             ),

                         Def(M2,
                             Margin("2px")
                             ),

                         Def(MX2,
                             MarginLeft("2px"),
                             MarginRight("2px")
                             ),

                         Def(MY2,
                             MarginTop("2px"),
                             MarginBottom("2px")
                             ),

                         Def(MR2,
                             MarginRight("2px")
                             ),

                          Def(ML2,
                             MarginLeft("2px")
                             ),

                          Def(MT2,
                             MarginTop("2px")
                             ),

                          Def(MB2,
                             MarginBottom("2px")
                             ),

                          Def(R10,
                              Right("10px")
                              ),

                          Def(Absolute,
                              Position("absolute")
                              ),

                          Def(Btn,
                              BaseBtn,
                              BackgroundColor("green"),
                              GetHoverRule("darkgreen")
                              ),

                         Def(BaseBtn,
                             BorderRadius("5px"),
                             Margin("4px 4px 4px 0px"),
                             Width("fit-content"),
                             Padding("4px")
                             ),

                         Def(AbortBtn,
                             BaseBtn,
                             BackgroundColor("red"),
                             GetHoverRule("orange")
                             ),

                         Def(SelectedBtn,
                             BaseBtn,
                             BackgroundColor("seagreen")
                             ),

                         Def(FlexRow,
                             FlexDirection("row")
                                 ),

                         Def(HoverWhite,
                            GetHoverRule("white")
                             )
            );
        private Rule GetHoverRule(string color)
        {
            return Def("&:hover", BackgroundColor(color));
        }

        public static readonly Style Absolute;
        public static readonly Style HoverWhite;

        public static readonly Style BCblack;
        public static readonly Style BCred;
        public static readonly Style TCblack;
        public static readonly Style TCwhite;
        public static readonly Style TCgreen;

        public static readonly Style M2;
        public static readonly Style MX2;
        public static readonly Style MY2;
        public static readonly Style MR2;
        public static readonly Style ML2;
        public static readonly Style MT2;
        public static readonly Style MB2;

        public static readonly Style Dropdown;
        public static readonly Style FlexRow;

        public static readonly Style BaseBtn;
        public static readonly Style Btn;
        public static readonly Style AbortBtn;
        public static readonly Style SelectedBtn;

        public static readonly Style R10;
        private enum ColorTheme
        {
            black,
            red,
        }


    }

}
