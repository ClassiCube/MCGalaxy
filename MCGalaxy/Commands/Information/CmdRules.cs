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
    public sealed class CmdRules : Command {
        public override string name { get { return "Rules"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Builder, "+ can send rules to other players") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("Agree", "agree"), new CommandAlias("Disagree", "disagree") }; }
        }
        
        public override void Use(Player p, string message) {
            TextFile rulesFile = TextFile.Files["Rules"];
            rulesFile.EnsureExists();
            
            if (message.CaselessEq("agree")) { Agree(p); return; }
            if (message.CaselessEq("disagree")) { Disagree(p); return; }
            
            Player who = p;
            if (message.Length > 0) {
                if (!CheckExtraPerm(p)) { MessageNeedExtra(p, 1); return; }
                who = PlayerInfo.FindMatches(p, message);
                if (who == null) return;
            }           
            if (who != null) who.hasreadrules = true;

            string[] rules = rulesFile.GetText();            
            Player.Message(who, "Server Rules:");
            Player.MessageLines(who, rules);
            
            if (who != null && p != who) {
                Player.Message(p, "Sent the rules to " + who.ColoredName + "%S.");
                Player.Message(who, p.ColoredName + " %Ssent you the rules.");
            }
        }
        
        void Agree(Player p) {
            if (Player.IsSuper(p)) { Player.Message(p, "Only in-game players can agree to the rules."); return; }
            if (!ServerConfig.AgreeToRulesOnEntry) { Player.Message(p, "agree-to-rules-on-entry is not enabled."); return; }            
            if (!p.hasreadrules) { Player.Message(p, "&9You must read %T/Rules &9before agreeing."); return; }
            if (Server.agreed.Contains(p.name)) { Player.Message(p, "You have already agreed to the rules."); return; }
            
            p.agreed = true;
            Player.Message(p, "Thank you for agreeing to follow the rules. You may now build and use commands!");
            Server.agreed.Add(p.name);
            Server.agreed.Save(false);
        }
        
        void Disagree(Player p) {
            if (Player.IsSuper(p)) { Player.Message(p, "Only in-game players can disagree with the rules."); return; }
            if (!ServerConfig.AgreeToRulesOnEntry) { Player.Message(p, "agree-to-rules-on-entry is not enabled."); return; }
            
            if (p.Rank > LevelPermission.Guest) {
                Player.Message(p, "Your awesomeness prevents you from using this command"); return;
            }
            p.Leave("If you don't agree with the rules, consider playing elsewhere.");
        }

        public override void Help(Player p) {
            if (CheckExtraPerm(p)) {
                Player.Message(p, "%T/Rules <player>");
                Player.Message(p, "%HDisplays server rules to <player>.");
                Player.Message(p, "%HIf <player> is not given, the rules are displayed to you.");
            } else {
                Player.Message(p, "%T/Rules");
                Player.Message(p, "%HDisplays the server rules.");
            }
            Player.Message(p, "%T/Rules agree %H- Agrees to the server's rules");
            Player.Message(p, "%T/Rules disagree %H- Disagrees with the server's rules");
        }
    }
}
