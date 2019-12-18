using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.BC.Game.Event;
using VnodeTest.GameEntities;
using GameID = ACL.ES.AggregateID<VnodeTest.BC.Game.Game>;


namespace VnodeTest.BC.Game
{
    public class GameProjection : Projection
    {
        private readonly Dictionary<GameID, GameEntry> Dict = new Dictionary<GameID, GameEntry>();

        public GameEntry this[GameID id] => Dict[id];
        public IEnumerable<GameEntry> Games => Dict.Values;


        public GameProjection(IEventStore store, IMessageBus bus) : base(store, bus)
        {
        }

        private void On(GameOpened @event)
        {
            Dict.Add(@event.ID, new GameEntry(@event.ID, @event.Gamemode));
        }
    }

    public class GameEntry
    {
        public GameID ID { get; }
        public int RepositoryID { get; }
        public Gamemode Gamemode { get; }
        public GameEntry(GameID id, Gamemode gamemode)
        {
            ID = id;
            Gamemode = gamemode;
        }
    }
}

