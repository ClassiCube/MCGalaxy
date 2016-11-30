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
    public sealed class LevelAccessController {
        
        /// <summary> Whether these access permissions apply to
        /// visit (true) or build (false) permission for the level. </summary>
        public readonly bool IsVisit;
        readonly Level lvl;

        public LevelAccessController(Level lvl, bool isVisit) {
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
        public LevelAccess Check(Player p) {
            if (Blacklisted.CaselessContains(p.name)) 
                return LevelAccess.Blacklisted;
            if (Whitelisted.CaselessContains(p.name))
                return LevelAccess.Whitelisted;
            
            if (p.Rank < Min) 
                return LevelAccess.BelowMinRank;
            string maxCmd = IsVisit ? "pervisitmax" : "perbuildmax";
            if (p.Rank > Max && !p.group.CanExecute(maxCmd)) 
                return LevelAccess.AboveMaxRank;
            return LevelAccess.Allowed;
        }
        
        /// <summary> Returns whether the given player is allowed for these access permissions. </summary>
        /// <remarks> If the player is not allowed by these access permissions,
        /// sends a message to the player describing why they are not. </remarks>
        public bool CheckDetailed(Player p, bool ignoreRankPerm = false) {
            LevelAccess result = Check(p);
            if (result == LevelAccess.Allowed) return true;
            if (result == LevelAccess.Whitelisted) return true;
            if (result == LevelAccess.AboveMaxRank && ignoreRankPerm) return true;
            if (result == LevelAccess.BelowMinRank && ignoreRankPerm) return true;
            
            if (result == LevelAccess.Blacklisted) {
                string action = IsVisit ? "going to" : "building in";
                Player.Message(p, "You are blacklisted from {1} {0}%S.", lvl.ColoredName, action);
            } else if (result == LevelAccess.BelowMinRank) {
                string action = IsVisit? "go to" : "build in";
                Player.Message(p, "Only {2}%S+ may {1} {0}%S.", 
                               lvl.ColoredName, action, Group.GetColoredName(Min));
            } else if (result == LevelAccess.AboveMaxRank) {
                string action = IsVisit? "go to" : "build in";
                Player.Message(p, "Only {2} %Sand below may {1} {0}%S.", 
                               lvl.ColoredName, action, Group.GetColoredName(Max));
            }
            return false;
        }
        
        
        /// <summary> Sets the minimum rank allowed to access these permissions. </summary>
        /// <returns> true if the minimum rank was changed, false if the given player
        /// had insufficient permission to change the minimum rank. </returns>
        public bool SetMin(Player p, Group grp) {
            string type = IsVisit ? "Min visit" : "Min build";
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
            string type = IsVisit ? "Max visit" : "Max build";
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
            
            bool removed = true;
            if (!Blacklisted.CaselessRemove(target)) {
                Whitelisted.Add(target);
                removed = false;
            }
            OnListChanged(p, target, true, removed);
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
            
            bool removed = true;
            if (!Whitelisted.CaselessRemove(target)) {
                Blacklisted.Add(target);
                removed = false;
            }
            OnListChanged(p, target, false, removed);
            return true;
        }
        
        
        bool CheckRank(Player p, LevelPermission perm, string type, bool newPerm) {
            if (p != null && perm > p.Rank) {
                Player.Message(p, "You cannot change the {0} rank of this level{1} higher than yours.", 
                               type.ToLower(), 
                               newPerm ? " to a rank" : ", as its current " + type.ToLower() + " rank is");
                return false;
            }
            return true;
        }
        
        bool CheckList(Player p, string name, bool whitelist) {
            string type = IsVisit ? "visit" : "build";
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
            Server.s.Log(type + " rank changed to " + grp.trueName + " on " + lvl.name + ".");
            Chat.MessageLevel(lvl, type + " rank changed to " + grp.ColoredName + "%S.");
            if (p != null && p.level != lvl)
                Player.Message(p, "{0} rank changed to {1} %Son {2}%S.", type, grp.ColoredName, lvl.ColoredName);
        }
        
        void OnListChanged(Player p, string name, bool whitelist, bool removed) {          
            string type = IsVisit ? "visit" : "build";
            string msg = PlayerInfo.GetColoredName(p, name);
            if (removed) {
                msg += " %Swas removed from the " + type + (whitelist ? " blacklist" : " whitelist");
            } else {
                msg += " %Swas " + type + (whitelist ? " whitelisted" : " blacklisted");
            }
            
            Update();
            Server.s.Log(msg + " on " + lvl.name);
            Chat.MessageLevel(lvl, msg);
            if (p != null && p.level != lvl)
                Player.Message(p, "{0} on %S{1}", msg, lvl.ColoredName);
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
                
                LevelAccess access = Check(p);
                p.AllowBuild = access == LevelAccess.Whitelisted || access == LevelAccess.Allowed;
            }
        }
        
        void UpdateAllowVisit() {
            if (!IsVisit || lvl == Server.mainLevel) return;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level != lvl) continue;
                
                LevelAccess access = Check(p);
                bool allowVisit = access == LevelAccess.Whitelisted || access == LevelAccess.Allowed;
                if (allowVisit) continue;
                
                Player.Message(p, "&cNo longer allowed to visit %S{0}", lvl.ColoredName);
                PlayerActions.ChangeMap(p, Server.mainLevel, false);
            }
        }
    }
    
    public enum LevelAccess {
        
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