/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.IO;

namespace MCGalaxy.Commands.Info { 
    public sealed class CmdRankInfo : Command {        
        public override string name { get { return "rankinfo"; } }
        public override string shortcut { get { return "ri"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdRankInfo() { }

        public override void Use(Player p, string message) {
            if (CheckSuper(p, message, "player name")) return;
            if (message == "") message = p.name;
            
            List<string> rankings = Server.RankInfo.FindMatches(p, message, "rankings");
            if (rankings == null) return;
            
            string target = PlayerMetaList.GetName(rankings[0]);
            Player.Message(p, "  Rankings for {0}:", PlayerInfo.GetColoredName(p, target));
            DateTime now = DateTime.Now;
            
            foreach (string line in rankings) {
                string[] parts = line.SplitSpaces();                
                string newRank = Group.GetColoredName(parts[7]);
                string oldRank = Group.GetColoredName(parts[8]);
                
                int min = int.Parse(parts[2]), hour = int.Parse(parts[3]);
                int day = int.Parse(parts[4]), month = int.Parse(parts[5]), year = int.Parse(parts[6]);
                DateTime timeRanked = new DateTime(year, month, day, hour, min, 0);
                
                string reason = parts.Length <= 9 ? "(no reason given)" : parts[9].Replace("%20", " ");
                TimeSpan delta = now - timeRanked;
               
                Player.Message(p, "&aFrom {0} &ato {1} &a{2} ago", 
                               oldRank, newRank, delta.Shorten(true, false));
                Player.Message(p, "&aBy %S{0}&a, reason: %S{1}", 
                               PlayerInfo.GetColoredName(p, parts[1]), reason);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/rankinfo [player]");
            Player.Message(p, "%HReturns details about that person's rankings.");
        }
    }
}
