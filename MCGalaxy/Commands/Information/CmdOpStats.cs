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
using MCGalaxy.DB;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands.Info { 
    public sealed class CmdOpStats : Command2 {
        public override string name { get { return "OpStats"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool UseableWhenFrozen { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            string end = DateTime.Now.ToString(Database.DateFormat);
            string start = "thismonth", name = null;
            string[] args = message.SplitSpaces();
            
            if (message.Length == 0 || ValidTimespan(message.ToLower())) {
                if (p.IsSuper) { SuperRequiresArgs(p, "player name"); return; }
                name = p.name;
                if (message.Length > 0) start = message.ToLower();
            } else {
                name = PlayerInfo.FindMatchesPreferOnline(p, args[0]);
                if (args.Length > 1 && ValidTimespan(args[1].ToLower()))
                    start = args[1].ToLower();
            }
            if (name == null) return;
            
            if (start == "today") {
                start = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            } else if (start == "yesterday")  {
                start = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
                end = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            } else if (start == "thismonth") {
                start = DateTime.Now.ToString("yyyy-MM-01 00:00:00");
            } else if (start == "lastmonth") {
                start = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-01 00:00:00");
                end = DateTime.Now.ToString("yyyy-MM-01 00:00:00");
            } else if (start == "all") {
                start = "0000-00-00 00:00:00";
            } else {
                Help(p); return;
            }

            p.Message("OpStats for {0} &Ssince {1}", p.FormatNick(name), start);
            
            int reviews = Count(start, end, name, "review", "LIKE 'next'");
            int ranks = Count(start, end, name, "setrank", "!=''");
            int promotes = Count(start, end, name, "setrank", "LIKE '+up%'");
            int demotes = Count(start, end, name, "setrank", "LIKE '-down%'");
            int promotesOld = Count(start, end, name, "promote");
            int demotesOld = Count(start, end, name, "demote");
            
            int mutes = Count(start, end, name, "mute");
            int freezes = Count(start, end, name, "freeze");
            int warns = Count(start, end, name, "warn");
            int kicks = Count(start, end, name, "kick");
            
            int bans = Count(start, end, name, "ban");
            int kickbans = Count(start, end, name, "kickban");
            int ipbans = Count(start, end, name, "banip");
            int xbans = Count(start, end, name, "xban");
            int tempbans = Count(start, end, name, "tempban");

            p.Message("  &a{0}&S bans, &a{1}&S IP-bans, &a{2}&S tempbans",
                           bans + kickbans + xbans, ipbans + xbans, tempbans);
            p.Message("  &a{0}&S mutes, &a{1}&S warns, &a{2}&S freezes, &a{3}&S kicks",
                           mutes, warns, freezes, kicks + kickbans + xbans);
            p.Message("  &a{0}&S reviews, &a{1}&S ranks (&a{2}&S promotes, &a{3}&S demotes)",
                           reviews, ranks + promotesOld + demotesOld,
                           promotes + promotesOld, demotes + demotesOld);
        }
        
        static bool ValidTimespan(string value) {
            return value == "today" || value == "yesterday" || value == "thismonth" || value == "lastmonth" || value == "all";
        }
 
        static int Count(string start, string end, string name, string cmd, string msg = "!=''") {
            const string whereSQL = "WHERE Time >= @0 AND Time < @1 AND Name LIKE @2 AND Cmd LIKE @3 AND Cmdmsg ";
            return Database.CountRows("Opstats", whereSQL + msg, start, end, name, cmd);
        }
        
        public override void Help(Player p) {
            p.Message("&T/OpStats [player] today/yesterday/thismonth/lastmonth/all");
            p.Message("&HDisplays information about operator command usage.");
        }
    }
}
