/*
	Written by RedNoodle
   
    Copyright 2011 MCGalaxy
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
namespace MCGalaxy.Commands
{
    public sealed class CmdOpStats : Command
    {
        public override string name { get { return "opstats"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "information"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdOpStats() { }
        public override void Use(Player p, string message)
        {
            bool debug = false;
            Player who = null;
            string timespanend = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string timespan = "thismonth";
            string timespanname = "This Month";
            bool tspanoption = false;
            if (message == "" && p != null) { who = p; } else { who = (message.Split(' ').Length > 1 ? Player.Find(message.Split(' ')[0]) : Player.Find(message)); }
            if (p != null && (message == "today" || message == "yesterday" || message == "thismonth" || message == "lastmonth" || message == "all")) { who = p; }
            if (p == null && message == "") { Help(p); return; }
            if (message.Split(' ').Length == 1 && (message == "today" || message == "yesterday" || message == "thismonth" || message == "lastmonth" || message == "all")) { timespan = message; }
            if (message.Split(' ').Length == 2 && (message.Split(' ')[1].ToLower() == "today" || message.Split(' ')[1].ToLower() == "yesterday" || message.Split(' ')[1].ToLower() == "thismonth" || message.Split(' ')[1].ToLower() == "lastmonth" || message.Split(' ')[1].ToLower() == "all")) { timespan = message.Split(' ')[1].ToLower(); }
            if (debug) { Player.SendMessage(p, "Message = " + message); }
            if (timespan.ToLower() == "today") 
            { 
                timespan = DateTime.Now.ToString("yyyy-MM-dd 00:00:00"); 
                timespanname = "Today"; 
                tspanoption = true; 
            }
            if (timespan.ToLower() == "yesterday") 
            { 
                timespan = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
                timespanend = DateTime.Now.ToString("yyyy-MM-dd 00:00:00"); 
                timespanname = "Yesterday"; 
                tspanoption = true; 
            }
            if (timespan.ToLower() == "thismonth") 
            { 
                timespan = DateTime.Now.ToString("yyyy-MM-01 00:00:00"); 
                tspanoption = true; 
            }
            if (timespan.ToLower() == "lastmonth") 
            { 
                timespan = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-01 00:00:00");
                timespanend = DateTime.Now.ToString("yyyy-MM-01 00:00:00");
                timespanname = "Last Month"; 
                tspanoption = true; 
            }
            if (timespan.ToLower() == "all") 
            { 
                timespan = "0000-00-00 00:00:00"; 
                timespanname = "ALL"; 
                tspanoption = true; 
            }
            if (!tspanoption) { Help(p); return; }
            if (debug) { Player.SendMessage(p, "Timespan = " + timespan); }
            if (debug) { Player.SendMessage(p, "TimespanName = " + timespanname); }
            if (who != null) { message = who.name; } // Online full player name is converted to message
            else
            {
                Database.AddParams("@Name", (message.Split(' ').Length > 1 ? message.Split(' ')[0] : message));
                using (DataTable playerDb = Database.fillData("SELECT * FROM Players WHERE Name=@Name"))
                {
                    if (playerDb.Rows != null && playerDb.Rows.Count > 0) // Check if player exists in database since we couldn't find player online
                    {
                        message = playerDb.Rows[0]["Name"].ToString(); // Proper case of player name is pulled from database and converted to message
                        playerDb.Dispose();
                    }
                    else
                    {
                        Player.SendMessage(p, "Unable to find player"); // Player wasn't online and didn't exist in database
                        return;
                    }
                }
            }
            DataTable reviewcount = Server.useMySQL ? MySQL.fillData("SELECT COUNT(ID) FROM Opstats WHERE Time >= '" + timespan + "' AND Time < '" + timespanend + "' AND Name LIKE '" + message + "' AND Cmd LIKE 'review' AND Cmdmsg LIKE 'next'") : SQLite.fillData("SELECT COUNT(ID) FROM Opstats WHERE Time >= '" + timespan + "' AND Time < '" + timespanend + "' AND Name LIKE '" + message + "' AND Cmd LIKE 'review' AND Cmdmsg LIKE 'next'");
            DataTable promotecount = Server.useMySQL ? MySQL.fillData("SELECT COUNT(ID) FROM Opstats WHERE Time >= '" + timespan + "' AND Time < '" + timespanend + "' AND Name LIKE '" + message + "' AND Cmd LIKE 'promote' AND Cmdmsg !=''") : SQLite.fillData("SELECT COUNT(ID) FROM Opstats WHERE Time >= '" + timespan + "' AND Time < '" + timespanend + "' AND Name LIKE '" + message + "' AND Cmd LIKE 'promote' AND Cmdmsg !=''");
            DataTable demotecount = Server.useMySQL ? MySQL.fillData("SELECT COUNT(ID) FROM Opstats WHERE Time >= '" + timespan + "' AND Time < '" + timespanend + "' AND Name LIKE '" + message + "' AND Cmd LIKE 'demote' AND Cmdmsg !=''") : SQLite.fillData("SELECT COUNT(ID) FROM Opstats WHERE Time >= '" + timespan + "' AND Time < '" + timespanend + "' AND Name LIKE '" + message + "' AND Cmd LIKE 'demote' AND Cmdmsg !=''");
            DataTable griefercount = Server.useMySQL ? MySQL.fillData("SELECT COUNT(ID) FROM Opstats WHERE Time >= '" + timespan + "' AND Time < '" + timespanend + "' AND Name LIKE '" + message + "' AND Cmd LIKE 'griefer' AND Cmdmsg !=''") : SQLite.fillData("SELECT COUNT(ID) FROM Opstats WHERE Time >= '" + timespan + "' AND Time < '" + timespanend + "' AND Name LIKE '" + message + "' AND Cmd LIKE 'griefer' AND Cmdmsg !=''");
            DataTable undocount = Server.useMySQL ? MySQL.fillData("SELECT COUNT(ID) FROM Opstats WHERE Time >= '" + timespan + "' AND Time < '" + timespanend + "' AND Name LIKE '" + message + "' AND Cmd LIKE 'undo' AND Cmdmsg !=''") : SQLite.fillData("SELECT COUNT(ID) FROM Opstats WHERE Time >= '" + timespan + "' AND Time < '" + timespanend + "' AND Name LIKE '" + message + "' AND Cmd LIKE 'undo' AND Cmdmsg !=''");
            DataTable freezecount = Server.useMySQL ? MySQL.fillData("SELECT COUNT(ID) FROM Opstats WHERE Time >= '" + timespan + "' AND Time < '" + timespanend + "' AND Name LIKE '" + message + "' AND Cmd LIKE 'freeze' AND Cmdmsg !=''") : SQLite.fillData("SELECT COUNT(ID) FROM Opstats WHERE Time >= '" + timespan + "' AND Time < '" + timespanend + "' AND Name LIKE '" + message + "' AND Cmd LIKE 'freeze' AND Cmdmsg !=''");
            DataTable mutecount = Server.useMySQL ? MySQL.fillData("SELECT COUNT(ID) FROM Opstats WHERE Time >= '" + timespan + "' AND Time < '" + timespanend + "' AND Name LIKE '" + message + "' AND Cmd LIKE 'mute' AND Cmdmsg !=''") : SQLite.fillData("SELECT COUNT(ID) FROM Opstats WHERE Time >= '" + timespan + "' AND Time < '" + timespanend + "' AND Name LIKE '" + message + "' AND Cmd LIKE 'mute' AND Cmdmsg !=''");
            DataTable warncount = Server.useMySQL ? MySQL.fillData("SELECT COUNT(ID) FROM Opstats WHERE Time >= '" + timespan + "' AND Time < '" + timespanend + "' AND Name LIKE '" + message + "' AND Cmd LIKE 'warn' AND Cmdmsg !=''") : SQLite.fillData("SELECT COUNT(ID) FROM Opstats WHERE Time >= '" + timespan + "' AND Time < '" + timespanend + "' AND Name LIKE '" + message + "' AND Cmd LIKE 'warn' AND Cmdmsg !=''");
            DataTable kickcount = Server.useMySQL ? MySQL.fillData("SELECT COUNT(ID) FROM Opstats WHERE Time >= '" + timespan + "' AND Time < '" + timespanend + "' AND Name LIKE '" + message + "' AND Cmd LIKE 'kick' AND Cmdmsg !=''") : SQLite.fillData("SELECT COUNT(ID) FROM Opstats WHERE Time >= '" + timespan + "' AND Time < '" + timespanend + "' AND Name LIKE '" + message + "' AND Cmd LIKE 'kick' AND Cmdmsg !=''");
            DataTable tempbancount = Server.useMySQL ? MySQL.fillData("SELECT COUNT(ID) FROM Opstats WHERE Time >= '" + timespan + "' AND Time < '" + timespanend + "' AND Name LIKE '" + message + "' AND Cmd LIKE 'tempban' AND Cmdmsg !=''") : SQLite.fillData("SELECT COUNT(ID) FROM Opstats WHERE Time >= '" + timespan + "' AND Time < '" + timespanend + "' AND Name LIKE '" + message + "' AND Cmd LIKE 'tempban' AND Cmdmsg !=''");
            DataTable bancount = Server.useMySQL ? MySQL.fillData("SELECT COUNT(ID) FROM Opstats WHERE Time >= '" + timespan + "' AND Time < '" + timespanend + "' AND Name LIKE '" + message + "' AND Cmd LIKE 'ban' AND Cmdmsg !=''") : SQLite.fillData("SELECT COUNT(ID) FROM Opstats WHERE Time >= '" + timespan + "' AND Time < '" + timespanend + "' AND Name LIKE '" + message + "' AND Cmd LIKE 'ban' AND Cmdmsg !=''");
            Player.SendMessage(p, (p == null ? "" : "&d") + "OpStats for " + (p == null ? "" : "&c") + message); // Use colorcodes if in game, don't use color if in console
            Player.SendMessage(p, (p == null ? "" : "&d") + "Showing " + timespanname + " Starting from " + timespan);
            Player.SendMessage(p, (p == null ? "" : "&0") + "----------------");
            Player.SendMessage(p, (p == null ? "" : "&a") + "Reviews - " + (p == null ? "" : "&5") + reviewcount.Rows[0]["COUNT(id)"]); // Count results within datatable
            Player.SendMessage(p, (p == null ? "" : "&a") + "Promotes - " + (p == null ? "" : "&5") + promotecount.Rows[0]["COUNT(id)"]);
            Player.SendMessage(p, (p == null ? "" : "&a") + "Demotes - " + (p == null ? "" : "&5") + demotecount.Rows[0]["COUNT(id)"]);
            Player.SendMessage(p, (p == null ? "" : "&a") + "Griefers - " + (p == null ? "" : "&5") + griefercount.Rows[0]["COUNT(id)"]);
            Player.SendMessage(p, (p == null ? "" : "&a") + "Undo - " + (p == null ? "" : "&5") + undocount.Rows[0]["COUNT(id)"]);
            Player.SendMessage(p, (p == null ? "" : "&a") + "Freezes - " + (p == null ? "" : "&5") + freezecount.Rows[0]["COUNT(id)"]);
            Player.SendMessage(p, (p == null ? "" : "&a") + "Mutes - " + (p == null ? "" : "&5") + mutecount.Rows[0]["COUNT(id)"]);
            Player.SendMessage(p, (p == null ? "" : "&a") + "Warns - " + (p == null ? "" : "&5") + warncount.Rows[0]["COUNT(id)"]);
            Player.SendMessage(p, (p == null ? "" : "&a") + "Kicks - " + (p == null ? "" : "&5") + kickcount.Rows[0]["COUNT(id)"]);
            Player.SendMessage(p, (p == null ? "" : "&a") + "Tempbans - " + (p == null ? "" : "&5") + tempbancount.Rows[0]["COUNT(id)"]);
            Player.SendMessage(p, (p == null ? "" : "&a") + "Bans - " + (p == null ? "" : "&5") + bancount.Rows[0]["COUNT(id)"]);
            reviewcount.Dispose();
            promotecount.Dispose();
            demotecount.Dispose();
            griefercount.Dispose();
            undocount.Dispose();
            freezecount.Dispose();
            mutecount.Dispose();
            warncount.Dispose();
            kickcount.Dispose();
            tempbancount.Dispose();
            bancount.Dispose();
		}
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/opstats [player] [today]|[yesterday]|[thismonth]|[lastmonth]|[all] - Displays information about operator command usage.");
        }
    }
}