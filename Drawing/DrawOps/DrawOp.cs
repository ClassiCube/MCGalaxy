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
using MCGalaxy.Util;

namespace MCGalaxy {
    
    public struct FillPos { public ushort X, Y, Z; }
    
    public struct ExtBlock {
        public byte Type, ExtType;

        public ExtBlock(byte type, byte extType) {
            Type = type; ExtType = extType;
        }
    }
}

namespace MCGalaxy.Drawing.Ops {
    
    public abstract class DrawOp {
        
        //public int TotalAffected; // blocks affected by the draw operation
        public int TotalModified; // blocks actually modified (e.g. some may not be due to permissions)
        
        protected internal int method;
        
        /// <summary> Minimum coordinates of the bounds of this drawing command. </summary>
        public Vec3U16 Min;
        
        /// <summary> Maximum coordinates of the bounds of this drawing command. </summary>
        public Vec3U16 Max;
        
        /// <summary> Coordinates of the first point selected by the user. </summary>
        public Vec3U16 Origin;
        
        /// <summary> Coordinates of the current block being processed by the drawing command. </summary>
        public Vec3U16 Coords;
        
        /// <summary> Level the draw operation is being performed upon. </summary>
        public Level Level;
        
        /// <summary> Whether the two given coordinates from the user should be adjusted,
        /// so that the first coordinate contains the minimum values on all three axes. </summary>
        public virtual bool MinMaxCoords { get { return true; } }
        
        /// <summary> Human friendly name of the draw operation. </summary>
        public abstract string Name { get; }
        
        /// <summary> Estimates the total number of blocks that the drawing commands affects. <br/>
        /// Note that this estimate assumes that all possibly affected blocks will be changed by the drawing command. </summary>
        public abstract int GetBlocksAffected(Level lvl, Vec3U16[] marks);
        
        public abstract void Perform(Vec3U16[] marks, Player p, Level lvl, Brush brush);
        
        public bool CanDraw(Vec3U16[] marks, Player p, out int affected) {
            affected = GetBlocksAffected(p.level, marks);
            if (affected > p.group.maxBlocks) {
                Player.SendMessage(p, "You tried to draw " + affected + " blocks.");
                Player.SendMessage(p, "You cannot draw more than " + p.group.maxBlocks + ".");
                return false;
            }
            return true;
        }
        
        public virtual bool DetermineDrawOpMethod(Level lvl, int affected) {
            if (affected > Server.DrawReloadLimit) {
                method = M_PSetTile;
                return true;
            } else {
                method = lvl.bufferblocks ? M_PBlockQueue : M_PBlockChange;
                return false;
            }
        }
        
        public virtual void Reset() {
            //TotalAffected = 0;
            TotalModified = 0;
        }
        
        protected void PlaceBlock(Player p, Level lvl, ushort x, ushort y, ushort z, Brush brush) {
            Coords.X = x; Coords.Y = y; Coords.Z = z;
            byte type = brush.NextBlock(this);
            if (type == Block.Zero) return;
            PlaceBlock(p, lvl, x, y, z, type, brush.NextExtBlock(this));
        }
        
        protected void PlaceBlock(Player p, Level lvl, ushort x, ushort y, ushort z, byte type, byte extType) {
            switch (method) {
                case M_PBlockQueue:
                    BlockQueue.Addblock(p, x, y, z, type, extType);
                    TotalModified++;
                    break;
                case M_PBlockChange:
                    lvl.Blockchange(p, x, y, z, type, extType);
                    TotalModified++;
                    break;
                case M_PSetTile:
                    byte old = lvl.GetTile(x, y, z);
                    if (old == Block.Zero || !lvl.CheckAffectPermissions(p, x, y, z, old, type))
                        return;
                    lvl.SetTile(x, y, z, type, p, extType);
                    p.loginBlocks++;
                    p.overallBlocks++;
                    TotalModified++;
                    break;
                case M_BlockChange:
                    lvl.Blockchange(x, y, z, type, extType);
                    TotalModified++;
                    break;
                case M_SetTile:
                    lvl.SetTile(x, y, z, type);
                    if (type == Block.custom_block)
                        lvl.SetExtTile(x, y, z, extType);
                    TotalModified++;
                    break;
            }
        }
        
        internal const int M_PBlockQueue = 0, M_PBlockChange = 1, M_PSetTile = 2;
        internal const int M_BlockChange = 3, M_SetTile = 4;
        
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
            
            int affected = 0;
            if (!op.CanDraw(marks, p, out affected))
                return false;
            if (brush != null) {
                const string format = "{0}({1}): affecting up to {2} blocks";
                Player.SendMessage(p, String.Format(format, op.Name, brush.Name, affected));
            } else if (affected != -1) {
                const string format = "{0}: affecting up to {1} blocks";
                Player.SendMessage(p, String.Format(format, op.Name, affected));
            }
            
            UndoDrawOpEntry entry = new UndoDrawOpEntry();
            entry.DrawOpName = op.Name;
            entry.LevelName = p.level.name;
            entry.Start = DateTime.UtcNow;
            
            bool needReveal = op.DetermineDrawOpMethod(p.level, affected);
            op.Perform(marks, p, p.level, brush);
            entry.End = DateTime.UtcNow;
            
            if (entry.Start > p.UndoBuffer.LastClear) {
                UndoDrawOpEntry[] items = p.UndoDrawOps.Items;
                if (items.Length == 25)
                    p.UndoDrawOps.Remove(items[0]);
            } else { // UndoBuffer has been cleared during the draw op.
                entry.Start = p.UndoBuffer.LastClear;
                p.RemoveInvalidUndos();
            }
            p.UndoDrawOps.Add(entry);
            DoReload(p, needReveal);
            return true;
        }
        
        static void DoReload(Player p, bool needReveal) {
            if (!needReveal) return;
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level.name.CaselessEq(p.level.name))
                    CmdReveal.ReloadMap(p, pl, true);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
