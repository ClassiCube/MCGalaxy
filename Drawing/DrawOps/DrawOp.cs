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
using MCGalaxy.Drawing.Brushes;

namespace MCGalaxy {
    
    public struct FillPos { public ushort X, Y, Z; }
    
    public struct ExtBlock {
        public byte Type, ExtType;

        public ExtBlock(byte type, byte extType) {
            Type = type; ExtType = extType;
        }
    }
}

namespace MCGalaxy.Drawing.Ops {
    
    public abstract partial class DrawOp {
        
        //public int TotalAffected; // blocks affected by the draw operation
        public int TotalModified; // blocks actually modified (e.g. some may not be due to permissions)
        
        protected internal int method;
        
        /// <summary> Minimum coordinates of the bounds of this drawing command. </summary>
        public Vec3U16 Min;
        
        /// <summary> Maximum coordinates of the bounds of this drawing command. </summary>
        public Vec3U16 Max;
        
        /// <summary> Coordinates of the first point selected by the user. </summary>
        public Vec3U16 Origin;
        
        /// <summary> Coordinates of the current block being processed by the drawing command. </summary>
        public Vec3U16 Coords;
        
        /// <summary> Level the draw operation is being performed upon. </summary>
        public Level Level;
        
        /// <summary> Whether the two given coordinates from the user should be adjusted,
        /// so that the first coordinate contains the minimum values on all three axes. </summary>
        public virtual bool MinMaxCoords { get { return true; } }
        
        /// <summary> Human friendly name of the draw operation. </summary>
        public abstract string Name { get; }
        
        /// <summary> Estimates the total number of blocks that the drawing commands affects. <br/>
        /// Note that this estimate assumes that all possibly affected blocks will be changed by the drawing command. </summary>
        public abstract long GetBlocksAffected(Level lvl, Vec3U16[] marks);
        
        public abstract void Perform(Vec3U16[] marks, Player p, Level lvl, Brush brush);
        
        public virtual bool CanDraw(Vec3U16[] marks, Player p, out long affected) {
            affected = GetBlocksAffected(Level, marks);
            if (p != null && affected > p.group.maxBlocks) {
                Player.SendMessage(p, "You tried to draw " + affected + " blocks.");
                Player.SendMessage(p, "You cannot draw more than " + p.group.maxBlocks + ".");
                return false;
            }
            return true;
        }
        
        public virtual bool DetermineDrawOpMethod(Level lvl, long affected) {
            if (affected > Server.DrawReloadLimit) {
                method = M_PSetTile;
                return true;
            } else {
                method = lvl.bufferblocks ? M_PBlockQueue : M_PBlockChange;
                return false;
            }
        }
        
        public virtual void Reset() {
            //TotalAffected = 0;
            TotalModified = 0;
        }
        
        protected void PlaceBlock(Player p, Level lvl, ushort x, ushort y, ushort z, Brush brush) {
            Coords.X = x; Coords.Y = y; Coords.Z = z;
            byte type = brush.NextBlock(this);
            if (type == Block.Zero) return;
            PlaceBlock(p, lvl, x, y, z, type, brush.NextExtBlock(this));
        }
        
        protected void PlaceBlock(Player p, Level lvl, ushort x, ushort y, ushort z, byte type, byte extType) {
            Level.BlockPos bP = default(Level.BlockPos);            
            switch (method) {
                case M_PBlockQueue:
                    if (!lvl.DoBlockchange(p, x, y, z, type, extType)) return;
                    bP.name = p.name;
                    bP.index = lvl.PosToInt(x, y, z);
                    bP.SetData(type, extType, type == 0);
                    
                    lvl.blockCache.Add(bP);                   
                    BlockQueue.Addblock(p, bP.index, type, extType);
                    TotalModified++;
                    break; 
                case M_PBlockChange:
                    if (!lvl.DoBlockchange(p, x, y, z, type, extType)) return;
                    bP.name = p.name;
                    bP.index = lvl.PosToInt(x, y, z);
                    
                    bP.SetData(type, extType, type == 0);
                    lvl.blockCache.Add(bP);                
                    Player.GlobalBlockchange(lvl, x, y, z, type, extType);
                    TotalModified++;
                    break;                    
                case M_PSetTile:
                    byte old = lvl.GetTile(x, y, z);
                    if (old == Block.Zero || !lvl.CheckAffectPermissions(p, x, y, z, old, type))
                        return;
                    lvl.SetTile(x, y, z, type, p, extType);
                    p.loginBlocks++;
                    p.overallBlocks++;
                    TotalModified++;
                    break;                    
                case M_BlockChange:
                    lvl.Blockchange(x, y, z, type, extType);
                    TotalModified++;
                    break;
                case M_SetTile:
                    lvl.SetTile(x, y, z, type);
                    if (type == Block.custom_block)
                        lvl.SetExtTile(x, y, z, extType);
                    TotalModified++;
                    break;
            }
        }
        
        internal const int M_PBlockQueue = 0, M_PBlockChange = 1, M_PSetTile = 2;
        internal const int M_BlockChange = 3, M_SetTile = 4;
    }
}
