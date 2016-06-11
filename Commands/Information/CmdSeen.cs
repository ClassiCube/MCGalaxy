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
using System.Data;
using MCGalaxy.SQL;
namespace MCGalaxy.Commands
{
    public sealed class CmdSeen : Command
    {
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
                Player.Message(p, pl.name + " was first seen: " + pl.firstLogin);
                Player.Message(p, pl.color + pl.name + " %Sis currently online."); return;
            }

            OfflinePlayer target = PlayerInfo.FindOfflineMatches(p, message);
            if (target == null) return;
            Player.Message(p, message + " was first seen: " + target.firstLogin);
            Player.Message(p, message + " was last seen: " + target.lastLogin);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/seen [player] - says when a player was first and last seen on the server");
        }
    }
}
