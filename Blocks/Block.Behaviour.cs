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
using System;

namespace MCGalaxy {
    
    public sealed partial class Block {
        
        /// <summary> Returns whether this block handles the player placing a block at the given coordinates. </summary>
        /// <remarks>If this returns true, the usual 'checking dirt/grass below' behaviour is skipped. </remarks>
        public delegate bool HandleDelete(Player p, byte block, ushort x, ushort y, ushort z);
        internal static HandleDelete[] deleteHandlers = new HandleDelete[256];
        
        /// <summary> Returns whether this block handles the player deleting a block at the given coordinates. </summary>
        /// <remarks>If this returns true, the usual 'checking dirt/grass below' behaviour is skipped. </remarks>
        public delegate bool HandlePlace(Player p, byte block, ushort x, ushort y, ushort z);
        internal static HandlePlace[] placeHandlers = new Block.HandlePlace[256];
        
        static void SetupCoreHandlers() {
            deleteHandlers[Block.firework] = FireworkDelete;
        }        
        
        static bool FireworkDelete(Player p, byte block, ushort x, ushort y, ushort z) {
            if (p.level.physics == 5) { p.RevertBlock(x, y, z); return true; }
            if (p.level.physics == 0) { p.RevertBlock(x, y, z); return true; }
            
            Random rand = new Random();
            ushort x2 = (ushort)(x + rand.Next(0, 2) - 1);
            ushort z2 = (ushort)(z + rand.Next(0, 2) - 1);
            byte b1 = p.level.GetTile(x2, (ushort)(y + 2), z2);
            byte b2 = p.level.GetTile(x2, (ushort)(y + 1), z2);
            
            if (b1 == Block.air && b2 == Block.air && p.level.CheckClear(x2, (ushort)(y + 1), z2) 
                && p.level.CheckClear(x2, (ushort)(y + 2), z2)) {
                p.level.Blockchange(x2, (ushort)(y + 2), z2, Block.firework);
                p.level.Blockchange(x2, (ushort)(y + 1), z2, Block.lavastill, false, "wait 1 dissipate 100");
            }
            p.RevertBlock(x, y, z); return true;
        }        
    }
}
