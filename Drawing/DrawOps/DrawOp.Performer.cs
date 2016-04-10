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
using MCGalaxy.Commands;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Util;

namespace MCGalaxy.Drawing {
    internal struct PendingDrawOp {
        public DrawOp Op;
        public Brush Brush;
        public Vec3U16[] Marks;
        public long Affected;
        public Level Level;
    }
}

namespace MCGalaxy.Drawing.Ops {
    
    public abstract partial class DrawOp {
        
        public static bool DoDrawOp(DrawOp op, Brush brush, Player p,
                                    ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2) {
            Vec3U16[] marks = new [] { new Vec3U16(x1, y1, z1), new Vec3U16(x2, y2, z2) };
            if (op.MinMaxCoords) {
                marks[0].X = Math.Min(x1, x2); marks[1].X = Math.Max(x1, x2);
                marks[0].Y = Math.Min(y1, y2); marks[1].Y = Math.Max(y1, y2);
                marks[0].Z = Math.Min(z1, z2); marks[1].Z = Math.Max(z1, z2);
            }
            return DoDrawOp(op, brush, p, marks);
        }
        
        public static bool DoDrawOp(DrawOp op, Brush brush, Player p, Vec3U16[] marks) {
            op.Origin = marks[0]; op.Min = marks[0]; op.Max = marks[0];
            for (int i = 1; i < marks.Length; i++) {
                op.Min = Vec3U16.Min(op.Min, marks[i]);
                op.Max = Vec3U16.Max(op.Max, marks[i]);
            }
            op.Level = p.level;
            if (!op.Level.DrawingAllowed) {
                Player.SendMessage(p, "Drawing commands are turned off on this map.");
                return false;
            }
            
            long affected = 0;
            if (!op.CanDraw(marks, p, out affected))
                return false;
            if (brush != null && affected != -1) {
                const string format = "{0}({1}): affecting up to {2} blocks";
                Player.SendMessage(p, String.Format(format, op.Name, brush.Name, affected));
            } else if (affected != -1) {
                const string format = "{0}: affecting up to {1} blocks";
                Player.SendMessage(p, String.Format(format, op.Name, affected));
            }
            
            AppendDrawOp(p, op, brush, marks, affected);
            return true;
        }
        
        static void AppendDrawOp(Player p, DrawOp op, Brush brush, Vec3U16[] marks, long affected) {
            PendingDrawOp pending = new PendingDrawOp();
            pending.Op = op;
            pending.Brush = brush;
            pending.Marks = marks;
            pending.Affected = affected;
            pending.Level = p.level;
            
            lock (p.pendingDrawOpsLock) {
                p.PendingDrawOps.Add(pending);
                // Another thread is already processing draw ops.
                if (p.PendingDrawOps.Count > 1) return;
            }
            ProcessDrawOps(p);
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
                entry.LevelName = item.Level.name;
                entry.Start = DateTime.UtcNow;
                // Use same time method as DoBlockchange writing to undo buffer
                int timeDelta = (int)DateTime.UtcNow.Subtract(Server.StartTime).TotalSeconds;
                entry.Start = Server.StartTime.AddTicks(timeDelta * TimeSpan.TicksPerSecond);
                
                bool needReveal = item.Op.DetermineDrawOpMethod(item.Level, item.Affected);
                item.Op.Perform(item.Marks, p, item.Level, item.Brush);
                timeDelta = (int)DateTime.UtcNow.Subtract(Server.StartTime).TotalSeconds;
                entry.End = Server.StartTime.AddTicks(timeDelta * TimeSpan.TicksPerSecond);
                
                p.DrawOps.Add(entry);
                if (p.DrawOps.Count > 200)
                    p.DrawOps.RemoveFirst();
                DoReload(p, item.Level, needReveal);
            }
        }
        
        static void DoReload(Player p, Level lvl, bool needReveal) {
            if (!needReveal) return;
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level.name.CaselessEq(lvl.name))
                    CmdReveal.ReloadMap(p, pl, true);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        
    }
}
