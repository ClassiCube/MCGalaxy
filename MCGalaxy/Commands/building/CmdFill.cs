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
using MCGalaxy.Blocks;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdFill : DrawCmd {
        public override string name { get { return "Fill"; } }
        public override string shortcut { get { return "f"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        protected override string PlaceMessage { get { return "Place or break a block to mark the area you wish to fill."; } }
        public override int MarksCount { get { return 1; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("F3D"), new CommandAlias("F2D", "2d"),
                    new CommandAlias("Fill3D"), new CommandAlias("Fill2D", "2d") }; }
        }
        
        protected override DrawMode GetMode(string[] parts) {
            if (parts[parts.Length - 1].CaselessEq("confirm")) {
                string prev = parts.Length >= 2 ? parts[parts.Length - 2] : "";
                return ParseFillMode(prev);
            }
            return ParseFillMode(parts[parts.Length - 1]);
        }
        
        DrawMode ParseFillMode(string msg) {
            if (msg == "normal")     return DrawMode.solid;
            if (msg == "up")         return DrawMode.up;
            if (msg == "down")       return DrawMode.down;
            if (msg == "layer")      return DrawMode.layer;
            if (msg == "vertical_x") return DrawMode.verticalX;
            if (msg == "vertical_z") return DrawMode.verticalZ;
            if (msg == "2d")         return DrawMode.volcano;
            return DrawMode.normal;
        }
        
        protected override DrawOp GetDrawOp(DrawArgs dArg) { return new FillDrawOp(); }

        protected override void GetBrush(DrawArgs dArgs) {
            int endCount = dArgs.DefaultBrushEndCount;
            if (IsConfirmed(dArgs.Message)) endCount++;
            dArgs.BrushArgs = dArgs.Message.Splice(0, endCount);
        }
        
        protected override bool DoDraw(Player p, Vec3S32[] marks, object state, BlockID block) {
            DrawArgs dArgs = (DrawArgs)state;
            ushort x = (ushort)marks[0].X, y = (ushort)marks[0].Y, z = (ushort)marks[0].Z;
            BlockID old = p.level.GetBlock(x, y, z);
            if (!CommandParser.IsBlockAllowed(p, "fill over", old)) return false;
            
            if (dArgs.Mode == DrawMode.volcano) dArgs.Mode = Calc2DFill(p, marks);            
            FillDrawOp op = (FillDrawOp)dArgs.Op;
            op.Positions = FillDrawOp.FloodFill(p, p.level.PosToInt(x, y, z), old, dArgs.Mode);
            int count = op.Positions.Count;
            
            bool confirmed = IsConfirmed(dArgs.Message), success = true;
            if (count < p.group.DrawLimit && count > p.level.ReloadThreshold && !confirmed) {
                Player.Message(p, "This fill would affect {0} blocks.", count);
                Player.Message(p, "If you still want to fill, type %T/Fill {0} confirm", dArgs.Message);
            } else {
                success = base.DoDraw(p, marks, state, block);
            }
            
            op.Positions = null;
            return success;
        }
        
        static DrawMode Calc2DFill(Player p, Vec3S32[] marks) {
            int lenX = Math.Abs(p.Pos.BlockX - marks[0].X);
            int lenY = Math.Abs(p.Pos.BlockY - marks[0].Y);
            int lenZ = Math.Abs(p.Pos.BlockZ - marks[0].Z);
            
            if (lenY >= lenX && lenY >= lenZ) return DrawMode.layer;
            return lenX >= lenZ ? DrawMode.verticalX : DrawMode.verticalZ;
        }
        
        static bool IsConfirmed(string message) {
            return message.CaselessEq("confirm") || message.CaselessEnds(" confirm");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Fill [brush args] <mode>");
            Player.Message(p, "%HFills the area specified with the output of the current brush.");
            Player.Message(p, "   %HModes: &fnormal/up/down/layer/vertical_x/vertical_z/2d");
            Player.Message(p, BrushHelpLine);
        }
    }
}
