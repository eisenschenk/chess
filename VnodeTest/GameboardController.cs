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
        public GameEntities.Gameboard GameBoard;
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
            return Fragment(
                GameBoard.Board.Where(x => x.Position % 8 == 0).Select(s => Row(GameBoard.Board.Skip( s.Position / 8).Take(8).Select(t=>RenderTile(t))))
            );
        }
        private static VNode RenderTile(GameEntities.Tile tile)
        {
            return Div(
                tile.Style,
                tile.ContainsPiece ? Text(tile.Piece.Sprite) : null
            );
        }
    }
}
