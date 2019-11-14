using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    class King : BasePiece
    {
        public King(int positionX, int positionY, PieceColor color) : base(positionX, positionY, color)
        {
            Value = PieceValue.King;
        }

        public override bool NotBlocked(ValueTuple<int, int> target, Gameboard gameboard)
        {
            if (Checkmate())
                return false;
            return true;
        }

        //TODO: currently placeholder
        private bool Checkmate()
        {
            return false;
        }

        public override List<ValueTuple<int, int>> GetValidMovements()
        {
            List<ValueTuple<int, int>> returnValues = new List<(int, int)>();
            for (int indexRow = 0; indexRow < 8; indexRow++)
                for (int indexCul = 0; indexCul < 8; indexCul++)
                {
                    if (Position != (indexRow, indexCul) 
                        && indexCul >= Position.Item2 - 1 && indexCul <= Position.Item2 + 1 
                        && indexRow >= Position.Item2 - 1 && indexRow <= Position.Item2 + 1)
                        returnValues.Add((indexRow, indexCul));
                }
            return returnValues;

        }
    }
}
