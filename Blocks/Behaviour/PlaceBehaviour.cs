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
using MCGalaxy.BlockPhysics;
using System;

namespace MCGalaxy.BlockBehaviour {
    
    internal static class PlaceBehaviour {

        internal static bool Grass(Player p, byte block, ushort x, ushort y, ushort z) {
            Level lvl = p.level;
            if (!(lvl.physics == 0 || lvl.physics == 5)) {
                p.ChangeBlock(x, y, z, block, 0); return false;
            }
            
            byte above = lvl.GetTile(x, (ushort)(y + 1), z), extAbove = 0;
            if (above == Block.custom_block) 
                extAbove = lvl.GetExtTile(x, (ushort)(y + 1), z);
            
            byte type = (above == Block.Zero || Block.LightPass(above, extAbove, lvl.CustomBlockDefs))
                ? Block.grass : Block.dirt;
            p.ChangeBlock(x, y, z, type, 0);
            return false;
        }
        
        internal static bool Stairs(Player p, byte block, ushort x, ushort y, ushort z) {
            if (!(p.level.physics == 0 || p.level.physics == 5)
                || p.level.GetTile(x, (ushort)(y - 1), z) != Block.staircasestep) {
                p.ChangeBlock(x, y, z, Block.staircasestep, 0); return false;
            }
            
            p.SendBlockchange(x, y, z, Block.air); //send the air block back only to the user.
            p.ChangeBlock(x, (ushort)(y - 1), z, Block.staircasefull, 0);
            return false;
        }
        
        internal static bool C4(Player p, byte block, ushort x, ushort y, ushort z) {
            if (p.level.physics == 0 || p.level.physics == 5) {
                p.RevertBlock(x, y, z); return false;
            }
            
            C4Data c4 = C4Physics.Find(p.level, p.c4circuitNumber);
            if (c4 != null) c4.list.Add(p.level.PosToInt(x, y, z));
            p.ChangeBlock(x, y, z, Block.c4, 0); return false;
        }
        
        internal static bool C4Det(Player p, byte block, ushort x, ushort y, ushort z) {
            if (p.level.physics == 0 || p.level.physics == 5) {
                p.c4circuitNumber = -1; p.RevertBlock(x, y, z); return false;
            }
            
            C4Data c4 = C4Physics.Find(p.level, p.c4circuitNumber);
            if (c4 != null) c4.detIndex = p.level.PosToInt(x, y, z);
            p.c4circuitNumber = -1; 
            p.ChangeBlock(x, y, z, Block.c4det, 0); return false;
        }
    }
}
