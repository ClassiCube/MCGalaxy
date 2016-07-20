/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.IO;
using MCGalaxy.Generator;

namespace MCGalaxy.Commands.World {
    public sealed class CmdNewLvl : Command {
        public override string name { get { return "newlvl"; } }
        public override string shortcut { get { return "gen"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdNewLvl() { }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length < 5 || args.Length > 6) { Help(p); return; }
            if (!MapGen.IsRecognisedTheme(args[4])) { MapGen.PrintThemes(p); return; }

            ushort x, y, z;
            string name = args[0].ToLower();
            if (!UInt16.TryParse(args[1], out x) || !UInt16.TryParse(args[2], out y) || !UInt16.TryParse(args[3], out z)) {
                Player.Message(p, "Invalid dimensions."); return;
            }
            
            string seed = args.Length == 6 ? args[5] : "";
            if (!MapGen.OkayAxis(x)) { Player.Message(p, "width must be divisible by 16, and >= 16"); return; }
            if (!MapGen.OkayAxis(y)) { Player.Message(p, "height must be divisible by 16, and >= 16"); return; }
            if (!MapGen.OkayAxis(z)) { Player.Message(p, "length must be divisible by 16, and >= 16."); return; }
            if (!CheckMapSize(p, x, y, z)) return;
 
            if (!ValidName(p, name, "level")) return;
            if (LevelInfo.ExistsOffline(name)) {
                Player.Message(p, "Level \"{0}\" already exists", name); return;
            }           

            try {
                using (Level lvl = new Level(name, x, y, z)) {
                    if (!MapGen.Generate(lvl, args[4], seed, p)) return;
                    LevelDB.CreateTables(name);
                    lvl.Save(true);
                    lvl.Dispose();
                }
            } finally {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            
            string format = seed != "" ? "Level \"{0}\" created with seed \"{1}\"" : "Level \"{0}\" created";
            Player.GlobalMessage(String.Format(format, name, seed));
        }
        
        internal static bool CheckMapSize(Player p, int x, int y, int z) {
            if (p == null) return true;
            int limit = p.Rank < LevelPermission.Admin ? Server.MapGenLimit : Server.MapGenLimitAdmin;
            if ((long)x * y * z <= limit ) return true;
            
            string text = "You cannot create a map with over ";
            if (limit > 1000 * 1000) text += (limit / (1000 * 1000)) + " million blocks";
            else if (limit > 1000) text += (limit / 1000) + " thousand blocks";
            else text += limit + " blocks";
            Player.Message(p, text);
            return false;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/newlvl [name] [width] [height] [length] [theme] <seed>");
            Player.Message(p, "%HCreates/generates a new level.");
            Player.Message(p, "  %HSizes must be >= 16 and <= 8192, and divisible by 16.");
            Player.Message(p, "  %HDue to limitations, other players don't show past 1024.");
            Player.Message(p, "  %HType %T/help newlvl themes %Hto see a list of themes.");
            Player.Message(p, "%HSeed is optional, and controls how the level is generated.");
            Player.Message(p, "  %HFlat theme: Seed specifies the grass height.");
            Player.Message(p, "  %HOther themes: Seed affects how terrain is generated.");
            Player.Message(p, "%HIf the seed is the same, the generated level will be the same.");
        }
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("theme") || message.CaselessEq("themes")) {
                MapGen.PrintThemes(p);
            } else {
                base.Help(p, message);
            }
        }
    }
}
