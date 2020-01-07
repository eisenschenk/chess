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
        public bool Created = false;
        public Gamemode Gamemode;
        public int RepositoryID;
        public class Handler : AggregateCommandHandler<Game>
        {
            public Handler(IRepository repository, IMessageBus bus) : base(repository, bus)
            {
            }
        }
        // aggregate rooot nur mit ID vom selben Typ aufrufen
        public static class Commands
        {
            public static void OpenGame(AggregateID<Game> id, Gamemode gamemode) =>
                MessageBus.Instance.Send(new OpenGame(id, gamemode));
            public static void RequestChallenge(AggregateID<Game> id, AggregateID<Account.Account> accountID, AggregateID<Account.Account> friendID) =>
                MessageBus.Instance.Send(new RequestChallenge(id, accountID, friendID));
            public static void DenyChallenge(AggregateID<Game> id, AggregateID<Account.Account> accountID, AggregateID<Account.Account> friendID) =>
                MessageBus.Instance.Send(new DenyChallenge(id, accountID, friendID));
            public static void AcceptChallenge(AggregateID<Game> id, AggregateID<Account.Account> accountID, AggregateID<Account.Account> friendID) =>
              MessageBus.Instance.Send(new AcceptChallenge(id, accountID, friendID));
            public static void CloseGame(AggregateID<Game> id) =>
                MessageBus.Instance.Send(new CloseGame(id));
            public static void SaveGame(AggregateID<Game> id, string moves) =>
                MessageBus.Instance.Send(new GameSaved(id, moves));
            public static void JoinGame(AggregateID<Game> id, AggregateID<Account.Account> accountID) =>
                MessageBus.Instance.Send(new GameJoined(id, accountID));
            public static void ResetGame(AggregateID<Game> id) =>
                MessageBus.Instance.Send(new GamesResetted(id));
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
            yield return new ChallengeDenied(command.ID, command.AccountID, command.FriendID);
        }
        public IEnumerable<IEvent> On(CloseGame command)
        {
            yield return new GameClosed(command.ID);
        }
        public IEnumerable<IEvent> On(SaveGame command)
        {
            yield return new GameSaved(command.ID, command.Moves);
        }
        public IEnumerable<IEvent> On(JoinGame command)
        {
            yield return new GameJoined(command.ID, command.AccountID);
        }
        public IEnumerable<IEvent> On(ResetGames command)
        {
            yield return new GamesResetted(command.ID);
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
