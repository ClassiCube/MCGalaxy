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
namespace MCGalaxy.Commands.Chatting {    
    public sealed class CmdRoll : MessageCmd {
        public override string name { get { return "Roll"; } }

        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces();
            int min = 1, max = 6;            
            if (message.Length > 0 && !CommandParser.GetInt(p, message, "Min", ref min)) return;
            if (args.Length    > 1 && !CommandParser.GetInt(p, args[1], "Max", ref max)) return;
            
            if (min > max) { int tmp = min; min = max; max = tmp; }            
            // rand.Next(min, max) is exclusive of max, so we need to use (max + 1)
            int adjMax = max == int.MaxValue ? int.MaxValue : max + 1;
            
            string msg = "λNICK %Srolled a &a" + new Random().Next(min, adjMax) + " %S(" + min + "|" + max + ")";
            TryMessage(p, msg);
        }
        
        public override void Help(Player p) {
            p.Message("%T/Roll [min] [max]");
            p.Message("%HRolls a random number between [min] and [max].");
        }
    }
}
