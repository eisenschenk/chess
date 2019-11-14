using ACL.UI.React;
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
        public GameEntities.Gameboard Board;
        public GameboardController()
        {
            Board = new GameEntities.Gameboard();
        }

        public static VNode Render()
        {
            return Div(
                RenderTile(new GameEntities.Tile(1))
                ); 
        }

        private VNode RenderBoard()
        {
            return Div();
            //for (int index = 0; index < 8; index++)
            //    Fragment(Board.Board.Where(y => y.Index / 8 <= index).Select(y => RenderTile(y)));
        }
        private static VNode RenderTile(GameEntities.Tile tile)
        {
            return Div(
                tile.TileStyle,
                tile.ContainsPiece ? Text(tile.Piece.Sprite) : null
            );
        }
    }
}
