/*
    Copyright 2011 MCForge
    
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
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdMaze : Command {
        public override string name { get { return "maze"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        
        public override void Use(Player p, string message) {
            int randomizer = 0;
            if (message.Length > 0 && !int.TryParse(message, out randomizer)) {
                Help(p); return;
            }
            
            Player.Message(p, "Place two blocks to determine the edges.");           
            p.MakeSelection(2, randomizer, DoMaze);
        }
        
        bool DoMaze(Player p, Vec3S32[] marks, object state, byte type, byte extType) {
            MazeDrawOp op = new MazeDrawOp();
            op.randomizer = (int)state;
            return DrawOp.DoDrawOp(op, null, p, marks);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/maze");
            Player.Message(p, "%HGenerates a random maze between two points.");
        }
    }
}
