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
namespace MCGalaxy.Commands.Building {
    public sealed class CmdMark : Command {
        public override string name { get { return "mark"; } }
        public override string shortcut { get { return "click"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("m"), new CommandAlias("x") }; }
        }

        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            // convert player pos to block coords
            Vec3U16 P = Vec3U16.ClampPos(p.pos[0], (ushort)(p.pos[1] - 32), p.pos[2], p.level);
            P.X /= 32; P.Y /= 32; P.Z /= 32;
            if (message != "" && !ParseCoords(message, p, ref P)) return;
            
            P = Vec3U16.Clamp(P.X, P.Y, P.Z, p.level);            
            if (!p.HasBlockchange) {
                Player.Message(p, "Cannot mark, no selection or cuboid in progress."); return;
            }
            p.ManualChange(P.X, P.Y, P.Z, 0, Block.rock, 0, false);
            Player.Message(p, "Mark placed at &b({0}, {1}, {2})", P.X, P.Y, P.Z);
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
        
        public override void Help(Player p) {
            Player.Message(p, "%T/mark [x y z] %H- Places a marker for selections or cuboids");
            Player.Message(p, "  %HIf no xyz is given, marks at where you are standing");
            Player.Message(p, "  %He.g. /mark 30 y 20 will mark at (30, last y, 20)");
        }
    }
}
