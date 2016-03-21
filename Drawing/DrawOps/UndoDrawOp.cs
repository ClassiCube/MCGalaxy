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
using MCGalaxy.Util;

namespace MCGalaxy.Drawing.Ops {

    public class UndoOnlineDrawOp : DrawOp {
        
        public override string Name { get { return "UndoOnline"; } }
        
        internal long seconds;
        internal Player who;
        internal Level saveLevel = null;
        
        public override int GetBlocksAffected(Level lvl, Vector3U16[] marks) { return 0; }
        
        public override void Perform(Vector3U16[] marks, Player p, Level lvl, Brush brush) {
            PerformUndo(p, ref saveLevel);
            bool foundUser = false;
            UndoFile.UndoPlayer(p, who.name.ToLower(), seconds, ref foundUser);
        }
        
        void PerformUndo(Player p, ref Level saveLvl) {
            UndoCache cache = who.UndoBuffer;
            UndoCacheNode node = cache.Tail;
            if (node == null) return;
            
            while (node != null) {
                Level lvl = LevelInfo.FindExact(node.MapName);
                if (lvl == null) { node = node.Prev; continue; }
                saveLvl = lvl;
                List<UndoCacheItem> items = node.Items;
                BufferedBlockSender buffer = new BufferedBlockSender(lvl);
                
                for (int i = items.Count - 1; i >= 0; i--) {
                    UndoCacheItem item = items[i];
                    ushort x, y, z;
                    node.Unpack(item.Index, out x, out y, out z);
                    DateTime time = node.BaseTime.AddTicks((item.TimeDelta + seconds) * TimeSpan.TicksPerSecond);
                    if (time < DateTime.UtcNow) { buffer.CheckIfSend(true); return; }
                    
                    byte b = lvl.GetTile(x, y, z);
                    byte newTile = 0, newExtTile = 0;
                    item.GetNewExtBlock(out newTile, out newExtTile);
                    if (b == newTile || Block.Convert(b) == Block.water || Block.Convert(b) == Block.lava) {
                        Player.UndoPos uP = default(Player.UndoPos);
                        byte extType = 0;
                        if (b == Block.custom_block) extType = lvl.GetExtTile(x, y, z);
                        byte tile = 0, extTile = 0;
                        item.GetExtBlock(out tile, out extTile);
                        
                        if (lvl.DoBlockchange(p, x, y, z, tile, extTile)) {
                            buffer.Add(lvl.PosToInt(x, y, z), tile, extTile);
                            buffer.CheckIfSend(false);
                        }
                        
                        uP.newtype = tile; uP.newExtType = extTile;
                        uP.type = b; uP.extType = extType;
                        uP.x = x; uP.y = y; uP.z = z;
                        uP.mapName = node.MapName;
                        time = node.BaseTime.AddTicks(item.TimeDelta * TimeSpan.TicksPerSecond);
                        uP.timeDelta = (int)time.Subtract(Server.StartTime).TotalSeconds;
                        if (p != null) p.RedoBuffer.Add(lvl, uP);
                    }
                }
                buffer.CheckIfSend(true);
                node = node.Prev;
            }
        }
    }
    
    public class UndoOfflineDrawOp : DrawOp {
        
        public override string Name { get { return "UndoOffline"; } }
        
        internal long seconds;
        internal string whoName;
        internal bool foundUser = false;
        
        public override int GetBlocksAffected(Level lvl, Vector3U16[] marks) { return 0; }
        
        public override void Perform(Vector3U16[] marks, Player p, Level lvl, Brush brush) {            
            UndoFile.UndoPlayer(p, whoName.ToLower(), seconds, ref foundUser);
        }
    }
    
    public class UndoPhysicsDrawOp : DrawOp {
        
        public override string Name { get { return "UndoPhysics"; } }
        
        internal long seconds;
        
        public override int GetBlocksAffected(Level lvl, Vector3U16[] marks) { return 0; }
        
        public override void Perform(Vector3U16[] marks, Player p, Level lvl, Brush brush) {
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
                lvl.Blockchange(x, y, z, oldType, true, "", oldExtType, false);
            }
            return true;
        }
    }
}
