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
    
    public sealed class CmdMoney : Command {
        
        public override string name { get { return "money"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        
        public override void Use(Player p, string message) {
            if (message == "") {
                if (p != null) Player.SendMessage(p, "You currently have %f" + p.money + " %3" + Server.moneys);
                else Player.SendMessage(p, "You must provide a player name when using this command from console.");
                return;
            }

            Player who = PlayerInfo.Find(message);
            if (who == null) {
                Economy.EcoStats ecos = Economy.RetrieveEcoStats(message);
                Player.SendMessage(p, ecos.playerName + "(%foffline%S) currently has %f" + ecos.money + " %3" + Server.moneys);
            } else {
                Player.SendMessage(p, who.FullName + " %Scurrently has %f" + who.money + " %3" + Server.moneys);
            }
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "%T/money <player>");
            Player.SendMessage(p, "%HShows how much %3" + Server.moneys + " %H<player> has.");
            Player.SendMessage(p, "%HIf <player> is not given, shows how much %3" + Server.moneys + " %Hyou have.");
        }
    }
}
