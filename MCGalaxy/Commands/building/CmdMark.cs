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
        public override string name { get { return "mark"; } }
        public override string shortcut { get { return "click"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("m"), new CommandAlias("x"),
                    new CommandAlias("markall", "all"), new CommandAlias("ma", "all") }; }
        }

        public override void Use(Player p, string message) {
            if (message.CaselessEq("all")) {
                if (!p.HasBlockchange) {
                    Player.Message(p, "Cannot mark, no selection in progress."); return;
                }
                
                Level lvl = p.level;
                PlaceMark(p, 0, 0, 0);
                PlaceMark(p, lvl.Width - 1, lvl.Height - 1, lvl.Length - 1);
                return;
            }
            
            // convert player pos to block coords
            Vec3U16 P = ClampPos(p.Pos, p.level);
            if (message != "" && !ParseCoords(message, p, ref P)) return;            
            P = Vec3U16.Clamp(P.X, P.Y, P.Z, p.level);
            
            if (p.HasBlockchange) {
                PlaceMark(p, P.X, P.Y, P.Z);
            } else {
                // We only want to activate blocks in the world
                byte old = p.level.GetTile(P.X, P.Y, P.Z);
                if (!p.CheckManualChange(old, ExtBlock.Air, false)) return;
                
                HandleDelete handler = BlockBehaviour.deleteHandlers[old];
                if (handler != null) {
                    handler(p, old, P.X, P.Y, P.Z);
                } else {
                    Player.Message(p, "Cannot mark, no selection in progress, " +
                                   "nor could the existing block at the coordinates be activated."); return;
                }
            }
        }

        static Vec3U16 ClampPos(Position pos, Level lvl) {
            Vec3S32 p = pos.BlockCoords;
            p.Y--;
            
            if (p.X < 0) p.X = 0;
            if (p.Y < 0) p.Y = 0;
            if (p.Z < 0) p.Z = 0;
            
            if (p.X >= lvl.Width) p.X = lvl.Width - 1;
            if (p.Y >= lvl.Height) p.Y = lvl.Height - 1;
            if (p.Z >= lvl.Length) p.Z = lvl.Length - 1;
            
            return (Vec3U16)p;
        }
        
        bool ParseCoords(string message, Player p, ref Vec3U16 P) {
            string[] args = message.ToLower().SplitSpaces();
            if (args.Length != 3) { Help(p); return false; }
            ushort value;
            
            for (int i = 0; i < 3; i++) {
                if (args[i] == "x") { P.X = p.lastClick.X;
                } else if (args[i] == "y") { P.Y = p.lastClick.Y;
                } else if (args[i] == "z") { P.Z = p.lastClick.Z;
                } else if (ushort.TryParse(args[i], out value)) {
                    if (i == 0) P.X = value;
                    else if (i == 1) P.Y = value;
                    else P.Z = value;
                } else {
                    Player.Message(p, "\"{0}\" was not valid", args[i]); return false;
                }
            }
            return true;
        }
        
        static void PlaceMark(Player p, int x, int y, int z) {
            ExtBlock block = p.GetHeldBlock();
            p.ManualChange((ushort)x, (ushort)y, (ushort)z, 0, block, false);
            Player.Message(p, "Mark placed at &b({0}, {1}, {2})", x, y, z);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/mark [x y z] %H- Places a marker for selections. (such as /cuboid)");
            Player.Message(p, "  %HIf no xyz is given, marks at where you are standing");
            Player.Message(p, "    %He.g. /mark 30 y 20 will mark at (30, last y, 20)");
            Player.Message(p, "  %HNote: If no selection is in progress, activates (e.g. doors) the existing block at those coordinates.");
            Player.Message(p, "%T/mark all %H- Places markers at min and max corners of the map");
        }
    }
}
