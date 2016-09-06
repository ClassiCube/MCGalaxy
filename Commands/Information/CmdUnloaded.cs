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

namespace MCGalaxy.Commands {
    
    public sealed class CmdUnloaded : Command {
        public override string name { get { return "unloaded"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdUnloaded() { }

        public override void Use(Player p, string message) {
            string[] files = Directory.GetFiles("levels", "*.lvl");
            Player.Message(p, "Unloaded maps (&c[no] %Sif not accessible): ");
            MultiPageOutput.Output(p, GetMaps(files), map => FormatMap(p, map), 
                                   "unloaded", "maps", message);
        }
        
        static List<string> GetMaps(string[] files) {
            List<string> maps = new List<string>(files.Length);
            Level[] loaded = LevelInfo.Loaded.Items;
            
            foreach (string file in files) {
                string map = Path.GetFileNameWithoutExtension(file);                
                if (IsLoaded(loaded, map)) continue;
                maps.Add(map);
            }
            return maps;
        }
        
        static string FormatMap(Player p, string map) {
            LevelPermission visitP, buildP;
            bool loadOnGoto;
            RetrieveProps(map, out visitP, out buildP, out loadOnGoto);
            
            string color = Group.findPerm(buildP).color;
            string visit = loadOnGoto && (p == null || p.Rank >= visitP) ? "" : " &c[no]" + color;
            return color + map + visit;
        }
        
        static bool IsLoaded(Level[] loaded, string level) {
            foreach (Level lvl in loaded) {
                if (lvl.name.CaselessEq(level)) return true;
            }
            return false;
        }
        
        static void RetrieveProps(string level, out LevelPermission visit,
                                  out LevelPermission build, out bool loadOnGoto) {
            visit = LevelPermission.Guest;
            build = LevelPermission.Guest;
            loadOnGoto = true;
            Group grp;
            
            string file = LevelInfo.FindPropertiesFile(level);
            if (file == null) return;
            SearchArgs args = new SearchArgs();
            PropertiesFile.Read(file, ref args, ProcessLine);
            
            grp = args.Visit == null ? null : Group.Find(args.Visit);
            if (grp != null) visit = grp.Permission;
            grp = args.Build == null ? null : Group.Find(args.Build);
            if (grp != null) build = grp.Permission;
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
            Player.Message(p, "%T/unloaded all %H- Lists all maps.");
            Player.Message(p, "%T/unloaded [1/2/3/..] %H- Shows only a few maps.");
            Player.Message(p, "%H Lists unloaded maps/levels, and their accessible state.");
        }
    }
}
