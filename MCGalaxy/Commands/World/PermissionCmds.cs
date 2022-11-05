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

namespace MCGalaxy.Commands.World
{
    public abstract class PermissionCmd : Command2
    {
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        
        public static bool Do(Player p, string[] args, int offset, bool max,
                              AccessController access, CommandData data, Level lvl) {
            for (int i = offset; i < args.Length; i++) {
                string arg = args[i];
                if (arg[0] == '+' || arg[0] == '-') {
                    if (!SetList(p, arg, access, data, lvl)) return false;
                } else if (max) {
                    Group grp = Matcher.FindRanks(p, arg);
                    if (grp == null) return false;
                    access.SetMax(p, data.Rank, lvl, grp);
                } else {
                    Group grp = Matcher.FindRanks(p, arg);
                    if (grp == null) return false;
                    access.SetMin(p, data.Rank, lvl, grp);
                }
            }
            return true;
        }

        static bool SetList(Player p, string name,
                            AccessController access, CommandData data, Level lvl) {
            bool include = name[0] == '+';
            string mode = include ? "whitelist" : "blacklist";
            name = name.Substring(1);
            if (name.Length == 0) {
                p.Message("You must provide a player name to {0}.", mode);
                return false;
            }
            
            name = PlayerInfo.FindMatchesPreferOnline(p, name);
            if (name == null) return false;
            
            if (!include && name.CaselessEq(p.name)) {
                p.Message("&WYou cannot blacklist yourself."); return false;
            }
            
            if (include) {
                access.Whitelist(p, data.Rank, lvl, name);
            } else {
                access.Blacklist(p, data.Rank, lvl, name);
            }
            return true;
        }
    }
    
    public abstract class LevelPermissionCmd : PermissionCmd
    {
        public abstract bool IsVisit { get; }
        
        public override void Use(Player p, string message, CommandData data) {
            const string maxPrefix = "-max ";
            bool max = message.CaselessStarts(maxPrefix);
            if (max) message = message.Substring(maxPrefix.Length);
            
            string[] args = message.SplitSpaces();
            if (message.Length == 0 || args.Length > 2) { Help(p); return; }
            
            if (args.Length == 1) {
                // special case /perbuild [permission] to current level
                if (p.IsSuper) {
                    SuperRequiresArgs(p, "level name");
                } else {
                    UpdatePerms(p, p.level.name, data, args, max);
                }
                return;
            }
            
            foreach (string name in args[0].SplitComma())
            {
                string map = Matcher.FindMaps(p, name);
                if (map == null) continue;
                
                UpdatePerms(p, map, data, args, max);
            }
        }
        
        void UpdatePerms(Player p, string map, CommandData data, string[] args, bool max) {
            Level lvl;
            LevelConfig cfg = LevelInfo.GetConfig(map, out lvl);
            int offset = args.Length == 1 ? 0 : 1;
            
            AccessController access;
            if (lvl == null) {
                access = new LevelAccessController(cfg, map, IsVisit);
            } else {
                access = IsVisit ? lvl.VisitAccess : lvl.BuildAccess;
            }
            Do(p, args, offset, max, access, data, lvl);
        }

        public override void Help(Player p) {
        	string action = IsVisit ? "visit" : "build on";
        	string verb   = IsVisit ? "visit" : "build";

            p.Message("&T/{0} [level] [rank]", name);
            p.Message("&HSets the lowest rank able to {0} the given level.", action);
            p.Message("&T/{0} -max [level] [Rank]", name);
            p.Message("&HSets the highest rank able to {0} the given level.", action);
            p.Message("&T/{0} [level] +[name]", name);
            p.Message("&HAllows [name] to {0}, even if their rank cannot.", verb);
            p.Message("&T/{0} [level] -[name]", name);
            p.Message("&HPrevents [name] from {0}ing, even if their rank can.", verb);
        }
    }
    
    public sealed class CmdPermissionBuild : LevelPermissionCmd
    {
        public override string name { get { return "PerBuild"; } }
        public override string shortcut { get { return "WBuild"; } }
        public override bool IsVisit { get { return false; } }
        
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("WorldBuild"), new CommandAlias("PerBuildMax", "-max") }; }
        }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "bypass max build rank restriction") }; }
        }
    }
    
    public sealed class CmdPermissionVisit : LevelPermissionCmd
    {
        public override string name { get { return "PerVisit"; } }
        public override string shortcut { get { return "WAccess"; } }
        public override bool IsVisit { get { return true; } }
        
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("WorldAccess"), new CommandAlias("PerVisitMax", "-max") }; }
        }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "bypass max visit rank restriction") }; }
        }
    }
}