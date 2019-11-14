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
        public bool ContainsPiece { get; set; }
        public PieceColor Color { get; }
        public Style TileStyle { get; set; }
        public BasePiece Piece { get; set; }
        public int Index { get; }

        public Tile(int index)
        {
            Color = GetColor(index % 2);
            TileStyle = GetStyle();
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
        private PieceColor GetColor(int color)
        {
            return color switch
            {
                0 => PieceColor.Black,
                1 => PieceColor.White,
                _ => PieceColor.Zero
            };
        }
    }
}
