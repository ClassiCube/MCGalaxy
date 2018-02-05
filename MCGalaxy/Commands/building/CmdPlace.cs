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
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdPlace : Command {
        public override string name { get { return "Place"; } }
        public override string shortcut { get { return "pl"; } }
        public override string type { get { return CommandTypes.Building; } }

        public override void Use(Player p, string message) {
            BlockID block = p.GetHeldBlock();
            Vec3S32 P = p.Pos.BlockCoords;
            P.Y = (p.Pos.Y - 32) / 32;

            string[] parts = message.SplitSpaces();
            switch (parts.Length) {
                case 1:
                    if (message.Length == 0) break;
                    if (!CommandParser.GetBlock(p, parts[0], out block)) return;
                    break;
                case 3:
                    if (!CommandParser.GetCoords(p, parts, 0, ref P)) return;
                    break;
                case 4:
                    if (!CommandParser.GetBlock(p, parts[0], out block)) return;
                    if (!CommandParser.GetCoords(p, parts, 1, ref P)) return;
                    break;
                default:
                    Help(p); return;
            }

            if (!CommandParser.IsBlockAllowed(p, "place", block)) return;            
            P.X = Clamp(P.X, p.level.Width);
            P.Y = Clamp(P.Y, p.level.Height);
            P.Z = Clamp(P.Z, p.level.Length);
            
            p.level.UpdateBlock(p, (ushort)P.X, (ushort)P.Y, (ushort)P.Z, block);
            string blockName = Block.GetName(p, block);
            if (!p.Ignores.DrawOutput) {
                Player.Message(p, "{3} block was placed at ({0}, {1}, {2}).", P.X, P.Y, P.Z, blockName);
            }
        }
        
        static int Clamp(int value, int axisLen) {
            if (value < 0) return 0;
            if (value >= axisLen) return axisLen - 1;
            return value;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Place <block>");
            Player.Message(p, "%HPlaces block at your feet.");
            Player.Message(p, "%T/Place <block> [x y z]");
            Player.Message(p, "%HPlaces block at [x y z]");
            Player.Message(p, "%HUse ~ before a coord to place relative to current position");
        }
    }
}
