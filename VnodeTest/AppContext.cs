using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VnodeTest
{
    public class AppContext
    {
        //private readonly SomeDataProjection SomeDataProjection;
        //private readonly SomeData.Handler SomeDataHandler;

        private readonly IRepository Repository;

        public AppContext()
        {
            string storePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "store.db");

            Type tEvent = typeof(IEvent);
            Type tTO = typeof(ITransferObject);
            var allTypes = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            var knownTypes = allTypes
                .Where(t => !t.IsAbstract)
                .Where(t => t.IsClass || t.IsValueType)
                .Where(t => tEvent.IsAssignableFrom(t) || tTO.IsAssignableFrom(t)).ToArray();

            JSONFileEventStore store = new JSONFileEventStore(storePath, knownTypes);
            Repository = new Repository(store);
            var bus = MessageBus.Instance;

            //SomeDataProjection = new SomeDataProjection(store, bus);
            //SomeDataProjection.Init();
            //SomeDataHandler = new SomeData.Handler(Repsitory, bus);
        }

        public SomeDataController CreateSomeDataController() =>
            new SomeDataController();
    }

}
