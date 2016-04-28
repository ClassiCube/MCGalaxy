/* 
    Copyright 2011 MCForge
    
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
using System.Collections.Generic;
using System.IO;
namespace MCGalaxy.Commands {
    
    public sealed class CmdFaq : Command {
        
        public override string name { get { return "faq"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.Builder, "Lowest rank that can send the faq to other players") }; }
        }

        public override void Use(Player p, string message) {
            if (!File.Exists("text/faq.txt")) {
                CP437Writer.WriteAllText("text/faq.txt", "Example: What does this server run on? This server runs on &bMCGalaxy");
            }
            List<string> faq = CP437Reader.ReadAllLines("text/faq.txt");

            Player who = p;
            if (message != "") {
                if (!CheckAdditionalPerm(p)) { MessageNeedPerms(p, "can send the FAQ to a player."); return; }
                who = PlayerInfo.FindOrShowMatches(p, message);
                if (who == null) return;
            }
            
            Player.SendMessage(who, "&cFAQ&f:");
            foreach (string line in faq)
                Player.SendMessage(who, "&f" + line);
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "/faq [player]- Displays frequently asked questions");
        }
    }
}
