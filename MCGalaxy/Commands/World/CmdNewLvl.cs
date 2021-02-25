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
            ushort x = 0, y = 0, z = 0;
            
            if (!MapGen.GetDimensions(p, args, 1, ref x, ref y, ref z)) return null;
            string seed = args.Length == 6 ? args[5] : "";
            
            if (gen != null && gen.Type == GenType.Advanced && !CheckExtraPerm(p, data, 1)) return null;
            return MapGen.Generate(p, gen, args[0], x, y, z, seed);
        }
        
        public override void Help(Player p) {
            p.Message("&T/NewLvl [name] [width] [height] [length] [theme] <seed>");
            p.Message("&HCreates/generates a new level.");
            p.Message("  &HSizes must be between 1 and 16384");
            p.Message("  &HSeed is optional, and controls how the level is generated");
            p.Message("&HUse &T/Help NewLvl themes &Hfor a list of themes.");
            p.Message("&HUse &T/Help NewLvl [theme] &Hfor details on how seeds affect levels generated with that theme.");
        }
        
        public override void Help(Player p, string message) {
            MapGen gen = MapGen.Find(message);
            
            if (message.CaselessEq("theme") || message.CaselessEq("themes")) {
                MapGen.PrintThemes(p);
            } else if (gen == null) {
                p.Message("No theme found with name \"{0}\".", message);
                p.Message("&HUse &T/Help NewLvl themes &Hfor a list of themes.");
            } else {
                p.Message(gen.Desc);
            }
        }
    }
}
