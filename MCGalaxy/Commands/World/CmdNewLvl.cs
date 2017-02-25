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
using System.Threading;
using MCGalaxy.Generator;

namespace MCGalaxy.Commands.World {
    public sealed class CmdNewLvl : Command {
        public override string name { get { return "newlvl"; } }
        public override string shortcut { get { return "gen"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Admin, "+ can generate maps with advanced themes") }; }
        }
        public CmdNewLvl() { }

        public override void Use(Player p, string message) { GenerateMap(p, message); }
        
        internal bool GenerateMap(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length < 5 || args.Length > 6) { Help(p); return false; }
            if (!MapGen.IsRecognisedTheme(args[4])) { MapGen.PrintThemes(p); return false; }

            ushort x = 0, y = 0, z = 0;
            string name = args[0].ToLower();
            if (!CommandParser.GetUShort(p, args[1], "Width",  ref x)) return false;
            if (!CommandParser.GetUShort(p, args[2], "Height", ref y)) return false;
            if (!CommandParser.GetUShort(p, args[3], "Length", ref z)) return false;
            
            string seed = args.Length == 6 ? args[5] : "";
            if (!MapGen.OkayAxis(x)) { Player.Message(p, "width must be divisible by 16, and >= 16"); return false; }
            if (!MapGen.OkayAxis(y)) { Player.Message(p, "height must be divisible by 16, and >= 16"); return false; }
            if (!MapGen.OkayAxis(z)) { Player.Message(p, "length must be divisible by 16, and >= 16."); return false; }
            if (!CheckMapSize(p, x, y, z)) return false;
 
            if (!Formatter.ValidName(p, name, "level")) return false;
            if (LevelInfo.MapExists(name)) {
                Player.Message(p, "Level \"{0}\" already exists", name); return false;
            }
            if (!MapGen.IsSimpleTheme(args[4]) && !CheckExtraPerm(p)) { 
                MessageNeedExtra(p, 1); return false;
            }

            if (p != null && Interlocked.CompareExchange(ref p.GeneratingMap, 1, 0) == 1) {
                Player.Message(p, "You are already generating a map, please wait until that map has finished generating first.");
                return false;
            }
            
            try {
                Player.Message(p, "Generating map \"{0}\"..", name);
                using (Level lvl = new Level(name, x, y, z)) {
                    if (!MapGen.Generate(lvl, args[4], seed, p)) return false;
                    
                    lvl.Save(true);
                    lvl.Dispose();
                    name = lvl.ColoredName;
                }
                
                string format = seed != "" ? "{0}%S created level {1}%S with seed \"{2}\"" : "{0}%S created level {1}";
                string pName = p == null ? "(console)" : p.ColoredName;
                Chat.MessageAll(format, pName, name, seed);
            } finally {
                if (p != null) Interlocked.Exchange(ref p.GeneratingMap, 0);
                Server.DoGC();
            }
            return true;
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
            Player.Message(p, "  %HHeightmap theme: Seed specifies url of heightmap image.");            
            Player.Message(p, "  %HOther themes: Seed affects how terrain is generated. " +
                           "If seed is the same, the generated level will be the same.");
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
