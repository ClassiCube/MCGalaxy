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

namespace MCGalaxy.Commands {
	
    public sealed class CmdAka : Command {
		
        public override string name { get { return "aka"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdAka() { }
        
        public override void Use(Player p, string message) {
            bool showInfected = p.aka; p.aka = !p.aka;
            Player[] players = PlayerInfo.Online;
            
            foreach (Player pl in players) {
            	if (pl.level != p.level || p == pl || !Player.CanSee(p, pl) || pl.referee) continue;
                
                p.SendDespawn(pl.id);
                string name = null;
                if (pl.infected && showInfected) {
                    name = Server.ZombieName != "" ? Colors.red + Server.ZombieName : Colors.red + pl.name;
                } else {
                    name = pl.color + pl.name;
                }
                p.SendSpawn(pl.id, name, pl.pos[0], pl.pos[1], pl.pos[2], pl.rot[0], pl.rot[1]);
            }
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/aka");
            Player.SendMessage(p, "%HToggles whether infected players show their actual names.");
        }
    }
}
