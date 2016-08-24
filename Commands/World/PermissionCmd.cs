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
    public abstract class PermissionCmd : Command {
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        
        protected Level GetArgs(Player p, string[] args, ref Group grp) {
            if (args.Length == 1 && Player.IsSuper(p)) {
                SuperRequiresArgs(p, "level"); return null;
            }
            Level level = args.Length == 1 ? p.level : LevelInfo.FindMatches(p, args[0]);
            if (level == null) return null;
            
            string rank = args.Length == 1 ? args[0] : args[1];
            grp = Group.FindMatches(p, rank);
            return grp != null ? level : null;
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
            
            UpdateAllowBuild(level);
            Level.SaveSettings(level);
            
            string msg = name + " was " + target + " " + mode + "ed";
            Server.s.Log(msg + " on " + level.name);
            Chat.MessageLevel(level, msg);
            if (p == null || p.level != level)
                Player.Message(p, msg + " on {0}.", level.name);
        }
        
        static void UpdateAllowBuild(Level level) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level != level) continue;
                p.AllowBuild = level.BuildAccess.Check(p, false);
            }
        }
    }
}