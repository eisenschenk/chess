using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ACL.UI.React.DOM;

namespace VnodeTest
{
    class GamemodeController
    {
        //public GameEntities.Gameboard Gameboard = new GameEntities.Gameboard(TimeSpan.FromSeconds(30000000));
        //private bool SelectionDone;
        //public GamemodeController() { }
        //public VNode Render()
        //{

        //    return SelectionDone ? Gameboard.GbController.Render() : RenderGameModeSelection();
        //}

        //public VNode RenderGameModeSelection()
        //{
        //    return Div(
        //        Button("Player vs. Player (Offline)", () => { SelectGameMode(false, false, false); Render(); }),
        //        Button("Player vs. AI (Offline)", () => SelectGameMode(false, true, false)),
        //        Button("AI vs. AI (Offline)", () => SelectGameMode(true, true, false)),
        //        Button("Player vs. Player (Online)", () => SelectGameMode(false, false, true))
        //    );
        //}
        //private void SelectGameMode(bool whiteAI, bool blackAI, bool online)
        //{
        //    Gameboard.PlayedByEngine = (whiteAI, blackAI);
        //    SelectionDone = true;
        //}
    }
}
