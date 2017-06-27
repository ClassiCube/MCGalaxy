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

    
    /// <summary> Encapuslates access permissions (visit or build) for a level/zone. </summary>
    public abstract class AccessController {
        
        /// <summary> Lowest allowed rank. </summary>
        public abstract LevelPermission Min { get; set; }
        
        /// <summary> Highest allowed rank. </summary>
        public abstract LevelPermission Max { get; set; }
        
        /// <summary> List of always allowed players, overrides rank allowances. </summary>
        public abstract List<string> Whitelisted { get; }
        
        /// <summary> List of never allowed players, ignores rank allowances. </summary>
        public abstract List<string> Blacklisted { get; }
        
        /// <summary> Returns the formatted name for the level/zone containing these access permissions. </summary>
        protected abstract string ColoredName { get; }
        protected abstract string Action { get; }
        protected abstract string ActionIng { get; }
        protected abstract string Type { get; }
        protected abstract string MaxCmd { get; }
        
        
        /// <summary> Returns the allowed state for the given player. </summary>
        public AccessResult Check(Player p) {
            if (Blacklisted.CaselessContains(p.name))
                return AccessResult.Blacklisted;
            if (Whitelisted.CaselessContains(p.name))
                return AccessResult.Whitelisted;
            
            if (p.Rank < Min) return AccessResult.BelowMinRank;
            if (p.Rank > Max && MaxCmd != null && !p.group.CanExecute(MaxCmd))
                return AccessResult.AboveMaxRank;
            return AccessResult.Allowed;
        }
        
        /// <summary> Returns whether the given player is allowed for these access permissions. </summary>
        /// <remarks> If the player is not allowed by these access permissions,
        /// sends a message to the player describing why they are not. </remarks>
        public bool CheckDetailed(Player p, bool ignoreRankPerm = false) {
            AccessResult result = Check(p);
            if (result == AccessResult.Allowed) return true;
            if (result == AccessResult.Whitelisted) return true;
            if (result == AccessResult.AboveMaxRank && ignoreRankPerm) return true;
            if (result == AccessResult.BelowMinRank && ignoreRankPerm) return true;
            
            if (result == AccessResult.Blacklisted) {
                Player.Message(p, "You are blacklisted from {1} {0}%S.", ColoredName, ActionIng);
            } else if (result == AccessResult.BelowMinRank) {
                Player.Message(p, "Only {2}%S+ may {1} {0}%S.",
                               ColoredName, Action, Group.GetColoredName(Min));
            } else if (result == AccessResult.AboveMaxRank) {
                Player.Message(p, "Only {2} %Sand below may {1} {0}%S.",
                               ColoredName, Action, Group.GetColoredName(Max));
            }
            return false;
        }
        
        
        /// <summary> Sets the minimum rank allowed to access these permissions. </summary>
        /// <returns> true if the minimum rank was changed, false if the given player
        /// had insufficient permission to change the minimum rank. </returns>
        public bool SetMin(Player p, Group grp) {
            string minType = "Min " + Type;
            if (!CheckRank(p, Min, minType, false)) return false;
            if (!CheckRank(p, grp.Permission, minType, true)) return false;
            
            Min = grp.Permission;
            OnPermissionChanged(p, grp, minType);
            return true;
        }
        
        /// <summary> Sets the minimum rank allowed to access these permissions. </summary>
        /// <returns> true if the minimum rank was changed, false if the given player
        /// had insufficient permission to change the minimum rank. </returns>
        public bool SetMax(Player p, Group grp) {
            string maxType = "Max " + Type;
            const LevelPermission ignore = LevelPermission.Nobody;
            if (Max != ignore && !CheckRank(p, Max, maxType, false)) return false;
            if (grp.Permission != ignore && !CheckRank(p, grp.Permission, maxType, true)) return false;
            
            Max = grp.Permission;
            OnPermissionChanged(p, grp, maxType);
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


        /// <summary> Called when min or max rank is changed. </summary>
        public abstract void OnPermissionChanged(Player p, Group grp, string type);
        
        /// <summary> Called when a whitelist or blacklist is changed. </summary>
        public abstract void OnListChanged(Player p, string name, bool whitelist, bool removedFromOpposite);
        
        bool CheckRank(Player p, LevelPermission perm, string type, bool newPerm) {
            if (p != null && perm > p.Rank) {
                Player.Message(p, "You cannot change the {0} rank of this level{1} higher than yours.",
                               type.ToLower(),
                               newPerm ? " to a rank" : ", as its current " + type.ToLower() + " rank is");
                return false;
            }
            return true;
        }
        
        /// <summary> Returns true if the player is allowed to modify these access permissions,
        /// and is also allowed to change the access permissions for the target player. </summary>
        bool CheckList(Player p, string name, bool whitelist) {
            string mode = whitelist ? "whitelist" : "blacklist";
            
            if (p != null && !CheckDetailed(p)) {
                Player.Message(p, "Hence you cannot modify the {0} {1}.", Type, mode); return false;
            }
            if (p != null && PlayerInfo.GetGroup(name).Permission > p.Rank) {
                Player.Message(p, "You cannot {0} players of a higher rank.", mode); return false;
            }
            return true;
        }
    }
    
    /// <summary> Encapuslates access permissions (visit or build) for a level. </summary>
    public sealed class LevelAccessController : AccessController {
        
        /// <summary> Whether these access permissions apply to
        /// visit (true) or build (false) permission for the level. </summary>
        public readonly bool IsVisit;
        readonly Level lvl;
        
        public LevelAccessController(Level lvl, bool isVisit) {
            this.lvl = lvl;
            IsVisit = isVisit;
        }
        
        /// <summary> Lowest allowed rank. </summary>
        public override LevelPermission Min {
            get { return IsVisit ? lvl.Config.VisitMin : lvl.Config.BuildMin; }
            set {
                if (IsVisit) lvl.Config.VisitMin = value;
                else lvl.Config.BuildMin = value;
            }
        }
        
        /// <summary> Highest allowed rank. </summary>
        public override LevelPermission Max {
            get { return IsVisit ? lvl.Config.VisitMax : lvl.Config.BuildMax; }
            set {
                if (IsVisit) lvl.Config.VisitMax = value;
                else lvl.Config.BuildMax = value;
            }
        }
        
        /// <summary> List of always allowed players, overrides rank allowances. </summary>
        public override List<string> Whitelisted {
            get { return IsVisit ? lvl.Config.VisitWhitelist : lvl.Config.BuildWhitelist; }
        }
        
        /// <summary> List of never allowed players, ignores rank allowances. </summary>
        public override List<string> Blacklisted {
            get { return IsVisit ? lvl.Config.VisitBlacklist : lvl.Config.BuildBlacklist; }
        }
        
        protected override string ColoredName { get { return lvl.ColoredName; } }
        protected override string Action { get { return IsVisit ? "go to" : "build in"; } }
        protected override string ActionIng { get { return IsVisit ? "going to" : "building in"; } }
        protected override string Type { get { return IsVisit ? "visit" : "build"; } }
        protected override string MaxCmd { get { return IsVisit ? "pervisitmax" : "perbuildmax"; } }
        
        
        /// <summary> Messages all player on the level (and source player) notifying them that the
        /// min or max rank changed, rechecks access permissions for all players on the level,
        /// and finally saves the level properties file. </summary>
        public override void OnPermissionChanged(Player p, Group grp, string type) {
            Update();
            Logger.Log(LogType.UserActivity, "{0} rank changed to {1} on {2}.", type, grp.trueName, lvl.name);
            Chat.MessageLevel(lvl, type + " rank changed to " + grp.ColoredName + "%S.");
            if (p != null && p.level != lvl)
                Player.Message(p, "{0} rank changed to {1} %Son {2}%S.", type, grp.ColoredName, lvl.ColoredName);
        }
        
        /// <summary> Messages all player on the level (and source player) notifying them that the
        /// target player was whitelisted or blacklisted, rechecks access permissions
        /// for all players on the level, and finally saves the level properties file. </summary>
        public override void OnListChanged(Player p, string name, bool whitelist, bool removedFromOpposite) {
            string type = IsVisit ? "visit" : "build";
            string msg = PlayerInfo.GetColoredName(p, name);
            if (removedFromOpposite) {
                msg += " %Swas removed from the " + type + (whitelist ? " blacklist" : " whitelist");
            } else {
                msg += " %Swas " + type + (whitelist ? " whitelisted" : " blacklisted");
            }
            
            Update();
            Logger.Log(LogType.UserActivity, "{0} on {1}", msg, lvl.name);
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
                
                AccessResult access = Check(p);
                p.AllowBuild = access == AccessResult.Whitelisted || access == AccessResult.Allowed;
            }
        }
        
        void UpdateAllowVisit() {
            if (!IsVisit || lvl == Server.mainLevel) return;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level != lvl) continue;
                
                AccessResult access = Check(p);
                bool allowVisit = access == AccessResult.Whitelisted || access == AccessResult.Allowed;
                if (allowVisit) continue;
                
                Player.Message(p, "&cNo longer allowed to visit %S{0}", lvl.ColoredName);
                PlayerActions.ChangeMap(p, Server.mainLevel);
            }
        }
    }
    
    public enum AccessResult {
        
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