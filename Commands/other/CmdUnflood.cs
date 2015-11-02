/*
	Copyright 2011 MCGalaxy
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
namespace MCGalaxy.Commands
{
    public sealed class CmdUnflood : Command
    {
        public override string name { get { return "unflood"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override void Help(Player p) { Player.SendMessage(p, "/unflood [liquid] - Unfloods the map you are on of [liquid]"); }
        public override void Use(Player p, string message)
        {
            if (p == null) { Player.SendMessage(p, "This command can only be used in-game!"); return; }
            if (String.IsNullOrEmpty(message)) { Help(p); return; }
            if (message.ToLower() != "all" && Block.Byte(message) == Block.Zero) { Player.SendMessage(p, "There is no block \"" + message + "\"."); return; }
            int phys = p.level.physics;
            Command.all.Find("physics").Use(p, "0");
            if (!p.level.Instant)
                Command.all.Find("map").Use(p, "instant");

            if (message.ToLower() == "all")
            {
                Command.all.Find("replaceall").Use(p, "lavafall air");
                Command.all.Find("replaceall").Use(p, "waterfall air");
                Command.all.Find("replaceall").Use(p, "lava_fast air");
                Command.all.Find("replaceall").Use(p, "active_lava air");
                Command.all.Find("replaceall").Use(p, "active_water air");
                Command.all.Find("replaceall").Use(p, "active_hot_lava air");
                Command.all.Find("replaceall").Use(p, "active_cold_water air");
                Command.all.Find("replaceall").Use(p, "fast_hot_lava air");
                Command.all.Find("replaceall").Use(p, "magma air");
            }
            else
            {
                Command.all.Find("replaceall").Use(p, message + " air");
            }

            if (p.level.Instant)
                Command.all.Find("map").Use(p, "instant");
            Command.all.Find("reveal").Use(p, "all");
            Command.all.Find("physics").Use(p, phys.ToString());
            Player.GlobalMessage("Unflooded!");
        }
    }
}
