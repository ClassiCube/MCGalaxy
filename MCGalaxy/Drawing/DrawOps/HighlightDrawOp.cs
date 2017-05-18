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
using MCGalaxy.DB;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Maths;
using MCGalaxy.Undo;

namespace MCGalaxy.Drawing.Ops {
    
    public class HighlightDrawOp : DrawOp {
        public override string Name { get { return "Highlight"; } }
        public override bool AffectedByTransform { get { return false; } }
        
        /// <summary> Point in time that the /highlight should go backwards up to. </summary>
        public DateTime Start = DateTime.MinValue;
        
        /// <summary> Block to highlight placements with. </summary>
        public ExtBlock PlaceHighlight = (ExtBlock)Block.green;
        
        /// <summary> Block to highlight deletions with. </summary>
        public ExtBlock DeleteHighlight = (ExtBlock)Block.red;
        
        
        internal string who;
        internal int[] ids;
        internal bool found = false;
        
        public HighlightDrawOp() {
            Flags = 0;
            Undoable = false;
        }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) { return -1; }
        
        public override void Perform(Vec3S32[] marks, Brush brush, Action<DrawOpBlock> output) {
            this.output = output;
            PerformHighlight();
            this.output = null;
        }
        
        void PerformHighlight() {
            if (ids.Length > 0) {
                // can't use "using" as it creates a local var, and read lock reference may be changed by DrawOpPerformer class
                try {
                    BlockDBReadLock = Level.BlockDB.Locker.AccquireRead();
                    if (Level.BlockDB.FindChangesBy(ids, Start, DateTime.MaxValue, out dims, HighlightBlock)) return;
                } finally {
                    if (BlockDBReadLock != null) BlockDBReadLock.Dispose();
                }
            }
            
            UndoFormatArgs args = new UndoFormatArgs(Player, Start, DateTime.MaxValue, Min, Max, output);
            args.PlaceHighlight = PlaceHighlight;
            args.DeleteHighlight = DeleteHighlight;
            UndoFormat.DoHighlight(who.ToLower(), ref found, args);
        }
        
        Action<DrawOpBlock> output;
        Vec3U16 dims;
        
        void HighlightBlock(BlockDBEntry e) {
            ExtBlock oldBlock = ExtBlock.FromRaw(e.OldRaw, (e.Flags & BlockDBFlags.OldCustom) != 0);
            if (oldBlock.BlockID == Block.Invalid) return; // Exported BlockDB SQL table entries don't have previous block
            ExtBlock newBlock = ExtBlock.FromRaw(e.NewRaw, (e.Flags & BlockDBFlags.NewCustom) != 0);
            
            ExtBlock highlight = (newBlock.BlockID == Block.air
                              || Block.Convert(oldBlock.BlockID) == Block.water || oldBlock.BlockID == Block.waterstill
                              || Block.Convert(oldBlock.BlockID) == Block.lava || oldBlock.BlockID == Block.lavastill)
                ? DeleteHighlight : PlaceHighlight;
            
            int x = e.Index % dims.X;
            int y = (e.Index / dims.X) / dims.Z;
            int z = (e.Index / dims.X) % dims.Z;
            
            if (x < Min.X || y < Min.Y || z < Min.Z) return;
            if (x > Max.X || y > Max.Y || z > Max.Z) return;
            output(Place((ushort)x, (ushort)y, (ushort)z, highlight));
            found = true;
        }
    }
}
