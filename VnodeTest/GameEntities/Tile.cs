using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.GameEntities
{
    public class Tile
    {
        public PieceColor Color { get; }
        public Style Style { get; }
        public BasePiece Piece { get; set; }
        public bool ContainsPiece => Piece == null ? false : true;
        public int Index { get; }
        public int Position { get; }

        public Tile(int index)
        {
            Position = index;
            Color = GetColor();
            Style = GetStyle();
        }
        private Style GetStyle()
        {
            return Color switch
            {
                PieceColor.Black => Styles.TileBlack,
                PieceColor.White => Styles.TileWhite,
                _ => Styles.TCwhite
            };
        }

        private PieceColor GetColor()
        {
            var rowEven = (Position / 8) % 2;
            if (rowEven == 0)
                return (Position % 2) switch
                {
                    0 => PieceColor.Black,
                    1 => PieceColor.White,
                    _ => PieceColor.Zero
                };
            return (Position % 2) switch
            {
                1 => PieceColor.White,
                0 => PieceColor.Black,
                _ => PieceColor.Zero
            };
        }
    }
}
