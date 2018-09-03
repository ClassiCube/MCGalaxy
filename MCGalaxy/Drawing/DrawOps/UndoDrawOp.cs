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
using System.IO;
using MCGalaxy.DB;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Maths;
using MCGalaxy.Undo;
using BlockID = System.UInt16;

namespace MCGalaxy.Drawing.Ops {

    public class UndoSelfDrawOp : UndoDrawOp {
        
        public UndoSelfDrawOp() {
            Flags = BlockDBFlags.UndoSelf;
        }
        
        public override string Name { get { return "UndoSelf"; } }
    }
    
    public class UndoDrawOp : DrawOp {
        public override string Name { get { return "Undo"; } }
        
        /// <summary> Point in time that the /undo should go backwards up to. </summary>
        public DateTime Start = DateTime.MinValue;
        
        /// <summary> Point in time that the /undo should start updating blocks. </summary>
        public DateTime End = DateTime.MaxValue;
        
        internal string who;
        internal int[] ids;
        internal bool found = false;
        
        public UndoDrawOp() {
            Flags = BlockDBFlags.UndoOther;
            AffectedByTransform = false;
        }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) { return -1; }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            this.output = output;
            PerformUndo();
            this.output = null;
        }
        
        void PerformUndo() {
            if (ids.Length > 0) {
                // can't use "using" as it creates a local var, and read lock reference may be changed by DrawOpPerformer class
                try {
                    BlockDBReadLock = Level.BlockDB.Locker.AccquireRead();
                    if (Level.BlockDB.FindChangesBy(ids, Start, End, out dims, UndoBlock)) return;
                } finally {
                    if (BlockDBReadLock != null) BlockDBReadLock.Dispose();
                }
            }
            
            UndoFormatArgs args = new UndoFormatArgs(Level.name, Start, End, OldUndoBlock);
            PerformOldUndo(args);
        }
        
        DrawOpOutput output;
        Vec3U16 dims;
        
        void UndoBlock(BlockDBEntry e) {
            BlockID block = e.OldBlock;
            if (block == Block.Invalid) return; // Exported BlockDB SQL table entries don't have previous block
            
            int x = e.Index % dims.X;
            int y = (e.Index / dims.X) / dims.Z;
            int z = (e.Index / dims.X) % dims.Z;
            
            if (x < Min.X || y < Min.Y || z < Min.Z) return;
            if (x > Max.X || y > Max.Y || z > Max.Z) return;
            output(Place((ushort)x, (ushort)y, (ushort)z, block));
            found = true;
        }
        
        
        void PerformOldUndo(UndoFormatArgs args) {
            List<string> files = UndoFormat.GetUndoFiles(who.ToLower());
            if (files.Count == 0) return;
            found = true;
            
            foreach (string file in files) {
                using (Stream s = File.OpenRead(file)) {
                    UndoFormat.GetFormat(file).EnumerateEntries(s, args);
                    if (args.Finished) break;
                }
            }
        }
        
        void OldUndoBlock(UndoFormatEntry P) {            
            if (P.X < Min.X || P.Y < Min.Y || P.Z < Min.Z) return;
            if (P.X > Max.X || P.Y > Max.Y || P.Z > Max.Z) return;
            
            BlockID cur = Level.GetBlock(P.X, P.Y, P.Z);
            if (cur == P.NewBlock || Block.Convert(cur) == Block.Water || Block.Convert(cur) == Block.Lava || cur == Block.Grass) {                
                DrawOpBlock block;
                block.X = P.X; block.Y = P.Y; block.Z = P.Z;
                block.Block = P.Block;
                output(block);
            }
        }
    }
}
