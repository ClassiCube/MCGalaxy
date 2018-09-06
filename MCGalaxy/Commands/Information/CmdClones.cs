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
using MCGalaxy.Commands.Moderation;

namespace MCGalaxy.Commands.Info {
    public sealed class CmdClones : Command2 {
        public override string name { get { return "Clones"; } }
        public override string shortcut { get { return "Alts"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override bool UseableWhenFrozen { get { return true; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("WhoIP") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            string name;
            if (message.Length == 0) {
                if (p.IsSuper) { SuperRequiresArgs(p, "IP address"); return; }
                message = p.ip;
            } else {
                message = ModActionCmd.FindIP(p, message, "Clones", out name);
                if (message == null) return;
            }

            List<string> accounts = PlayerInfo.FindAccounts(message);
            if (accounts.Count == 0) {
                p.Message("No players last played with the given IP.");
            } else {
                p.Message("These players have the same IP:");
                p.Message(accounts.Join(alt => PlayerInfo.GetColoredName(p, alt)));
            }
        }

        public override void Help(Player p) {
            p.Message("%T/Clones [name]");
            p.Message("%HFinds everyone with the same IP as [name]");
            p.Message("%T/Clones [ip address]");
            p.Message("%HFinds everyone who last played or is playing on the given IP");
        }
    }
}
