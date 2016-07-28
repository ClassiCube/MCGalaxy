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

namespace MCGalaxy.Commands.World {   
    static class PermissionCmd {
        
        public static void Use(Player p, string[] args, bool skipNobodyPerm, string target,
                               Func<Level, LevelPermission> getter, Action<Level, LevelPermission> setter) {
            if (args.Length == 1 && p == null) {
                Player.Message(p, "You must provide a level name when using this command from console.");
                return;
            }
            Level level = args.Length == 1 ? p.level : LevelInfo.FindMatches(p, args[0]);
            if (level == null) return;
            
            string rank = args.Length == 1 ? args[0] : args[1];
            Group grp = Group.FindMatches(p, rank);
            if (grp == null) return;

            if (p != null && getter(level) > p.Rank) {
                if (skipNobodyPerm || (getter(level) != LevelPermission.Nobody)) {
                    Player.Message(p, "You cannot change the {0} of a level with a {0} higher than your rank.", target);
                    return;
                }
            }
            
            if (p != null && grp.Permission > p.Rank) {
                if (skipNobodyPerm || (grp.Permission != LevelPermission.Nobody)) {
                    Player.Message(p, "You cannot change the {0} of a level to a {0} higher than your rank.", target);
                    return;
                }
            }
            
            setter(level, grp.Permission);
            Level.SaveSettings(level);
            Server.s.Log(level.name + " " + target + " permission changed to " + grp.Permission + ".");
            Chat.GlobalMessageLevel(level, target + " permission changed to " + grp.ColoredName + "%S.");
            if (p == null || p.level != level)
                Player.Message(p, "{0} permission changed to {1}%S on {2}.", target, grp.ColoredName, level.name);
        }
        
        public static void UseList(Player p, string[] args, string target,
                                   Func<Level, LevelPermission> getter,
                                   Func<Level, List<string>> wlGetter, Func<Level, List<string>> blGetter) {
            if (args.Length == 1 && Player.IsSuper(p)) {
                Command.SuperRequiresArgs(target, p, "level"); return;
            }
            Level level = args.Length == 1 ? p.level : LevelInfo.FindMatches(p, args[0]);
            if (level == null) return;
            
            string name = args.Length == 1 ? args[0] : args[1];
            string mode = name[0] == '+' ? "whitelist" : "blacklist";
            List<string> list = name[0] == '+' ? wlGetter(level) : blGetter(level);
            List<string> other = name[0] == '+' ? blGetter(level) : wlGetter(level);
            name = name.Substring(1);            
            
            if (name == "") {
                Player.Message(p, "You must provide a player name to {0}.", mode); return;
            }
            if (p != null && name.CaselessEq(p.name)) {
                Player.Message(p, "You cannot {0} yourself.", mode); return;
            }
            
            if (p != null && getter(level) > p.Rank) {
                Player.Message(p, "You cannot change the {0} {1} permissions " +
                               "for a player higher than your rank.", target, mode); return;
            }
            if (p != null && blGetter(level).CaselessContains(p.name)) {
                Player.Message(p, "You cannot change {0} permissions as you are blacklisted.", target); return;
            }
            if (p != null && PlayerInfo.GetGroup(name).Permission > p.Rank) {
                Player.Message(p, "You cannot whitelist/blacklist players of a higher rank."); return;
            }
            
            if (list.CaselessContains(name)) {
                Player.Message(p, "\"{0}\" is already {1}ed.", name, mode); return;
            }
            list.Add(name);
            other.CaselessRemove(name);
            
            Level.SaveSettings(level);
            string msg = name + " was " + target + " " + mode + "ed";
            Server.s.Log(msg + " on " + level.name);
            Chat.GlobalMessageLevel(level, msg);
            if (p == null || p.level != level)
                Player.Message(p, msg + " on {0}.", level.name);
        }
    }
}