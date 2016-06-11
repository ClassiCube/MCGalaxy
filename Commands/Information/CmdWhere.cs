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

namespace MCGalaxy.Commands {
    
    public sealed class CmdWhere : Command {
        public override string name { get { return "where"; } }
        public override string shortcut { get { return "where"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        
        public override void Use(Player p, string message) {
            if (message == "") message = p.name;
            int matches;
            Player pl = PlayerInfo.FindMatches(p, message, out matches);
            if (pl == null) return;
            if (pl.level.CurrentGame() != null && !(p == null || p.Game.Referee)) {
                Player.Message(p, "You can only use /where on people in games when you are in referee mode."); return;
            }
            
            short x = (short)pl.pos[0], y = (short)pl.pos[1], z = (short)pl.pos[2];
            Player.Message(p, "{0}%S is on {1} (X:&b{2}%S Y:&b{3}%S Z:&b{4}%S)",
                           pl.ColoredName, pl.level.name, x >> 5, (y - 51) >> 5, z >> 5);
            Player.Message(p, "Yaw: &b{0}%S degrees, Pitch: &b{1}%S degrees",
                           (int)(p.rot[0] * 360 / 256), (int)(p.rot[1] * 360 / 256));
        }      

        public override void Help(Player p) {
            Player.Message(p, "%T/where [name] %H- Displays level, position, " +
                           "and orienatation of that player.");
        }
    }
}
