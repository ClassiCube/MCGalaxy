/*
    Written by RedNoodle
   
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
    
    public sealed class CmdOpStats : Command {
        public override string name { get { return "opstats"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdOpStats() { }
        
        public override void Use(Player p, string message) {
            string end = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string start = "thismonth";
            string spanName = null;
            string[] args = message.Split(' ');
            
            Player target = null;
            if (message == "" || ValidTimespan(message.ToLower())) {
                if (p == null) { Help(p); return; }
                target = p;
                if (message != "")
                    start = message.ToLower();
            } else {
                target = PlayerInfo.Find(args[0]);
                if (args.Length > 1 && ValidTimespan(args[1].ToLower()))
                    start = args[1].ToLower();
            }
            
            if (start == "today") {
                start = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                spanName = "Today";
            } else if (start == "yesterday")  {
                start = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
                end = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                spanName = "Yesterday";
            } else if (start == "thismonth") {
                start = DateTime.Now.ToString("yyyy-MM-01 00:00:00");
                spanName = "This Month";
            } else if (start == "lastmonth") {
                start = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-01 00:00:00");
                end = DateTime.Now.ToString("yyyy-MM-01 00:00:00");
                spanName = "Last Month";
            } else if (start == "all") {
                start = "0000-00-00 00:00:00";
                spanName = "ALL";
            } else {
                Help(p); return;
            }
            
            string name = null;
            if (target != null) {
                name = target.name;
            } else {
                name = PlayerInfo.FindOfflineNameMatches(p, args[0]);
                if (name == null) return;
            }

            Player.Message(p, "OpStats for {0} %Ssince {1}",
                           PlayerInfo.GetColoredName(p, name), start);
            
            int reviews = Query(start, end, name, "review", "LIKE 'next'");
            int ranks = Query(start, end, name, "setrank", "!=''");
            int promotes = Query(start, end, name, "setrank", "LIKE '+up%'");
            int demotes = Query(start, end, name, "setrank", "LIKE '-down%'");
            int promotesOld = Query(start, end, name, "promote");
            int demotesOld = Query(start, end, name, "demote");
            
            int mutes = Query(start, end, name, "mute");
            int freezes = Query(start, end, name, "freeze");
            int warns = Query(start, end, name, "warn");
            int kicks = Query(start, end, name, "kick");
            
            int bans = Query(start, end, name, "ban");
            int kickbans = Query(start, end, name, "kickban");
            int ipbans = Query(start, end, name, "banip");
            int xbans = Query(start, end, name, "xban");
            int tempbans = Query(start, end, name, "tempban");

            Player.Message(p, "  &a{0}%S bans, &a{1}%S IP-bans, &a{2}%S tempbans",
                           bans + kickbans + xbans, ipbans + xbans, tempbans);
            Player.Message(p, "  &a{0}%S mutes, &a{1}%S warns, &a{2}%S freezes, &a{3}%S kicks",
                           mutes, warns, freezes, kicks + kickbans + xbans);
            Player.Message(p, "  &a{0}%S reviews, &a{1}%S ranks (&a{2}%S promotes, &a{3}%S demotes)",
                           reviews, ranks + promotesOld + demotesOld,
                           promotes + promotesOld, demotes + demotesOld);
        }
        
        static bool ValidTimespan(string value) {
            return value == "today" || value == "yesterday" || value == "thismonth" || value == "lastmonth" || value == "all";
        }
        
        static void DoQuery(Player p, string group, string start, string end, string name, string cmd, string msg) {
            DataTable table = Database.Backend.GetRows(
                "Opstats", "COUNT(ID)" ,"WHERE Time >= @0 AND Time < @1 AND " +
                "Name LIKE @2 AND Cmd LIKE @3 AND Cmdmsg " + msg, start, end, name, cmd);
            
            // don't use colour codes in cli or gui
            Player.Message(p, (p == null ? "" : "&a") + group + (p == null ? "" : "&5") + table.Rows[0]["COUNT(id)"]);
            table.Dispose();
        }
        
        static int Query(string start, string end, string name, string cmd, string msg = "!=''") {
            using (DataTable table = Database.Backend.GetRows(
                "Opstats", "COUNT(ID)" ,"WHERE Time >= @0 AND Time < @1 AND " +
                "Name LIKE @2 AND Cmd LIKE @3 AND Cmdmsg " + msg, start, end, name, cmd)) {
                string count = table.Rows[0]["COUNT(id)"].ToString();
                return PlayerData.ParseInt(count);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/opstats [player] today/yesterday/thismonth]/lastmonth/all");
            Player.Message(p, "%HDisplays information about operator command usage.");
        }
    }
}
