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
            deleteHandlers[Block.rocketstart] = RocketStartDelete;
            deleteHandlers[Block.firework] = FireworkDelete;
        }

        static bool RocketStartDelete(Player p, byte block, ushort x, ushort y, ushort z) {
            if (p.level.physics < 2 || p.level.physics == 5) { p.RevertBlock(x, y, z); return true; }
            
            int newZ = 0, newX = 0, newY = 0;
            p.SendBlockchange(x, y, z, Block.rocketstart);
            if ( p.rot[0] < 48 || p.rot[0] > ( 256 - 48 ) )
                newZ = -1;
            else if ( p.rot[0] > ( 128 - 48 ) && p.rot[0] < ( 128 + 48 ) )
                newZ = 1;

            if ( p.rot[0] > ( 64 - 48 ) && p.rot[0] < ( 64 + 48 ) )
                newX = 1;
            else if ( p.rot[0] > ( 192 - 48 ) && p.rot[0] < ( 192 + 48 ) )
                newX = -1;

            if ( p.rot[1] >= 192 && p.rot[1] <= ( 192 + 32 ) )
                newY = 1;
            else if ( p.rot[1] <= 64 && p.rot[1] >= 32 )
                newY = -1;

            if ( 192 <= p.rot[1] && p.rot[1] <= 196 || 60 <= p.rot[1] && p.rot[1] <= 64 ) { newX = 0; newZ = 0; }

            byte b1 = p.level.GetTile((ushort)( x + newX * 2 ), (ushort)( y + newY * 2 ), (ushort)( z + newZ * 2 ));
            byte b2 = p.level.GetTile((ushort)( x + newX ), (ushort)( y + newY ), (ushort)( z + newZ ));
            if ( b1 == Block.air && b2 == Block.air && p.level.CheckClear((ushort)( x + newX * 2 ), (ushort)( y + newY * 2 ), (ushort)( z + newZ * 2 )) 
                && p.level.CheckClear((ushort)( x + newX ), (ushort)( y + newY ), (ushort)( z + newZ )) ) {
                p.level.Blockchange((ushort)( x + newX * 2 ), (ushort)( y + newY * 2 ), (ushort)( z + newZ * 2 ), Block.rockethead);
                p.level.Blockchange((ushort)( x + newX ), (ushort)( y + newY ), (ushort)( z + newZ ), Block.fire);
            }
            return false;
        }
        
        static bool FireworkDelete(Player p, byte block, ushort x, ushort y, ushort z) {
            if (p.level.physics == 0 || p.level.physics == 5) { p.RevertBlock(x, y, z); return true; }
            
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
            p.RevertBlock(x, y, z); return false;
        }
    }
}
