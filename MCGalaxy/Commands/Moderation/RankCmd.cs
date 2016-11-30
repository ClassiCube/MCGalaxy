/*
    Copyright 2015 MCGalaxy team
    
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
using System.IO;

namespace MCGalaxy.Commands.Moderation {
    internal static class RankCmd {
        
        internal static void ChangeRank(string name, Group oldRank, Group newRank,
                                        Player who, bool saveToNewRank = true) {
            Server.reviewlist.Remove(name);
            oldRank.playerList.Remove(name);
            oldRank.playerList.Save();
            
            if (saveToNewRank) {
                newRank.playerList.Add(name);
                newRank.playerList.Save();
            }
            if (who == null) return;

            Entities.DespawnEntities(who, false);
            if (who.color == "" || who.color == who.group.color)
                who.color = newRank.color;
            
            who.group = newRank;
            LevelAccess access = who.level.BuildAccess.Check(who);
            who.AllowBuild = access == LevelAccess.Whitelisted || access == LevelAccess.Allowed;
            
            who.SetPrefix();
            who.Send(Packet.UserType(who));
            Entities.SpawnEntities(who, false);
        }
        
        internal static string FindName(Player p, string action, 
                                        string cmd, string cmdSuffix,
                                        string name, ref string reason) {
            if (!Formatter.ValidName(p, name, "player")) return null;
            string match = MatchName(p, ref name);
            string confirmed = IsConfirmed(reason);
            if (confirmed != null) reason = confirmed;
            
            if (match != null) {
                if (match.CaselessEq(name)) return match;
                // Not an exact match, may be wanting to ban a non-existent account
                Player.Message(p, "1 player matches \"{0}\": {1}", name, match);
            }

            if (confirmed != null) return name;
            string msgReason = String.IsNullOrEmpty(reason) ? "" : " " + reason;
            Player.Message(p, "If you still want to {0} \"{1}\", use %T/{3} {1}{4} confirm{2}",
                           action, name, msgReason, cmd, cmdSuffix);
            return null;
        }
        
        static string MatchName(Player p, ref string name) {
            int matches = 0;
            Player target = PlayerInfo.FindMatches(p, name, out matches);
            if (matches > 1) return null;
            if (matches == 1) { name = target.name; return name; }
            
            Player.Message(p, "Searching PlayerDB...");
            return PlayerInfo.FindOfflineNameMatches(p, name);
        }
        
        static string IsConfirmed(string reason) {
            if (reason == null) return null;
            if (reason.CaselessEq("confirm"))
                return "";
            if (reason.CaselessStarts("confirm "))
                return reason.Substring("confirm ".Length);
            return null;
        }
    }
}
