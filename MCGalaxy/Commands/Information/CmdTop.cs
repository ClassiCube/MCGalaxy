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
using MCGalaxy.DB;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands.Info {
    public sealed class CmdTop : Command {
        public override string name { get { return "top"; } }
        public override string shortcut { get { return "most"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdTop() { }
        public override CommandAlias[] Aliases {
            get { return new [] { new CommandAlias("topten", null, "10"), new CommandAlias("topfive", null, "5"),
                    new CommandAlias("top10", null, "10"), }; }
        }
        
        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces();
            if (args.Length < 2) { Help(p); return; }
            
            int offset = ParseOffset(p, args);
            int limit = ParseLimit(p, args);
            if (limit == -1 || offset == -1) return;
            
            TopStat stat = FindTopStat(args[0]);
            if (stat == null) {
                Player.Message(p, "/Top: Unrecognised type \"{0}\".", args[0]);
                return;
            }
            
            string order = stat.Ascending ? "asc" : "desc";
            string strLimit = " LIMIT " + offset + "," + limit;
            DataTable db = Database.Backend.GetRows(stat.Table, "DISTINCT Name, " + stat.Column,
                                                    "ORDER BY " + stat.Column + " " + order + strLimit);
            
            Player.Message(p, "&a{0}:", stat.Title());
            for (int i = 0; i < db.Rows.Count; i++) {
            	string player = PlayerInfo.GetColoredName(p, db.Rows[i]["Name"].ToString());
                string item = db.Rows[i][stat.Column].ToString();
                Player.Message(p, "{0}) {1} %S- {2}", offset + (i + 1), player, stat.Formatter(item));
            }
            db.Dispose();
        }
        
        static int ParseLimit(Player p, string[] args) {
            int limit = 0;
            string limitArg = args[args.Length - 1];
            
            if (!CommandParser.GetInt(p, limitArg, "Limit", ref limit, 1, 15)) return -1;
            return limit;
        }
        
        static int ParseOffset(Player p, string[] args) {
            if (args.Length <= 2) return 0;
            int offset = 0;
            
            if (!CommandParser.GetInt(p, args[1], "Offset", ref offset, 0)) return -1;
            return offset;
        }
        
        static TopStat FindTopStat(string input) {
            foreach (TopStat stat in TopStat.Stats) {
                if (stat.Identifier.CaselessEq(input)) return stat;
            }
            
            int number;
            if (int.TryParse(input, out number)) {
                // Backwards compatibility where top used to take a number
                if (number >= 1 && number <= TopStat.Stats.Count)
                    return TopStat.Stats[number - 1];
            }
            return null;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/top [stat] <offset> [number of players to show] ");
            Player.Message(p, "%HPrints a list of players who have the " +
                           "most/top of a particular stat. Available stats:");
            Player.Message(p, "&f" + TopStat.Stats.Join(stat => stat.Identifier));
        }
    }
}
