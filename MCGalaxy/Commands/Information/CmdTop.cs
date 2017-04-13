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
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdTop() { }
        public override CommandAlias[] Aliases {
            get { return new [] { new CommandAlias("topten", null, "10"), new CommandAlias("topfive", null, "5") }; }
        }
        
        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces();
            if (args.Length < 2) { Help(p); return; }
            
            int offset = ParseOffset(p, args);
            int limit = ParseLimit(p, args);            
            if (limit == -1 || offset == -1) return;
            
            string col, title;
            string table = "Players", order = "desc";
            switch (args[0]) {
                case "1":
                    col = "TotalLogin";
                    title = "&aMost logins:";
                    break;
                case "2":
                    col = "TotalDeaths";
                    title = "&aMost deaths:";
                    break;
                case "3":
                    col = "money";
                    title = "&aMost " + Server.moneys + ":";
                    break;
                case "4":
                    col = "firstlogin";
                    title = "&aOldest players:";
                    order = "asc";
                    break;
                case "5":
                    col = "lastlogin";
                    title = "&aMost recent players:";
                    break;
                case "6":
                    col = "TotalKicked";
                    title = "&aMost times kicked:";
                    break;
                case "7":
                    col = "totalBlocks & " + PlayerData.LowerBitsMask;
                    title = "&aMost blocks modified:";
                    break;
                case "8":
                    col = "totalCuboided & " + PlayerData.LowerBitsMask;
                    title = "&aMost blocks drawn:";
                    break;
                case "9":
                    col = "totalBlocks >> " + PlayerData.LowerBits;
                    title = "&aMost blocks placed:";
                    break;
                case "10":
                    col = "totalCuboided >> " + PlayerData.LowerBits;
                    title = "&aMost blocks deleted:";
                    break;
                case "11":
                    col = "TotalInfected";
                    title = "&aMost players infected:";
                    table = "ZombieStats"; break;
                case "12":
                    col = "TotalRounds";
                    title = "&aMost rounds survived:";
                    table = "ZombieStats"; break;
                case "13":
                    col = "MaxInfected";
                    title = "&aMost consecutive infections:";
                    table = "ZombieStats"; break;
                case "14":
                    col = "MaxRounds";
                    title = "&aMost consecutive rounds survived:";
                    table = "ZombieStats"; break;
                default:
                    Player.Message(p, "/Top: Unrecognised type \"{0}\".", args[0]);
                    return;
            }
            
            string strLimit = " LIMIT " + offset + "," + limit;
            DataTable db = Database.Backend.GetRows(table, "DISTINCT Name, " + col, 
                                                    "ORDER BY " + col + " " + order + strLimit);
            
            Player.Message(p, title);
            for (int i = 0; i < db.Rows.Count; i++) {
                Player.Message(p, "{0}) {1} - {2}", offset + (i + 1), db.Rows[i]["Name"], db.Rows[i][col]);
            }
            db.Dispose();
        }
        
        static int ParseLimit(Player p, string[] args) {
            int limit = 0;
            string limitArg = args[args.Length - 1];
            
            if (!CommandParser.GetInt(p, args[1], "Limit", ref limit, 1, 15)) return -1;
            return limit;
        }
        
        static int ParseOffset(Player p, string[] args) {
            if (args.Length <= 2) return 0;
            int offset = 0;
            
            if (!CommandParser.GetInt(p, args[1], "Offset", ref offset, 0)) return -1;
            return offset;         
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/top [stat] <offset> [number of players to show] ");
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
