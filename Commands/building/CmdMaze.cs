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
using System.Collections;
using System.Security.Cryptography;
using MCGalaxy.Drawing;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Commands
{
    public sealed class CmdMaze : Command
    {
        public override string name { get { return "maze"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        
        public override void Use(Player p, string message) {
        	CatchPos cpos = default(CatchPos);
            if (message.Length > 0 && !int.TryParse(message, out cpos.randomizer)) {
                Help(p); return;
            }
        	
            Player.Message(p, "Place two blocks to determine the edges.");           
            p.ClearBlockchange();
            p.blockchangeObject = cpos;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            cpos.x = x; cpos.y = y; cpos.z = z;
            p.blockchangeObject = cpos;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        
        void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            MazeDrawOp drawOp = new MazeDrawOp();
            drawOp.randomizer = cpos.randomizer;
            
            if (!DrawOp.DoDrawOp(drawOp, null, p, cpos.x, cpos.y, cpos.z, x, y, z))
                return;
            if (p.staticCommands)
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        public override void Help(Player p) {
        	Player.Message(p, "%T/maze");
        	Player.Message(p, "%HGenerates a random maze between two points.");
        }
        
        struct CatchPos { public ushort x, y, z; public int randomizer; }
    }
}
