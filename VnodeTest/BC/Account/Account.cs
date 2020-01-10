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
        private string Password;

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
            public static void LoginAccount(AggregateID<Account> id, string password) =>
                MessageBus.Instance.Send(new LoginAccount(id, password));
            public static void LogoutAccount(AggregateID<Account> id) => MessageBus.Instance.Send(new LogoutAccount(id));
        }

        public IEnumerable<IEvent> On(RegisterAccount command)
        {
            yield return new AccountRegistered(command.ID, command.Username, PasswordHelper.HashAndSalt(command.Password));
        }
        public IEnumerable<IEvent> On(LoginAccount command)
        {
            if (!PasswordHelper.IsPasswordMatch(command.Password, Password))
                throw new ArgumentException("password not correct");
            yield return new AccountLoggedIn(command.ID);
        }
        public IEnumerable<IEvent> On(Account command)
        {
            yield return new AccountLoggedOut(command.ID);
        }
        //TODO: language reference // switch etc.
        public override void Apply(IEvent @event)
        {
            switch (@event)
            {
                case AccountRegistered registered:
                    Password = registered.Password;
                    break;
            }
        }
    }
}
