/*
    Copyright 2015 MCGalaxy
        
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

    public abstract class DrawOp {
        
        public int TotalAffected; // blocks affected by the draw operation
        public int TotalModified; // blocks actually modified (e.g. some may not be due to permissions)
        
        protected const int reloadLimit = 10000;
        protected int method;
        
        public abstract string Name { get; }
        
        public abstract int GetBlocksAffected(ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2);
        
        public abstract void Perform(ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2,
                                     Player p, Level lvl, Brush brush);
        
        public bool CanDraw(ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2,
                           Player p, out int affected) {
            affected = GetBlocksAffected(x1, y1, z1, x2, y2, x2);
            if (affected > p.group.maxBlocks) {
                Player.SendMessage(p, "You tried to " + Name + " " + affected + " blocks.");
                Player.SendMessage(p, "You cannot " + Name + " more than " + p.group.maxBlocks + ".");
                return false;
            }
            return true;
        }
        
        public bool DetermineDrawOpMethod(Level lvl, int affected) {
            if (affected > 10000) {
                method = MethodSetTile;
                return true;
            } else {
                method = lvl.bufferblocks && !lvl.Instant ?
                    MethodBlockQueue : MethodBlockChange;
                return false;    
            }
        }
        
        public virtual void Reset() {
            TotalAffected = 0;
            TotalModified = 0;
        }
        
        protected void PlaceBlock(Player p, Level lvl, ushort x, ushort y, ushort z, byte type) {
            switch (method) {
                case MethodBlockQueue:
                    BlockQueue.Addblock(p, x, y, z, type);
                    break;
                case MethodBlockChange:
                    p.level.Blockchange(p, x, y, z, type);
                    break;
                case MethodSetTile:
                    p.level.SetTile(x, y, z, type, p);
                    break;
            }
        }
        
        const int MethodBlockQueue = 0, MethodBlockChange = 1, MethodSetTile = 2;
    }
}
