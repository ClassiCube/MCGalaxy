/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.Linq;

namespace MCGalaxy.Commands
{
    public sealed class CmdCuboid : Command
    {
        public override string name { get { return "cuboid"; } }
        public override string shortcut { get { return "z"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }

        public static byte wait;

        public override void Use(Player p, string message) {
            string[] parts = message.Split(' ');
            for (int i = 0; i < parts.Length; i++)
                parts[i] = parts[i].ToLower();
            wait = 0;
            CatchPos cpos = default(CatchPos);
            
            if (parts.Length > 2) {
                wait = 1;
                Help(p); return;
            } else if (parts.Length == 2) {                
                byte type = GetBlock(p, parts[0]);
                if (type == 255) return;
                SolidType solid = GetType(p, parts[1]);
                if (solid == SolidType.Invalid) {
                    Help(p); return;
                }
                cpos.solid = solid;
                cpos.type = type;
                p.blockchangeObject = cpos;
            } else if (parts.Length == 1) {
                byte type = 0xFF;
                SolidType solid = GetType(p, parts[0]);
                if (solid == SolidType.Invalid) {
                    solid = SolidType.solid;
                    type = GetBlock(p, parts[0]);
                    if (type == 255) return;
                }
                cpos.solid = solid;
                cpos.type = type;
                p.blockchangeObject = cpos;
            } else {
                cpos.solid = SolidType.solid;
                cpos.type = 0xFF;
                p.blockchangeObject = cpos;
            }
            if (p.pyramidsilent == false) {
                Player.SendMessage(p, "Place two blocks to determine the edges.");
            }
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type) {
            RevertAndClearState(p, x, y, z);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z;
            p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }

        public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type) {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            type = cpos.type == 0xFF ? p.bindings[type] : cpos.type;
            DrawOp drawOp = null;
            Brush brush = null;

            switch (cpos.solid) {
                case SolidType.solid:
                    drawOp = new CuboidDrawOp(); break;
                case SolidType.hollow:
                    drawOp = new CuboidHollowsDrawOp(); break;
                case SolidType.walls:
                    drawOp = new CuboidWallsDrawOp(); break;
                case SolidType.holes:
                    drawOp = new CuboidHolesDrawOp(); break;
                case SolidType.wire:
                    drawOp = new CuboidWireframeDrawOp(); break;
                case SolidType.random:
                    drawOp = new CuboidDrawOp();
                    brush = new RandomBrush(type); break;
            }
            
            if (brush == null) brush = new SolidBrush(type);
            ushort x1 = Math.Min(cpos.x, x), x2 = Math.Max(cpos.x, x);
            ushort y1 = Math.Min(cpos.y, y), y2 = Math.Max(cpos.y, y);
            ushort z1 = Math.Min(cpos.z, z), z2 = Math.Max(cpos.z, z);
            
            int affected = 0;
            if (!drawOp.CanDraw(x1, y1, z1, x2, y2, z2, p, out affected))
                return;
            if (p.pyramidsilent == false)
                Player.SendMessage(p, "Drawing an estimated " + affected + " blocks.");
            bool needReveal = drawOp.DetermineDrawOpMethod(p.level, affected);
            drawOp.Perform(x1, y1, z1, x2, y2, z2, p, p.level, brush);
            
            if (needReveal) {
                foreach (Player pl in Player.players) {
                    if (pl.level.name.ToLower() == p.level.name.ToLower())
                        Command.all.Find("reveal").Use(p, pl.name);
                }
            }
            wait = 2;
            if (p.staticCommands)
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        static SolidType GetType(Player p, string msg) {
            if (msg == "solid")
                return SolidType.solid;
            else if (msg == "hollow")
                return SolidType.hollow;
            else if (msg == "walls")
                return SolidType.walls;
            else if (msg == "holes")
                return SolidType.holes;
            else if (msg == "wire")
                return SolidType.wire;
            else if (msg == "random")
                return SolidType.random;
            return SolidType.Invalid;
        }
        
        static byte GetBlock(Player p, string msg) {
            byte type = Block.Byte(msg);
            if (type == 255) {
                Player.SendMessage(p, "There is no block \"" + msg + "\".");
                wait = 1; return 255;
            }
            if (!Block.canPlace(p, type)) {
                Player.SendMessage(p, "Cannot place that.");
                wait = 1; return 255;
            }
            return type;
        }
        
        struct CatchPos {
            public SolidType solid;
            public byte type;
            public ushort x, y, z;
        }

        enum SolidType {
            solid, hollow, walls,
            holes, wire, random,
            Invalid = -1,
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/cuboid [type] <solid/hollow/walls/holes/wire/random> - create a cuboid of blocks.");
        }
    }
}
