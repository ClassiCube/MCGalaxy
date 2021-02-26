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
    public sealed class CmdAwards : Command2 {
        public override string name { get { return "Awards"; } }
        public override string type { get { return CommandTypes.Economy; } }

        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces();
            if (args.Length > 2) { Help(p); return; }
            int offset = 0;           
            string name = p.name;

            if (args.Length == 2 || (message.Length > 0 && !IsListModifier(args[0]))) {
                offset = 1;
                name = PlayerInfo.FindMatchesPreferOnline(p, args[0]);               
                if (name == null) return;
            }

            List<Awards.Award> awards = Awards.AwardsList;
            if (awards.Count == 0) { p.Message("This server has no awards yet."); return; }
            
            List<string> playerAwards = Awards.GetPlayerAwards(name);
            StringFormatter<Awards.Award> formatter = (award) => FormatPlayerAward(award, playerAwards);
            
            string cmd = name.Length == 0 ? "awards" : "awards " + name;
            string modifier = args.Length > offset ? args[offset] : "";
            
            MultiPageOutput.Output(p, awards, formatter,
                                   cmd, "Awards", modifier, true);
        }
        
        static string FormatPlayerAward(Awards.Award award, List<string> awards) {
            bool has = awards != null && awards.CaselessContains(award.Name);
            return (has ? "&a" : "&c") + award.Name + ": &7" + award.Description;
        }
        
        public override void Help(Player p) {
            p.Message("&T/Awards <player> &H- Lists awards");
            p.Message("&HAppears &agreen &Hif player has an award, &cred &Hif not");
        }
    }
}
