/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
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
namespace MCGalaxy.Commands
{
    public sealed class CmdSetspawn : Command
    {
        public override string name { get { return "setspawn"; } }
        public override string shortcut { get { return ""; } }
       public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdSetspawn() { }

        public override void Use(Player p, string message) {
            if (message != "") { 
        		Help(p); return; 
        	}
        	
            Player.SendMessage(p, "Spawn location set to your current position.");
            p.level.spawnx = (ushort)(p.pos[0] / 32);
            p.level.spawny = (ushort)(p.pos[1] / 32);
            p.level.spawnz = (ushort)(p.pos[2] / 32);
            p.level.rotx = p.rot[0];
            p.level.roty = 0;
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%a/setspawn");
            Player.SendMessage(p, "%eSets the default spawn location of the map you are currently located in.");  
        }
    }
}
