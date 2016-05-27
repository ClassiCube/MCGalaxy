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
    
    public sealed class CmdRoll : Command {
        
        public override string name { get { return "roll"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdRoll() { }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            Random rand = new Random();
            int min, max;
            
            if (!int.TryParse(args[0], out min)) min = 1;
            if (args.Length == 1 || !int.TryParse(args[1], out max)) max = 7;
            if (min > max) { int a = min; min = max; max = a; }

            Player.GlobalMessage(p.ColoredName + " %Srolled a &a" + rand.Next(min, max + 1) + " %S(" + min + "|" + max + ")");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/roll [min] [max] - Rolls a random number between [min] and [max].");
        }
    }
}
