using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.BC.Friendship.Command;
using VnodeTest.BC.Friendship.Event;

namespace VnodeTest.BC.Friendship
{
    public class Friendship : AggregateRoot<Friendship>
    {

        public class Handler : AggregateCommandHandler<Friendship>
        {
            public Handler(IRepository repository, IMessageBus bus) : base(repository, bus)
            {
            }

        }
        public static class Commands
        {
            //public static void AcceptFriendRequest(AggregateID<Friendship> id, AggregateID<Account.Account> friendIDa, AggregateID<Account.Account> friendIDb) =>
            //    PM.AddFriendPM.PMAddFriend(id, friendIDa, friendIDb);
            public static void AcceptFriendRequest(AggregateID<Friendship> id, AggregateID<Account.Account> friendIDa, AggregateID<Account.Account> friendIDb) =>
                MessageBus.Instance.Send(new AddFriend(id, friendIDa, friendIDb));
            //public static void AbortFriend(AggregateID<Friendship> id, AggregateID<Account.Account> friendIDa, AggregateID<Account.Account> friendIDb) =>
            //    PM.DeleteFriendPM.PMDeleteFriend(id, friendIDa, friendIDb);
            public static void AbortFriend(AggregateID<Friendship> id, AggregateID<Account.Account> friendIDa, AggregateID<Account.Account> friendIDb) =>
                MessageBus.Instance.Send(new DeleteFriend(id, friendIDa, friendIDb));
            public static void RequestFriend(AggregateID<Friendship> id, AggregateID<Account.Account> friendIDa, AggregateID<Account.Account> friendIDb) =>
                MessageBus.Instance.Send(new RequestFriendship(id, friendIDa, friendIDb));
            public static void DenyFriendRequest(AggregateID<Friendship> id, AggregateID<Account.Account> friendIDa, AggregateID<Account.Account> friendIDb) =>
                MessageBus.Instance.Send(new DenyFriendRequest(id, friendIDa, friendIDb));
        }
        public IEnumerable<IEvent> On(AddFriend command)
        {
            if (command.ID == default)
                throw new Exception("Friends ID not valid");
            yield return new FriendAdded(command.ID, command.FriendIDa, command.FriendIDb);
        }
        public IEnumerable<IEvent> On(DeleteFriend command)
        {
            if (command.ID == default)
                throw new Exception("Not Friends with this ID");
            yield return new FriendDeleted(command.ID, command.FriendIDa, command.FriendIDb);
        }
        public IEnumerable<IEvent> On(AbortFriendship command)
        {
            yield return new FriendshipAborted(command.ID, command.FriendIDa, command.FriendIDb);
        }
        public IEnumerable<IEvent> On(RequestFriendship command)
        {
            yield return new FriendshipRequested(command.ID, command.FriendIDa, command.FriendIDb);
        }
        public IEnumerable<IEvent> On(AcceptFriendRequest command)
        {
            yield return new FriendRequestAccepted(command.ID, command.FriendIDa, command.FriendIDb);
        }
        public IEnumerable<IEvent> On(DenyFriendRequest command)
        {
            yield return new FriendRequestDenied(command.ID, command.FriendIDa, command.FriendIDb);
        }

        public override void Apply(IEvent @event)
        {
           
        }
    }
}
