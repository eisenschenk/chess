using ACL.UI.React;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ACL.UI.React.DOM;

namespace VnodeTest
{
    class SearchbarComponent
    {
        private string SearchQuery;
        private VNode RefreshReference;
        private bool IsSelected;
        private void InputChanged(string input)
        {
            SearchQuery = input;
            RefreshReference.Refresh();
        }


        public static VNode Render(IEnumerable<string> content, int pageSize = 10)
        {
            return new ComponentNode<SearchbarComponent>(state =>
                Div(
                    RenderSearchBar(state, content, pageSize),
                    Div(state.IsSelected ? RenderSearchWindow(state, content, pageSize) : null))
                    );
        }
        private static VNode RenderSearchWindow(SearchbarComponent state, IEnumerable<string> content, int pageSize)
        {
            var searchResult = content.Where(s => s.Contains(state.SearchQuery ?? String.Empty));

            return Div(
                 Styles.Dropdown & Styles.MX2,
               PaginationComponent<string>.Render(searchResult, s => Text(s, Styles.TCblack & Styles.HoverWhite), pageSize)
            );
        }

        private static VNode RenderSearchBar(SearchbarComponent state, IEnumerable<string> content, int pageSize)
        {
            return state.RefreshReference = Div(
                Text("Searchbar:", Styles.ML2),
                Input(state.SearchQuery, sq => state.SearchQuery = sq, Styles.MB2 & Styles.ML2, onchange: state.InputChanged)
                    .WithOnFocusIn(()=> state.IsSelected = true)
                    .WithOnFocusOut(() => state.IsSelected = false)
                );
        }
    }
}
