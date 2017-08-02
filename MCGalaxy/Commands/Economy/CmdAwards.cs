/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the educational Community License, Version 2.0 and
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
using MCGalaxy.Eco;

namespace MCGalaxy.Commands.Eco {
    public sealed class CmdAwards : Command {        
        public override string name { get { return "Awards"; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }

        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces();
            if (args.Length > 2) { Help(p); return; }
            string plName = "", modifier = args[args.Length - 1];
            int ignored;
            
            if (args.Length == 2) {
                plName = PlayerInfo.FindMatchesPreferOnline(p, args[0]);
                if (plName == null) return;
            } else if (message.Length > 0 && !message.CaselessEq("all")) {
                if (!int.TryParse(args[0], out ignored)) {
                    modifier = "";
                    plName = PlayerInfo.FindMatchesPreferOnline(p, args[0]);
                    if (plName == null) return;
                }
            }

            List<Awards.Award> awards = GetAwards(plName);
            if (awards.Count == 0) {
                if (plName.Length > 0) {
                    Player.Message(p, "{0} %Shas no awards.", 
                                   PlayerInfo.GetColoredName(p, plName));
                } else {
                    Player.Message(p, "This server has no awards yet.");
                }
                return;
            }
            
            string cmd = plName.Length == 0 ? "awards" : "awards " + plName;
            MultiPageOutput.Output(p, awards, FormatAward,
                                   cmd, "Awards", modifier, true);
        }
        
        static List<Awards.Award> GetAwards(string plName) {
            if (plName.Length == 0) return Awards.AwardsList;
            
            List<Awards.Award> awards = new List<Awards.Award>();
            foreach (string s in Awards.GetPlayerAwards(plName)) {
                Awards.Award award = new Awards.Award();
                award.Name = s;
                award.Description = Awards.GetDescription(s);
                awards.Add(award);
            }
            return awards;
        }
        
        static string FormatAward(Awards.Award award) {
            return "&6" + award.Name + ": &7" + award.Description;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Awards <player> %H- Lists awards that player has.");
            Player.Message(p, "%H  If <player> is not given, lists all awards the server has.");
        }
    }
}
