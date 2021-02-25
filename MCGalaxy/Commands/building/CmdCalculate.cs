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
    public sealed class CmdCalculate : Command2 {
        public override string name { get { return "Calculate"; } }
        public override string shortcut { get { return "Calc"; } }
        public override string type { get { return CommandTypes.Building; } }
        
        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces();
            if (args.Length < 2) { Help(p); return; }
            
            double n1 = 0, n2 = 0, result = 0;
            string r1 = args[0], op = args[1], r2 = null, format = null;
            
            if (!Utils.TryParseDouble(r1, out n1)) {
                p.Message("&W\"{0}\" is not a valid number.", r1); return;
            }

            char sym = op[0];
            if (sym == '+' || sym == '-' || sym == '*' || sym == '/' || sym == '^') {
                if (args.Length == 2 ||op.Length > 1) { Help(p); return; }
                r2 = args[2];
                
                if (!Utils.TryParseDouble(r2, out n2)) {
                    p.Message("&W\"{0}\" is not a valid number.", r2); return;
                }
                
                if (sym == '+') { result = n1 + n2; }
                if (sym == '-') { result = n1 - n2; }
                if (sym == '*') { result = n1 * n2; }
                if (sym == '/') { result = n1 / n2; }
                if (sym == '^') { result = Math.Pow(n1, n2); }
                
                format = "&aResult&f: {0} {1} {2} = {3}";
            } else if (op == "sqrt") {
                result = Math.Sqrt(n1);
                format = "&aResult&f: Square root of {0} = {3}";
            } else if (op == "square") {
                result = n1 * n1;
                format = "&aResult&f: Square of {0} = {3}";
            }  else if (op == "cubed") {
                result = n1 * n1 * n1;
                format = "&aResult&f: Cube of {0} = {3}";
            } else {
                p.Message("&WOnly supported operators are: +, -, *, /, sqrt, square, or cubed");
                return;
            }
            
            p.Message(format, r1, op, r2, result);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Calculate [num1] [operation] [num2]");
            p.Message("&H[operation] can be +, -, /, or *");
            p.Message("&T/Calculate [num1] [operation]");
            p.Message("&H[operation] can be sqrt, square, or cubed");
        }
    }
}
