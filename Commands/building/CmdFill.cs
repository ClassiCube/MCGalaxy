/*
    Copyright 2011 MCGalaxy
    
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
using MCGalaxy.Drawing;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Commands {
    
    public sealed class CmdFill : DrawCmd {
        
        public override string name { get { return "fill"; } }
        public override string shortcut { get { return "f"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdFill() { }
        
        protected override SolidType GetType(string msg) {
            if (msg == "default")
                return SolidType.solid;
            else if (msg == "up")
                return SolidType.up;
            else if (msg == "down")
                return SolidType.down;
            else if (msg == "layer")
                return SolidType.layer;
            else if (msg == "vertical_x")
                return SolidType.verticalX;
            else if (msg == "vertical_z")
                return SolidType.verticalZ;
            return SolidType.Invalid;
        }
        
        protected override string PlaceMessage {
            get { return "Destroy the block you wish to fill."; }
        }
        
        protected override void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            p.ClearBlockchange();
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            byte oldType = p.level.GetTile(x, y, z), oldExtType = 0;
            if (oldType == Block.custom_block)
                oldExtType = p.level.GetExtTile(x, y, z);
            p.RevertBlock(x, y, z);
            GetRealBlock(type, extType, p, ref cpos);

            if (cpos.type == oldType) {
                if (cpos.type != Block.custom_block) {
                    Player.SendMessage(p, "Cannot fill with the same type."); return;
                }
                if (cpos.extType == oldExtType) {
                    Player.SendMessage(p, "Cannot fill with the same type."); return;
                }
            }
            if (!Block.canPlace(p, oldType) && !Block.BuildIn(oldType)) { Player.SendMessage(p, "Cannot fill with that."); return; }

            SparseBitSet bits = new SparseBitSet(p.level.Width, p.level.Height, p.level.Length);
            List<int> buffer = new List<int>(), origins = new List<int>();
            FloodFill(p, x, y, z, oldType, oldExtType, cpos.solid, bits, buffer, origins, 0);

            int totalFill = origins.Count;
            for (int i = 0; i < totalFill; i++) {
                int pos = origins[i];
                p.level.IntToPos(pos, out x, out y, out z);
                FloodFill(p, x, y, z, oldType, oldExtType, cpos.solid, bits, buffer, origins, 0);
                totalFill = origins.Count;
            }
            
            FillDrawOp drawOp = new FillDrawOp();
            drawOp.Positions = buffer;
            SolidBrush brush = new SolidBrush(cpos.type, cpos.extType);
            if (!DrawOp.DoDrawOp(drawOp, brush, p, 0, 0, 0, 0, 0, 0))
                return;
            bits.Clear();
            drawOp.Positions = null;           

            if (p.staticCommands)
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        protected override void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) { }

        void FloodFill(Player p, ushort x, ushort y, ushort z, byte oldType, byte oldExtType, SolidType fillType,
                       SparseBitSet bits, List<int> buffer, List<int> origins, int depth) {
            if (bits.Get(x, y, z)) return;
            int index = p.level.PosToInt(x, y, z);
            if (depth > 2000) { origins.Add(index); return; }
            bits.Set(x, y, z, true);
            buffer.Add(index);

            if (fillType != SolidType.verticalX) { // x
                if (CheckTile(p, (ushort)(x + 1), y, z, oldType, oldExtType))
                    FloodFill(p, (ushort)(x + 1), y, z, oldType, oldExtType, fillType, bits, buffer, origins, depth + 1);
                if (CheckTile(p, (ushort)(x - 1), y, z, oldType, oldExtType))
                    FloodFill(p, (ushort)(x - 1), y, z, oldType, oldExtType, fillType, bits, buffer, origins, depth + 1);
            }

            if (fillType != SolidType.verticalZ) { // z
                if (CheckTile(p, x, y, (ushort)(z + 1), oldType, oldExtType))
                    FloodFill(p, x, y, (ushort)(z + 1), oldType, oldExtType, fillType, bits, buffer, origins, depth + 1);
                if (CheckTile(p, x, y, (ushort)(z - 1), oldType, oldExtType))
                    FloodFill(p, x, y, (ushort)(z - 1), oldType, oldExtType, fillType, bits, buffer, origins, depth + 1);
            }

            if (!(fillType == SolidType.down || fillType == SolidType.layer)) { // y up
                if (CheckTile(p, x, (ushort)(y + 1), z, oldType, oldExtType))
                    FloodFill(p, x, (ushort)(y + 1), z, oldType, oldExtType, fillType, bits, buffer, origins, depth + 1);
            }

            if (!(fillType == SolidType.up || fillType == SolidType.layer)) { // y down
                if (CheckTile(p, x, (ushort)(y - 1), z, oldType, oldExtType))
                    FloodFill(p, x, (ushort)(y - 1), z, oldType, oldExtType, fillType, bits, buffer, origins, depth + 1);
            }
        }
        
        bool CheckTile(Player p, ushort x, ushort y, ushort z, byte oldTile, byte oldExtTile) {
            byte tile = p.level.GetTile(x, y, z);

            if (tile == oldTile && tile == Block.custom_block) {
                byte extTile = p.level.GetExtTile(x, y, z);
                return extTile == oldExtTile;
            }
            return tile == oldTile;
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/fill [block] [type] - Fills the area specified with [block].");
            Player.SendMessage(p, "[types] - up, down, layer, vertical_x, vertical_z");
        }
    }
}
