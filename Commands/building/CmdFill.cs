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
            byte oldType = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, oldType);
            GetRealBlock(type, extType, p, ref cpos);

            if (cpos.type == oldType) { Player.SendMessage(p, "Cannot fill with the same type."); return; }
            if (!Block.canPlace(p, oldType) && !Block.BuildIn(oldType)) { Player.SendMessage(p, "Cannot fill with that."); return; }

            SparseBitSet bits = new SparseBitSet(p.level.Width, p.level.Height, p.level.Length);
            List<Pos> buffer = new List<Pos>();
            fromWhere.Clear();
            FloodFill(p, x, y, z, oldType, cpos.solid, bits, ref buffer, 0);

            int totalFill = fromWhere.Count;
            for (int i = 0; i < totalFill; i++) {
                Pos pos = fromWhere[i];
                FloodFill(p, pos.x, pos.y, pos.z, oldType, cpos.solid, bits, ref buffer, 0);
                totalFill = fromWhere.Count;
            }
            fromWhere.Clear();

            if (buffer.Count > p.group.maxBlocks) {
                Player.SendMessage(p, "You tried to fill " + buffer.Count + " blocks.");
                Player.SendMessage(p, "You cannot fill more than " + p.group.maxBlocks + ".");
                return;
            }
            
            if (buffer.Count < 10000) {
                if (p.level.bufferblocks && !p.level.Instant) {
                    foreach (Pos pos in buffer)
                        BlockQueue.Addblock(p, pos.x, pos.y, pos.z, cpos.type);
                } else {
                    foreach (Pos pos in buffer)
                        p.level.Blockchange(p, pos.x, pos.y, pos.z, cpos.type);
                }
            } else {
                p.SendMessage("You tried to cuboid over 10000 blocks, reloading map for faster fill.");
                foreach (Pos pos in buffer)
                    p.level.SetTile(pos.x, pos.y, pos.z, cpos.type, p);
                
                foreach (Player pl in Player.players) {
                    if (pl.level.name.ToLower() == p.level.name.ToLower())
                        Command.all.Find("reveal").Use(p, pl.name);
                }
            }
            
            Player.SendMessage(p, "Filled " + buffer.Count + " blocks.");
            buffer.Clear();
            buffer = null;
            bits.Clear();
            bits = null;

            if (p.staticCommands)
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        protected override void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) { }

        List<Pos> fromWhere = new List<Pos>();
        void FloodFill(Player p, ushort x, ushort y, ushort z, byte oldType, SolidType fillType,
                       SparseBitSet bits, ref List<Pos> buffer, int depth) {
            Pos pos;
            pos.x = x; pos.y = y; pos.z = z;
            if (bits.Get(x, y, z)) return;

            if (depth > 2000) {
                fromWhere.Add(pos); return;
            }
            bits.Set(x, y, z, true);
            buffer.Add(pos);

            if (fillType != SolidType.verticalX) { // x
                if (p.level.GetTile((ushort)(x + 1), y, z) == oldType)
                    FloodFill(p, (ushort)(x + 1), y, z, oldType, fillType, bits, ref buffer, depth + 1);
                if (p.level.GetTile((ushort)(x - 1), y, z) == oldType)
                    FloodFill(p, (ushort)(x - 1), y, z, oldType, fillType, bits, ref buffer, depth + 1);
            }

            if (fillType != SolidType.verticalZ) { // z
                if (p.level.GetTile(x, y, (ushort)(z + 1)) == oldType)
                    FloodFill(p, x, y, (ushort)(z + 1), oldType, fillType, bits, ref buffer, depth + 1);
                if (p.level.GetTile(x, y, (ushort)(z - 1)) == oldType)
                    FloodFill(p, x, y, (ushort)(z - 1), oldType, fillType, bits, ref buffer, depth + 1);
            }

            if (!(fillType == SolidType.down || fillType == SolidType.layer)) { // y up
                if (p.level.GetTile(x, (ushort)(y + 1), z) == oldType)
                    FloodFill(p, x, (ushort)(y + 1), z, oldType, fillType, bits, ref buffer, depth + 1);
            }

            if (!(fillType == SolidType.up || fillType == SolidType.layer)) { // y down
                if (p.level.GetTile(x, (ushort)(y - 1), z) == oldType)
                    FloodFill(p, x, (ushort)(y - 1), z, oldType, fillType, bits, ref buffer, depth + 1);
            }
        }

        public struct Pos { public ushort x, y, z; }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/fill [block] [type] - Fills the area specified with [block].");
            Player.SendMessage(p, "[types] - up, down, layer, vertical_x, vertical_z");
        }
    }
}
