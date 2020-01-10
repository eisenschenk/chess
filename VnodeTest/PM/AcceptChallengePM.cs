using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameID = ACL.ES.AggregateID<VnodeTest.BC.Game.Game>;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.Account.Account>;
using ACL.MQ;

namespace VnodeTest.PM
{
    public class AcceptChallengePM
    {



        public static void PMacceptChallenge(GameID gameID, AccountID senderID, AccountID receiverID)
        {
            BC.Game.Game.Commands.JoinGame(gameID, senderID);
            BC.Game.Game.Commands.JoinGame(gameID, receiverID);
            MessageBus.Instance.Send(new BC.Game.Command.AcceptChallenge(gameID, receiverID, senderID));
            BC.Game.Game.Commands.DeleteUnwantedChallenges(gameID, senderID, receiverID);

            //MessageBus.Instance.Send(new BC.Game.Command.JoinGame(gameID, senderID));
            //MessageBus.Instance.Send(new BC.Game.Command.JoinGame(gameID, receiverID));
        }
    }
}
