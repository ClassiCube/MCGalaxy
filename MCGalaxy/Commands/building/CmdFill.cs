/*
    Copyright 2011 MCForge
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdFill : DrawCmd {
        public override string name { get { return "fill"; } }
        public override string shortcut { get { return "f"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        protected override string PlaceMessage { get { return "Destroy the block you wish to fill."; } }
        public override int MarksCount { get { return 1; } }
        
        protected override DrawMode GetMode(string[] parts) {
            if (parts[parts.Length - 1].CaselessEq("confirm")) {
                string prev = parts.Length >= 2 ? parts[parts.Length - 2] : "";
                return ParseFillMode(prev);
            }
            return ParseFillMode(parts[parts.Length - 1]);
        }
        
        DrawMode ParseFillMode(string msg) {
            if (msg == "normal") return DrawMode.solid;
            else if (msg == "up") return DrawMode.up;
            else if (msg == "down") return DrawMode.down;
            else if (msg == "layer") return DrawMode.layer;
            else if (msg == "vertical_x") return DrawMode.verticalX;
            else if (msg == "vertical_z") return DrawMode.verticalZ;
            return DrawMode.normal;
        }
        
        protected override DrawOp GetDrawOp(DrawArgs dArg) { return new FillDrawOp(); }

        protected override string GetBrush(Player p, DrawArgs dArgs, ref int offset) {
            offset = dArgs.Mode == DrawMode.normal ? 0 : 1;
            if (IsConfirmed(dArgs.Message)) offset++;
            return p.BrushName;
        }
        
        protected override bool DoDraw(Player p, Vec3S32[] marks,
                                       object state, byte block, byte extBlock) {
            DrawArgs dArgs = (DrawArgs)state;
            ushort x = (ushort)marks[0].X, y = (ushort)marks[0].Y, z = (ushort)marks[0].Z;
            byte oldBlock = p.level.GetTile(x, y, z), oldExtBlock = 0;
            if (oldBlock == Block.custom_block)
                oldExtBlock = p.level.GetExtTile(x, y, z);

            if (!Block.canPlace(p, oldBlock) && !Block.BuildIn(oldBlock)) {
                Formatter.MessageBlock(p, "fill over ", oldBlock); return false;
            }
            
            FillDrawOp op = (FillDrawOp)dArgs.Op;
            op.Positions = FloodFill(p, p.level.PosToInt(x, y, z), oldBlock, oldExtBlock, dArgs.Mode);
            int count = op.Positions.Count;
            
            bool confirmed = IsConfirmed(dArgs.Message), success = true;
            if (count < p.group.maxBlocks && count > Server.DrawReloadLimit && !confirmed) {
                Player.Message(p, "This fill would affect {0} blocks.", count);
                Player.Message(p, "If you still want to fill, type %T/fill {0} confirm", dArgs.Message);
            } else {
                success = base.DoDraw(p, marks, state, block, extBlock);
            }            
  
            op.Positions = null;
            return success;
        }
        
        
        unsafe List<int> FloodFill(Player p, int index, byte block, byte extBlock, DrawMode mode) {
            Level lvl = p.level;
            SparseBitSet bits = new SparseBitSet(lvl.Width, lvl.Height, lvl.Length);
            List<int> buffer = new List<int>();
            Queue<int> temp = new Queue<int>();
            
            const int max = 65536;
            int count = 0, oneY = lvl.Width * lvl.Length;
            int* pos = stackalloc int[max];
            pos[0] = index; count++;
            
            while (count > 0 && buffer.Count <= p.group.maxBlocks) {
                index = pos[count - 1]; count--;
                ushort x = (ushort)(index % lvl.Width);
                ushort y = (ushort)((index / lvl.Width) / lvl.Length);
                ushort z = (ushort)((index / lvl.Width) % lvl.Length);
                
                if (temp.Count > 0) { pos[count] = temp.Dequeue(); count++; }
                if (bits.Get(x, y, z)) continue;
                
                bits.Set(x, y, z, true);
                buffer.Add(index);
                
                if (mode != DrawMode.verticalX) { // x
                    if (Check(p, (ushort)(x + 1), y, z, block, extBlock)) {
                        if (count == max) { temp.Enqueue(index + 1); }
                        else { pos[count] = index + 1; count++; }
                    }
                    if (Check(p, (ushort)(x - 1), y, z, block, extBlock)) {
                        if (count == max) { temp.Enqueue(index - 1); }
                        else { pos[count] = index - 1; count++; }
                    }
                }

                if (mode != DrawMode.verticalZ) { // z
                    if (Check(p, x, y, (ushort)(z + 1), block, extBlock)) {
                        if (count == max) { temp.Enqueue(index + lvl.Width); }
                        else { pos[count] = index + lvl.Width; count++; }
                    }
                    if (Check(p, x, y, (ushort)(z - 1), block, extBlock)) {
                        if (count == max) { temp.Enqueue(index - lvl.Width); }
                        else { pos[count] = index - lvl.Width; count++; }
                    }
                }

                if (!(mode == DrawMode.down || mode == DrawMode.layer)) { // y up
                    if (Check(p, x, (ushort)(y + 1), z, block, extBlock)) {
                        if (count == max) { temp.Enqueue(index + oneY); }
                        else { pos[count] = index + oneY; count++; }
                    }
                }

                if (!(mode == DrawMode.up || mode == DrawMode.layer)) { // y down
                    if (Check(p, x, (ushort)(y - 1), z, block, extBlock)) {
                        if (count == max) { temp.Enqueue(index - oneY); }
                        else { pos[count] = index - oneY; count++; }
                    }
                }
            }
            bits.Clear();
            return buffer;
        }
        
        static bool Check(Player p, ushort x, ushort y, ushort z, byte block, byte extBlock) {
            byte curBlock = p.level.GetTile(x, y, z);

            if (curBlock == block && curBlock == Block.custom_block) {
                byte curExtBlock = p.level.GetExtTile(x, y, z);
                return curExtBlock == extBlock;
            }
            return curBlock == block;
        }
        
        static bool IsConfirmed(string message) {
            return message.CaselessEq("confirm") || message.CaselessEnds(" confirm");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/fill [brush args] <mode>");
            Player.Message(p, "%HFills the area specified with the output of the current brush.");
            Player.Message(p, "   %HModes: &fnormal/up/down/layer/vertical_x/vertical_z");
            Player.Message(p, "   %HFor help about brushes, type %T/help brush%H.");
        }
    }
}
