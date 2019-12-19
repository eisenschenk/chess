using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VnodeTest.BC.Account;

namespace VnodeTest
{
    public class AppContext
    {
        private readonly AccountProjection AccountProjection;
        private readonly Account.Handler AccountHandler;

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

            AccountProjection = new AccountProjection(store, bus);
            AccountProjection.Init();
            AccountHandler = new Account.Handler(Repository, bus);
        }

        public GameboardController CreateGameboardController() =>
            new GameboardController(AccountProjection);

        public LoginController CreateLoginController() =>
            new LoginController(AccountProjection);

        public UserController CreateUserController(AccountEntry accountEntry) =>
           new UserController(accountEntry, AccountProjection);
    }
}


