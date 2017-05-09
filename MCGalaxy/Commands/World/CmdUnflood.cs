/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
namespace MCGalaxy.Commands.World {
    public sealed class CmdUnflood : Command {
        public override string name { get { return "unflood"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        
        public override void Use(Player p, string message)
        {
            if (p == null) { MessageInGameOnly(p); return; }
            if (message == "") { Help(p); return; }
            
            if (!message.CaselessEq("all") && Block.Byte(message) == Block.Invalid) {
                Player.Message(p, "There is no block \"" + message + "\"."); return;
            }            
            int phys = p.level.physics;
            CmdPhysics.SetPhysics(p.level, 0);
            
            Command cmd = Command.all.Find("replaceall");
            string args = message.CaselessEq("all") ?
                "lavafall waterfall lava_fast active_lava active_water " +
                "active_hot_lava active_cold_water fast_hot_lava magma geyser" : message;
            cmd.Use(p, args + " air");

            CmdPhysics.SetPhysics(p.level, phys);
            Chat.MessageLevel(p.level, "Unflooded!");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/unflood [liquid]");
            Player.Message(p, "%HUnfloods the map you are currently in of [liquid].");
            Player.Message(p, "%H  If [liquid] is \"all\", unfloods the map of all liquids.");
        }
    }
}
