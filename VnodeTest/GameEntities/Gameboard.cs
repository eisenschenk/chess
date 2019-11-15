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

        private void PutPiecesInStartingPosition()
        {
            for (int index = 0; index < 2; index++)
                PutPiecesByColor(index);

        }
        private void PutPiecesByColor(int index)
        {
            if (index == 0)
            {
                for (int pawns = 0; pawns < 8; pawns++)
                    Board[pawns].Piece = new Pawn(pawns, PieceColor.White);
                Board[0].Piece = new Rook(0, PieceColor.White);
                Board[1].Piece = new Knight(0, PieceColor.White);
                Board[2].Piece = new Bishop(0, PieceColor.White);
                Board[3].Piece = new Queen(0, PieceColor.White);
                Board[4].Piece = new King(0, PieceColor.White);
                Board[5].Piece = new Bishop(0, PieceColor.White);
                Board[6].Piece = new Knight(0, PieceColor.White);
                Board[7].Piece = new Rook(0, PieceColor.White);
                return;
            }
            for (int pawns = 48; pawns < 56; pawns++)
                Board[pawns].Piece = new Pawn(pawns, PieceColor.White);
            Board[56].Piece = new Rook(0, PieceColor.White);
            Board[57].Piece = new Knight(0, PieceColor.White);
            Board[58].Piece = new Bishop(0, PieceColor.White);
            Board[59].Piece = new Queen(0, PieceColor.White);
            Board[60].Piece = new King(0, PieceColor.White);
            Board[61].Piece = new Bishop(0, PieceColor.White);
            Board[62].Piece = new Knight(0, PieceColor.White);
            Board[63].Piece = new Rook(0, PieceColor.White);
        }
    }
}
