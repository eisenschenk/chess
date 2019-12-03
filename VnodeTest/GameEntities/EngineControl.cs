using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ACL.UI.React.DOM;

namespace VnodeTest.GameEntities
{
    public class EngineControl
    {
        public Process Engine { get; }
        public string EngineMove;
        public EngineControl()
        {
            var startinfo = new ProcessStartInfo("C:\\Users\\eisenschenk\\Downloads\\stockfish-10-win\\Windows\\stockfish_10_x64.exe")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false,
            };
            Engine = Process.Start(startinfo);
        }
        public void MakeEngineMove(Gameboard gameboard)
        {
            var output = string.Empty;
            Engine.StandardInput.WriteLine($"position fen \"{gameboard.GetFeNotation()}\"");
            Engine.StandardInput.WriteLine("setoption name MultiPV value 3");
            Engine.StandardInput.WriteLine("go movetime 3000");
            while (!output.StartsWith("bestmove"))
                output = Engine.StandardOutput.ReadLine();
            output = output.Remove(0, 8);
            var _output = output.Split();
            EngineMove = _output[1];
            var XY = GetCoordinates(EngineMove);
            gameboard.TryMove(gameboard.Board[XY.start], gameboard.Board[XY.target]);
        }


        private static (int start, int target) GetCoordinates(string input)
        {
            var startX = Gameboard.ParseStringXToInt(input[0].ToString());
            var startY = Gameboard.ParseStringYToInt(input[1].ToString());
            var targetX = Gameboard.ParseStringXToInt(input[2].ToString());
            var targetY = Gameboard.ParseStringYToInt(input[3].ToString());
            return (startX + startY * 8, targetX + targetY * 8);
        }


    }
}
