﻿using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.BC.Account;
using VnodeTest.GameEntities;

namespace VnodeTest.BC.Game.Event
{

    public class GameOpened : AggregateEvent<Game>
    {
        public Gamemode Gamemode { get; }
        public double Clocktimer { get; }

        public GameOpened(AggregateID<Game> id, Gamemode gamemode, double clocktimer) : base(id)
        {
            Gamemode = gamemode;
            Clocktimer = clocktimer;
        }
    }
}
