/*
    Copyright 2011 MCForge

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
using MCGalaxy.Commands.Moderation;

namespace MCGalaxy.Commands.Misc {
    public sealed class CmdFakeRank : Command {
        public override string name { get { return "fakerank"; } }
        public override string shortcut { get { return "frk"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        
        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces();
            if (message.Length == 0 || args.Length < 2) { Help(p); return; }
            Player who = PlayerInfo.FindMatches(p, args[0]);
            Group newRank = Matcher.FindRanks(p, args[1]);
            if (who == null || newRank == null) return;
            
            if (p != null && who.Rank > p.Rank) {
                MessageTooHighRank(p, "fakerank", true); return;
            }
            
            if (newRank.Permission == LevelPermission.Banned) {
                string banner = p == null ? "console" : p.ColoredName;
                Chat.MessageGlobal("{0} %Swas &8banned %Sby {1}%S.", 
                                who.ColoredName, banner);
            } else {
                string reason = newRank.Permission >= who.Rank ? 
                    ServerConfig.DefaultPromoteMessage : ServerConfig.DefaultDemoteMessage;
                string rankMsg = ModActionCmd.FormatRankChange(who.group, newRank, who.name, reason);
                
                Chat.MessageGlobal(rankMsg);
                Player.Message(who, "You are now ranked {0}%S, type /help for your new set of commands.", 
                               newRank.ColoredName);
            }
            
            who.color = newRank.Color;
            Entities.GlobalRespawn(who);
            who.SetPrefix();
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/fakerank [player] [rank]");
            Player.Message(p, "%HGives [player] the appearance of being ranked to [rank].");
        }
    }
}
