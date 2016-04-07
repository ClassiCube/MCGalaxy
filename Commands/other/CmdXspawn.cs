/*
	Copyright 2011 MCForge
	
	Written by jordanneil23
		
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

namespace MCGalaxy.Commands {
	public class CmdXspawn : Command {
    	public override string name { get { return "xspawn"; } }
		public override string shortcut { get { return ""; } }
		public override string type { get { return CommandTypes.Other; } }
		public override bool museumUsable { get { return false; } }
		public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
		
        public override void Use(Player p, string message) {
        	Player pl = PlayerInfo.FindOrShowMatches(p, message);
            if (pl == null) return;
            if (pl == p) {
                Player.SendMessage(p, "Use /spawn to respawn yourself."); return;
            }
            if (p != null && pl.group.Permission >= p.group.Permission) {
                MessageTooHighRank(p, "respawn", false); return;
            }
            
            Command.all.Find("spawn").Use(pl, "");
            Player.SendMessage(p, "Succesfully spawned " + pl.color + pl.DisplayName + "%S.");
            string src = p == null ? "(console)" : p.color + p.DisplayName;
            Player.GlobalMessage(pl.color + pl.name + " %Swas respawned by " + src + "%S.");
        }
		
		public override void Help(Player p) {
			Player.SendMessage(p, "/xspawn [player] - Spawn another player.");
            Player.SendMessage(p, "WARNING: It says who used it!");
		}
	}
}
