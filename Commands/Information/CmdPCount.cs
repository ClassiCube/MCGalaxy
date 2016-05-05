/*
    Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
 
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
using System.Data;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands {
    
    public sealed class CmdPCount : Command {
        
        public override string name { get { return "pcount"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }

        public override void Use(Player p, string message) {
            int bancount = Group.findPerm(LevelPermission.Banned).playerList.All().Count;

            DataTable table = Database.fillData("SELECT COUNT(id) FROM Players");
            Player.Message(p, "A total of " + table.Rows[0]["COUNT(id)"] + " unique players have visited this server.");
            Player.Message(p, "Of these players, " + bancount + " have been banned.");
            table.Dispose();
            int count = 0, hiddenCount = 0;
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (!Entities.CanSee(p, pl)) continue;
                count++;
                if (pl.hidden) hiddenCount++;
            }
            
            string verb = count == 1 ? "is " : "are ";
            string qualifier = count == 1 ? " player" : " players";
            if (hiddenCount == 0)
                Player.Message(p, "There " + verb + count + qualifier + " online.");
            else
                Player.Message(p, "There " + verb + count + qualifier + " online (" + hiddenCount + " hidden).");
        }

        public override void Help(Player p) {
            Player.Message(p, "/pcount - Displays the number of players online and total.");
        }
    }
}
