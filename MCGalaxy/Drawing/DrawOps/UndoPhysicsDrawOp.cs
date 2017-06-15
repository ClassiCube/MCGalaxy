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
using MCGalaxy.Blocks.Physics;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Maths;

namespace MCGalaxy.Drawing.Ops {

    public class UndoPhysicsDrawOp : DrawOp {
        public override string Name { get { return "UndoPhysics"; } }
        public override bool AffectedByTransform { get { return false; } }
        
        internal DateTime Start;
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) { return -1; }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            if (Level.UndoBuffer.Count != Server.physUndo) {
                int count = Level.currentUndo;
                for (int i = count; i >= 0; i--) {
                    try {
                        if (!CheckBlockPhysics(Player, Level, i)) break;
                    } catch { }
                }
            } else {
                int count = Level.currentUndo;
                for (int i = count; i >= 0; i--) {
                    try {
                        if (!CheckBlockPhysics(Player, Level, i)) break;
                    } catch { }
                }
                for (int i = Level.UndoBuffer.Count - 1; i > count; i--) {
                    try {
                        if (!CheckBlockPhysics(Player, Level, i)) break;
                    } catch { }
                }
            }
        }
        
        bool CheckBlockPhysics(Player p, Level lvl, int i) {
            Level.UndoPos undo = lvl.UndoBuffer[i];
            byte b = lvl.GetTile(undo.index);
            DateTime time = Server.StartTime.AddTicks((undo.flags >> 2) * TimeSpan.TicksPerSecond);
            if (time < Start) return false;
            
            byte newType = (undo.flags & 2) != 0 ? Block.custom_block : undo.newRaw;
            if (b == newType || Block.Convert(b) == Block.water || Block.Convert(b) == Block.lava) {
                ushort x, y, z;
                lvl.IntToPos(undo.index, out x, out y, out z);
                int undoIndex = lvl.currentUndo;
                lvl.currentUndo = i;
                lvl.currentUndo = undoIndex;
                
                ExtBlock oldBlock = ExtBlock.FromRaw(undo.oldRaw, (undo.flags & 1) != 0);
                lvl.Blockchange(x, y, z, oldBlock, true, default(PhysicsArgs), false);
            }
            return true;
        }
    }
}
