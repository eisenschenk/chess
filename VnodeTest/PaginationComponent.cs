using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ACL.UI.React.DOM;

namespace VnodeTest
{
    class PaginationComponent<T>
    {
        private int PageNumber;
        public static VNode Render(IEnumerable<T> collection, Func<T, VNode> renderEntry, int pageSize = 10)
        {
            return new ComponentNode<PaginationComponent<T>>(state =>
            Div(
                Div(
                    collection
                   .Skip(state.PageNumber * pageSize)
                   .Take(pageSize)
                   .Select(renderEntry)),
                Row(
                    Button("Previous", Styles.Btn, () => state.SetPageNumber(collection, -1, pageSize)),
                    Button("Next", Styles.Btn, () => state.SetPageNumber(collection, +1, pageSize))
            )));
        }
        private void SetPageNumber(IEnumerable<T> collection, int direction, int pageSize)
        {
            if (PageNumber + direction <= collection.Count() / pageSize && PageNumber + direction >= 0)
                PageNumber += direction;
        }


    }
}
