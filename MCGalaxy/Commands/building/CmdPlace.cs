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
    public sealed class CmdPlace : Command {
        public override string name { get { return "Place"; } }
        public override string shortcut { get { return "pl"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }

        public override void Use(Player p, string message) {
            ExtBlock block = p.GetHeldBlock();
            int x = p.Pos.BlockX, y = (p.Pos.Y - 32) / 32, z = p.Pos.BlockZ;

            string[] parts = message.SplitSpaces();
            switch (parts.Length) {
                case 1:
                    if (message.Length == 0) break;
                    if (!CommandParser.GetBlock(p, parts[0], out block)) return;
                    break;
                case 3:
                    if (!CommandParser.GetInt(p, parts[0], "X", ref x)) return;
                    if (!CommandParser.GetInt(p, parts[1], "Y", ref y)) return;
                    if (!CommandParser.GetInt(p, parts[2], "Z", ref z)) return;
                    break;
                case 4:
                    if (!CommandParser.GetBlock(p, parts[0], out block)) return;
                    if (!CommandParser.GetInt(p, parts[1], "X", ref x)) return;
                    if (!CommandParser.GetInt(p, parts[2], "Y", ref y)) return;
                    if (!CommandParser.GetInt(p, parts[3], "Z", ref z)) return;
                    break;
                default:
                    Help(p); return;
            }

            if (!CommandParser.IsBlockAllowed(p, "place", block)) return;
            
            x = Clamp(x, p.level.Width);
            y = Clamp(y, p.level.Height);
            z = Clamp(z, p.level.Length);
            
            p.level.UpdateBlock(p, (ushort)x, (ushort)y, (ushort)z, block);
            string blockName = p.level.BlockName(block);
            if (!p.Ignores.DrawOutput) {
                Player.Message(p, "{3} block was placed at ({0}, {1}, {2}).", x, y, z, blockName);
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
        }
    }
}
