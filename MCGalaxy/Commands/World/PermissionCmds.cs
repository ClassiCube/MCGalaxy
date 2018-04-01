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
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        
        public static bool Do(Player p, string[] args, int offset, bool max, AccessController access) {
            for (int i = offset; i < args.Length; i++) {
                string arg = args[i];
                if (arg[0] == '+' || arg[0] == '-') {
                    if (!SetList(p, access, arg)) return false;
                } else if (max) {
                    Group grp = Matcher.FindRanks(p, arg);
                    if (grp == null) return false;
                    access.SetMax(p, grp);
                } else {
                    Group grp = Matcher.FindRanks(p, arg);
                    if (grp == null) return false;
                    access.SetMin(p, grp);
                }
            }
            return true;
        }
        
        protected void DoLevel(Player p, string message, bool visit) {
            const string maxPrefix = "-max ";
            bool max = message.CaselessStarts(maxPrefix);
            if (max) message = message.Substring(maxPrefix.Length);
            
            string[] args = message.SplitSpaces();
            if (message.Length == 0 || args.Length > 2) { Help(p); return; }
            if (args.Length == 1 && Player.IsSuper(p)) {
                Command.SuperRequiresArgs(name, p, "level"); return;
            }
            
            string map = args.Length == 1 ? p.level.name : Matcher.FindMaps(p, args[0]);
            if (map == null) return;           
            Level lvl;
            LevelConfig cfg = LevelInfo.GetConfig(map, out lvl);
            int offset = args.Length == 1 ? 0 : 1;
            
            AccessController access;
            if (lvl == null) {
                access = new LevelAccessController(cfg, map, visit);
            } else {
                access = visit ? lvl.VisitAccess : lvl.BuildAccess;
            }
            Do(p, args, offset, max, access);
        }
        
        static bool SetList(Player p, AccessController access, string name) {
            bool include = name[0] == '+';
            string mode = include ? "whitelist" : "blacklist";
            name = name.Substring(1);
            if (name.Length == 0) {
                Player.Message(p, "You must provide a player name to {0}.", mode); 
                return false;
            }
            
            if (!Formatter.ValidName(p, name, "player")) return false;
            name = PlayerInfo.FindMatchesPreferOnline(p, name);
            if (name == null) return false;
            
            if (p != null && name.CaselessEq(p.name)) {
                Player.Message(p, "You cannot {0} yourself.", mode); return false;
            }
            
            if (include) {
                access.Whitelist(p, name);
            } else {
                access.Blacklist(p, name);
            }
            return true;
        }      

        protected void ShowHelp(Player p, string action, string action2) {
            Player.Message(p, "%T/{0} [map] [rank]", name);
            Player.Message(p, "%HSets the lowest rank able to {0} the given map.", action);
            Player.Message(p, "%T/{0} -max [map] [Rank]", name);
            Player.Message(p, "%HSets the highest rank able to {0} the given map.", action);
            Player.Message(p, "%T/{0} [map] +[name]", name);
            Player.Message(p, "%HAllows [name] to {0}, even if their rank cannot.", action2);
            Player.Message(p, "%T/{0} [map] -[name]", name);
            Player.Message(p, "%HPrevents [name] from {0}ing, even if their rank can.", action2);
        }
    }
    
    public sealed class CmdPermissionBuild : PermissionCmd {        
        public override string name { get { return "PerBuild"; } }
        public override string shortcut { get { return "WBuild"; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("WorldBuild"), new CommandAlias("PerBuildMax", "-max") }; }
        }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ bypasses max build rank restriction") }; }
        }
        
        public override void Use(Player p, string message) { DoLevel(p, message, false); }    
        public override void Help(Player p) { ShowHelp(p, "build on", "build"); }
    }
    
    public sealed class CmdPermissionVisit : PermissionCmd {       
        public override string name { get { return "PerVisit"; } }
        public override string shortcut { get { return "WAccess"; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("WorldAccess"), new CommandAlias("PerVisitMax", "-max") }; }
        }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ bypasses max visit rank restriction") }; }
        }

        public override void Use(Player p, string message) { DoLevel(p, message, true); }        
        public override void Help(Player p) { ShowHelp(p, "visit", "visit"); }
    }    
}