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
        
        public static void PrintRanks(LevelPermission minRank, List<LevelPermission> allowed,
                                      List<LevelPermission> disallowed, StringBuilder builder) {
            builder.Append(Group.GetColoredName(minRank) + "%S+");
            if (allowed != null && allowed.Count > 0) {
                foreach (LevelPermission perm in allowed)
                    builder.Append(", " + Group.GetColoredName(perm));
                builder.Append("%S");
            }
            
            if (disallowed != null && disallowed.Count > 0) {
                builder.Append( " (except ");
                foreach (LevelPermission perm in disallowed)
                    builder.Append(Group.GetColoredName(perm) + ", ");
                builder.Remove(builder.Length - 2, 2);
                builder.Append("%S)");
            }
        }
        
        public static void PrintCommandInfo(Player p, Command cmd) {
            CommandPerms perms = CommandPerms.Find(cmd.name);
            StringBuilder builder = new StringBuilder();
            builder.Append("Usable by: ");
            if (perms == null) {
                builder.Append(Group.GetColoredName(cmd.defaultRank) + "%S+");
            } else {
                PrintRanks(perms.MinRank, perms.Allowed, perms.Disallowed, builder);
            }
            Player.Message(p, builder.ToString());            
            PrintAliases(p, cmd);
            
            List<CommandExtraPerms> extraPerms = CommandExtraPerms.FindAll(cmd.name);
            if (cmd.ExtraPerms == null) extraPerms.Clear();
            if (extraPerms.Count == 0) return;
            
            Player.Message(p, "%TExtra permissions:");
            foreach (CommandExtraPerms extra in extraPerms) {
                Player.Message(p, "{0}) {1}%S{2}", extra.Number, 
                               Group.GetColoredName(extra.MinRank), extra.Description);
            }
        }
        
        static void PrintAliases(Player p, Command cmd) {
            StringBuilder dst = new StringBuilder("Shortcuts: %T");
            if (!String.IsNullOrEmpty(cmd.shortcut)) {
                dst.Append('/').Append(cmd.shortcut).Append(", ");
            }
            FindAliases(Alias.coreAliases, cmd, dst);
            FindAliases(Alias.aliases, cmd, dst);
            
            if (dst.Length == "Shortcuts: %T".Length) return;
            Player.Message(p, dst.ToString(0, dst.Length - 2));
        }
        
        static void FindAliases(List<Alias> aliases, Command cmd, StringBuilder dst) {
            foreach (Alias a in aliases) {
                if (!a.Target.CaselessEq(cmd.name)) continue;
                
                dst.Append('/').Append(a.Trigger);
                string args = a.Prefix == null ? a.Suffix : a.Prefix;
                if (args == null) { dst.Append(", "); continue; }
                
                string name = String.IsNullOrEmpty(cmd.shortcut) ? cmd.name : cmd.shortcut;
                if (name.Length > cmd.name.Length) name = cmd.name;
                dst.Append(" for /").Append(name + " " + args);
                dst.Append(", ");
            }
        }
        
        public static void MessageNeedMinPerm(Player p, string action, LevelPermission perm) {
            Player.Message(p, "Only {0}%S{1}", Group.GetColoredName(perm), action);
        }
        
        public static bool ValidName(Player p, string name, string type) {
            if (Player.ValidName(name)) return true;
            Player.Message(p, "\"{0}\" is not a valid {1} name.", name, type);
            return false;
        }
    }
}
