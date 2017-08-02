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
using MCGalaxy.Util;

namespace MCGalaxy.Commands.Info {
    public sealed class CmdOpRules : Command {
        public override string name { get { return "OpRules"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }

        public override void Use(Player p, string message) {
            TextFile oprulesFile = TextFile.Files["OpRules"];
            oprulesFile.EnsureExists();

            Player who = p;
            if (message.Length > 0) {
                who = PlayerInfo.FindMatches(p, message);
                if (who == null) return;
                if (p != null && p.Rank < who.Rank) {
                    MessageTooHighRank(p, "send oprules to", false); return;
                }
            }

            string[] oprules = oprulesFile.GetText();
            Player.Message(who, "Server OPRules:");
            Player.MessageLines(who, oprules);
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/OpRules [player]");
            Player.Message(p, "%HDisplays server oprules to a player");
        }
    }
}
