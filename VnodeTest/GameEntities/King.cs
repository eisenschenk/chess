using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    class King : BasePiece
    {
        public King(int position, PieceColor color) : base(position, color)
        {
            Value = PieceValue.King;
        }

        //public King(int position, PieceColor color, bool hasMoved, int startPosition) : base(position, color)
        //{
        //    Value = PieceValue.King;
        //    HasMoved = hasMoved;
        //    StartPosition = startPosition;
        //}

        protected override IEnumerable<int> GetPotentialMovements(Gameboard gameboard)
        {
            return GetDiagonals(gameboard, 1).Concat(GetStraightLines(gameboard, 1)).Concat(GetCastlingPositions(gameboard));
        }

        private IEnumerable<int> GetCastlingPositions(Gameboard gameboard)
        {
            bool EmptyAndNoCheck(int direction, BasePiece rookTile)
            {
                if (!(rookTile is Rook) || rookTile.HasMoved)
                    return false;

                if (direction < 0)
                {
                    for (int index = Position + direction; index >= rookTile.Position; index--)
                        if (gameboard.Board[index] != null)
                            return false;
                }
                else
                {
                    for (int index = Position + direction; index <= rookTile.Position; index++)
                        if (gameboard.Board[index] != null)
                            return false;
                }
                if (gameboard.CheckDetection(Color)
                    || HypotheticalMove(gameboard, Position + direction).CheckDetection(Color) || HypotheticalMove(gameboard, Position + 2 * direction).CheckDetection(Color))
                    return false;
                return true;
            }
            if (!HasMoved && (Position == 4 || Position == 60))
            {

                var rookTileLeft = gameboard.Board[Position - 4];
                var rookTileRight = gameboard.Board[Position + 3];

                if (rookTileLeft != default && EmptyAndNoCheck(-1, rookTileLeft))
                    yield return Position - 2;
                if (rookTileRight != null && EmptyAndNoCheck(1, rookTileRight))
                    yield return Position + 2;
            }
        }

        public override BasePiece Copy() => new King(Position, Color);
    }
}
