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

        protected override IEnumerable<int> GetPotentialMovements(Gameboard gameboard)
        {
            return GetDiagonals(gameboard, 1).Concat(GetStraightLines(gameboard, 1)).Concat(GetCastlingPositions(gameboard));
        }

        private IEnumerable<int> GetCastlingPositions(Gameboard gameboard)
        {
            bool EmptyAndNoCheck(int direction)
            {
                if (direction < Position)
                {
                    var rookTile = gameboard.Board[Position - 4];
                    if (HasMoved || !rookTile.ContainsPiece || !(rookTile.Piece is Rook) || rookTile.Piece.HasMoved)
                        return false;
                    for (int index = Position + direction; index > Position - 4; index--)
                        if (gameboard.Board[index].ContainsPiece)
                            return false;
                    if (gameboard.CheckForGameOver()
                    || HypotheticalMove(gameboard, Position - 1).CheckForGameOver() || HypotheticalMove(gameboard, Position - 2).CheckForGameOver())
                        return false;
                    return true;
                }
                else
                {
                    var rookTile = gameboard.Board[Position + 3];
                    if (HasMoved || !rookTile.ContainsPiece || !(rookTile.Piece is Rook) || rookTile.Piece.HasMoved)
                        return false;
                    for (int index = Position + direction; index > Position + 3; index++)
                        if (gameboard.Board[index].ContainsPiece)
                            return false;
                    if (gameboard.CheckForGameOver()
                        || HypotheticalMove(gameboard, Position + 1).CheckForGameOver() || HypotheticalMove(gameboard, Position + 2).CheckForGameOver())
                        return false;
                    return true;
                }
            }
            if (EmptyAndNoCheck(-1))
                yield return Position - 2;
            if (EmptyAndNoCheck(1))
                yield return Position + 2;
        }

        public override BasePiece Copy() => new King(Position, Color);
    }
}
