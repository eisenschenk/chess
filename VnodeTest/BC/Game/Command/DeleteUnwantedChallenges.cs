using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameID = ACL.ES.AggregateID<VnodeTest.BC.Game.Game>;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.Account.Account>;

namespace VnodeTest.BC.Game.Command
{
    public class DeleteUnwantedChallenges : AggregateCommand<Game>
    {
        public DeleteUnwantedChallenges(GameID id, AccountID receiver, AccountID challenger) : base(id)
        {
            Receiver = receiver;
            Challenger = challenger;
        }

        public AccountID Receiver { get; }
        public AccountID Challenger { get; }
    }
}
