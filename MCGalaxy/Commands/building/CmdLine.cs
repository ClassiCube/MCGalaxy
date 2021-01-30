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
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdLine : DrawCmd {
        public override string name { get { return "Line"; } }
        public override string shortcut { get { return "l"; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("ln") }; }
        }
        
        protected override string SelectionType { get { return "endpoints"; } }
        protected override string PlaceMessage { get { return "Place or break two blocks to determine the endpoints."; } }
        
        protected override DrawMode GetMode(string[] parts) {
            string msg = parts[0];
            if (msg == "normal")    return DrawMode.solid;
            if (msg == "walls")     return DrawMode.walls;
            if (msg == "straight")  return DrawMode.straight;
            if (msg == "connected") return DrawMode.wire;
            return DrawMode.normal;
        }
        
        protected override DrawOp GetDrawOp(DrawArgs dArgs) {
            LineDrawOp line = new LineDrawOp();
            if (dArgs.Mode == DrawMode.wire) {
                dArgs.Player.Message("&HIn connected lines mode, endpoint of each line also forms the " +
                                     "start point of next line. Use &T/Abort &Hto stop drawing");
            }
            
            line.WallsMode = dArgs.Mode == DrawMode.walls;
            string msg = dArgs.Message;
            if (msg.IndexOf(' ') == -1 || dArgs.Mode == DrawMode.normal) return line;
            
            string arg = msg.Substring(msg.LastIndexOf(' ') + 1);
            ushort len;
            if (ushort.TryParse(arg, out len)) line.MaxLength = len;
            return line;
        }
        
        protected override void GetMarks(DrawArgs dArgs, ref Vec3S32[] m) {
            if (dArgs.Mode != DrawMode.straight) return;
            int dx = Math.Abs(m[0].X - m[1].X), dy = Math.Abs(m[0].Y - m[1].Y), dz = Math.Abs(m[0].Z - m[1].Z);

            if (dx > dy && dx > dz) {
                m[1].Y = m[0].Y; m[1].Z = m[0].Z;
            } else if (dy > dx && dy > dz) {
                m[1].X = m[0].X; m[1].Z = m[0].Z;
            } else if (dz > dy && dz > dx) {
                m[1].X = m[0].X; m[1].Y = m[0].Y;
            }
        }
        
        protected override void GetBrush(DrawArgs dArgs) {
            LineDrawOp line = (LineDrawOp)dArgs.Op;
            int endCount = 0;
            if (line.MaxLength != int.MaxValue) endCount++;
            dArgs.BrushArgs = dArgs.Message.Splice(dArgs.ModeArgsCount, endCount);
        }
        
        protected override bool DoDraw(Player p, Vec3S32[] marks, object state, ushort block) {
            if (!base.DoDraw(p, marks, state, block)) return false;
            DrawArgs dArgs = (DrawArgs)state;
            if (dArgs.Mode != DrawMode.wire) return true;
            
            // special for connected line mode
            p.MakeSelection(MarksCount, "Selecting endpoints for &S" + dArgs.Op.Name, dArgs, DoDraw);
            Vec3U16 pos = p.lastClick;
            p.DoBlockchangeCallback(pos.X, pos.Y, pos.Z, p.GetHeldBlock());
            return false;
        }
        
        public override void Help(Player p) {
            p.Message("&T/Line <brush args>");
            p.Message("&HCreates a line between two points.");
            p.Message("&T/Line [mode] <brush args> <length>");
            p.Message("&HModes: &fnormal/walls/straight/connected");
            p.Message("&HLength optionally specifies max number of blocks in the line");
            p.Message(BrushHelpLine);
        }
    }
}
