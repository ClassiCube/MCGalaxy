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
            string spanEnd = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string spanStart = "thismonth";
            string spanName = null;
            string[] args = message.Split(' ');
            
            Player target = null;
            if (message == "" || ValidTimespan(message.ToLower())) {
                if (p == null) { Help(p); return; }
                target = p;
                if (message != "")
                    spanStart = message.ToLower();
            } else {
                target = PlayerInfo.Find(args[0]);
                if (args.Length > 1 && ValidTimespan(args[1].ToLower()))
                    spanStart = args[1].ToLower();
            }
            
            if (spanStart == "today") {
                spanStart = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                spanName = "Today";
            } else if (spanStart == "yesterday")  {
                spanStart = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
                spanEnd = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                spanName = "Yesterday";
            } else if (spanStart == "thismonth") {
                spanStart = DateTime.Now.ToString("yyyy-MM-01 00:00:00");
                spanName = "This Month";
            } else if (spanStart == "lastmonth") {
                spanStart = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-01 00:00:00");
                spanEnd = DateTime.Now.ToString("yyyy-MM-01 00:00:00");
                spanName = "Last Month";
            } else if (spanStart == "all") {
                spanStart = "0000-00-00 00:00:00";
                spanName = "ALL";
            } else {
                Help(p); return;
            }
            
            if (target != null) {
                message = target.name;
            } else {
                message = PlayerInfo.FindOfflineNameMatches(p, message);
                if (message == null) return;
            }

            Player.Message(p, (p == null ? "" : "&d") + "OpStats for " + (p == null ? "" : "&c") + message); // Use colorcodes if in game, don't use color if in console
            Player.Message(p, (p == null ? "" : "&d") + "Showing " + spanName + " Starting from " + spanStart);
            Player.Message(p, (p == null ? "" : "&0") + "----------------");
            
            DoQuery(p, "Reviews - ", spanStart, spanEnd, message, "review", "LIKE 'next'");
            DoQuery(p, "Promotes - ", spanStart, spanEnd, message, "promote", "!=''");
            DoQuery(p, "Demotes - ", spanStart, spanEnd, message, "demote", "!=''");
            DoQuery(p, "Undo - ", spanStart, spanEnd, message, "undo", "!=''");
            DoQuery(p, "Freezes - ", spanStart, spanEnd, message, "freeze", "!=''");
            DoQuery(p, "Mutes - ", spanStart, spanEnd, message, "mute", "!=''");
            DoQuery(p, "Warns - ", spanStart, spanEnd, message, "warn", "!=''");
            DoQuery(p, "Kicks - ", spanStart, spanEnd, message, "kick", "!=''");
            DoQuery(p, "Tempbans - ", spanStart, spanEnd, message, "tempban", "!=''");
            DoQuery(p, "Bans - ", spanStart, spanEnd, message, "ban", "!=''");
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
        
        public override void Help(Player p) {
            Player.Message(p, "%T/opstats [player] today/yesterday/thismonth]/lastmonth/all");
            Player.Message(p, "%HDisplays information about operator command usage.");
        }
    }
}
