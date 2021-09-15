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
using MCGalaxy.Games;

namespace MCGalaxy.Commands.Bots {
    public sealed class CmdBotWhere : Command2 {
        public override string name { get { return "BotWhere"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool SuperUseable { get { return false; } }
        
        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) return;

            PlayerBot target = Matcher.FindBots(p, message);
            if (target == null) return;
            
            int x = target.Pos.X, y = target.Pos.Y - Entities.CharacterHeight, z = target.Pos.Z;
            p.Message("Position of bot {0}:", target.DisplayName);
            p.Message("   X: &b{0:F5} &SY: &b{1:F5} &SZ: &b{2:F5}",
                           x / 32.0, y / 32.0, z / 32.0);
            
            p.Message("   Yaw: &b{0} &Sdegrees, Pitch: &b{1} &Sdegrees",
                           Orientation.PackedToDegrees(p.Rot.RotY),
                           Orientation.PackedToDegrees(p.Rot.HeadX));
        }      

        public override void Help(Player p) {
            p.Message("&T/BotWhere [name]");
            p.Message("&HDisplays position, and orientation of that bot.");
        }
    }
}
