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
using MCGalaxy.BlockPhysics;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Undo;

namespace MCGalaxy.Drawing.Ops {
    
    public class RedoSelfDrawOp : DrawOp {
        public override string Name { get { return "RedoSelf"; } }
        public override bool AffectedByTransform { get { return false; } }
        
        /// <summary> Point in time that the /undo should go backwards up to. </summary>
        public DateTime Start = DateTime.MinValue;
        
        /// <summary> Point in time that the /undo should start updating blocks. </summary>
        public DateTime End = DateTime.MaxValue;
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) { return -1; }
        
        public override IEnumerable<DrawOpBlock> Perform(Vec3S32[] marks, Player p, Level lvl, Brush brush) {
            UndoCache cache = p.UndoBuffer;
            using (IDisposable locker = cache.ClearLock.AccquireReadLock()) {
                if (RedoBlocks(p)) yield break;
            }
            
            bool found = false;
            UndoFormat.DoRedo(p, p.name.ToLower(), Start, End, ref found);
            yield break;
        }
        
        bool RedoBlocks(Player p) {
            UndoFormatArgs args = new UndoFormatArgs(p, Start);
            UndoFormat format = new UndoFormatOnline(p.UndoBuffer);
            UndoFormat.DoRedo(null, End, format, args);
            return args.Stop;
        }
    }
}
