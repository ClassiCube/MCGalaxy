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
using MCGalaxy.Undo;
using MCGalaxy.Maths;

namespace MCGalaxy.Drawing.Ops {
    
    public class RedoSelfDrawOp : DrawOp {
        public override string Name { get { return "RedoSelf"; } }
        
        /// <summary> Point in time that the /undo should go backwards up to. </summary>
        public DateTime Start = DateTime.MinValue;
        
        /// <summary> Point in time that the /undo should start updating blocks. </summary>
        public DateTime End = DateTime.MaxValue;
        
        public RedoSelfDrawOp() {
            Flags = BlockDBFlags.RedoSelf;
            AffectedByTransform = false;
        }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) { return -1; }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            int[] ids = NameConverter.FindIds(Player.name);
            if (ids.Length == 0) return;
            
            this.output = output;
            // can't use "using" as it creates a local var, and read lock reference may be changed by DrawOpPerformer class
            try {
                BlockDBReadLock = Level.BlockDB.Locker.AccquireRead();
                if (Level.BlockDB.FindChangesBy(ids, Start, End, out dims, RedoBlock)) return;
            } finally {
                if (BlockDBReadLock != null) BlockDBReadLock.Dispose();
            }
            this.output = null;
        }
        
        DrawOpOutput output;
        Vec3U16 dims;
        
        void RedoBlock(BlockDBEntry e) {
            ushort block = Block.FromRaw(e.OldRaw, (e.Flags & BlockDBFlags.OldCustom) != 0);
            if (block == Block.Invalid) return; // Exported BlockDB SQL table entries don't have previous block
            if ((e.Flags & BlockDBFlags.UndoSelf) == 0) return; // not an undo
            
            int x = e.Index % dims.X;
            int y = (e.Index / dims.X) / dims.Z;
            int z = (e.Index / dims.X) % dims.Z;
            output(Place((ushort)x, (ushort)y, (ushort)z, block));
        }
    }
}
