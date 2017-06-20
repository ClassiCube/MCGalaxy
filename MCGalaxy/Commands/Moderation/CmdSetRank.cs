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

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdSetRank : Command {
        public override string name { get { return "setrank"; } }
        public override string shortcut { get { return "rank"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("pr", "+up"), new CommandAlias("de", "-down"),
                    new CommandAlias("promote", "+up"), new CommandAlias("demote", "-down") }; }
        }

        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces(3);
            if (args.Length < 2) { Help(p); return; }
            string rank = null, name = null;
            string reason = args.Length > 2 ? args[2] : null;
            
            if (args[0].CaselessEq("+up")) {
                rank = args[0];
                name = ModActionCmd.FindName(p, "promote", "promote", "", args[1], ref reason);
            } else if (args[0].CaselessEq("-down")) {
                rank = args[0];
                name = ModActionCmd.FindName(p, "demote", "demote", "", args[1], ref reason);
            } else {
                rank = args[1];
                name = ModActionCmd.FindName(p, "rank", "rank", " " + rank, args[0], ref reason);
            }
            if (name == null) return;
            
            Player who = PlayerInfo.FindExact(name);
            if (p == who && who != null) { Player.Message(p, "Cannot change your own rank."); return; }
            
            Group curRank = who != null ? who.group : PlayerInfo.GetGroup(name);
            Group newRank = TargetRank(p, rank.ToLower(), curRank);
            if (newRank == null) return;
            
            if (curRank == newRank) {
                Player.Message(p, "{0} %Sis already ranked {1}",
                               PlayerInfo.GetColoredName(p, name), curRank.ColoredName);
                return;
            }
            if (!CanChangeRank(name, curRank, newRank, who, p, ref reason)) return;
            
            ModAction action = new ModAction(name, p, ModActionType.Rank, reason);
            action.targetGroup = curRank;
            action.Metadata = newRank;
            OnModActionEvent.Call(action);
        }
        
        internal static bool CanChangeRank(string name, Group curRank, Group newRank,
                                           Player who, Player p, ref string reason) {
            Group banned = Group.BannedRank;
            if (reason == null) {
                reason = newRank.Permission >= curRank.Permission ?
                    Server.defaultPromoteMessage : Server.defaultDemoteMessage;
            }
            reason = ModActionCmd.ExpandReason(p, reason);
            if (reason == null) return false;
            
            if (newRank == banned) {
                Player.Message(p, "Use /ban to change a player's rank to {0}%S.", banned.ColoredName); return false;
            }
            if (curRank == banned) {
                Player.Message(p, "Use /unban to change a player's rank from %S{0}.", banned.ColoredName); return false;
            }
            if (p != null && curRank.Permission >= p.Rank) {
                MessageTooHighRank(p, "change the rank of", false); return false;
            }
            if (p != null && newRank.Permission >= p.Rank) {
                Player.Message(p, "Cannot rank a player to a rank equal to or higher than yours."); return false;
            }
            if (newRank.Permission == curRank.Permission) {
                Player.Message(p, "{0} %Sis already ranked {1}.", 
                               PlayerInfo.GetColoredName(p, name), curRank.ColoredName); return false;
            }
            
            if (who == null) return true;
            Group.because(who, newRank);
            if (Group.cancelrank) { Group.cancelrank = false; return false; }
            return true;
        }
        
        static Group TargetRank(Player p, string name, Group curRank) {
            if (name == "+up") return NextRankUp(p, curRank);
            if (name == "-down") return NextRankDown(p, curRank);
            return Matcher.FindRanks(p, name);
        }
        
        static Group NextRankDown(Player p, Group curRank) {
            int index = Group.GroupList.IndexOf(curRank);
            if (index > 0) {
                Group next = Group.GroupList[index - 1];
                if (next.Permission > LevelPermission.Banned) return next;
            }
            Player.Message(p, "No lower ranks exist"); return null;
        }
        
        static Group NextRankUp(Player p, Group curRank) {
            int index = Group.GroupList.IndexOf(curRank);
            if (index < Group.GroupList.Count - 1) {
                Group next = Group.GroupList[index + 1];
                if (next.Permission < LevelPermission.Nobody) return next;
            }
            Player.Message(p, "No higher ranks exist"); return null;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/rank [player] [rank] <reason>");
            Player.Message(p, "%HSets that player's rank/group, with an optional reason.");
            Player.Message(p, "%HTo see available ranks, type %T/viewranks");
            Player.Message(p, "%HFor <reason>, @number can be used as a shortcut for that rule.");
        }
    }
}
