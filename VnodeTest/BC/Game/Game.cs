using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.BC.Game.Command;
using VnodeTest.BC.Game.Event;
using VnodeTest.GameEntities;

namespace VnodeTest.BC.Game
{
    public class Game : AggregateRoot<Game>
    {
        private bool Created = false;
        private Gamemode Gamemode;
        public class Handler : AggregateCommandHandler<Game>
        {
            public Handler(IRepository repository, IMessageBus bus) : base(repository, bus)
            {
            }
        }
        public static class Commands
        {
            //TODO acceptchallenge PM, 
            public static void OpenGame(AggregateID<Game> id, Gamemode gamemode) =>
                MessageBus.Instance.Send(new OpenGame(id, gamemode));
            public static void RequestChallenge(AggregateID<Game> id, AggregateID<Account.Account> accountID, AggregateID<Account.Account> friendID) =>
                MessageBus.Instance.Send(new RequestChallenge(id, accountID, friendID));
            public static void DenyChallenge(AggregateID<Game> id) =>
                MessageBus.Instance.Send(new DenyChallenge(id));
            public static void AcceptChallenge(AggregateID<Game> id, AggregateID<Account.Account> accountID, AggregateID<Account.Account> friendID) =>
              MessageBus.Instance.Send(new AcceptChallenge(id, accountID, friendID));
            public static void DeleteGame(AggregateID<Game> id) =>
                MessageBus.Instance.Send(new DeleteGame(id));
            public static void EndGame(AggregateID<Game> id, string moves) =>
                MessageBus.Instance.Send(new EndGame(id, moves));
            public static void JoinGame(AggregateID<Game> id, AggregateID<Account.Account> accountID) =>
                MessageBus.Instance.Send(new GameJoined(id, accountID));
            
        }
        //hier kann man 2 oder mehr events starten
        public IEnumerable<IEvent> On(OpenGame command)
        {
            yield return new GameOpened(command.ID, command.Gamemode);
        }
        public IEnumerable<IEvent> On(RequestChallenge command)
        {
            yield return new ChallengeRequested(command.ID, command.AccountID, command.FriendID);
        }
        public IEnumerable<IEvent> On(AcceptChallenge command)
        {
            yield return new ChallengeAccepted(command.ID, command.AccountID, command.FriendID);
        }
        public IEnumerable<IEvent> On(DenyChallenge command)
        {
            yield return new ChallengeDenied(command.ID);
        }
        public IEnumerable<IEvent> On(DeleteGame command)
        {
            yield return new GameDeleted(command.ID);
        }
        public IEnumerable<IEvent> On(EndGame command)
        {
            yield return new GameEnded(command.ID, command.Moves);
        }
        public IEnumerable<IEvent> On(JoinGame command)
        {
            yield return new GameJoined(command.ID, command.AccountID);
        }

        public override void Apply(IEvent @event)
        {
            switch (@event)
            {
                case GameOpened registered:
                    Created = true;
                    ID = registered.ID;
                    Gamemode = registered.Gamemode;
                    break;
            }
        }
    }
}
