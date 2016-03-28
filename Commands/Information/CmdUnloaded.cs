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
                    Player.SendMessage(p, "Unloaded levels [Accessible]: ");
                    Player.SendMessage(p, list.Remove(0, 2).ToString());
                    if (files.Length > 50) { Player.SendMessage(p, "For a more structured list, use /unloaded <1/2/3/..>"); }
                } else {
                    Player.SendMessage(p, "No maps are unloaded");
                }
            } else {
                if (end > files.Length) end = files.Length;
                if (start > files.Length) { Player.SendMessage(p, "No maps beyond number " + files.Length); return; }
                
                StringBuilder list = ListMaps(p, all, files, start, end);
                if (list.Length > 0) {
                    Player.SendMessage(p, "Unloaded levels [Accessible] (" + start + " to " + end + "):");
                    Player.SendMessage(p, list.Remove(0, 2).ToString());
                } else {
                    Player.SendMessage(p, "No maps are unloaded");
                }
            }
        }
        
        StringBuilder ListMaps(Player p, bool all, FileInfo[] files, int start, int end) {
            StringBuilder builder = new StringBuilder();
            Level[] loaded = LevelInfo.Loaded.Items;
            for (int i = start; i < end; i++) {
                string level = files[i].Name.Replace(".lvl", "");
                if (!all && loaded.Any(l => l.name.CaselessEq(level))) continue;
                
                string visit = GetLoadOnGoto(level) && (p == null || p.group.Permission >= GetPerVisitPermission(level)) ? "%aYes" : "%cNo";
                builder.Append(", ").Append(Group.findPerm(GetPerBuildPermission(level)).color + level + " &b[" + visit + "&b]");
            }
            return builder;
        }

        LevelPermission GetPerVisitPermission(string level) {
            string value = LevelInfo.FindOfflineProperty(level, "pervisit");
            if (value == null) return LevelPermission.Guest;
            Group grp = Group.Find(value);
            return grp == null ? LevelPermission.Guest : grp.Permission;
        }

        LevelPermission GetPerBuildPermission(string level) {
            string value = LevelInfo.FindOfflineProperty(level, "perbuild");
            if (value == null) return LevelPermission.Guest;
            Group grp = Group.Find(value);
            return grp == null ? LevelPermission.Guest : grp.Permission;
        }

        bool GetLoadOnGoto(string level) {
            string value = LevelInfo.FindOfflineProperty(level, "loadongoto");
            bool load;
            if (!bool.TryParse(value, out load)) return true;
            return load;
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "%f/unloaded %S- Lists all unloaded levels, and their accessible state.");
            Player.SendMessage(p, "%f/unloaded all %S- Lists all loaded and unloaded levels, and their accessible state.");
            Player.SendMessage(p, "%f/unloaded <1/2/3/..> %S- Shows a compact list.");
            Player.SendMessage(p, "%f/unloaded all <1/2/3/..> %S- Shows a compact list.");
        }
    }
}
