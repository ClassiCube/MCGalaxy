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
using System.Data;
using System.Text;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands {
    
    public sealed class CmdClones : Command {
        public override string name { get { return "clones"; } }
        public override string shortcut { get { return "alts"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdClones() { }

        public override void Use(Player p, string message) {
            message = Colors.EscapeColors(message);
            if (message == "" && p != null) message = p.name;

            string name = null;
            int matches = 0;
            Player who = PlayerInfo.FindMatches(p, message, out matches);
            if (matches > 1) return;
            if (who == null) {
                Player.Message(p, "Could not find player. Searching Player DB.");
                OfflinePlayer target = PlayerInfo.FindOfflineMatches(p, message);
                if (target == null) return;
                message = target.ip; name = target.name;
            } else {
                message = who.ip; name = who.name;
            }

            List<string> alts = PlayerInfo.FindAccounts(message);
            if (alts.Count == 0) { Player.Message(p, "Could not find any record of the player entered."); return; }
            if (alts.Count == 1) { Player.Message(p, message + " has no clones."); return; }
            Group grp = Group.findPerm(LevelPermission.Banned);
           
            StringBuilder builder = new StringBuilder();
            foreach (string alt in alts) {
                if (Group.IsBanned(alt))
                    builder.Append(grp.color + alt + "%S");
                else
                    builder.Append(alt);
                builder.Append(", ");
            }

            Player.Message(p, "These players have the same IP address:");
            Player.Message(p, builder.ToString(0, builder.Length - 2));
        }

        public override void Help(Player p) {
            Player.Message(p, "/clones <name> - Finds everyone with the same IP as <name>");
        }
    }
}
