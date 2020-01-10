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

        public void CloseGamesAfterChallengeExpires()
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                while (true)
                    foreach (GameEntry entry in Games.ToArray())
                        if (!StillValid(entry))
                            Game.Commands.DeleteGame(entry.ID);
            });
        }
        //TODO gamedelete & gameended
        private bool StillValid(GameEntry entry)
        {
            bool isValid = entry.Created.AddSeconds(entry.Timer) > DateTime.Now;

            return entry.Game.HasOpenSpots && isValid;
        }

        private void On(GameOpened @event)
        {
            Dict.Add(@event.ID, new GameEntry(@event.ID, @event.Gamemode));
        }
        private void On(ChallengeRequested @event)
        {
            Dict[@event.ID].Challenger = @event.AccountID;
            Dict[@event.ID].Challenged = @event.FriendID;
        }
        private void On(ChallengeAccepted @event)
        {
            //TODO: ->PM
            //TOASK: can i use commands here?
            var gameIDs = Dict.Values.Where(x => x.Challenged == @event.AccountID || x.Challenged == @event.FriendID
                || x.Challenger == @event.AccountID || x.Challenger == @event.FriendID).Select(f => f.ID);
            foreach (GameID id in gameIDs)
                Dict.Remove(id);
        }
        private void On(GameDeleted @event)
        {
            Dict.Remove(@event.ID);
        }
        private void On(GameEnded @event)
        {
            Dict[@event.ID].AllMoves = @event.Moves;
        }
        private void On(ChallengeDenied @event)
        {
            Dict.Remove(@event.ID);
        }
        private void On(GameJoined @event)
        {
            var entry = Dict[@event.ID];
            if (@event.AccountID != entry.Challenged && entry.PlayerWhite == default)
                entry.PlayerWhite = @event.AccountID;
            else
                entry.PlayerBlack = @event.AccountID;
        }
    }

    public class GameEntry
    {
        public GameID ID { get; }
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
        public VnodeTest.Game Game;
        public bool GameOver => Winner.HasValue;
        public PieceColor? Winner => Game?.Winner;


        public GameEntry(GameID id, Gamemode gamemode, double playerClockTime = 50000)
        {
            ID = id;
            Gamemode = gamemode;
            Game = new VnodeTest.Game(id, gamemode, new Gameboard(), playerClockTime);
        }

    }
}

