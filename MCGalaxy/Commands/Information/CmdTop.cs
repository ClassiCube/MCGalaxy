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
using System.Collections.Generic;
using System.Data;
using MCGalaxy.DB;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands.Info {
    public sealed class CmdTop : Command2 {
        public override string name { get { return "Top"; } }
        public override string shortcut { get { return "Most"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override CommandAlias[] Aliases {
            get { return new [] { new CommandAlias("TopTen", "10"), new CommandAlias("TopFive", "5"),
                    new CommandAlias("Top10", "10"), }; }
        }
        
        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces();
            if (args.Length < 2) { Help(p); return; }
            
            int maxResults = 0, offset = 0;
            if (!CommandParser.GetInt(p, args[0], "Max results", ref maxResults, 1, 15)) return;

            TopStat stat = FindTopStat(args[1]);
            if (stat == null) {
                p.Message("&WUnrecognised type \"{0}\".", args[1]); return;
            }
            
            if (args.Length > 2) {
                if (!CommandParser.GetInt(p, args[2], "Offset", ref offset, 0)) return;
            }
            
            string limit = " LIMIT " + offset + "," + maxResults;
            List<string[]> stats = Database.GetRows(stat.Table, "DISTINCT Name, " + stat.Column,
                                                    "ORDER BY" + stat.OrderBy + limit);
            
            p.Message("&a{0}:", stat.Title());
            for (int i = 0; i < stats.Count; i++) {
                string nick  = p.FormatNick(stats[i][0]);
                string value = stat.Formatter(stats[i][1]);
                p.Message("{0}) {1} &S- {2}", offset + (i + 1), nick, value);
            }
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
            p.Message("&T/Top [max results] [stat] <offset>");
            p.Message("&HPrints a list of players who have the " +
                           "most/top of a particular stat. Available stats:");
            p.Message("&f" + TopStat.Stats.Join(stat => stat.Identifier));
        }
    }
}
