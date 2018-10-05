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
using BlockID = System.UInt16;

namespace MCGalaxy.Drawing.Ops {

    public class UndoPhysicsDrawOp : DrawOp {
        public override string Name { get { return "UndoPhysics"; } }        
        internal DateTime Start;

        public UndoPhysicsDrawOp() {
            AffectedByTransform = false;
        }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) { return -1; }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            if (Level.UndoBuffer.Count != Server.Config.PhysicsUndo) {
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
            if (undo.Time < Start) return false;
            
            ushort x, y, z;
            lvl.IntToPos(undo.Index, out x, out y, out z);
            BlockID cur = lvl.GetBlock(x, y, z);
            
            BlockID newBlock = undo.NewBlock;
            if (cur == newBlock || Block.Convert(cur) == Block.Water || Block.Convert(cur) == Block.Lava) {
                lvl.Blockchange(x, y, z, undo.OldBlock, true, default(PhysicsArgs), false);
            }
            return true;
        }
    }
}
