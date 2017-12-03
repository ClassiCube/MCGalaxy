/*
    Copyright 2015 MCGalaxy
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.opensource.org/licenses/ecl2.php
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdCalculate : Command {
        public override string name { get { return "Calculate"; } }
        public override string shortcut { get { return "Calc"; } }
        public override string type { get { return CommandTypes.Building; } }
        
        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces();
            if (args.Length < 2) { Help(p); return; }
            
            float n1, n2; string op = args[1];
            if (!Utils.TryParseDecimal(args[0], out n1)) {
                Player.Message(p, "\"{0}\" is not a valid number.", args[0]); return;
            }

            if (op == "+" || op == "-" || op == "*" || op == "/" || op == "^") {
                if (args.Length == 2) { Help(p); return; }
                if (!Utils.TryParseDecimal(args[2], out n2)) {
                    Player.Message(p, "\"{0}\" is not a valid number.", args[2]); return;
                }
                
                float result = 0;
                if (op == "+") { result = n1 + n2; }
                if (op == "-") { result = n1 - n2; }
                if (op == "*") { result = n1 * n2; }
                if (op == "/") { result = n1 / n2; }
                if (op == "^") { result = (float)Math.Pow(n1, n2); }
                
                Player.Message(p, "&aResult&f: {0} {1} {2} = {3}", n1, op, n2, result);
            } else if (op == "sqrt") {
                double sqrt = Math.Sqrt(n1);
                Player.Message(p, "&aResult&f: Square Root of {0} = {1}", n1, sqrt);
            } else if (op == "square") {
                Player.Message(p, "&aResult&f: Square of {0} = {1}", n1, n1 * n1);
            }  else if (op == "cubed") {
                Player.Message(p, "&aResult&f: Cube of {0} = {1}", n1, n1 * n1 * n1);
            } else {
                Player.Message(p, "&cOnly supported operators are: +, -, *, /, sqrt, square, or cubed");
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Calculate [num1] [operation] [num2]");
            Player.Message(p, "%H[operation] can be +, -, /, or *");
            Player.Message(p, "%T/Calculate [num1] [operation]");
            Player.Message(p, "%H[operation] can be sqrt, square, or cubed");
        }
    }
}
