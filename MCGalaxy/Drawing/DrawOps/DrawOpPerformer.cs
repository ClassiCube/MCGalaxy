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
using MCGalaxy.Blocks.Physics;
using MCGalaxy.Commands;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Network;
using MCGalaxy.Undo;
using MCGalaxy.Maths;

namespace MCGalaxy.Drawing {
    internal struct PendingDrawOp {
        public DrawOp Op;
        public Brush Brush;
        public Vec3S32[] Marks;
    }
}

namespace MCGalaxy.Drawing.Ops {
    
    public static class DrawOpPerformer {
        
        public static bool Do(DrawOp op, Brush brush, Player p,
                              ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2) {
            Vec3S32[] marks = new Vec3S32[] { new Vec3S32(x1, y1, z1), new Vec3S32(x2, y2, z2) };
            return Do(op, brush, p, marks);
        }
        
        public static bool Do(DrawOp op, Brush brush, Player p,
                              Vec3S32[] marks, bool checkLimit = true) {
            op.SetMarks(marks);
            Level lvl = p == null ? null : p.level;
            op.SetLevel(lvl);
            op.Player = p;
            
            if (lvl != null && !lvl.DrawingAllowed) {
                Player.Message(p, "Drawing commands are turned off on this map.");
                return false;
            }
            if (lvl != null && lvl.BuildAccess.Check(p) == AccessResult.Blacklisted) {
                Player.Message(p, "You are blacklisted from building in this map, " +
                               "hence you cannot draw in this map");
                return false;
            }
            
            long affected = op.BlocksAffected(lvl, marks);
            if (p != null && op.AffectedByTransform)
                p.Transform.GetBlocksAffected(ref affected);
            
            if (checkLimit && !op.CanDraw(marks, p, affected))
                return false;
            if (brush != null && affected != -1) {
                const string format = "{0}({1}): affecting up to {2} blocks";
                Player.Message(p, format, op.Name, brush.Name, affected);
            } else if (affected != -1) {
                const string format = "{0}: affecting up to {1} blocks";
                Player.Message(p, format, op.Name, affected);
            }
            
            AppendDrawOp(p, op, brush, marks, affected);
            return true;
        }
        
        static void AppendDrawOp(Player p, DrawOp op, Brush brush, Vec3S32[] marks, long affected) {
            if (p == null) {
                BufferedBlockSender buffer = new BufferedBlockSender(op.Level);
                op.Perform(marks, brush, b => ConsoleOutputBlock(b, op.Level, buffer));
                buffer.Send(true);
                return;
            }
            
            PendingDrawOp item = new PendingDrawOp();
            item.Op = op;
            item.Brush = brush;
            item.Marks = marks;
            
            lock (p.pendingDrawOpsLock) {
                p.PendingDrawOps.Add(item);
                // Another thread is already processing draw ops.
                if (p.PendingDrawOps.Count > 1) return;
            }
            ProcessDrawOps(p);
        }
        
        static void ConsoleOutputBlock(DrawOpBlock b, Level lvl, BufferedBlockSender buffer) {
            int index = lvl.PosToInt(b.X, b.Y, b.Z);
            if (!lvl.DoPhysicsBlockchange(index, b.Block, false,
                                          default(PhysicsArgs), b.ExtBlock)) return;
            buffer.Add(index, b.Block, b.ExtBlock);
        }
        
        static void ProcessDrawOps(Player p) {
            while (true) {
                PendingDrawOp item;
                lock (p.pendingDrawOpsLock) {
                    if (p.PendingDrawOps.Count == 0) return;
                    item = p.PendingDrawOps[0];
                    p.PendingDrawOps.RemoveAt(0);
                    
                    // Flush any remaining draw ops if the player has left the server.
                    // (so as to not keep alive references)
                    if (p.disconnected) {
                        p.PendingDrawOps.Clear();
                        return;
                    }
                }
                
                UndoDrawOpEntry entry = new UndoDrawOpEntry();
                entry.DrawOpName = item.Op.Name;
                entry.LevelName = item.Op.Level.name;
                entry.Start = DateTime.UtcNow;
                // Use same time method as DoBlockchange writing to undo buffer
                int timeDelta = (int)DateTime.UtcNow.Subtract(Server.StartTime).TotalSeconds;
                entry.Start = Server.StartTime.AddTicks(timeDelta * TimeSpan.TicksPerSecond);
                
                if (item.Brush != null) item.Brush.Configure(item.Op, p);
                DoDrawOp(item, p);
                timeDelta = (int)DateTime.UtcNow.Subtract(Server.StartTime).TotalSeconds + 1;
                entry.End = Server.StartTime.AddTicks(timeDelta * TimeSpan.TicksPerSecond);
                
                p.DrawOps.Add(entry);
                if (p.DrawOps.Count > 200)
                    p.DrawOps.RemoveFirst();  
                
                if (item.Op.TotalModified > Server.DrawReloadLimit)
                    DoReload(p, item.Op.Level);
                item.Op.TotalModified = 0; // reset total modified (as drawop instances are used in static mode)
            }
        }
        
        static void DoDrawOp(PendingDrawOp item, Player p) {
            Level lvl = item.Op.Level;
            DrawOpOutputter outputter = new DrawOpOutputter(item.Op);
            
            if (item.Op.AffectedByTransform) {
                p.Transform.Perform(item.Marks, p, lvl, item.Op, item.Brush, outputter.Output);
            } else {
                item.Op.Perform(item.Marks, item.Brush, outputter.Output);
            }
        }

        static void DoReload(Player p, Level lvl) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level.name.CaselessEq(lvl.name))
                    LevelActions.ReloadMap(p, pl, true);
            }
            Server.DoGC();
        }

        
        class DrawOpOutputter {
            readonly DrawOp op;
            
            public DrawOpOutputter(DrawOp op) { this.op = op; }
            
            public void Output(DrawOpBlock b) {
                if (b.Block == Block.Invalid) return;
                Level lvl = op.Level;
                Player p = op.Player;
                if (b.X >= lvl.Width || b.Y >= lvl.Height || b.Z >= lvl.Length) return;
                
                int index = b.X + lvl.Width * (b.Z + b.Y * lvl.Length);
                byte old = lvl.blocks[index], oldExt = 0;
                if (old == Block.custom_block) oldExt = lvl.GetExtTileNoCheck(b.X, b.Y, b.Z);
                if (old == Block.Invalid) return;
                
                // Check to make sure the block is actually different and that we can change it
                bool same = old == b.Block;
                if (same && b.Block == Block.custom_block) {
                    same = oldExt == b.ExtBlock;
                }
                if (same || !lvl.CheckAffectPermissions(p, b.X, b.Y, b.Z, old, b.Block, b.ExtBlock)) {
                    return;
                }
                
                
                // Set the block (inlined)
                lvl.blocks[index] = b.Block;
                lvl.changed = true;
                if (old == Block.custom_block && b.Block != Block.custom_block) {
                    lvl.RevertExtTileNoCheck(b.X, b.Y, b.Z);
                }
                if (b.Block == Block.custom_block) {
                    lvl.SetExtTileNoCheck(b.X, b.Y, b.Z, b.ExtBlock);
                }
                if (p != null) {
                    lvl.BlockDB.Cache.Add(p, b.X, b.Y, b.Z, op.Flags, old, oldExt, b.Block, b.ExtBlock);
                }
                p.loginBlocks++; p.overallBlocks++; p.TotalDrawn++; // increment block stats inline
                
                
                // Potentially buffer the block change
                if (op.TotalModified == Server.DrawReloadLimit) {
                    Player.Message(p, "Changed over {0} blocks, preparing to reload map..", Server.DrawReloadLimit);
                    lock (lvl.queueLock)
                        lvl.blockqueue.Clear();
                } else if (op.TotalModified < Server.DrawReloadLimit) {
                    same = old == Block.custom_block
                        ? oldExt == b.ExtBlock : Block.Convert(old) == Block.Convert(b.Block);
                    if (!same) BlockQueue.Addblock(p, index, b.Block, b.ExtBlock);
                    
                    if (lvl.physics > 0) {
                        if (old == Block.sponge && b.Block != Block.sponge)
                            OtherPhysics.DoSpongeRemoved(lvl, index, false);
                        if (old == Block.lava_sponge && b.Block != Block.lava_sponge)
                            OtherPhysics.DoSpongeRemoved(lvl, index, true);

                        if (lvl.ActivatesPhysics(b.Block, b.ExtBlock)) lvl.AddCheck(index);
                    }
                }
                op.TotalModified++;
                
                
                // Attempt to prevent the BlockDB from growing too large (> 1,000,000 entries)
                int count = lvl.BlockDB.Cache.Count;
                if (count == 0 || (count % 1000000) != 0) return;
                
                // if drawop has a read lock on BlockDB (e.g. undo/redo), we must release it here
                bool hasReadLock = false;
                if (op.BlockDBReadLock != null) {
                    op.BlockDBReadLock.Dispose();
                    hasReadLock = true;
                }
                
                using (IDisposable wLock = lvl.BlockDB.Locker.AccquireWrite(100)) {
                    if (wLock != null) lvl.BlockDB.WriteEntries();
                }
                
                if (!hasReadLock) return;
                op.BlockDBReadLock = lvl.BlockDB.Locker.AccquireRead();
            }
        }
    }
}
