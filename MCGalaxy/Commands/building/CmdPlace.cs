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
    public sealed class CmdPlace : Command2 {
        public override string name { get { return "Place"; } }
        public override string shortcut { get { return "pl"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool SuperUseable { get { return false; } }
        
        public override void Use(Player p, string message, CommandData data) {
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
            P = p.level.ClampPos(P);
            
            p.level.UpdateBlock(p, (ushort)P.X, (ushort)P.Y, (ushort)P.Z, block);
            string blockName = Block.GetName(p, block);
            if (!p.Ignores.DrawOutput) {
                p.Message("{1} block was placed at ({0}).", P, blockName);
            }
        }
        
        public override void Help(Player p) {
            p.Message("&T/Place <block>");
            p.Message("&HPlaces block at your feet.");
            p.Message("&T/Place <block> [x y z]");
            p.Message("&HPlaces block at [x y z]");
            p.Message("&HUse ~ before a coord to place relative to current position");
        }
    }
}
