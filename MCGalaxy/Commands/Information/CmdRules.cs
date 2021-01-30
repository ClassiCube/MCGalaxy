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
using MCGalaxy.Util;

namespace MCGalaxy.Commands.Info {
    public sealed class CmdRules : Command2 {
        public override string name { get { return "Rules"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Builder, "can send rules to others") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("Agree", "agree"), new CommandAlias("Disagree", "disagree") }; }
        }
        
        public override void Use(Player p, string message, CommandData data) {
            TextFile rulesFile = TextFile.Files["Rules"];
            rulesFile.EnsureExists();
            
            if (message.CaselessEq("agree")) { Agree(p); return; }
            if (message.CaselessEq("disagree")) { Disagree(p, data); return; }
            
            Player target = p;
            if (message.Length > 0) {
                if (!CheckExtraPerm(p, data, 1)) return;
                target = PlayerInfo.FindMatches(p, message);
                if (target == null) return;
            }
            if (target != null) target.hasreadrules = true;

            string[] rules = rulesFile.GetText();
            target.Message("Server Rules:");
            target.MessageLines(rules);
            
            if (target != null && p != target) {
            	p.Message("Sent the rules to {0}&S.", p.FormatNick(target));
            	target.Message("{0} &Ssent you the rules.", target.FormatNick(p));
            }
        }
        
        void Agree(Player p) {
            if (p.IsSuper) { p.Message("Only in-game players can agree to the rules."); return; }
            if (!Server.Config.AgreeToRulesOnEntry) { p.Message("agree-to-rules-on-entry is not enabled."); return; }
            if (!p.hasreadrules) { p.Message("&9You must read &T/Rules &9before agreeing."); return; }
            
            if (!Server.agreed.Add(p.name)) {
                p.Message("You have already agreed to the rules.");
            } else {                
                p.agreed = true;
                p.Message("Thank you for agreeing to follow the rules. You may now build and use commands!");
                Server.agreed.Save(false);
            }
        }
        
        void Disagree(Player p, CommandData data) {
            if (p.IsSuper) { p.Message("Only in-game players can disagree with the rules."); return; }
            if (!Server.Config.AgreeToRulesOnEntry) { p.Message("agree-to-rules-on-entry is not enabled."); return; }
            
            if (data.Rank > LevelPermission.Guest) {
                p.Message("Your awesomeness prevents you from using this command"); return;
            }
            p.Leave("If you don't agree with the rules, consider playing elsewhere.");
        }

        public override void Help(Player p) {
            if (HasExtraPerm(p, p.Rank, 1)) {
                p.Message("&T/Rules [player] &H- Displays server rules to [player]");
            }
            p.Message("&T/Rules &H- Displays the server rules to you");
            p.Message("&T/Rules agree &H- Agrees to the server's rules");
            p.Message("&T/Rules disagree &H- Disagrees with the server's rules");
        }
    }
}
