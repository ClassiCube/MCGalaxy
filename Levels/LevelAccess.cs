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

namespace MCGalaxy {

    /// <summary> Encapuslates access permissions (visit or build) for a level. </summary>
    public sealed class LevelAccess {
        
        /// <summary> Whether these access permissions apply to
        /// visit (true) or build (false) permission for the level. </summary>
        public readonly bool IsVisit;
        readonly Level lvl;

        public LevelAccess(Level lvl, bool isVisit) {
            this.lvl = lvl;
            IsVisit = isVisit;
        }
        
        /// <summary> Lowest allowed rank. </summary>
        public LevelPermission Min {
            get { return IsVisit ? lvl.permissionvisit : lvl.permissionbuild; }
            set {
                if (IsVisit) lvl.permissionvisit = value;
                else lvl.permissionbuild = value;
            }
        }
        
        /// <summary> Highest allowed rank. </summary>
        public LevelPermission Max {
            get { return IsVisit ? lvl.pervisitmax : lvl.perbuildmax; }
            set {
                if (IsVisit) lvl.pervisitmax = value;
                else lvl.perbuildmax = value;
            }
        }
        
        /// <summary> List of always allowed players, overrides rank allowances. </summary>
        public List<string> Whitelisted {
            get { return IsVisit ? lvl.VisitWhitelist : lvl.BuildWhitelist; }
        }
        
        /// <summary> List of never allowed players, ignores rank allowances. </summary>
        public List<string> Blacklisted {
            get { return IsVisit ? lvl.VisitBlacklist : lvl.BuildBlacklist; }
        }
        
        
        /// <summary> Returns whether the given player is allowed by these access permissions. </summary>
        public bool Check(Player p, bool ignoreRankPerm = false) {
            if (Blacklisted.CaselessContains(p.name)) return false;
            if (Whitelisted.CaselessContains(p.name) || ignoreRankPerm) return true;
            
            if (p.Rank < Min) return false;
            string maxCmd = IsVisit ? "pervisitmax" : "perbuildmax";
            if (p.Rank > Max && !p.group.CanExecute(maxCmd)) return false;
            return true;
        }
        
        /// <summary> Returns whether the given player is allowed for these access permissions. </summary>
        /// <remarks> If the player is not allowed by these access permissions,
        /// sends a message to the player describing why they are not. </remarks>
        public bool CheckDetailed(Player p, bool ignoreRankPerm = false) {
            string name = lvl.name;
            string action = IsVisit ? "going to" : "building in";
            if (Blacklisted.CaselessContains(p.name)) {
                Player.Message(p, "You are blacklisted from {1} {0}.", name, action); return false;
            }
            if (Whitelisted.CaselessContains(p.name) || ignoreRankPerm) return true;
            
            action = IsVisit? "go to" : "build in";
            if (p.Rank < Min) {
                Group grp = Group.findPerm(Min);
                string grpName = grp == null ? "&f" + Min : grp.ColoredName;
                Player.Message(p, "Only {2}%S+ may {1} {0}.", name, action, grpName); return false;
            }
            
            string maxCmd = IsVisit ? "pervisitmax" : "perbuildmax";
            if (p.Rank > Max && !p.group.CanExecute(maxCmd)) {
                Group grp = Group.findPerm(Max);
                string grpName = grp == null ? "&f" + Max : grp.ColoredName;
                Player.Message(p, "Only {2}%S and below may {1} {0}.", name, action, grpName); return false;
            }
            return true;
        }
        
        
        /// <summary> Sets the minimum rank allowed to access these permissions. </summary>
        /// <returns> true if the minimum rank was changed, false if the given player
        /// had insufficient permission to change the minimum rank. </returns>
        public bool SetMin(Player p, Group grp) {
            string target = IsVisit ? "pervisit" : "perbuild";
            if (!CheckRank(p, grp, target)) return false;            
            Min = grp.Permission;
            
            UpdateAllowBuild();
            OnPermissionChanged(p, grp, target);
            return true;
        }
        
        /// <summary> Sets the minimum rank allowed to access these permissions. </summary>
        /// <returns> true if the minimum rank was changed, false if the given player
        /// had insufficient permission to change the minimum rank. </returns>
        public bool SetMax(Player p, Group grp) {
            string target = IsVisit ? "pervisitmax" : "perbuildmax";
            if (grp.Permission != LevelPermission.Nobody && !CheckRank(p, grp, target)) return false;           
            Max = grp.Permission;
            
            UpdateAllowBuild();
            OnPermissionChanged(p, grp, target);
            return true;
        }
        
        
        bool CheckRank(Player p, Group grp, string target) {
            if (p != null && Min > p.Rank) {
                Player.Message(p, "You cannot change the {0} of a level with a {0} higher than your rank.", target);
                return false;
            }
            if (p != null && grp.Permission > p.Rank) {
                Player.Message(p, "You cannot change the {0} of a level to a {0} higher than your rank.", target);
                return false;
            }
            return true;
        }
        
        void OnPermissionChanged(Player p, Group grp, string target) {
            Level.SaveSettings(lvl);
            Server.s.Log(lvl.name + " " + target + " permission changed to " + grp.Permission + ".");
            Chat.MessageLevel(lvl, target + " permission changed to " + grp.ColoredName + "%S.");
            if (p == null || p.level != lvl)
                Player.Message(p, "{0} permission changed to {1}%S on {2}.", target, grp.ColoredName, lvl.name);
        }
        
        void UpdateAllowBuild() {
            if (IsVisit) return;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level != lvl) continue;
                p.AllowBuild = lvl.BuildAccess.Check(p, false);
            }
        }
    }
}