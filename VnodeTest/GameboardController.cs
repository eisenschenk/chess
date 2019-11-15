﻿using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ACL.UI.React.DOM;

namespace VnodeTest
{
    public class GameboardController
    {
        public GameEntities.Gameboard GameBoard;
        private VNode[] GameRows = new VNode[8];
        private GameEntities.Tile Selected;
        public GameboardController()
        {
            GameBoard = new GameEntities.Gameboard();
        }

        public VNode Render()
        {
            return Div(
                RenderBoard()
                );
        }

        private VNode RenderBoard()
        {

            for (int indexRow = 0; indexRow < 8; indexRow++)
                GameRows[indexRow] = Row(GameBoard.Board.Where(x => x.Position / 8 == indexRow).Select(t => RenderTile(t)));
            return Fragment(GameRows.Select(x => x));

        }
        private VNode RenderTile(GameEntities.Tile tile)
        {
            var div = Div(
                tile.Style & (tile == Selected ? Styles.Selected : tile.BorderStyle),
                tile.ContainsPiece ? Text(tile.Piece.Sprite, Styles.FontSize3) : null
            );
            div.OnClick = () => Select(tile);
            return div;
        }
        private void Select(GameEntities.Tile tile)
        {
            if (Selected == null && tile.ContainsPiece)
                Selected = tile;
            else if (Selected == tile)
                Selected = null;
            else
            {
                TryMove(Selected, tile);
                Selected = null;
            }
        }
        private bool TryMove(GameEntities.Tile start, GameEntities.Tile target)
        {
            if (!start.Piece.GetValidMovements(GameBoard).Contains(target.Position))
                return false;
            target.Piece = start.Piece;
            start.Piece = null;
            return true;
        }
    }
}
