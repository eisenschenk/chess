﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    class Rook : BasePiece
    {
        public Rook(int position, PieceColor color) : base(position, color)
        {
            Value = PieceValue.Rook;
        }

        protected override IEnumerable<int> GetPotentialMovements(Gameboard gameboard)
        {
            return GetStraightLines(gameboard);
        }

        public override BasePiece Copy() => new Rook(Position, Color);
    }
}
