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
namespace MCGalaxy.Commands.World {    
    public sealed class CmdSetspawn : Command {        
        public override string name { get { return "SetSpawn"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message) {
            if (message.Length > 0) { Help(p); return;  }
            
            Player.Message(p, "Spawn location set to your current position.");
            p.level.spawnx = (ushort)p.Pos.BlockX;
            p.level.spawny = (ushort)p.Pos.BlockY;
            p.level.spawnz = (ushort)p.Pos.BlockZ;
            p.level.rotx = p.Rot.RotY;
            p.level.roty = p.Rot.HeadX;
            p.level.Changed = true;
            Entities.Spawn(p, p);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/SetSpawn");
            Player.Message(p, "%HSets the default spawn location of the map you are currently located in.");  
        }
    }
}
