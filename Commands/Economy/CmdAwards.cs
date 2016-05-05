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
using System.Collections.Generic;

namespace MCGalaxy.Commands {
	
    public sealed class CmdAwards : Command {
		
        public override string name { get { return "awards"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length > 2) { Help(p); return; }

            int page = 0;
            string plName = "";
            if (args.Length == 2) {
                plName = args[0];
                Player who = PlayerInfo.Find(plName);
                if (who != null) plName = who.name;
                
                if (!int.TryParse(args[1], out page)) { Help(p); return; }
            } else if (message != "") {
                if (!int.TryParse(args[0], out page)) {
                    plName = args[0];
                    Player who = PlayerInfo.Find(plName);
                    if (who != null) plName = who.name;
                }
            }
            if (page < 0) {
                Player.Message(p, "Cannot display pages less than 0"); return;
            }

            List<Awards.Award> awards = GetAwards(plName);
            if (awards.Count == 0) {
                if (plName != "") Player.Message(p, "The player has no awards!");
                else Player.Message(p, "There are no awards in this server yet");
                return;
            }

            int start = (page - 1) * 5;
            if (start > awards.Count) {
                Player.Message(p, "There aren't that many awards, try a smaller number.");
                return;
            }
            OutputAwards(p, page, start, plName, awards);
        }
        
        static List<Awards.Award> GetAwards(string plName) {
            if (plName == "") return Awards.AwardsList;
            
            List<Awards.Award> awards = new List<Awards.Award>();
            foreach (string s in Awards.GetPlayerAwards(plName)) {
                Awards.Award award = new Awards.Award();
                award.Name = s;
                award.Description = Awards.GetDescription(s);
                awards.Add(award);
            }
            return awards;
        }
        
        static void OutputAwards(Player p, int page, int start,
                                 string plName, List<Awards.Award> awards) {
            if (plName != "")
                Player.Message(p, Server.FindColor(plName) + plName + " %Shas the following awards:");
            else
                Player.Message(p, "Awards available: ");

            if (page == 0) {
                foreach (Awards.Award award in awards)
                    Player.Message(p, "&6" + award.Name + ": &7" + award.Description);

                if (awards.Count > 8) 
                    Player.Message(p, "&5Use &b/awards " + plName + " 1/2/3/... &5for a more ordered list");
            } else {
                for (int i = start; i < Math.Min(awards.Count, start + 5); i++) {
                    Awards.Award award = awards[i];
                    Player.Message(p, "&6" + award.Name + ": &7" + award.Description);
                }
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/awards [player] %H- Gives a list of awards that player has.");
            Player.Message(p, "$HIf [player] is not given, lists all awards the server has.");
            Player.Message(p, "%HSpecify 1/2/3/... after to get an ordered list.");
        }
    }
}
