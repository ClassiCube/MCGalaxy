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
using MCGalaxy.Blocks;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdMark : Command {
        public override string name { get { return "Mark"; } }
        public override string shortcut { get { return "click"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("m"), new CommandAlias("x"),
                    new CommandAlias("MarkAll", "all"), new CommandAlias("ma", "all") }; }
        }

        public override void Use(Player p, string message) {
            if (message.CaselessEq("all")) {
                if (!DoMark(p, 0, 0, 0)) {
                    Player.Message(p, "Cannot mark, no selection in progress.");
                } else {                    
                    Level lvl = p.level;
                    DoMark(p, lvl.Width - 1, lvl.Height - 1, lvl.Length - 1);
                }
                return;
            }
            
            Vec3S32 P = p.Pos.BlockCoords;
            P.Y = (p.Pos.Y - 32) / 32;
            if (message.Length > 0 && !ParseCoords(message, p, ref P)) return;
            
            P.X = Clamp(P.X, p.level.Width);
            P.Y = Clamp(P.Y, p.level.Height);
            P.Z = Clamp(P.Z, p.level.Length);
            if (DoMark(p, P.X, P.Y, P.Z)) return;
            
            Vec3U16 mark = (Vec3U16)P;
            // We only want to activate blocks in the world
            ExtBlock old = p.level.GetBlock(mark.X, mark.Y, mark.Z);
            if (!p.CheckManualChange(old, ExtBlock.Air, true)) return;
            
            HandleDelete handler = p.level.deleteHandlers[old.Index];
            if (handler != null) {
                handler(p, old, mark.X, mark.Y, mark.Z);
            } else {
                Player.Message(p, "Cannot mark, no selection in progress, " +
                               "nor could the existing block at the coordinates be activated."); return;
            }
        }
        
        bool ParseCoords(string message, Player p, ref Vec3S32 P) {
            string[] args = message.SplitSpaces();
            // Expand /mark ~4 into /mark ~4 ~4 ~4
            if (args.Length == 1) {
                args = new string[] { message, message, message };
            }
            if (args.Length != 3) { Help(p); return false; }
            
            // Hacky workaround for backwards compatibility
            if (args[0].CaselessEq("X")) args[0] = p.lastClick.X.ToString();
            if (args[1].CaselessEq("Y")) args[1] = p.lastClick.Y.ToString();
            if (args[2].CaselessEq("Z")) args[2] = p.lastClick.Z.ToString();
            
            return CommandParser.GetCoords(p, args, 0, ref P);
        }
        
        static int Clamp(int value, int axisLen) {
            if (value < 0) return 0;
            if (value >= axisLen) return axisLen - 1;
            return value;
        }
        
        internal static bool DoMark(Player p, int x, int y, int z) {
            if (!p.HasBlockChange()) return false;
            if (!p.Ignores.DrawOutput) {
                Player.Message(p, "Mark placed at &b({0}, {1}, {2})", x, y, z);
            }
            
            ExtBlock block = p.GetHeldBlock();
            p.DoBlockchangeCallback((ushort)x, (ushort)y, (ushort)z, block);            
            return true;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Mark <x y z> %H- Places a marker for selections, e.g for %T/z");
            Player.Message(p, "%HUse ~ before a coordinate to mark relative to current position");
            Player.Message(p, "%HIf no coordinates are given, marks at where you are standing");
            Player.Message(p, "%HIf only x coordinate is given, it is used for y and z too");
            Player.Message(p, "  %He.g. /mark 30 y 20 will mark at (30, last y, 20)");
            Player.Message(p, "%T/Mark all %H- Places markers at min and max corners of the map");
            Player.Message(p, "%HActivates the block (e.g. door) if no selection is in progress");
        }
    }
}
