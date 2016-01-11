/*
    Copyright 2011 MCGalaxy
        
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

namespace MCGalaxy.Commands {
    
    public sealed class CmdLine : DrawCmd {
        
        public override string name { get { return "line"; } }
        public override string shortcut { get { return "l"; } }
        public CmdLine() {}
        
        protected override int MaxArgs { get { return 3; } }
        
        protected override SolidType GetType(string msg) {
            if (msg == "walls")
                return SolidType.walls;
            else if (msg == "straight")
                return SolidType.straight;
            return SolidType.solid;
        }
        
        protected override void OnUse(Player p, string msg, string[] parts, ref CatchPos cpos) {
            if (parts.Length == 2 || parts.Length == 3) {
                string arg = parts[parts.Length - 1];
                ushort len;
                if (!ushort.TryParse(arg, out len)) {
                    if (arg == "walls" || arg == "straight" || arg == "normal") return;                
                    Player.SendMessage(p, msg + " is not valid length, assuming maximum length allowed.");
                } else {
                    cpos.data = len;
                }
            } 
        }

        protected override void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            GetRealBlock(type, extType, p, ref cpos);
            List<Pos> buffer = new List<Pos>();
            Pos pos = new Pos();

            if (cpos.solid == SolidType.straight) { 
                int xdif = Math.Abs(cpos.x - x);
                int ydif = Math.Abs(cpos.y - y);
                int zdif = Math.Abs(cpos.z - z);

                if (xdif > ydif && xdif > zdif) {
                    y = cpos.y; z = cpos.z;
                } else if (ydif > xdif && ydif > zdif) {
                    x = cpos.x; z = cpos.z;
                } else if (zdif > ydif && zdif > xdif) {
                    y = cpos.y; x = cpos.x;
                }
            }

            Line lx, ly, lz;
            int[] pixel = { cpos.x, cpos.y, cpos.z };        
            int dx = x - cpos.x, dy = y - cpos.y, dz = z - cpos.z;
            lx.inc = Math.Sign(dx); ly.inc = Math.Sign(dy); lz.inc = Math.Sign(dz);

            int xLen = Math.Abs(dx), yLen = Math.Abs(dy), zLen = Math.Abs(dz);
            lx.dx2 = xLen << 1; ly.dx2 = yLen << 1; lz.dx2 = zLen << 1;
            lx.index = 0; ly.index = 1; lz.index = 2;

            if (xLen >= yLen && xLen >= zLen)
                DoLine(ly, lz, lx, xLen, pixel, buffer);
            else if (yLen >= xLen && yLen >= zLen)
                DoLine(lx, lz, ly, yLen, pixel, buffer);
            else
                DoLine(ly, lx, lz, zLen, pixel, buffer);

            pos.x = (ushort)pixel[0]; pos.y = (ushort)pixel[1]; pos.z = (ushort)pixel[2];
            buffer.Add(pos);
            int maxLen = cpos.data == null ? int.MaxValue : (ushort)cpos.data;

            int count = Math.Min(buffer.Count, maxLen);
            if (cpos.solid == SolidType.walls)
                count *= Math.Abs(cpos.y - y);

            if (count > p.group.maxBlocks) {
                Player.SendMessage(p, "You tried to draw " + count + " blocks at once.");
                Player.SendMessage(p, "You are limited to " + p.group.maxBlocks);
                return;
            }

            for (int i = 0; i < maxLen && i < buffer.Count; i++) {
                pos = buffer[i];
                if (cpos.solid == SolidType.walls) {
                    for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); yy++) {
                        p.level.Blockchange(p, pos.x, yy, pos.z, type, extType);
                    }
                } else {
                    p.level.Blockchange(p, pos.x, pos.y, pos.z, type, extType);
                }
            }

            count = Math.Min(maxLen, buffer.Count);
            Player.SendMessage(p, "Line was " + count + " blocks long.");
            if (p.staticCommands)
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        struct Line { public int dx2, inc, index; }
        
        struct Pos { public ushort x, y, z; }
        
        static void DoLine(Line l1, Line l2, Line l3, int len, int[] pixel, List<Pos> buffer) {
            int err_1 = l1.dx2 - len, err_2 = l2.dx2 - len;
            Pos pos;
            for (int i = 0; i < len; i++) {
                pos.x = (ushort)pixel[0]; pos.y = (ushort)pixel[1]; pos.z = (ushort)pixel[2];
                buffer.Add(pos);

                if (err_1 > 0) {
                    pixel[l1.index] += l1.inc;
                    err_1 -= l3.dx2;
                }
                if (err_2 > 0) {
                    pixel[l2.index] += l2.inc;
                    err_2 -= l3.dx2;
                }
                err_1 += l1.dx2; err_2 += l2.dx2;
                pixel[l3.index] += l3.inc;
            }
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/line [block type] <mode> <length>");
            Player.SendMessage(p, "   %HCreates a line between two points.");
            Player.SendMessage(p, "   %HMode can be: normal/walls/straight");
            Player.SendMessage(p, "   %HLength specifies the max number of blocks in the line.");
        }
    }
}
