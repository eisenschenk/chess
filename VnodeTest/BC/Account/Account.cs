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
            public static void LoginAccount(AggregateID<Account> id, string username, string password) =>
                MessageBus.Instance.Send(new LoginAccount(id, username, password));
            public static void AddFriend(AggregateID<Account> id, AggregateID<Account> friendID) => MessageBus.Instance.Send(new AddFriend(id, friendID));
            public static void DeleteFriend(AggregateID<Account> id, AggregateID<Account> friendID) => MessageBus.Instance.Send(new DeleteFriend(id, friendID));
        }


        public IEnumerable<IEvent> On(RegisterAccount command)
        {
            if (string.IsNullOrWhiteSpace(command.Password))
                throw new Exception("password cannot be empty");
            if (command.Password.Length < 6)
                throw new Exception("password must be longer than 5 characters");

            yield return new AccountRegistered(command.ID, command.Username, command.Password);
        }
        public IEnumerable<IEvent> On(LoginAccount command)
        {
            if (string.IsNullOrWhiteSpace(command.Password))
                throw new Exception("password cannot be empty");

            yield return new AccountLoggedIn(command.ID, command.Username, command.Password);
        }
        public IEnumerable<IEvent> On(AddFriend command)
        {
            if (command.ID == null)
                throw new Exception("Friends ID not valid");
            yield return new FriendAdded(command.ID, command.FriendID);
        }
        public IEnumerable<IEvent> On(DeleteFriend command)
        {
            if (command.ID == null)
                throw new Exception("Not Friends with this ID");
            yield return new FriendDeleted(command.ID, command.FriendID);

        }


        public override void Apply(IEvent @event)
        {
            switch (@event)
            {
                case AccountRegistered registered:
                    Created = true;
                    ID = registered.ID;
                    Username = registered.Username;
                    Password = registered.Password;
                    break;
                case AccountLoggedIn loggedin:
                    LoggedIn = true; ;
                    break;
                case FriendAdded fadded:
                    Friends.Add(fadded.FriendID);
                    break;
                case FriendDeleted fdeleted:
                    Friends.Remove(fdeleted.ID);
                    break;
            }
        }
    }
}
