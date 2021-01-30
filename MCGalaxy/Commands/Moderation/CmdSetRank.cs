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
using System;
using System.IO;
using MCGalaxy.Events;
using MCGalaxy.Events.GroupEvents;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdSetRank : Command2 {
        public override string name { get { return "SetRank"; } }
        public override string shortcut { get { return "Rank"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("pr", "+up"), new CommandAlias("de", "-down"),
                    new CommandAlias("Promote", "+up"), new CommandAlias("Demote", "-down") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces(3);
            if (args.Length < 2) { Help(p); return; }
            string rankName, target;
            string reason = args.Length > 2 ? args[2] : null;
            
            if (args[0].CaselessEq("+up")) {
                rankName = args[0];
                target = ModActionCmd.FindName(p, "promote", "Promote", "", args[1], ref reason);
            } else if (args[0].CaselessEq("-down")) {
                rankName = args[0];
                target = ModActionCmd.FindName(p, "demote", "Demote", "", args[1], ref reason);
            } else {
                rankName = args[1];
                target = ModActionCmd.FindName(p, "rank", "Rank", " " + rankName, args[0], ref reason);
            }
            
            if (target == null) return;
            if (p.name.CaselessEq(target)) {
                p.Message("Cannot change your own rank."); return;
            }
            
            Group curRank = PlayerInfo.GetGroup(target);
            Group newRank = TargetRank(p, rankName, curRank);
            if (newRank == null) return;
            
            if (curRank == newRank) {
                p.Message("{0} &Sis already ranked {1}",
                          p.FormatNick(target), curRank.ColoredName);
                return;
            }
            if (!CanChangeRank(target, curRank, newRank, p, data, ref reason)) return;
            
            ModAction action = new ModAction(target, p, ModActionType.Rank, reason);
            action.targetGroup = curRank;
            action.Metadata = newRank;
            OnModActionEvent.Call(action);
        }
        
        internal static bool CanChangeRank(string name, Group curRank, Group newRank,
                                           Player p, CommandData data, ref string reason) {
            Group banned = Group.BannedRank;
            if (reason == null) {
                reason = newRank.Permission >= curRank.Permission ?
                    Server.Config.DefaultPromoteMessage : Server.Config.DefaultDemoteMessage;
            }
            reason = ModActionCmd.ExpandReason(p, reason);
            if (reason == null) return false;
            
            if (newRank == banned) {
                p.Message("Use /ban to change a player's rank to {0}&S.", banned.ColoredName); return false;
            }
            if (curRank == banned) {
                p.Message("Use /unban to change a player's rank from &S{0}.", banned.ColoredName); return false;
            }
            
            if (!CheckRank(p, data, name, curRank.Permission, "change the rank of", false)) return false;            
            if (!p.IsConsole && newRank.Permission >= data.Rank) {
                p.Message("Cannot rank a player to a rank equal to or higher than yours."); return false;
            }
            
            if (newRank.Permission == curRank.Permission) {
                p.Message("{0} &Sis already ranked {1}.",
                          p.FormatNick(name), curRank.ColoredName); return false;
            }
            
            bool cancel = false;
            OnChangingGroupEvent.Call(name, curRank, newRank, ref cancel);
            return !cancel;
        }
        
        static Group TargetRank(Player p, string name, Group curRank) {
            if (name.CaselessEq("+up"))   return NextRankUp(p, curRank);
            if (name.CaselessEq("-down")) return NextRankDown(p, curRank);
            return Matcher.FindRanks(p, name);
        }
        
        static Group NextRankDown(Player p, Group curRank) {
            int index = Group.GroupList.IndexOf(curRank);
            if (index > 0) {
                Group next = Group.GroupList[index - 1];
                if (next.Permission > LevelPermission.Banned) return next;
            }
            p.Message("No lower ranks exist"); return null;
        }
        
        static Group NextRankUp(Player p, Group curRank) {
            int index = Group.GroupList.IndexOf(curRank);
            if (index < Group.GroupList.Count - 1) {
                Group next = Group.GroupList[index + 1];
                if (next.Permission < LevelPermission.Nobody) return next;
            }
            p.Message("No higher ranks exist"); return null;
        }
        
        public override void Help(Player p) {
            p.Message("&T/SetRank [player] [rank] <reason>");
            p.Message("&HSets that player's rank/group, with an optional reason.");
            p.Message("&HTo see available ranks, type &T/ViewRanks");
            p.Message("&HFor <reason>, @number can be used as a shortcut for that rule.");
        }
    }
}
