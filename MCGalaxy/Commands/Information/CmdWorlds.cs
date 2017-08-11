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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MCGalaxy.Commands.Info {
    public sealed class CmdWorlds : Command {
        public override string name { get { return "Worlds"; } }
        public override string shortcut { get { return "Unloaded"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }

        public override void Use(Player p, string message) {
            string[] files = LevelInfo.AllMapFiles();
            for (int i = 0; i < files.Length; i++) {
                files[i] = Path.GetFileNameWithoutExtension(files[i]);
            }
            
            Player.Message(p, "Maps (&c[no] %Sif not visitable): ");
            MultiPageOutput.Output(p, files, (map) => FormatMap(p, map),
                                   "Worlds", "maps", message, false);
        }
        
        static string FormatMap(Player p, string map) {
            LevelPermission visitP, buildP;
            bool loadOnGoto;
            RetrieveProps(map, out visitP, out buildP, out loadOnGoto);
            
            LevelPermission maxPerm = visitP;
            if (maxPerm < buildP) maxPerm = buildP;
            
            string visit = loadOnGoto && (p == null || p.Rank >= visitP) ? "" : " &c[no]";
            return Group.GetColor(maxPerm) + map + visit;
        }
        
        static void RetrieveProps(string level, out LevelPermission visit,
                                  out LevelPermission build, out bool loadOnGoto) {
            visit = LevelPermission.Guest;
            build = LevelPermission.Guest;
            loadOnGoto = true;
            
            string file = LevelInfo.FindPropertiesFile(level);
            if (file == null) return;
            SearchArgs args = new SearchArgs();
            PropertiesFile.Read(file, ref args, ProcessLine);
            
            visit = Group.ParsePermOrName(args.Visit, visit);
            build = Group.ParsePermOrName(args.Build, build);
            if (!bool.TryParse(args.LoadOnGoto, out loadOnGoto))
                loadOnGoto = true;
        }
        
        static void ProcessLine(string key, string value, ref SearchArgs args) {
            if (key.CaselessEq("pervisit")) {
                args.Visit = value;
            } else if (key.CaselessEq("perbuild")) {
                args.Build = value;
            } else if (key.CaselessEq("loadongoto")) {
                args.LoadOnGoto = value;
            }
        }
        
        struct SearchArgs { public string Visit, Build, LoadOnGoto; }

        public override void Help(Player p) {
            Player.Message(p, "%T/Worlds");
            Player.Message(p, "%HLists maps/levels, and their accessible state.");
        }
    }
}
