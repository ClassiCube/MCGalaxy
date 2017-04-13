/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
namespace MCGalaxy.Commands {
    
    public sealed class CmdRoll : MessageCmd {
        public override string name { get { return "roll"; } }
        public CmdRoll() { }

        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces();
            Random rand = new Random();
            int min, max;
            
            if (!int.TryParse(args[0], out min)) min = 1;
            if (args.Length == 1 || !int.TryParse(args[1], out max)) max = 6;
            if (min > max) { int a = min; min = max; max = a; }
            
            // rand.Next(min, max) is exclusive of max, so we need to use (max + 1)
            int adjMax = max == int.MaxValue ? int.MaxValue : max + 1;
            string msg = p.ColoredName + " %Srolled a &a" + rand.Next(min, adjMax) + " %S(" + min + "|" + max + ")";
            TryMessage(p, msg);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/roll [min] [max]");
            Player.Message(p, "%HRolls a random number between [min] and [max].");
        }
    }
}
