/*
    Copyright 2011 MCForge
    
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

namespace MCGalaxy.Commands.Building {    
    public sealed class CmdSPlace : Command {       
        public override string name { get { return "splace"; } }
        public override string shortcut { get { return "set"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdSPlace() { }

        public override void Use(Player p, string message) {
            ushort distance = 0, interval = 0;
            if (message == "") { Help(p); return; }
            if (p == null) { MessageInGameOnly(p); return; }
            
            string[] parts = message.Split(' ');
            if (!ushort.TryParse(parts[0], out distance)) {
                Player.Message(p, "Distance must be a number less than 65536."); return;
            }
            if (parts.Length > 1 && !ushort.TryParse(parts[1], out interval)) {
                Player.Message(p, "Interval must be a number less than 65536."); return;
            }

            if (distance < 1) {
                Player.Message(p, "Enter a distance greater than 0."); return;
            }
            if (interval >= distance) {
                Player.Message(p, "The Interval cannot be greater than the distance."); return;
            }

            CatchPos cpos = default(CatchPos);
            cpos.givenMessage = message;
            cpos.distance = distance; cpos.interval = interval;
            p.blockchangeObject = cpos;
            Player.Message(p, "Place two blocks to determine direction.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        
        public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            ushort distance = cpos.distance, interval = cpos.interval;
            if (x == cpos.x && y == cpos.y && z == cpos.z) { Player.Message(p, "No direction was selected"); return; }
            
            int dirX = 0, dirY = 0, dirZ = 0;
            int dx = Math.Abs(cpos.x - x), dy = Math.Abs(cpos.y - y), dz = Math.Abs(cpos.z - z);
            if (dy > dx && dy > dz) dirY = y > cpos.y ? 1 : -1;
            else if (dx > dz) dirX = x > cpos.x ? 1 : -1;
            else dirZ = z > cpos.z ? 1 : -1;
            
            ushort endX = (ushort)(cpos.x + dirX * distance);
            ushort endY = (ushort)(cpos.y + dirY * distance);
            ushort endZ = (ushort)(cpos.z + dirZ * distance);            
            p.level.UpdateBlock(p, endX, endY, endZ, Block.rock, 0);   
            
            if (interval > 0) {
                ushort xx = cpos.x, yy = cpos.y, zz = cpos.z;
                int delta = 0;
                while (xx < p.level.Width && yy < p.level.Height && zz < p.level.Length && delta < distance) {
                    p.level.UpdateBlock(p, xx, yy, zz, Block.rock, 0);
                    xx = (ushort)(xx + dirX * interval);
                    yy = (ushort)(yy + dirY * interval);
                    zz = (ushort)(zz + dirZ * interval);
                    delta = Math.Abs(xx - cpos.x) + Math.Abs(yy - cpos.y) + Math.Abs(zz - cpos.z);
                }
            } else {
                p.level.UpdateBlock(p, cpos.x, cpos.y, cpos.z, Block.rock, 0);
            }

            if (interval > 0)
                Player.Message(p, "Placed stone blocks " + interval + " apart");
            else
                Player.Message(p, "Placed stone blocks " + distance + " apart");
            if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        struct CatchPos {
            public ushort x, y, z; public string givenMessage;
            public ushort distance, interval;
        }

        public override void Help(Player p) {
            Player.Message(p, "/splace [distance] [interval] - Measures a set [distance] and places a stone block at each end.");
            Player.Message(p, "Optionally place a block at set [interval] between them.");
        }
    }
}
