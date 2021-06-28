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
using MCGalaxy.DB;

namespace MCGalaxy.Commands.Info {
    public sealed class CmdSeen : Command2 {
        public override string name { get { return "Seen"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool UseableWhenFrozen { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) {
                if (p.IsSuper) { SuperRequiresArgs(p, "player name"); return; }
                message = p.name;
            }
            if (!Formatter.ValidName(p, message, "player")) return;

            int matches;
            Player pl = PlayerInfo.FindMatches(p, message, out matches);
            if (matches > 1) return;
            if (matches == 1) {
                Show(p, pl.ColoredName, pl.FirstLogin, pl.LastLogin);
                p.Message("{0} &Sis currently online.", p.FormatNick(pl));
                return;
            }

            p.Message("Searching PlayerDB..");
            PlayerData target = PlayerDB.Match(p, message);
            if (target == null) return;
            Show(p, target.Name, target.FirstLogin, target.LastLogin);
        }
        
        static void Show(Player p, string name, DateTime first, DateTime last) {
            TimeSpan firstDelta = DateTime.Now - first;
            TimeSpan lastDelta  = DateTime.Now - last;
            
            name = p.FormatNick(name);
            p.Message("{0} &Swas first seen at {1:H:mm} on {1:yyyy-MM-dd} ({2} ago)", name, first, firstDelta.Shorten());
            p.Message("{0} &Swas last seen at {1:H:mm} on {1:yyyy-MM-dd} ({2} ago)",  name, last,  lastDelta.Shorten());
        }
        
        public override void Help(Player p) {
            p.Message("&T/Seen [player]");
            p.Message("&HSays when a player was first and last seen on the server");
        }
    }
}
