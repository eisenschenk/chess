using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    public abstract class BasePiece
    {
        public PieceColor Color { get; set; }
        public PieceValue Value { get; set; }
        public string Sprite { get; }
        public int Position { get; set; }
        public int[] ValidMoves { get; set; }

        public BasePiece(int position, PieceColor color)
        {
            Position = position;
            Color = color;
            ValidMoves = GetValidMovements();
        }

        public abstract int[] GetValidMovements();
    }
}
