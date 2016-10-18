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
        
        protected void UseList(Player p, string[] args, bool isVisit) {
            string target = isVisit ? "pervisit" : "perbuild";
            if (args.Length == 1 && Player.IsSuper(p)) {
                Command.SuperRequiresArgs(target, p, "level"); return;
            }
            Level level = args.Length == 1 ? p.level : LevelInfo.FindMatches(p, args[0]);
            if (level == null) return;
            LevelAccess access = isVisit ? level.VisitAccess : level.BuildAccess;
            
            string name = args.Length == 1 ? args[0] : args[1];
            bool include = name[0] == '+';
            string mode = include ? "whitelist" : "blacklist";
            name = name.Substring(1);
            
            if (name == "") {
                Player.Message(p, "You must provide a player name to {0}.", mode); return;
            }
            if (p != null && name.CaselessEq(p.name)) {
                Player.Message(p, "You cannot {0} yourself.", mode); return;
            }
            
            if (include) {
                access.Whitelist(p, name);
            } else {
                access.Blacklist(p, name);
            }
        }
        
        protected void MaxHelp(Player p, string action) {
            Player.Message(p, "%T/{0} [Level] [Rank]", name);
            Player.Message(p, "%HSets the highest rank able to {0} the given level.", action);
        }
        
        protected void NormalHelp(Player p, string action, string action2) {
            Player.Message(p, "%T/{0} [level] [rank]", name);
            Player.Message(p, "%HSets the lowest rank able to {0} the given level.", action);
            Player.Message(p, "%T/{0} [level] +[name]", name);
            Player.Message(p, "%HAllows [name] to {0}, even if their rank cannot.", action2);
            Player.Message(p, "%T/{0} [level] -[name]", name);
            Player.Message(p, "%HPrevents [name] from {0}ing, even if their rank can.", action2);
        }
    }
}