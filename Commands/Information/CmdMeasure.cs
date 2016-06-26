/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
    
    public sealed class CmdMeasure : Command {
        
        public override string name { get { return "measure"; } }
        public override string shortcut { get { return "ms"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }

        public override void Use(Player p, string message) {
            if (message.IndexOf(' ') != -1) { Help(p); return; }
            CatchPos cpos = default(CatchPos);
            if (message != "") {
                cpos.toIgnore = Block.Byte(message);
                if (cpos.toIgnore == Block.Zero) {
                    Player.Message(p, "Could not find block specified"); return;
                }
            }
            
            p.blockchangeObject = cpos;
            Player.Message(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += PlacedMark1;
        }
        
        void PlacedMark1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.Blockchange += PlacedMark2;
        }
        
        void PlacedMark2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            ushort minX = Math.Min(cpos.x, x), maxX = Math.Max(cpos.x, x);
            ushort minY = Math.Min(cpos.y, y), maxY = Math.Max(cpos.y, y);
            ushort minZ = Math.Min(cpos.z, z), maxZ = Math.Max(cpos.z, z);
            int foundBlocks = 0;

            for (ushort yy = minY; yy <= maxY; yy++)
                for (ushort zz = minZ; zz <= maxZ; zz++)
                    for (ushort xx = minX; xx <= maxX; xx++)
            {
                if (p.level.GetTile(xx, yy, zz) != cpos.toIgnore) foundBlocks++;
            }

            int width = maxX - minX + 1, height = maxY - minY + 1, length = maxZ - minZ + 1;
            Player.Message(p, "Measuring between (" + minX + ", " + minY + ", " + minZ +
                               ") and (" + maxX + ", " + maxY + ", " + maxZ + ")");
            Player.Message(p, "Area is " + width + " wide, " + height + " high, " + length + " long." +
                               " Volume is " + (width * height * length) + " blocks." );
            string name = " non-" + Block.Name(cpos.toIgnore);
            Player.Message(p, "There are " + foundBlocks + name + " blocks in the area.");
            if (p.staticCommands) p.Blockchange += PlacedMark1;
        }
        
        struct CatchPos { public ushort x, y, z; public byte toIgnore; }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/measure [ignore]");
            Player.Message(p, "%HMeasures all the blocks between two points");
            Player.Message(p, "%H [ignore] is optional, and specifies the block which is not counted.");
        }
    }
}
