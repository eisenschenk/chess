using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.BC.Account.Command;
using VnodeTest.BC.Account.Event;

namespace VnodeTest.BC.Account
{
    public class Account : AggregateRoot<Account>
    {
        private bool Created = false;
        private string Username;
        private string Password;
        private bool LoggedIn;
        private List<AggregateID<Account>> Friends = new List<AggregateID<Account>>();
        private List<AggregateID<Account>> PendingFriendRequests = new List<AggregateID<Account>>();
        private List<AggregateID<Account>> ReceivedFriendReuests = new List<AggregateID<Account>>();

        //TODO: nur einmal friends ids etc in die commands rein, da sie in der projection schon vorhanden sind!!!!
        public class Handler : AggregateCommandHandler<Account>
        {
            public Handler(IRepository repository, IMessageBus bus) : base(repository, bus)
            {
            }
        }

        public static class Commands
        {
            public static void RegisterAccount(AggregateID<Account> id, string username, string password) =>
                MessageBus.Instance.Send(new RegisterAccount(id, username, password));
            public static void LoginAccount(AggregateID<Account> id, string username, string password, string hashsalt) =>
                MessageBus.Instance.Send(new LoginAccount(id, username, password, hashsalt));
            public static void LogoutAccount(AggregateID<Account> id) => MessageBus.Instance.Send(new LogoutAccount(id));
        }

        //hier password hash&salten

        public IEnumerable<IEvent> On(RegisterAccount command)
        {
            Password = PasswordHelper.HashAndSalt(command.Password);
            yield return new AccountRegistered(command.ID, command.Username, Password);
        }
        public IEnumerable<IEvent> On(LoginAccount command)
        {
                yield return new AccountLoggedIn(command.ID, command.Username, command.Hashsalt);
        }
        public IEnumerable<IEvent> On(LogoutAccount command)
        {
            yield return new AccountLoggedOut(command.ID);
        }

        public override void Apply(IEvent @event)
        {
            switch (@event)
            {
                case AccountRegistered registered:
                    Created = true;
                    ID = registered.ID;
                    Username = registered.Username;
                    break;
                case AccountLoggedIn loggedin:
                    LoggedIn = true;
                    break;

            }
        }
    }
}
