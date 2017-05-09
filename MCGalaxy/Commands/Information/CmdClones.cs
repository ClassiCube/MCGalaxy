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
    public sealed class CmdClones : Command {
        public override string name { get { return "clones"; } }
        public override string shortcut { get { return "alts"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("whoip") }; }
        }
        public CmdClones() { }

        public override void Use(Player p, string message)
        {
            if (message == "" && p != null) {
                message = p.ip;
            } else {
                message = ModActionCmd.FindIP(p, message, "find alts of", "clones");
                if (message == null) return;
            }

            List<string> accounts = PlayerInfo.FindAccounts(message);
            if (accounts.Count == 0) {
                Player.Message(p, "No players last played with the given IP.");
            } else {
                Player.Message(p, "These players have the same IP:");
                Player.Message(p, accounts.Join(alt => PlayerInfo.GetColoredName(p, alt)));
            }
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/clones [name]");
            Player.Message(p, "%HFinds everyone with the same IP as [name]");
            Player.Message(p, "%T/clones [ip address]");
            Player.Message(p, "%HFinds everyone who last played or is playing on the given IP");
        }
    }
}
