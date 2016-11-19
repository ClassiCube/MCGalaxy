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
 
namespace MCGalaxy.Commands.Building {
    public sealed class CmdMark : Command {
        public override string name { get { return "mark"; } }
        public override string shortcut { get { return "click"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("m"), new CommandAlias("x"),
                    new CommandAlias("markall", "all"), new CommandAlias("ma", "all") }; }
        }

        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            
            if (message.CaselessEq("all")) {
                if (!p.HasBlockchange) {
                    Player.Message(p, "Cannot mark, no selection or cuboid in progress."); return;
                }
                
                Level lvl = p.level;
                PlaceMark(p, 0, 0, 0);
                PlaceMark(p, lvl.Width - 1, lvl.Height - 1, lvl.Length - 1);
                return;
            }
            
            // convert player pos to block coords
            Vec3U16 P = Vec3U16.ClampPos(p.pos[0], (ushort)(p.pos[1] - 32), p.pos[2], p.level);
            P.X /= 32; P.Y /= 32; P.Z /= 32;
            if (message != "" && !ParseCoords(message, p, ref P)) return;            
            P = Vec3U16.Clamp(P.X, P.Y, P.Z, p.level);
            
            if (p.HasBlockchange) {
                PlaceMark(p, P.X, P.Y, P.Z);
            } else {
                // We only want to activate blocks in the world
                byte old = p.level.GetTile(P.X, P.Y, P.Z);
                if (!p.CheckManualChange(old, Block.air, false)) return;
                
                HandleDelete handler = BlockBehaviour.deleteHandlers[old];
                if (handler != null) {
                    handler(p, old, P.X, P.Y, P.Z);
                } else {
                    Player.Message(p, "Cannot mark, no selection or cuboid in progress, " +
                	               "nor could the existing block at the coordinates be activated."); return;
                }
            }
        }
        
        bool ParseCoords(string message, Player p, ref Vec3U16 P) {
            string[] args = message.ToLower().Split(' ');
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
            byte extBlock = 0;
            byte block = p.GetActualHeldBlock(out extBlock);
            p.ManualChange((ushort)x, (ushort)y, (ushort)z, 0, block, extBlock, false);
            Player.Message(p, "Mark placed at &b({0}, {1}, {2})", x, y, z);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/mark [x y z] %H- Places a marker for selections or cuboids");
            Player.Message(p, "  %HIf no xyz is given, marks at where you are standing");
            Player.Message(p, "    %He.g. /mark 30 y 20 will mark at (30, last y, 20)");
            Player.Message(p, "  %HNote: If no selection is in progress, activates (e.g. doors) the existing block at those coordinates.");
            Player.Message(p, "%T/mark all %H- Places markers at min and max corners of the map");
        }
    }
}
