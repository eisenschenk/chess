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

        public static class Commands
        {
            public static void OpenGame(AggregateID<Game> id, Gamemode gamemode, int repositoryID) =>
                MessageBus.Instance.Send(new OpenGame(id, gamemode, repositoryID));
        }

        public IEnumerable<IEvent> On(OpenGame command)
        {
            yield return new GameOpened(command.ID, command.Gamemode, command.RepositoryID);
        }

        public override void Apply(IEvent @event)
        {
            switch (@event)
            {
                case GameOpened registered:
                    Created = true;
                    ID = registered.ID;
                    Gamemode = registered.Gamemode;
                    RepositoryID = registered.RepositoryID;
                    break;
            }
        }
    }
}
