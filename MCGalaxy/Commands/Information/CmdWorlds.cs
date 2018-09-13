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
using System.IO;

namespace MCGalaxy.Commands.Info {
    public sealed class CmdWorlds : Command2 {
        public override string name { get { return "Worlds"; } }
        public override string shortcut { get { return "Unloaded"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool UseableWhenFrozen { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            string[] files = LevelInfo.AllMapFiles();
            p.Message("Maps (&c[no] %Sif not visitable): ");
            MultiPageOutput.Output(p, files, (file) => FormatMap(p, file),
                                   "Worlds", "maps", message, false);
        }
        
        static string FormatMap(Player p, string file) {
            LevelPermission visitP, buildP;
            bool loadOnGoto;
            string map = Path.GetFileNameWithoutExtension(file);
            RetrieveProps(map, out visitP, out buildP, out loadOnGoto);
            
            LevelPermission maxPerm = visitP;
            if (maxPerm < buildP) maxPerm = buildP;
            
            string visit = loadOnGoto && p.Rank >= visitP ? "" : " &c[no]";
            return Group.GetColor(maxPerm) + map + visit;
        }
        
        static void RetrieveProps(string level, out LevelPermission visit,
                                  out LevelPermission build, out bool loadOnGoto) {
            visit = LevelPermission.Guest;
            build = LevelPermission.Guest;
            loadOnGoto = true;
            
            string propsPath = LevelInfo.PropsPath(level);
            SearchArgs args = new SearchArgs();
            if (!PropertiesFile.Read(propsPath, ref args, ProcessLine)) return;
            
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
            p.Message("%T/Worlds");
            p.Message("%HLists maps/levels, and their accessible state.");
        }
    }
}
