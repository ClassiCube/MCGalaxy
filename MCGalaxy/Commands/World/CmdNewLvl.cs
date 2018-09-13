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
using System.Threading;
using MCGalaxy.Generator;

namespace MCGalaxy.Commands.World {
    public sealed class CmdNewLvl : Command2 {
        public override string name { get { return "NewLvl"; } }
        public override string shortcut { get { return "Gen"; } }
        public override string type { get { return CommandTypes.World; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Admin, "can generate maps with advanced themes") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces();
            if (args.Length < 5 || args.Length > 6) { Help(p); return; }
            
            Level lvl = null;
            try {
                lvl = GenerateMap(p, args, data);
                if (lvl == null) return;
                
                lvl.Save(true);
            } finally {
                if (lvl != null) lvl.Dispose();
                Server.DoGC();
            }
        }
        
        internal Level GenerateMap(Player p, string[] args, CommandData data) {
            if (args.Length < 5) return null;
            MapGen gen = MapGen.Find(args[4]);
            if (gen == null) { MapGen.PrintThemes(p); return null; }

            ushort x = 0, y = 0, z = 0;
            string name = args[0].ToLower();
            if (!GetDimensions(p, args, 1, ref x, ref y, ref z)) return null;
            string seed = args.Length == 6 ? args[5] : "";
            
            if (!Formatter.ValidName(p, name, "level")) return null;
            if (LevelInfo.MapExists(name)) {
                p.Message("Level \"{0}\" already exists", name); return null;
            }
            if (gen.Type == GenType.Advanced && !CheckExtraPerm(p, data, 1)) return null;

            if (Interlocked.CompareExchange(ref p.GeneratingMap, 1, 0) == 1) {
                p.Message("You are already generating a map, please wait until that map has finished generating first.");
                return null;
            }
            
            Level lvl;
            try {
                p.Message("Generating map \"{0}\"..", name);
                lvl = new Level(name, x, y, z);
                if (!gen.Generate(p, lvl, seed)) { lvl.Dispose(); return null; }

                string format = seed.Length > 0 ? "{0}%S created level {1}%S with seed \"{2}\"" : "{0}%S created level {1}";
                string msg = string.Format(format, p.ColoredName, lvl.ColoredName, seed);
                Chat.MessageGlobal(msg);
            } finally {
                Interlocked.Exchange(ref p.GeneratingMap, 0);
                Server.DoGC();
            }
            return lvl;
        }
        
        
        internal static bool GetDimensions(Player p, string[] args, int i, ref ushort x, ref ushort y, ref ushort z) {
            return 
                CheckMapAxis(p, args[i    ], "Width",  ref x) &&
                CheckMapAxis(p, args[i + 1], "Height", ref y) &&
                CheckMapAxis(p, args[i + 2], "Length", ref z) &&
                CheckMapVolume(p, x, y, z);
        }
        
        static bool CheckMapAxis(Player p, string input, string type, ref ushort len) {
            if (!CommandParser.GetUShort(p, input, type, ref len)) return false;
            if (len == 0) { p.Message("%W{0} cannot be 0.", type); return false; }
            if (len > 16384) { p.Message("%W{0} must be 16384 or less.", type); return false; }
            
            if ((len % 16) != 0) {
                p.Message("%WMap {0} of {1} blocks is not divisible by 16!", type, len);
                p.Message("%WAs such, you may see rendering artifacts on some clients.");
            }
            return true;
        }
        
         static bool CheckMapVolume(Player p, int x, int y, int z) {
            if (p.IsConsole) return true;
            int limit = p.group.GenVolume;
            if ((long)x * y * z <= limit) return true;
            
            string text = "%WYou cannot create a map with over ";
            if (limit > 1000 * 1000) text += (limit / (1000 * 1000)) + " million blocks";
            else if (limit > 1000) text += (limit / 1000) + " thousand blocks";
            else text += limit + " blocks";
            p.Message(text);
            return false;
        }
        
        public override void Help(Player p) {
            p.Message("%T/NewLvl [name] [width] [height] [length] [theme] <seed>");
            p.Message("%HCreates/generates a new level.");
            p.Message("  %HSizes must be between 1 and 16384");
            p.Message("  %HSeed is optional, and controls how the level is generated");
            p.Message("%HUse %T/Help NewLvl themes %Hfor a list of themes.");
            p.Message("%HUse %T/Help NewLvl [theme] %Hfor details on how seeds affect levels generated with that theme.");
        }
        
        public override void Help(Player p, string message) {
            MapGen gen = MapGen.Find(message);
            
            if (message.CaselessEq("theme") || message.CaselessEq("themes")) {
                MapGen.PrintThemes(p);
            } else if (gen == null) {
                p.Message("No theme found with name \"{0}\".", message);
                p.Message("%HUse %T/Help NewLvl themes %Hfor a list of themes.");
            } else {
                p.Message(gen.Desc);
            }
        }
    }
}
