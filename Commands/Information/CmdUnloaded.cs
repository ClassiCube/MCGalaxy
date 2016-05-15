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
using System.Linq;
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
            bool all = false;
            int start = 0, end = 0;
            if (message.CaselessStarts("all")) { 
                all = true;
                int index = message.IndexOf(' ');
                message = message.Substring(index == -1 ? message.Length : (index + 1));
            }
            if (message != "") {
                if (!int.TryParse(message, out end) || end <= 0) { Help(p); return; }
                end *= 50;
                start = end - 50;
            }

            DirectoryInfo di = new DirectoryInfo("levels/");
            FileInfo[] files = di.GetFiles("*.lvl");
            if (end == 0) {
                StringBuilder list = ListMaps(p, all, files, 0, files.Length);
                if (list.Length > 0) {
                    Player.Message(p, "Unloaded levels [Accessible]: ");
                    Player.Message(p, list.Remove(0, 2).ToString());
                    if (files.Length > 50) { Player.Message(p, "For a more structured list, use /unloaded <1/2/3/..>"); }
                } else {
                    Player.Message(p, "No maps are unloaded");
                }
            } else {
                if (end > files.Length) end = files.Length;
                if (start > files.Length) { Player.Message(p, "No maps beyond number " + files.Length); return; }
                
                StringBuilder list = ListMaps(p, all, files, start, end);
                if (list.Length > 0) {
                    Player.Message(p, "Unloaded levels [Accessible] (" + start + " to " + end + "):");
                    Player.Message(p, list.Remove(0, 2).ToString());
                } else {
                    Player.Message(p, "No maps are unloaded");
                }
            }
        }
        
        static StringBuilder ListMaps(Player p, bool all, FileInfo[] files, int start, int end) {
            StringBuilder builder = new StringBuilder();
            Level[] loaded = LevelInfo.Loaded.Items;
            for (int i = start; i < end; i++) {
                string level = files[i].Name.Replace(".lvl", "");
                if (!all && loaded.Any(l => l.name.CaselessEq(level))) continue;
                
                LevelPermission visitP, buildP;
                bool loadOnGoto;
                RetrieveProps(level, out visitP, out buildP, out loadOnGoto);
                
                string visit = loadOnGoto && (p == null || p.group.Permission >= visitP) ? "%aYes" : "%cNo";
                builder.Append(", ").Append(Group.findPerm(buildP).color + level + " &b[" + visit + "&b]");
            }
            return builder;
        }
        
        static void RetrieveProps(string level, out LevelPermission visit,
                                  out LevelPermission build, out bool loadOnGoto) {
            visit = LevelPermission.Guest;
            build = LevelPermission.Guest;
            loadOnGoto = true;
            Group grp;
            
            string file = LevelInfo.GetPropertiesPath(level);
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
            Player.Message(p, "%f/unloaded %S- Lists all unloaded levels, and their accessible state.");
            Player.Message(p, "%f/unloaded all %S- Lists all loaded and unloaded levels, and their accessible state.");
            Player.Message(p, "%f/unloaded <1/2/3/..> %S- Shows a compact list.");
            Player.Message(p, "%f/unloaded all <1/2/3/..> %S- Shows a compact list.");
        }
    }
}
