using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ACL.UI.React.DOM;

namespace VnodeTest
{
    //Game doesnt finish right now
    public class GameboardController
    {
        public GameEntities.Gameboard GameBoard;
        private VNode[] GameRows = new VNode[8];
        private GameEntities.Tile Selected;
        private bool Promotion;
        private bool GameOver;
        private GameEntities.PieceColor CurrentPlayerColor = GameEntities.PieceColor.White;
        public GameboardController()
        {
            GameBoard = new GameEntities.Gameboard();
        }

        public VNode Render()
        {
            if (GameOver)
                return RenderGameOver();
            return Div(
                Promotion ? RenderPromotionSelection() : RenderBoard(GameBoard)
            );
        }

        private VNode RenderBoard(GameEntities.Gameboard board)
        {
            for (int indexRow = 0; indexRow < 8; indexRow++)
                GameRows[indexRow] = Row(board.Board.Where(x => x.Position / 8 == indexRow).Select(t => RenderTile(t)));
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

        private VNode RenderGameOver()
        {
            string winner;
            switch (CurrentPlayerColor)
            {
                case GameEntities.PieceColor.Black: winner = "White"; break;
                case GameEntities.PieceColor.White: winner = "Black"; break;
                case GameEntities.PieceColor.Zero: winner = "error"; break;
                default: winner = "error"; break;
            }
            return Text($"Gameover! {winner} won");
        }

        private VNode RenderPromotionSelection()
        {
            List<GameEntities.Tile> promotionSelect = new List<GameEntities.Tile>();
            promotionSelect.Add(new GameEntities.Tile(new GameEntities.Rook(0, Selected.Piece.Color), 0));
            promotionSelect.Add(new GameEntities.Tile(new GameEntities.Knight(1, Selected.Piece.Color), 1));
            promotionSelect.Add(new GameEntities.Tile(new GameEntities.Bishop(2, Selected.Piece.Color), 2));
            promotionSelect.Add(new GameEntities.Tile(new GameEntities.Queen(3, Selected.Piece.Color), 3));
            return Div(
                Styles.M2,
                Text($"Select Piece you want the Pawn to be promoted to.", Styles.FontSize1p5),
                Row(Fragment(promotionSelect.Select(x => RenderTile(x))))
            );
        }

        private void Select(GameEntities.Tile tile)
        {
            if (Promotion == true)
            {
                GameBoard.Board[Selected.Position].Piece = tile.Piece;
                GameBoard.Board[Selected.Position].Piece.Position = Selected.Position;
                Selected = null;
                Promotion = false;
                return;
            }
            if (Selected == null && tile.ContainsPiece && tile.Piece.Color == CurrentPlayerColor)
                Selected = tile;
            else if (Selected == tile)
                Selected = null;
            else if (Selected != null)
                if (TryMove(Selected, tile))
                {
                    TryEnablePromotion(tile);
                    ChangeCurrentPlayer();
                    CheckForGameOver();
                }
        }

        private bool TryCastling(GameEntities.Tile start, GameEntities.Tile target)
        {
            if (target.ContainsPiece)
                if (target.Piece is GameEntities.Rook && start.Piece is GameEntities.King
                && !target.Piece.HasMoved && !start.Piece.HasMoved
                && start.Piece.Color == target.Piece.Color
                && target.Piece.Position == target.Piece.StartPosition
                && start.Piece.Position == start.Piece.StartPosition
                && (target.Piece.GetStraightLines(GameBoard).Contains(start.Piece.Position + 1)
                || target.Piece.GetStraightLines(GameBoard).Contains(start.Piece.Position - 1)))
                {
                    int direction = 1;
                    if (start.Piece.Position > target.Piece.Position)
                        direction *= -1;

                    GameBoard.Board[start.Position + 2 * direction].Piece = start.Piece;
                    GameBoard.Board[start.Position + 2 * direction].Piece.Position = start.Position + 2 * direction;
                    GameBoard.Board[start.Position + direction].Piece = target.Piece;
                    GameBoard.Board[start.Position + direction].Piece.Position = start.Position + direction;

                    GameBoard.Board[start.Position].Piece = null;
                    GameBoard.Board[target.Position].Piece = null;
                    Selected = null;
                    return true;
                }
            return false;
        }

        private bool TryMove(GameEntities.Tile start, GameEntities.Tile target)
        {
            if (!TryCastling(start, target))
            {
                if (!start.Piece.GetValidMovements(GameBoard).Contains(target.Position) || OwnKingIsCheckedAfterMove(start, target))
                    return false;
                MovePiece(start, target);
                Selected = null;
                return true;
            }
            return false;
        }

        private void CheckForGameOver()
        {
            foreach (GameEntities.Tile tile in GameBoard.Board.Where(t => t.ContainsPiece && t.Piece.Color == CurrentPlayerColor))
                foreach (int potentialmove in tile.Piece.GetValidMovements(GameBoard))
                    if (!OwnKingIsCheckedAfterMove(tile, GameBoard.Board[potentialmove]))
                        return;
            GameOver = true;
        }

        private void MovePiece(GameEntities.Tile start, GameEntities.Tile target)
        {
            target.Piece = start.Piece;
            target.Piece.Position = target.Position;
            start.Piece = null;
        }

        private void ChangeCurrentPlayer()
        {
            if (CurrentPlayerColor == GameEntities.PieceColor.White)
                CurrentPlayerColor = GameEntities.PieceColor.Black;
            else
                CurrentPlayerColor = GameEntities.PieceColor.White;
        }

        private void TryEnablePromotion(GameEntities.Tile tile)
        {
            if (tile.Piece is GameEntities.Pawn && (tile.Piece.Position > 55 || tile.Piece.Position < 7))
            {
                Selected = tile;
                Promotion = true;
            }
        }

        private bool OwnKingIsCheckedAfterMove(GameEntities.Tile source, GameEntities.Tile targetTile)
        {
            GameEntities.Tile start = source.Copy();
            GameEntities.Tile target = targetTile.Copy();
            var futureGameBoard = GameBoard.Copy();
            var potentialmoves = new List<int>();
            futureGameBoard.Board[target.Position].Piece = start.Piece;
            futureGameBoard.Board[target.Position].Piece.Position = target.Position;
            futureGameBoard.Board[start.Position].Piece = null;
            var kingSameColorPosition = futureGameBoard.Board
                .Where(t => t.ContainsPiece && t.Piece.Color == start.Piece.Color && t.Piece is GameEntities.King)
                .First().Piece.Position;
            var enemyPieces = futureGameBoard.Board.Where(x => x.ContainsPiece && x.Piece.Color != start.Piece.Color);
            foreach (GameEntities.Tile tile in enemyPieces)
                potentialmoves.AddRange(tile.Piece.GetValidMovements(futureGameBoard));
            if (potentialmoves.Contains(kingSameColorPosition))
                return true;
            return false;
        }

    }
}
