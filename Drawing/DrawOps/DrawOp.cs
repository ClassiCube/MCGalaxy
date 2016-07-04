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
using System.Collections.Generic;
using MCGalaxy.Drawing.Brushes;

namespace MCGalaxy {
    
    public struct FillPos { public ushort X, Y, Z; }
    
    public struct ExtBlock {
        public byte Type, ExtType;

        public ExtBlock(byte type, byte extType) {
            Type = type; ExtType = extType;
        }
    }
    
    public struct DrawOpBlock {
        public ushort X, Y, Z;
        public byte Type, ExtType;
    }
}

namespace MCGalaxy.Drawing.Ops {
    
    public abstract partial class DrawOp {
        
        //public int TotalAffected; // blocks affected by the draw operation
        public int TotalModified; // blocks actually modified (e.g. some may not be due to permissions)
        
        protected internal int method;
        
        /// <summary> Minimum coordinates of the bounds of this drawing command. </summary>
        public Vec3S32 Min;
        
        /// <summary> Maximum coordinates of the bounds of this drawing command. </summary>
        public Vec3S32 Max;
        
        /// <summary> Coordinates of the first point selected by the user. </summary>
        public Vec3S32 Origin;
        
        /// <summary> Coordinates of the current block being processed by the drawing command. </summary>
        /// <remarks> Note: You should treat this as coordinates, it is a DrawOpBlock struct for performance reasons. </remarks>
        public DrawOpBlock Coords;
        
        /// <summary> Level the draw operation is being performed upon. </summary>
        public Level Level;
        
        /// <summary> Human friendly name of the draw operation. </summary>
        public abstract string Name { get; }
        
        /// <summary> Estimates the total number of blocks that the drawing commands affects. <br/>
        /// Note that this estimate assumes that all possibly affected blocks will be changed by the drawing command. </summary>
        public abstract long GetBlocksAffected(Level lvl, Vec3S32[] marks);
        
        public abstract IEnumerable<DrawOpBlock> Perform(Vec3S32[] marks, Player p, Level lvl, Brush brush);
        
        public virtual bool CanDraw(Vec3S32[] marks, Player p, out long affected) {
            affected = GetBlocksAffected(Level, marks);
            if (p != null && affected > p.group.maxBlocks) {
                Player.Message(p, "You tried to draw " + affected + " blocks.");
                Player.Message(p, "You cannot draw more than " + p.group.maxBlocks + ".");
                return false;
            }
            return true;
        }
        
        public virtual void SetMarks(Vec3S32[] marks) {
            Origin = marks[0]; Min = marks[0]; Max = marks[0];
            for (int i = 1; i < marks.Length; i++) {
                Min = Vec3S32.Min(Min, marks[i]);
                Max = Vec3S32.Max(Max, marks[i]);
            }
        }
        
        public virtual void Reset() {
            //TotalAffected = 0;
            TotalModified = 0;
        }
        
        protected DrawOpBlock Place(ushort x, ushort y, ushort z, Brush brush) {
            Coords.X = x; Coords.Y = y; Coords.Z = z;
            Coords.Type = brush.NextBlock(this);
            
            if (Coords.Type != Block.Zero)
                Coords.ExtType = brush.NextExtBlock(this);
            return Coords;
        }
        
        protected DrawOpBlock Place(ushort x, ushort y, ushort z, byte type, byte extType) {
            Coords.X = x; Coords.Y = y; Coords.Z = z;
            Coords.Type = type; Coords.ExtType = extType;
            return Coords;
        }
        
        protected Vec3U16 Clamp(Vec3S32 pos) {
            pos.X = Math.Max(0, Math.Min(pos.X, Level.Width - 1));
            pos.Y = Math.Max(0, Math.Min(pos.Y, Level.Height - 1));
            pos.Z = Math.Max(0, Math.Min(pos.Z, Level.Length - 1));
            return new Vec3U16((ushort)pos.X, (ushort)pos.Y, (ushort)pos.Z);
        }
    }
}
