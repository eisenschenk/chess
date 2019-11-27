using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    public class Gameboard
    {
        public Tile[] Board = new Tile[64];

        public Gameboard()
        {
            for (int index = 0; index < 64; index++)
                Board[index] = new Tile(index);
            PutPiecesInStartingPosition();
        }

        public Gameboard(IEnumerable<Tile> collection)
        {
            Board = collection.ToArray();
        }

        private void PutPiecesInStartingPosition()
        {
            for (int pawns = 8; pawns < 16; pawns++)
                Board[pawns].Piece = new Pawn(pawns, PieceColor.White);
            Board[0].Piece = new Rook(0, PieceColor.White);
            Board[1].Piece = new Knight(1, PieceColor.White);
            Board[2].Piece = new Bishop(2, PieceColor.White);
            Board[3].Piece = new Queen(3, PieceColor.White);
            Board[4].Piece = new King(4, PieceColor.White);
            Board[5].Piece = new Bishop(5, PieceColor.White);
            Board[6].Piece = new Knight(6, PieceColor.White);
            Board[7].Piece = new Rook(7, PieceColor.White);

            for (int pawns = 48; pawns < 56; pawns++)
                Board[pawns].Piece = new Pawn(pawns, PieceColor.Black);
            Board[56].Piece = new Rook(56, PieceColor.Black);
            Board[57].Piece = new Knight(57, PieceColor.Black);
            Board[58].Piece = new Bishop(58, PieceColor.Black);
            Board[59].Piece = new Queen(59, PieceColor.Black);
            Board[60].Piece = new King(60, PieceColor.Black);
            Board[61].Piece = new Bishop(61, PieceColor.Black);
            Board[62].Piece = new Knight(62, PieceColor.Black);
            Board[63].Piece = new Rook(63, PieceColor.Black);
        }

        public Gameboard Copy() => new Gameboard(Board.Select(t => t.Copy()));
    }
}
