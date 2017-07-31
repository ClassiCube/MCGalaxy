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

namespace MCGalaxy.Commands.Info {
    public sealed class CmdWhere : Command {
        public override string name { get { return "Where"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        
        public override void Use(Player p, string message) {
            if (message.Length == 0) message = p.name;
            int matches;
            Player pl = PlayerInfo.FindMatches(p, message, out matches);
            if (pl == null) return;
            if (pl.level.CurrentGame() != null && !(p == null || p.Game.Referee)) {
                Player.Message(p, "You can only use /where on people in games when you are in referee mode."); return;
            }
            
            int x = pl.Pos.X, y = pl.Pos.Y - Entities.CharacterHeight, z = pl.Pos.Z;
            Player.Message(p, "{0} %Sis on {1}", pl.ColoredName, pl.level.ColoredName);
            Player.Message(p, "   X: &b{0:F5} %SY: &b{1:F5} %SZ: &b{2:F5}",
                           x / 32.0, y / 32.0, z / 32.0);
            
            Player.Message(p, "   Yaw: &b{0} %Sdegrees, Pitch: &b{1} %Sdegrees",
                           Orientation.PackedToDegrees(p.Rot.RotY),
                           Orientation.PackedToDegrees(p.Rot.HeadX));
        }      

        public override void Help(Player p) {
            Player.Message(p, "%T/Where [name]");
            Player.Message(p, "%HDisplays level, position, and orienatation of that player.");
        }
    }
}
