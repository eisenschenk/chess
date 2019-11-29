﻿using ACL.UI.React;
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
        public Style BorderStyle { get; }
        public BasePiece Piece { get; set; }
        public bool ContainsPiece => Piece == null ? false : true;
        public int Position { get; }
        public (int X, int Y) PositionXY => (Position % 8, Position / 8);

        public Tile(int index)
        {
            Position = index;
            Color = GetColor();
            Style = GetBaseStyle();
            BorderStyle = GetBorderStyle();
        }

        public Tile(BasePiece piece, int index)
        {
            Position = index;
            Color = GetColor();
            Style = GetBaseStyle();
            BorderStyle = GetBorderStyle();
            Piece = piece;
        }

        private Style GetBaseStyle()
        {
            return Color switch
            {
                PieceColor.Black => Styles.TileBlack,
                PieceColor.White => Styles.TileWhite,
                _ => Styles.TCwhite
            };
        }

        private Style GetBorderStyle()
        {
            return Color switch
            {
                PieceColor.Black => Styles.BorderBlack,
                PieceColor.White => Styles.BorderWhite,
                _ => Styles.TCwhite
            };
        }

        private PieceColor GetColor()
        {
            //Boolsche algebra ^ = XOR
            var rowEven = (Position / 8) % 2 == 0;
            if (rowEven)
                return (Position % 2) switch
                {
                    0 => PieceColor.Black,
                    1 => PieceColor.White,
                    _ => PieceColor.Zero
                };
            return (Position % 2) switch
            {
                0 => PieceColor.White,
                1 => PieceColor.Black,
                _ => PieceColor.Zero
            };
        }

        internal Tile Copy() => new Tile(Piece?.Copy(), Position);

    }
}
