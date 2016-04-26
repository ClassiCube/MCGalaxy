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
        public override string shortcut { get { return "top"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdTop() { }

        public override void Use(Player p, string message)
        {
            string[] args;
            if (message == "" || (args = message.Split(' ')).Length < 2) {
                Help(p); return;
            }
            
            string strLimit = args[1];
            int limit = 0;
            if (!Int32.TryParse(strLimit, out limit)) {
                Player.SendMessage(p, string.Format("/Top: Invalid limit \"{0}\".", strLimit));
                return;
            }
            if (limit < 1) {
                Player.SendMessage(p, string.Format("/Top: \"{0}\" is too small a limit, the min is 1.", strLimit));
                return;
            } else if (limit > 15) {
                Player.SendMessage(p, string.Format("/Top: \"{0}\" is too big a limit, the max is 15.", strLimit));
                return;
            }
            
            string columnName, title;
            string select = "Name", table = "Players";
            string order = "desc";
            switch (args[0]) {
                case "1":
                    columnName = "TotalLogin";
                    title = "&aTop " + strLimit + " number of logins:";
                    break;
                case "2":
                    columnName = "TotalDeaths";
                    title = "&aTop " + strLimit + " number of deaths:";
                    break;
                case "3":
                    columnName = "money";
                    title = "&aTop " + strLimit + " amounts of money:";
                    select = "player"; table = "Economy";
                    break;
                case "4":
                    columnName = "firstlogin";
                    title = "&aFirst players:";
                    order = "asc";
                    break;
                case "5":
                    columnName = "lastlogin";
                    title = "&aMost recent players:";
                    break;
                case "6":
                    columnName = "TotalBlocks";
                    title = "&aTop " + strLimit + " number of blocks modified:";
                    break;
                case "7":
                    columnName = "TotalKicked";
                    title = "&aTop " + strLimit + " number of kicks:";
                    break;
                case "8":
                    columnName = "TotalInfected";
                    title = "&aTop total infections:";
                    table = "ZombieStats"; break;
                case "9":
                    columnName = "TotalRounds";
                    title = "&aTop rounds survived:";
                    table = "ZombieStats"; break;
                case "10":
                    columnName = "MaxInfected";
                    title = "&aTop consecutive infections:";
                    table = "ZombieStats"; break;
                case "11":
                    columnName = "MaxRounds";
                    title = "&aTop consecutive rounds survived:";
                    table = "ZombieStats"; break;
                default:
                    Player.SendMessage(p, string.Format("/Top: Unrecognised type \"{0}\".", args[0]));
                    return;
            }
            
            const string query = "SELECT distinct {0}, {1} from {2} order by {3} {4} limit {5}";
            DataTable db = Database.fillData(
                string.Format(query, select, columnName.ToLower(), table,
                              columnName.ToLower(), order, limit));
            
            Player.SendMessage(p, title);
            for (int i = 0; i < db.Rows.Count; i++) {
                Player.SendMessage(p, (i + 1) + ") " + db.Rows[i][select] + " - [" + db.Rows[i][columnName] + "]");
            }
            db.Dispose();
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%2/top [#] [number] - Prints a particular top list with number of entries");
            SendRecognisedTypes(p);
        }
        
        internal static void SendRecognisedTypes(Player p) {
            Player.SendMessage(p, "1) Most logins   2) Most deaths");
            Player.SendMessage(p, "3) Money   4) First Players");
            Player.SendMessage(p, "5) Recent Players   6) Blocks Modified");
            Player.SendMessage(p, "7) Most kicks");
            if (!Server.zombie.Running) return;
            Player.SendMessage(p, "8) Most infected   9) Most rounds survived");
            Player.SendMessage(p, "10) Max infected   11) Max rounds survived");
        }
    }
    
    public sealed class CmdTopFive : Command {
        
        public override string name { get { return "topfive"; } }
        public override string shortcut { get { return "5"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdTopFive() { }

        public override void Use(Player p, string message)  {
            if (message == "") { Help(p); return; }
            Command.all.Find("top").Use(p, message + " 5 ");
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%2/topfive [#] - Prints a particular top five list");
            CmdTop.SendRecognisedTypes(p);
        }
    }
    
    public sealed class CmdTopTen : Command {
        
        public override string name { get { return "topten"; } }
        public override string shortcut { get { return "10"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdTopTen() { }

        public override void Use(Player p, string message)  {
            if (message == "") { Help(p); return; }
            Command.all.Find("top").Use(p, message + " 10 ");
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%2/topten [#] - Prints a particular top ten list");
            CmdTop.SendRecognisedTypes(p);
        }
    }
}
