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
        //private readonly Dictionary<AggregateID<Account.Account>, Account.AccountEntry> Accounts = new Dictionary<AggregateID<Account.Account>, Account.AccountEntry>();

        public GameEntry this[GameID id] => Dict[id];
        public IEnumerable<GameEntry> Games => Dict.Values;


        public GameProjection(IEventStore store, IMessageBus bus) : base(store, bus)
        {
        }

        private void On(GameOpened @event)
        {
            GameRepository.Instance.AddGame(@event.RepositoryID, @event.Gamemode, new Gameboard());
            Dict.Add(@event.ID, new GameEntry(@event.ID, @event.RepositoryID, @event.Gamemode));
        }
        private void On(ChallengeRequested @event)
        {
            Dict[@event.ID].Challenger = @event.AccountID;
            Dict[@event.ID].Challenged = @event.FriendID;
        }
        private void On(ChallengeAccepted @event)
        {
        }
        private void On(ChallengeDenied @event)
        {
            Dict[@event.ID].Challenger = default;
            Dict[@event.ID].Challenged = default;
        }
    }

    public class GameEntry
    {
        public GameID ID { get; }
        public int RepositoryID { get; }
        public Gamemode Gamemode { get; }
        public bool LoggedIn;
        public AggregateID<Account.Account> Challenger { get; set; }
        public AggregateID<Account.Account> Challenged { get; set; }

        public GameEntry(GameID id, int repositoryID, Gamemode gamemode)
        {
            ID = id;
            RepositoryID = repositoryID;
            Gamemode = gamemode;
        }
    }
}

