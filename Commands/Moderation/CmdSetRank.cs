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
namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdSetRank : Command {
        public override string name { get { return "setrank"; } }
        public override string shortcut { get { return "rank"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("promote", "+up"), new CommandAlias("demote", "-down") }; }
        }
        public CmdSetRank() { }

        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces(3);
            if (args.Length < 2) { Help(p); return; }
            string rank = null, name = null;
            
            if (args[0].CaselessEq("+up") || args[0].CaselessEq("-down")) {
                rank = args[0]; name = args[1];
            } else {
                rank = args[1]; name = args[0];
            }
            
            string reason = args.Length > 2 ? args[2] : null, rankMsg = null;
            Player who = PlayerInfo.Find(name);
            if (who == null) {
                Group group = Group.findPlayerGroup(name);
                Group newRank = TargetRank(p, rank.ToLower(), group);
                if (newRank == null) return;
                
                if (!ChangeRank(name, group, newRank, null, p, ref reason)) return;                
                rankMsg = name + " &f(offline)%S's rank was set to " + newRank.ColoredName + "%S. (" + reason + "%S)";
                Player.GlobalMessage(rankMsg);
            } else if (who == p) {
                Player.Message(p, "Cannot change your own rank."); return;
            } else {
                Group newRank = TargetRank(p, rank.ToLower(), who.group);
                if (newRank == null) return;
                
                if (!ChangeRank(who.name, who.group, newRank, who, p, ref reason)) return;
                rankMsg = who.ColoredName + "%S's rank was set to " + newRank.ColoredName + "%S. (" + reason + "%S)";
                Player.GlobalMessage(rankMsg);
                Entities.DespawnEntities(who, false);
                
                if (who.color == "" || who.color == who.group.color) 
                    who.color = newRank.color;
                who.group = newRank;
                who.SetPrefix();

                who.SendMessage("You are now ranked " + newRank.ColoredName + "%S, type /help for your new set of commands.");
                who.SendUserType(Block.canPlace(who.Rank, Block.blackrock));
                Entities.SpawnEntities(who, false);
            }
            Server.IRC.Say(rankMsg);
        }
        
        bool ChangeRank(string name, Group group, Group newRank, Player who, Player p, ref string reason) {
            Group banned = Group.findPerm(LevelPermission.Banned);
            if (reason == null) {
                reason = newRank.Permission >= group.Permission ? 
                    Server.defaultPromoteMessage : Server.defaultDemoteMessage;
            }
            
            if (group == banned || newRank == banned) {
                Player.Message(p, "Cannot change the rank to or from \"" + banned.name + "\"."); return false;
            }
            if (p != null && (group.Permission >= p.Rank || newRank.Permission >= p.Rank)) {
                MessageTooHighRank(p, "change the rank of", false); return false;
            }
            if (p != null && (newRank.Permission >= p.Rank)) {
                Player.Message(p, "Cannot change the rank of a player to a rank equal or higher to yours."); return false;
            }
            
            if (who != null) {
                Group.because(who, newRank);
                if (Group.cancelrank) {
                    Group.cancelrank = false; return false;
                }
            }
            
            Server.reviewlist.Remove(name);
            group.playerList.Remove(name);
            group.playerList.Save();
            newRank.playerList.Add(name);
            newRank.playerList.Save();
            WriteRankInfo(p, name, newRank, group, reason);
            return true;
        }
        
        static void WriteRankInfo(Player p, string name, Group newRank, Group group, string reason) {
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString();
            string day = DateTime.Now.Day.ToString();
            string hour = DateTime.Now.Hour.ToString();
            string minute = DateTime.Now.Minute.ToString();
            string assigner = p == null ? "(console)" : p.name;

            string line = name + " " + assigner + " " + minute + " " + hour + " " + day + " " + month
                + " " + year + " " + newRank.name + " " + group.name + " " + reason.Replace(" ", "%20");
            Server.RankInfo.Append(line);            
        }
        
        static Group TargetRank(Player p, string name, Group curGroup) {
            if (name == "+up") return NextRankUp(p, curGroup);
            if (name == "-down") return NextRankDown(p, curGroup);
            return Group.FindOrShowMatches(p, name);
        }
        
        static Group NextRankDown(Player p, Group curGroup) {
            int index = Group.GroupList.IndexOf(curGroup);
            if (index > 0) {
                Group next = Group.GroupList[index - 1];
                if (next.Permission > LevelPermission.Banned) return next;
            }
            Player.Message(p, "No lower ranks exist"); return null;
        }
        
        static Group NextRankUp(Player p, Group curGroup) {
            int index = Group.GroupList.IndexOf(curGroup);
            if (index < Group.GroupList.Count - 1) {
                Group next = Group.GroupList[index + 1];
                if (next.Permission < LevelPermission.Nobody) return next;
            }
            Player.Message(p, "No higher ranks exist"); return null;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/rank <player> <rank> <reason> - Sets a player's rank.");
            Player.Message(p, "Valid Ranks are: " + Group.concatList(true, true));
        }
    }
}
