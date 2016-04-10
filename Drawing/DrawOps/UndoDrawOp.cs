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
using MCGalaxy.Util;

namespace MCGalaxy.Drawing.Ops {

	public class UndoSelfDrawOp : UndoOnlineDrawOp {		
		public override string Name { get { return "UndoSelf"; } }
	}
	
    public class UndoOnlineDrawOp : DrawOp {
        
        public override string Name { get { return "UndoOnline"; } }
        
        /// <summary> Point in time that the /undo should go backwards up to. </summary>
        public DateTime Start = DateTime.MinValue;
        
        /// <summary> Point in time that the /undo should start updating blocks. </summary>
        public DateTime End = DateTime.MaxValue;
        
        internal Player who;
        internal Level saveLevel = null;
        
        public override long GetBlocksAffected(Level lvl, Vec3U16[] marks) { return -1; }
        
        public override void Perform(Vec3U16[] marks, Player p, Level lvl, Brush brush) {
            PerformUndo(p, ref saveLevel);
            bool foundUser = false;
            UndoFile.UndoPlayer(p, who.name.ToLower(), marks, Start, ref foundUser);
        }
        
        void PerformUndo(Player p, ref Level saveLvl) {
            UndoCache cache = who.UndoBuffer;
            UndoCacheNode node = cache.Tail;
            if (node == null) return;
            
            Vec3U16 min = Min, max = Max;
            bool undoArea = min.X != ushort.MaxValue;
            Player.UndoPos Pos = default(Player.UndoPos);
            int timeDelta = (int)DateTime.UtcNow.Subtract(Server.StartTime).TotalSeconds;
            
            while (node != null) {
                Level lvl = LevelInfo.FindExact(node.MapName);
                if (lvl == null || (p.level != null && !p.level.name.CaselessEq(lvl.name))) {
                    node = node.Prev; continue;
                }
                Pos.mapName = lvl.name;
                
                saveLvl = lvl;
                List<UndoCacheItem> items = node.Items;
                BufferedBlockSender buffer = new BufferedBlockSender(lvl);
                if (!undoArea) { 
                    min = new Vec3U16(0, 0, 0);
                    max = new Vec3U16((ushort)(lvl.Width - 1), (ushort)(lvl.Height - 1), (ushort)(lvl.Length - 1));
                }
                
                for (int i = items.Count - 1; i >= 0; i--) {
                    UndoCacheItem item = items[i];
                    node.Unpack(item.Index, out Pos.x, out Pos.y, out Pos.z);
                    if (Pos.x < min.X || Pos.y < min.Y || Pos.z < min.Z ||
                        Pos.x > max.X || Pos.y > max.Y || Pos.z > max.Z) continue;
                    
                    DateTime time = node.BaseTime.AddTicks(item.TimeDelta * TimeSpan.TicksPerSecond);
                    if (time > End) continue;
                    if (time < Start) { buffer.CheckIfSend(true); return; }
                    
                    item.GetNewExtBlock(out Pos.newtype, out Pos.newExtType);
                    item.GetExtBlock(out Pos.type, out Pos.extType);
                    UndoFile.UndoBlock(p, lvl, Pos, timeDelta, buffer);
                }
                buffer.CheckIfSend(true);
                node = node.Prev;
            }
        }
    }
    
    public class UndoOfflineDrawOp : DrawOp {
        
        public override string Name { get { return "UndoOffline"; } }
        
        /// <summary> Point in time that the /undo should go backwards up to. </summary>
        public DateTime Start = DateTime.MinValue;
        
        internal string whoName;
        internal bool foundUser = false;
        
        public override long GetBlocksAffected(Level lvl, Vec3U16[] marks) { return -1; }
        
        public override void Perform(Vec3U16[] marks, Player p, Level lvl, Brush brush) {            
            UndoFile.UndoPlayer(p, whoName.ToLower(), marks, Start, ref foundUser);
        }
    }
    
    public class UndoPhysicsDrawOp : DrawOp {
        
        public override string Name { get { return "UndoPhysics"; } }
        
        internal long seconds;
        
        public override long GetBlocksAffected(Level lvl, Vec3U16[] marks) { return -1; }
        
        public override void Perform(Vec3U16[] marks, Player p, Level lvl, Brush brush) {
            if (lvl.UndoBuffer.Count != Server.physUndo) {
                int count = lvl.currentUndo;
                for (int i = count; i >= 0; i--) {
                    try {
                        if (!CheckBlockPhysics(p, lvl, seconds, i)) break;
                    } catch { }
                }
            } else {
                int count = p.level.currentUndo;
                for (int i = count; i >= 0; i--) {
                    try {
                        if (!CheckBlockPhysics(p, lvl, seconds, i)) break;
                    } catch { }
                }
                for (int i = lvl.UndoBuffer.Count - 1; i > count; i--) {
                    try {
                        if (!CheckBlockPhysics(p, lvl, seconds, i)) break;
                    } catch { }
                }
            }
        }
        
        bool CheckBlockPhysics(Player p, Level lvl, long seconds, int i) {
            Level.UndoPos undo = lvl.UndoBuffer[i];
            byte b = lvl.GetTile(undo.index);
            DateTime time = Server.StartTime.AddTicks((undo.flags >> 2) * TimeSpan.TicksPerSecond);
            if (time.AddTicks(seconds * TimeSpan.TicksPerSecond) < DateTime.UtcNow) return false;
            
            byte newType = (undo.flags & 2) != 0 ? Block.custom_block : undo.newRawType;
            if (b == newType || Block.Convert(b) == Block.water || Block.Convert(b) == Block.lava) {
                ushort x, y, z;
                lvl.IntToPos(undo.index, out x, out y, out z);
                int undoIndex = p.level.currentUndo;
                lvl.currentUndo = i;
                lvl.currentUndo = undoIndex;
                byte oldType = (undo.flags & 1) != 0 ? Block.custom_block : undo.oldRawType;
                byte oldExtType = (undo.flags & 1) != 0 ? undo.oldRawType : (byte)0;
                lvl.Blockchange(x, y, z, oldType, true, default(PhysicsArgs), oldExtType, false);
            }
            return true;
        }
    }
}
