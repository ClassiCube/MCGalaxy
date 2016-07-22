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

namespace MCGalaxy {
    public static class Formatter {
        
        internal static void PrintRanks(LevelPermission minRank, List<LevelPermission> allowed,
                                        List<LevelPermission> disallowed, StringBuilder builder) {
            builder.Append(GetColoredRank(minRank) + "%S+");
            if (allowed != null && allowed.Count > 0) {
                foreach (LevelPermission perm in allowed)
                    builder.Append(", " + GetColoredRank(perm) + "%S");
            }
            
            if (disallowed != null && disallowed.Count > 0) {
                builder.Append( " (but not ");
                foreach (LevelPermission perm in disallowed)
                    builder.Append(GetColoredRank(perm) + "%S, ");
                builder.Remove(builder.Length - 2, 2);
                builder.Append(")");
            }
        }
        
        internal static void PrintCommandInfo(Player p, Command cmd) {
            var perms = GrpCommands.allowedCommands.Find(C => C.commandName == cmd.name);
            StringBuilder builder = new StringBuilder();
            builder.Append("Usable by: ");
            if (perms == null) {
                builder.Append(GetColoredRank(cmd.defaultRank) + "%S+");
            } else {
                PrintRanks(perms.lowestRank, perms.allow, perms.disallow, builder);
            }
            Player.Message(p, builder.ToString());
            
            PrintAliases(p, cmd);
            CommandPerm[] addPerms = cmd.ExtraPerms;
            if (addPerms == null) return;
            
            Player.Message(p, "%TExtra permissions:");
            for (int i = 0; i < addPerms.Length; i++) {
                var extra = CommandOtherPerms.Find(cmd, i + 1);
                LevelPermission perm = (LevelPermission)extra.Permission;
                Player.Message(p, "{0}) {1}%S{2}", i + 1, GetColoredRank(perm), extra.Description);
            }
        }
        
        static void PrintAliases(Player p, Command cmd) {
            StringBuilder dst = new StringBuilder("Shortcuts: ");
            if (!String.IsNullOrEmpty(cmd.shortcut)) {
                dst.Append('/').Append(cmd.shortcut).Append(", ");
            }
            FindAliases(Alias.coreAliases, cmd, dst);
            FindAliases(Alias.aliases, cmd, dst);
            
            if (dst.Length == "Shortcuts: ".Length) return;
            Player.Message(p, dst.ToString(0, dst.Length - 2));
        }
        
        static void FindAliases(List<Alias> aliases, Command cmd, StringBuilder dst) {
            foreach (Alias a in aliases) {
                if (!a.Target.CaselessEq(cmd.name)) continue;
                
                dst.Append('/').Append(a.Trigger);
                string args = a.Prefix == null ? a.Suffix : a.Prefix;
                if (args != null) {
                    string name = String.IsNullOrEmpty(cmd.shortcut)
                        ? cmd.name : cmd.shortcut;
                    dst.Append(" for /").Append(name + " " + args);
                }
                dst.Append(", ");
            }
        }
        
        static string GetColoredRank(LevelPermission perm) {
            Group grp = Group.findPerm(perm);
            if (grp != null) return grp.ColoredName;
            return "&f" + (int)perm;
        }
    }
}
