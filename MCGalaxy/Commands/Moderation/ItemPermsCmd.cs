/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.Collections.Generic;

namespace MCGalaxy.Commands.Moderation {
    public abstract class ItemPermsCmd : Command {
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        
        protected void SetPerms(Player p, string[] args, ItemPerms perms, string type) {
            string grpName = args[1];          
            if (p != null && !perms.UsableBy(p.Rank)) {
                Player.Message(p, "You rank cannot use this {0}.", type); return;
            }
            
            if (grpName[0] == '+') {
                Group grp = GetGroup(p, grpName.Substring(1));
                if (grp == null) return;

                Allow(perms, grp.Permission);
                UpdatePerms(perms, p, " %Scan now be used by " + grp.ColoredName);
            } else if (grpName[0] == '-') {
                Group grp = GetGroup(p, grpName.Substring(1));
                if (grp == null) return;

                if (p != null && p.Rank == grp.Permission) {
                    Player.Message(p, "You cannot disallow your own rank from using a {0}.", type); return;
                }
                
                Disallow(perms, grp.Permission);
                UpdatePerms(perms, p, " %Sis no longer usable by " + grp.ColoredName);
            } else {
                Group grp = GetGroup(p, grpName);
                if (grp == null) return;

                perms.MinRank = grp.Permission;
                UpdatePerms(perms, p, " %Sis now usable by " + grp.ColoredName + "%S+");
            }
        }
        
        protected abstract void UpdatePerms(ItemPerms perms, Player p, string msg);
        
        static void Allow(ItemPerms perms, LevelPermission rank) {
            if (perms.Disallowed != null && perms.Disallowed.Contains(rank)) {
                perms.Disallowed.Remove(rank);
            } else if (perms.Allowed == null || !perms.Allowed.Contains(rank)) {
                if (perms.Allowed == null) perms.Allowed = new List<LevelPermission>();
                perms.Allowed.Add(rank);
            }
        }
        
        static void Disallow(ItemPerms perms, LevelPermission rank) {
            if (perms.Allowed != null && perms.Allowed.Contains(rank)) {
                perms.Allowed.Remove(rank);
            } else if (perms.Disallowed == null || !perms.Disallowed.Contains(rank)) {
                if (perms.Disallowed == null) perms.Disallowed = new List<LevelPermission>();
                perms.Disallowed.Add(rank);
            }
        }
        
        protected static Group GetGroup(Player p, string grpName) {
            Group grp = Matcher.FindRanks(p, grpName);
            if (grp == null) return null;
            
            if (p != null && grp.Permission > p.Rank) {
                Player.Message(p, "Cannot set permissions to a rank higher than yours."); return null;
            }
            return grp;
        }
        
        protected static void Announce(Player p, string msg) {
            Chat.MessageAll("&d" + msg);
            if (Player.IsSuper(p)) Player.Message(p, msg);
        }
    }
}
