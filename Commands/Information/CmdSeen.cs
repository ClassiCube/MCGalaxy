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
using System;

namespace MCGalaxy.Commands {
    public sealed class CmdSeen : Command {
        public override string name { get { return "seen"; } }
        public override string shortcut { get { return ""; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override string type { get { return CommandTypes.Information; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }

            int matches;
            Player pl = PlayerInfo.FindMatches(p, message, out matches);
            if (matches > 1) return;
            if (matches == 1) {
                Show(p, pl.ColoredName, pl.firstLogin, pl.lastLogin);
                Player.Message(p, pl.ColoredName + " %Sis currently online.");
                return;
            }

            Player.Message(p, "Searching PlayerDB..");
            OfflinePlayer target = PlayerInfo.FindOfflineMatches(p, message);
            if (target == null) return;
            Show(p, target.name, DateTime.Parse(target.firstLogin), 
                 DateTime.Parse(target.lastLogin));
        }
        
        static void Show(Player p, string name, DateTime first, DateTime last) {
            TimeSpan firstDelta = DateTime.Now - first;
            TimeSpan lastDelta = DateTime.Now - last;
            Player.Message(p, "{0} %Swas first seen at {1:H:mm} on {1:d} ({2} ago)", name, first, firstDelta.Shorten());
            Player.Message(p, "{0} %Swas last seen at {1:H:mm} on {1:d} ({2} ago)", name, last, lastDelta.Shorten());
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/seen [player]");
            Player.Message(p, "%HSays when a player was first and last seen on the server");
        }
    }
}
