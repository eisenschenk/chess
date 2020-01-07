﻿using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.GameEntities;

namespace VnodeTest.BC.Game.Command
{
    public class OpenGame : AggregateCommand<Game>
    {
        public Gamemode Gamemode { get; }
        public OpenGame(AggregateID<Game> id, Gamemode gamemode) : base(id)
        {
            Gamemode = gamemode;
        }




    }
}
