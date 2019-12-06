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
    public interface IEngine
    {
        string GetEngineMove(string feNotation);
    }

    public class EngineControl : IEngine
    {
        public Process Engine { get; }
        public BasePiece Promotion { get; set; }
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
        public string GetEngineMove(string feNotation)
        {
            var output = string.Empty;
            Engine.StandardInput.WriteLine($"position fen \"{feNotation}\"");
            Engine.StandardInput.WriteLine("setoption name MultiPV value 3");
            Engine.StandardInput.WriteLine("go movetime 3000");
            while (!output.StartsWith("bestmove"))
                output = Engine.StandardOutput.ReadLine();
            output = output.Remove(0, 8);
            var _output = output.Split();
            return _output[1];
        }       
    }
}
