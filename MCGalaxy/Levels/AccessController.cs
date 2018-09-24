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
using System.Text;
using MCGalaxy.Commands;

namespace MCGalaxy {
    
    /// <summary> Encapuslates access permissions (visit or build) for a level/zone. </summary>
    public abstract class AccessController {
        
        public abstract LevelPermission Min { get; set; }
        public abstract LevelPermission Max { get; set; }
        public abstract List<string> Whitelisted { get; }
        public abstract List<string> Blacklisted { get; }
        
        protected abstract string ColoredName { get; }
        protected abstract string Action { get; }
        protected abstract string ActionIng { get; }
        protected abstract string Type { get; }
        protected abstract string MaxCmd { get; }
        
        
        public bool CheckAllowed(Player p) {
            AccessResult access = Check(p.name, p.Rank);
            return access == AccessResult.Allowed || access == AccessResult.Whitelisted;
        }
        
        public AccessResult Check(string name, LevelPermission rank) {
            if (Blacklisted.CaselessContains(name)) return AccessResult.Blacklisted;
            if (Whitelisted.CaselessContains(name)) return AccessResult.Whitelisted;
            
            if (rank < Min) return AccessResult.BelowMinRank;
            if (rank > Max && MaxCmd != null && !CommandExtraPerms.Find(MaxCmd, 1).UsableBy(rank)) {
                return AccessResult.AboveMaxRank;
            }
            return AccessResult.Allowed;
        }
        
        public bool CheckDetailed(Player p) { return CheckDetailed(p, p.Rank); }
        public bool CheckDetailed(Player p, LevelPermission plRank) {
            AccessResult access = Check(p.name, plRank);
            if (access == AccessResult.Allowed) return true;
            if (access == AccessResult.Whitelisted) return true;
            
            if (access == AccessResult.Blacklisted) {
                p.Message("You are blacklisted from {0} {1}", ActionIng, ColoredName);
                return false;
            }
            
            string whitelist = "";
            if (Whitelisted.Count > 0) {
                whitelist = "(and " + Whitelisted.Join(pl => PlayerInfo.GetColoredName(p, pl)) + "%S) ";
            }
            
            if (access == AccessResult.BelowMinRank) {
                p.Message("Only {2}%S+ {3}may {0} {1}",
                               Action, ColoredName, Group.GetColoredName(Min), whitelist);
            } else if (access == AccessResult.AboveMaxRank) {
                p.Message("Only {2} %Sand below {3}may {0} {1}",
                               Action, ColoredName, Group.GetColoredName(Max), whitelist);
            }
            return false;
        }
        
        public void Describe(Player p, StringBuilder perms) {
            perms.Append(Group.GetColoredName(Min) + "%S+");
            if (Max != LevelPermission.Nobody) {
                perms.Append(" up to " + Group.GetColoredName(Max));
            }
            
            List<string> whitelist = Whitelisted;
            foreach (string name in whitelist) {
                perms.Append(", " + PlayerInfo.GetColoredName(p, name));
            }

            List<string> blacklist = Blacklisted;
            if (blacklist.Count == 0) return;
            
            perms.Append(" %S(except ");
            foreach (string name in blacklist) {
                perms.Append(PlayerInfo.GetColoredName(p, name) + ", ");
            }
            perms.Remove(perms.Length - 2, 2);
            perms.Append("%S)");
        }
        

        public bool SetMin(Player p, LevelPermission plRank, Level lvl, Group grp) {
            if (!CheckRank(p, plRank, grp.Permission, false)) return false;
            
            Min = grp.Permission;
            OnPermissionChanged(p, lvl, grp, "Min ");
            return true;
        }

        public bool SetMax(Player p, LevelPermission plRank, Level lvl, Group grp) {
            if (!CheckRank(p, plRank, grp.Permission, true)) return false;

            Max = grp.Permission;
            OnPermissionChanged(p, lvl, grp, "Max ");
            return true;
        }

        public bool Whitelist(Player p, LevelPermission plRank, Level lvl, string target) {
            if (!CheckList(p, plRank, target, true)) return false;
            if (Whitelisted.CaselessContains(target)) {
                p.Message("{0} %Sis already whitelisted.", PlayerInfo.GetColoredName(p, target));
                return true;
            }
            
            bool removed = true;
            if (!Blacklisted.CaselessRemove(target)) {
                Whitelisted.Add(target);
                removed = false;
            }
            OnListChanged(p, lvl, target, true, removed);
            return true;
        }
        
        public bool Blacklist(Player p, LevelPermission plRank, Level lvl, string target) {
            if (!CheckList(p, plRank, target, false)) return false;
            if (Blacklisted.CaselessContains(target)) {
                p.Message("{0} %Sis already blacklisted.", PlayerInfo.GetColoredName(p, target));
                return true;
            }
            
            bool removed = true;
            if (!Whitelisted.CaselessRemove(target)) {
                Blacklisted.Add(target);
                removed = false;
            }
            OnListChanged(p, lvl, target, false, removed);
            return true;
        }


        public void OnPermissionChanged(Player p, Level lvl, Group grp, string type) {
            string msg = type + Type + " rank changed to " + grp.ColoredName;
            ApplyChanges(p, lvl, msg);
        }
        
        public void OnListChanged(Player p, Level lvl, string name, bool whitelist, bool removedFromOpposite) {
            string msg = PlayerInfo.GetColoredName(p, name);
            if (removedFromOpposite) {
                msg += " %Swas removed from the " + Type + (whitelist ? " blacklist" : " whitelist");
            } else {
                msg += " %Swas " + Type + (whitelist ? " whitelisted" : " blacklisted");
            }
            ApplyChanges(p, lvl, msg);
        }
        
        protected abstract void ApplyChanges(Player p, Level lvl, string msg);
        
        bool CheckRank(Player p, LevelPermission plRank, LevelPermission perm, bool max) {
            string mode = max ? "max" : "min";
            if (!CheckDetailed(p, plRank)) {                
                p.Message("%WHence you cannot change the {1} {0} rank.", Type, mode); return false;
            }
            
            if (perm <= plRank || max && perm == LevelPermission.Nobody) return true;          
            p.Message("%WYou cannot change the {1} {0} rank of {2} %Wto a rank higher than yours.",
                      Type, mode, ColoredName);
            return false;
        }
        
        bool CheckList(Player p, LevelPermission plRank, string name, bool whitelist) {
            if (!CheckDetailed(p, plRank)) {
                string mode = whitelist ? "whitelist" : "blacklist";
                p.Message("%WHence you cannot modify the {0} {1}.", Type, mode); return false;
            }
            
            Group group = PlayerInfo.GetGroup(name);
            if (group.Permission <= plRank) return true;
            
            if (!whitelist) {
                p.Message("%WYou cannot blacklist players of a higher rank.");
                return false;
            } else if (Check(name, group.Permission) == AccessResult.Blacklisted) {
                p.Message("{0} %Sis blacklisted from {1} {2}%S.",
                               PlayerInfo.GetColoredName(p, name), ActionIng, ColoredName);
                return false;
            }
            return true;
        }
    }
    
    /// <summary> Encapuslates access permissions (visit or build) for a level. </summary>
    public sealed class LevelAccessController : AccessController {
        
        public readonly bool IsVisit;
        readonly LevelConfig cfg;
        readonly string lvlName;
        
        public LevelAccessController(LevelConfig cfg, string levelName, bool isVisit) {
            this.cfg = cfg;
            this.lvlName = levelName;
            IsVisit = isVisit;
        }

        public override LevelPermission Min {
            get { return IsVisit ? cfg.VisitMin : cfg.BuildMin; }
            set {
                if (IsVisit) cfg.VisitMin = value;
                else cfg.BuildMin = value;
            }
        }

        public override LevelPermission Max {
            get { return IsVisit ? cfg.VisitMax : cfg.BuildMax; }
            set {
                if (IsVisit) cfg.VisitMax = value;
                else cfg.BuildMax = value;
            }
        }

        public override List<string> Whitelisted {
            get { return IsVisit ? cfg.VisitWhitelist : cfg.BuildWhitelist; }
        }

        public override List<string> Blacklisted {
            get { return IsVisit ? cfg.VisitBlacklist : cfg.BuildBlacklist; }
        }
        
        protected override string ColoredName { get { return cfg.Color + lvlName; } }
        protected override string Action { get { return IsVisit ? "go to" : "build in"; } }
        protected override string ActionIng { get { return IsVisit ? "going to" : "building in"; } }
        protected override string Type { get { return IsVisit ? "visit" : "build"; } }
        protected override string MaxCmd { get { return IsVisit ? "PerVisit" : "PerBuild"; } }

        
        protected override void ApplyChanges(Player p, Level lvl, string msg) {
            Update(lvl);
            Logger.Log(LogType.UserActivity, "{0} %Son {1}", msg, lvlName);            
            if (lvl != null) lvl.Message(msg);
            
            if (p != Player.Console && p.level != lvl) {
                p.Message("{0} %Son {1}", msg, ColoredName);
            }
        }
        
        void Update(Level lvl) {
            cfg.SaveFor(lvlName);
            if (lvl == null) return;
            if (IsVisit && lvl == Server.mainLevel) return;
            Player[] players = PlayerInfo.Online.Items;
            
            foreach (Player p in players) {
                if (p.level != lvl) continue;
                bool allowed = CheckAllowed(p);
                
                if (!IsVisit) {
                    p.AllowBuild = allowed;
                } else if (!allowed) {                    
                    p.Message("%WNo longer allowed to visit %S{0}", ColoredName);
                    PlayerActions.ChangeMap(p, Server.mainLevel);
                }
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