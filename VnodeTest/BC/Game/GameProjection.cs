using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public void StartThread()
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                while (true)
                    foreach (GameEntry entry in Games.ToArray())
                        if (!StillValid(entry))
                            Game.Commands.CloseGame(entry.ID);
            });
        }

        private bool StillValid(GameEntry entry)
        {
            bool IsValid()
            {
                return entry.Created.AddSeconds(entry.Timer) > DateTime.Now;
            }

            return GameRepository.Instance.TryGetGame(entry.ID, out var game) && game.HasOpenSpots && IsValid();
        }
        private void On(GameOpened @event)
        {
            GameRepository.Instance.AddGame(@event.ID, @event.Gamemode, new Gameboard());
            Dict.Add(@event.ID, new GameEntry(@event.ID, @event.Gamemode));
        }
        private void On(ChallengeRequested @event)
        {
            Dict[@event.ID].Challenger = @event.AccountID;
            Dict[@event.ID].Challenged = @event.FriendID;
        }
        private void On(ChallengeAccepted @event)
        {
            var gameIDs = Dict.Values.Where(x => x.Challenged == @event.AccountID || x.Challenged == @event.FriendID
                || x.Challenger == @event.AccountID || x.Challenger == @event.FriendID).Select(f => f.ID);
            foreach (GameID id in gameIDs)
            {
                Dict[id].Challenged = default;
                Dict[id].Challenger = default;
            }
        }
        private void On(GameClosed @event)
        {
            Dict.Remove(@event.ID);
        }
        private void On(ChallengeDenied @event)
        {
            Dict.Remove(@event.ID);
        }
        private void On(GameSaved @event)
        {
            Dict[@event.ID].AllMoves = @event.Moves;
        }
        private void On(GameJoined @event)
        {
            var entry = Dict[@event.ID];
            if (@event.AccountID != entry.Challenged && entry.PlayerWhite == default)
                entry.PlayerWhite = @event.AccountID;
            else
                entry.PlayerBlack = @event.AccountID;
        }
        private void On(GamesResetted @event)
        {
        }
    }

    public class GameEntry
    {
        public GameID ID { get; }
        public int RepositoryID { get; }
        public Gamemode Gamemode { get; }
        public string AllMoves;
        public bool LoggedIn;
        public DateTime Created = DateTime.Now;
        public int Timer = 30;
        public TimeSpan Elapsed => DateTime.Now - Created;
        public AggregateID<Account.Account> Challenger { get; set; }
        public AggregateID<Account.Account> Challenged { get; set; }
        public AggregateID<Account.Account> PlayerWhite { get; set; }
        public AggregateID<Account.Account> PlayerBlack { get; set; }


        public GameEntry(GameID id, Gamemode gamemode)
        {
            ID = id;
            Gamemode = gamemode;
        }

    }
}

