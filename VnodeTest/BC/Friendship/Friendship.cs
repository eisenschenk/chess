using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.Friendship
{
   public  class Friendship : AggregateRoot<Friendship>
    {

        public class Handler : AggregateCommandHandler<Friendship>
        {
            public Handler(IRepository repository, IMessageBus bus) : base(repository, bus)
            {
            }

        }
        public static class Commands
        {
            //public static void .. => ..
        }
        //public IEnumerable<IEvent> On(RegisterAccount command)
        public override void Apply(IEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
