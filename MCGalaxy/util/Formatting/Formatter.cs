/*
    Copyright 2015 MCGalaxy
    
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
using System.Text;
using MCGalaxy.Blocks;
using MCGalaxy.Commands;

namespace MCGalaxy {
    public static class Formatter {
        
        public static void PrintCommandInfo(Player p, Command cmd) {
            ItemPerms perms = CommandPerms.Find(cmd.name) ?? new ItemPerms(cmd.defaultRank);
            p.Message("Usable by: " + perms.Describe());
            PrintAliases(p, cmd);
            
            List<CommandExtraPerms> extraPerms = CommandExtraPerms.FindAll(cmd.name);
            if (cmd.ExtraPerms == null) extraPerms.Clear();
            if (extraPerms.Count == 0) return;
            
            p.Message("&TExtra permissions:");
            foreach (CommandExtraPerms extra in extraPerms) {
                p.Message("{0}) {1} {2}", extra.Num, extra.Describe(), extra.Desc);
            }
        }
        
        static void PrintAliases(Player p, Command cmd) {
            StringBuilder dst = new StringBuilder("Shortcuts: &T");
            if (!String.IsNullOrEmpty(cmd.shortcut)) {
                dst.Append('/').Append(cmd.shortcut).Append(", ");
            }
            FindAliases(Alias.coreAliases, cmd, dst);
            FindAliases(Alias.aliases, cmd, dst);
            
            if (dst.Length == "Shortcuts: &T".Length) return;
            p.Message(dst.ToString(0, dst.Length - 2));
        }
        
        static void FindAliases(List<Alias> aliases, Command cmd, StringBuilder dst) {
            foreach (Alias a in aliases) {
                if (!a.Target.CaselessEq(cmd.name)) continue;
                
                dst.Append('/').Append(a.Trigger);
                if (a.Format == null) { dst.Append(", "); continue; }
                
                string name = String.IsNullOrEmpty(cmd.shortcut) ? cmd.name : cmd.shortcut;
                if (name.Length > cmd.name.Length) name = cmd.name;
                string args = a.Format.Replace("{args}", "[args]");
                
                dst.Append(" for /").Append(name + " " + args);
                dst.Append(", ");
            }
        }
        
        public static void MessageNeedMinPerm(Player p, string action, LevelPermission perm) {
            p.Message("Only {0}&S{1}", Group.GetColoredName(perm), action);
        }
        
        public static bool ValidName(Player p, string name, string type) {
            if (Player.ValidName(name)) return true;
            p.Message("\"{0}\" is not a valid {1} name.", name, type);
            return false;
        }
		
        public static bool ValidMapName(Player p, string name) {
            if (LevelInfo.ValidName(name)) return true;
            p.Message("\"{0}\" is not a valid level name.", name);
            return false;
        }
    }
}
