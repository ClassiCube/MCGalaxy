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
using MCGalaxy.DB;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy {
    
    public struct DrawOpBlock {
        public ushort X, Y, Z;
        public BlockID Block;
    }
}

namespace MCGalaxy.Drawing.Ops {
    
    /// <summary> Performs on action on a block output from a draw operation. </summary>
    public delegate void DrawOpOutput(DrawOpBlock block);
    
    public abstract partial class DrawOp {
        
        //public long TotalAffected; // blocks affected by the draw operation
        public long TotalModified; // blocks actually modified (e.g. some may not be due to permissions)
        
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
        
        /// <summary> Player that is executing the draw operation. </summary>
        public Player Player;

        /// <summary> Level the draw operation is being performed upon. </summary>
        public Level Level;
        
        /// <summary> BlockDB change flags for blocks affected by this draw operation. </summary>
        public ushort Flags = BlockDBFlags.Drawn;
        
        /// <summary> Lock held on the associated level's BlockDB. Can be null. </summary>
        public IDisposable BlockDBReadLock;
        
        /// <summary> Whether this draw operation can be undone. </summary>
        public bool Undoable = true;
        
        /// <summary> Whether this draw operation can be used on maps that have drawing disabled. </summary>
        public bool AlwaysUsable;
        
        
        /// <summary> Human friendly name of the draw operation. </summary>
        public abstract string Name { get; }
        
        /// <summary> Whether the output of this draw operation is affected by the player's current Transform. </summary>
        public bool AffectedByTransform = true;
        
        /// <summary> Estimates the total number of blocks that the drawing commands affects. <br/>
        /// Note that this estimate assumes that all possibly affected blocks will be changed by the drawing command. </summary>
        public abstract long BlocksAffected(Level lvl, Vec3S32[] marks);
        
        public abstract void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output);
        
        
        public void Setup(Player p, Level lvl, Vec3S32[] marks) {
            Player = p;
            SetMarks(marks);
            SetLevel(lvl);
        }
        
        public virtual bool CanDraw(Vec3S32[] marks, Player p, long affected) {
            if (affected <= p.group.DrawLimit) return true;
            p.Message("You tried to draw " + affected + " blocks.");
            p.Message("You cannot draw more than " + p.group.DrawLimit + ".");
            return false;
        }
        
        public virtual void SetMarks(Vec3S32[] marks) {
            Origin = marks[0]; Min = marks[0]; Max = marks[0];
            for (int i = 1; i < marks.Length; i++) {
                Min = Vec3S32.Min(Min, marks[i]);
                Max = Vec3S32.Max(Max, marks[i]);
            }
        }
        
        /// <summary> Sets the level associated with this draw operation. </summary>
        public void SetLevel(Level lvl) {
            Level = lvl;
            clip = new Vec3S32(lvl.Width - 1, lvl.Height - 1, lvl.Length - 1);
        }
        
        
        protected DrawOpBlock Place(ushort x, ushort y, ushort z, Brush brush) {
            Coords.X = x; Coords.Y = y; Coords.Z = z;
            Coords.Block = brush.NextBlock(this);
            return Coords;
        }
        
        protected DrawOpBlock Place(ushort x, ushort y, ushort z, BlockID block) {
            Coords.X = x; Coords.Y = y; Coords.Z = z;
            Coords.Block = block;
            return Coords;
        }
        
        Vec3S32 clip = new Vec3S32(ushort.MaxValue);
        protected Vec3U16 Clamp(Vec3S32 pos) {
            pos.X = Math.Max(0, Math.Min(pos.X, clip.X));
            pos.Y = Math.Max(0, Math.Min(pos.Y, clip.Y));
            pos.Z = Math.Max(0, Math.Min(pos.Z, clip.Z));
            return new Vec3U16((ushort)pos.X, (ushort)pos.Y, (ushort)pos.Z);
        }
    }
}
