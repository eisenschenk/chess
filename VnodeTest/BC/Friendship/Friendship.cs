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
            public static void AcceptFriendRequest(AggregateID<Friendship> id) =>
                MessageBus.Instance.Send(new AcceptFriendRequest(id));
            public static void AbortFriend(AggregateID<Friendship> id) =>
                MessageBus.Instance.Send(new AbortFriendship(id));
            public static void RequestFriend(AggregateID<Friendship> id, AggregateID<Account.Account> sender, AggregateID<Account.Account> receiver) =>
                MessageBus.Instance.Send(new RequestFriendship(id, sender, receiver));
            public static void DenyFriendRequest(AggregateID<Friendship> id) =>
                MessageBus.Instance.Send(new DenyFriendRequest(id));
        }

        public IEnumerable<IEvent> On(AbortFriendship command)
        {
            yield return new FriendshipAborted(command.ID);
        }
        public IEnumerable<IEvent> On(RequestFriendship command)
        {
            if (command.Sender == command.Receiver)
                throw new Exception("cant befriend yourself");
            yield return new FriendshipRequested(command.ID, command.Sender, command.Receiver);
        }
        public IEnumerable<IEvent> On(AcceptFriendRequest command)
        {
            yield return new FriendRequestAccepted(command.ID);
        }
        public IEnumerable<IEvent> On(DenyFriendRequest command)
        {
            yield return new FriendRequestDenied(command.ID);
        }

        public override void Apply(IEvent @event)
        {

        }
    }
}
