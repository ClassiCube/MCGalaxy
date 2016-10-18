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
        
        
        /// <summary> Returns the allowed state for the given player. </summary>
        public LevelAccessResult Check(Player p, bool ignoreRankPerm = false) {
            if (Blacklisted.CaselessContains(p.name)) 
                return LevelAccessResult.Blacklisted;
            if (Whitelisted.CaselessContains(p.name))
                return LevelAccessResult.Whitelisted;
            if (ignoreRankPerm) 
                return LevelAccessResult.Allowed;
            
            if (p.Rank < Min) 
                return LevelAccessResult.BelowMinRank;
            string maxCmd = IsVisit ? "pervisitmax" : "perbuildmax";
            if (p.Rank > Max && !p.group.CanExecute(maxCmd)) 
                return LevelAccessResult.AboveMaxRank;
            return LevelAccessResult.Allowed;
        }
        
        /// <summary> Returns whether the given player is allowed for these access permissions. </summary>
        /// <remarks> If the player is not allowed by these access permissions,
        /// sends a message to the player describing why they are not. </remarks>
        public bool CheckDetailed(Player p, bool ignoreRankPerm = false) {
            LevelAccessResult result = Check(p, ignoreRankPerm);           
            if (result == LevelAccessResult.Allowed) return true;
            if (result == LevelAccessResult.Whitelisted) return true;
            
            if (result == LevelAccessResult.Blacklisted) {
                string action = IsVisit ? "going to" : "building in";
                Player.Message(p, "You are blacklisted from {1} {0}.", lvl.name, action);
            } else if (result == LevelAccessResult.BelowMinRank) {
                string action = IsVisit? "go to" : "build in";
                Player.Message(p, "Only {2}%S+ may {1} {0}.", 
                               lvl.name, action, Group.GetColoredName(Min));
            } else if (result == LevelAccessResult.AboveMaxRank) {
                string action = IsVisit? "go to" : "build in";
                Player.Message(p, "Only {2} %Sand below may {1} {0}.", 
                               lvl.name, action, Group.GetColoredName(Max));
            }
            return false;
        }
        
        
        /// <summary> Sets the minimum rank allowed to access these permissions. </summary>
        /// <returns> true if the minimum rank was changed, false if the given player
        /// had insufficient permission to change the minimum rank. </returns>
        public bool SetMin(Player p, Group grp) {
            string type = IsVisit ? "pervisit" : "perbuild";
            if (!CheckRank(p, Min, type, false)) return false;
            if (!CheckRank(p, grp.Permission, type, true)) return false;
            
            Min = grp.Permission;        
            OnPermissionChanged(p, grp, type);
            return true;
        }
        
        /// <summary> Sets the minimum rank allowed to access these permissions. </summary>
        /// <returns> true if the minimum rank was changed, false if the given player
        /// had insufficient permission to change the minimum rank. </returns>
        public bool SetMax(Player p, Group grp) {
            string type = IsVisit ? "pervisitmax" : "perbuildmax";
            const LevelPermission ignore = LevelPermission.Nobody;
            if (Max != ignore && !CheckRank(p, Max, type, false)) return false;
            if (grp.Permission != ignore && !CheckRank(p, grp.Permission, type, true)) return false;
            
            Max = grp.Permission;
            OnPermissionChanged(p, grp, type);
            return true;
        }
        
        /// <summary> Allows a player to access these permissions. </summary>
        /// <returns> true if the target is whitelisted, false if the given player
        /// had insufficient permission to whitelist the target. </returns>
        public bool Whitelist(Player p, string target) {
            if (!CheckList(p, target, true)) return false;
            if (Whitelisted.CaselessContains(target)) {
                Player.Message(p, "\"{0}\" is already whitelisted.", target); return true;
            }
            
            if (!Blacklisted.CaselessRemove(target)) 
                Whitelisted.Add(target);
            OnListChanged(p, target, true);
            return true;
        }
        
        /// <summary> Prevents a player from acessing these permissions. </summary>
        /// <returns> true if the target is blacklisted, false if the given player
        /// had insufficient permission to blacklist the target. </returns>
        public bool Blacklist(Player p, string target) {
            if (!CheckList(p, target, false)) return false;
            if (Blacklisted.CaselessContains(target)) {
                Player.Message(p, "\"{0}\" is already blacklisted.", target); return true;
            }
            
            if (!Whitelisted.CaselessRemove(target)) 
                Blacklisted.Add(target);
            OnListChanged(p, target, false);
            return true;
        }
        
        
        bool CheckRank(Player p, LevelPermission perm, string type, bool newPerm) {
            if (p != null && perm > p.Rank) {
                Player.Message(p, "You cannot change the {0} of a level {1} a {0} higher than your rank.", 
                               type, newPerm ? "to" : "with");
                return false;
            }
            return true;
        }
        
        bool CheckList(Player p, string name, bool whitelist) {
            string type = IsVisit ? "pervisit" : "perbuild";
            string mode = whitelist ? "whitelist" : "blacklist";
            
            if (p != null && !CheckDetailed(p)) {
                Player.Message(p, "Hence you cannot modify the {0} {1}.", type, mode); return false;
            }
            if (p != null && PlayerInfo.GetGroup(name).Permission > p.Rank) {
                Player.Message(p, "You cannot {0} players of a higher rank.", mode); return false;
            }
            return true;
        }
        
        void OnPermissionChanged(Player p, Group grp, string type) {
            Update();
            Server.s.Log(type + " permission changed to " + grp.trueName + " on " + lvl.name + ".");
            Chat.MessageLevel(lvl, type + " permission changed to " + grp.ColoredName + "%S.");
            if (p != null && p.level != lvl)
                Player.Message(p, "{0} permission changed to {1} %Son {2}.", type, grp.ColoredName, lvl.name);
        }
        
        void OnListChanged(Player p, string name, bool whitelist) {
            Update();
            string type = IsVisit ? "pervisit" : "perbuild";
            string msg = name + " was " + type + (whitelist ? " whitelisted" : " blacklisted");
            Server.s.Log(msg + " on " + lvl.name);
            Chat.MessageLevel(lvl, msg);
            if (p != null && p.level != lvl)
                Player.Message(p, "{0} on {1}", msg, lvl.name);
        }
        
        
        void Update() {
            Level.SaveSettings(lvl);
            UpdateAllowBuild();
            UpdateAllowVisit();
        }
        
        void UpdateAllowBuild() {
            if (IsVisit) return;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level != lvl) continue;
                
                LevelAccessResult access = Check(p, false);
                p.AllowBuild = access == LevelAccessResult.Whitelisted 
                    || access == LevelAccessResult.Allowed;
            }
        }
        
        void UpdateAllowVisit() {
            if (!IsVisit || lvl == Server.mainLevel) return;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level != lvl) continue;
                
                LevelAccessResult access = Check(p, false);
                bool allowVisit = access == LevelAccessResult.Whitelisted 
                    || access == LevelAccessResult.Allowed;
                if (allowVisit) continue;
                
                Player.Message(p, "&cNo longer allowed to visit %S{0}", lvl.name);
                PlayerActions.ChangeMap(p, Server.mainLevel, false);
            }
        }
    }
    
    public enum LevelAccessResult {
        
        /// <summary> The player is whitelisted and always allowed. </summary>
        Whitelisted,
        
        /// <summary> The player is blacklisted and never allowed. </summary>
        Blacklisted,
        
        /// <summary> The player is allowed (by their rank) </summary>
        Allowed,
        
        /// <summary> The player's rank is below the minimum rank allowed. </summary>
        BelowMinRank,
        
        /// <summary> The player's rank is above the maximum rank allowed. </summary>
        AboveMaxRank,
    }
}