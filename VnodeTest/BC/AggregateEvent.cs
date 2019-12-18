using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC
{
    public class AggregateEvent<T> : ACL.ES.AggregateEvent<T, BC.Account.Account>
    {
        public AggregateEvent(AggregateID<T> id, AggregateID<BC.Account.Account>? userID = default, DateTimeOffset timestamp = default)
            // userID is null when program created an event, then we can use current user in session. when event is imported, userID is never null
            : base(id, userID ?? (/*Session.CurrentUser?.UserID ??*/ default), timestamp) { }

    }

}
