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
    class SearchbarComponent<T> where T: ISearchable
    {
        private string SearchQuery;
        private VNode RefreshReference;
        private bool IsSelected;
        private void InputChanged(string input)
        {
            SearchQuery = input;
            RefreshReference.Refresh();
        }


        public static VNode Render(IEnumerable<T> content,  Action<T> selectEntry, int pageSize = 10)
        {
            return new ComponentNode<SearchbarComponent<T>>(state =>
                Div(
                    RenderSearchBar(state, content, pageSize),
                    Div(state.IsSelected ? RenderSearchWindow(state, content, selectEntry, pageSize) : null))
            );
        }
        private static VNode RenderSearchWindow(SearchbarComponent<T> state, IEnumerable<T> content, Action<T> selectEntry, int pageSize)
        {
            var searchResult = content.Where(s => s.IsMatch(state.SearchQuery ?? String.Empty));

            return Div(
                Styles.Dropdown & Styles.MX2,
                PaginationComponent<T>.Render(searchResult, s => s.Render(), selectEntry, pageSize)
            );
        }

        private static VNode RenderSearchBar(SearchbarComponent<T> state, IEnumerable<T> content, int pageSize)
        {
            return state.RefreshReference = Div(
                Text("Searchbar:", Styles.ML2),
                Input(state.SearchQuery, sq => state.SearchQuery = sq, Styles.MB2 & Styles.ML2, onchange: state.InputChanged)
                    .WithOnFocusIn(() => state.IsSelected = true)
                    .WithOnFocusOut(() => state.IsSelected = false)
            );
        }
    }
}
