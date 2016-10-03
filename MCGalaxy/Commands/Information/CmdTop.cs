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
using System;
using System.Data;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands {    
    public sealed class CmdTop : Command {
        
        public override string name { get { return "top"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdTop() { }
        public override CommandAlias[] Aliases {
            get { return new [] { new CommandAlias("topten", null, "10"), new CommandAlias("topfive", null, "5") }; }
        }
        
        public override void Use(Player p, string message) {
            string[] args;
            if (message == "" || (args = message.Split(' ')).Length < 2) {
                Help(p); return;
            }
            
            string strLimit = args[1];
            int limit = 0;
            if (!Int32.TryParse(strLimit, out limit)) {
                Player.Message(p, "/Top: Invalid limit \"{0}\".", strLimit);
                return;
            }
            if (limit < 1) {
                Player.Message(p, "/Top: \"{0}\" is too small a limit, the min is 1.", strLimit);
                return;
            } else if (limit > 15) {
                Player.Message(p, "/Top: \"{0}\" is too big a limit, the max is 15.", strLimit);
                return;
            }
            
            string col, title;
            string table = "Players", order = "desc";
            switch (args[0]) {
                case "1":
                    col = "TotalLogin";
                    title = "&aTop " + strLimit + " number of logins:";
                    break;
                case "2":
                    col = "TotalDeaths";
                    title = "&aTop " + strLimit + " number of deaths:";
                    break;
                case "3":
                    col = "money";
                    title = "&aTop " + strLimit + " amounts of money:";
                    break;
                case "4":
                    col = "firstlogin";
                    title = "&aFirst players:";
                    order = "asc";
                    break;
                case "5":
                    col = "lastlogin";
                    title = "&aMost recent players:";
                    break;
                case "6":
                    col = "TotalKicked";
                    title = "&aTop " + strLimit + " number of kicks:";
                    break;
                case "7":
                    col = "totalBlocks & " + PlayerData.LowerBitsMask;
                    title = "&aTop " + strLimit + " number of blocks modified:";
                    break;
                case "8":
                    col = "totalCuboided & " + PlayerData.LowerBitsMask;
                    title = "&aTop " + strLimit + " number of blocks drawn:";
                    break;
                case "9":
                    col = "totalBlocks >> " + PlayerData.LowerBits;
                    title = "&aTop " + strLimit + " number of blocks placed:";
                    break;
                case "10":
                    col = "totalCuboided >> " + PlayerData.LowerBits;
                    title = "&aTop " + strLimit + " number of blocks deleted:";
                    break;
                case "11":
                    col = "TotalInfected";
                    title = "&aTop total infections:";
                    table = "ZombieStats"; break;
                case "12":
                    col = "TotalRounds";
                    title = "&aTop rounds survived:";
                    table = "ZombieStats"; break;
                case "13":
                    col = "MaxInfected";
                    title = "&aTop consecutive infections:";
                    table = "ZombieStats"; break;
                case "14":
                    col = "MaxRounds";
                    title = "&aTop consecutive rounds survived:";
                    table = "ZombieStats"; break;
                default:
                    Player.Message(p, "/Top: Unrecognised type \"{0}\".", args[0]);
                    return;
            }
            
            DataTable db = Database.Backend.GetRows(table, "DISTINCT Name, " + col, 
                                                    "ORDER BY " + col + " " + order + " LIMIT " + limit);
            
            Player.Message(p, title);
            for (int i = 0; i < db.Rows.Count; i++) {
                Player.Message(p, (i + 1) + ") " + db.Rows[i]["Name"] + " - [" + db.Rows[i][col] + "]");
            }
            db.Dispose();
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/top [stat] [number of players to show]");
            Player.Message(p, "%HPrints a list of players who have the " +
                           "most/top of a particular stat. Available stats:");
            
            Player.Message(p, "1) Most logins, 2) Most deaths, 3) Money");
            Player.Message(p, "4) First joined, 5) Recently joined, 6) Most kicks");
            Player.Message(p, "7) Blocks modified, 8) Blocks drawn");
            Player.Message(p, "9) Blocks placed, 10) Blocks deleted");
             
            if (!Server.zombie.Running) return;
            Player.Message(p, "11) Most infected, 12) Most rounds survived");
            Player.Message(p, "13) Max infected, 14) Max rounds survived");
        }
    }
}
