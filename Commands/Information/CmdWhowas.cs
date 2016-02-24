/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
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
    public sealed class CmdWhowas : Command
    {
        public override string name { get { return "whowas"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            Player pl = PlayerInfo.Find(message);
            if (pl != null && !pl.hidden) {
                Player.SendMessage(p, pl.color + pl.name + Server.DefaultColor + " is online, using /whois instead.");
                Command.all.Find("whois").Use(p, message);
                return;
            }

            if (message.IndexOf("'") != -1) { Player.SendMessage(p, "Cannot parse request."); return; }
            string syntax = Server.useMySQL ? "SELECT * FROM Players WHERE Name=@Name COLLATE utf8_general_ci" :
            	"SELECT * FROM Players WHERE Name=@Name COLLATE NOCASE";           
            Database.AddParams("@Name", message);
            DataTable table = Database.fillData(syntax);
            
            string plGroup = Group.findPlayer(message.ToLower());
            if (table.Rows.Count == 0) { Player.SendMessage(p, Group.Find(plGroup).color + message + " %Shas the rank of " + Group.Find(plGroup).color + plGroup); return; }
            string title = table.Rows[0]["Title"].ToString();
            string color = Colors.Parse(table.Rows[0]["color"].ToString().Trim());
            if (String.IsNullOrEmpty(color)) color = Group.Find(plGroup).color;
            string tcolor = Colors.Parse(table.Rows[0]["title_color"].ToString().Trim());
            if (String.IsNullOrEmpty(title))
                Player.SendMessage(p, color + message + " %Shas :");
            else
                Player.SendMessage(p, color + "[" + tcolor + title + color + "] " + message + " %Shas :");
            Player.SendMessage(p, "> > the rank of " + Group.Find(plGroup).color + plGroup);
            
            Group nobody = Group.findPerm(LevelPermission.Nobody);
            if (nobody == null || (!nobody.commands.Contains("pay") && !nobody.commands.Contains("give") && !nobody.commands.Contains("take")))
                Player.SendMessage(p, "> > &a" + table.Rows[0]["Money"] + Server.DefaultColor + " " + Server.moneys);
                
            Player.SendMessage(p, "> > &cdied &a" + table.Rows[0]["TotalDeaths"] + Server.DefaultColor + " times");
            Player.SendMessage(p, "> > &bmodified &a" + table.Rows[0]["totalBlocks"] + " &eblocks.");
            Player.SendMessage(p, "> > was last seen on &a" + table.Rows[0]["LastLogin"]);
            Player.SendMessage(p, "> > " + TotalTime(table.Rows[0]["TimeSpent"].ToString()));
            Player.SendMessage(p, "> > first logged into the server on &a" + table.Rows[0]["FirstLogin"]);
            Player.SendMessage(p, "> > logged in &a" + table.Rows[0]["totalLogin"] + Server.DefaultColor + " times, &c" + table.Rows[0]["totalKicked"] + Server.DefaultColor + " of which ended in a kick.");
            Player.SendMessage(p, "> > " + Awards.awardAmount(message) + " awards");
            if (Ban.IsBanned(message)) {
                string[] data = Ban.GetBanData(message);
                Player.SendMessage(p, "> > was banned by " + data[0] + " for " + data[1] + " on " + data[2]);
            }

            if (Server.Devs.ContainsInsensitive(message)) Player.SendMessage(p, "> > Player is a &9Developer");
            else if (Server.Mods.ContainsInsensitive(message)) Player.SendMessage(p, "> > Player is a &9MCGalaxy Moderator");
            else if (Server.GCmods.ContainsInsensitive(message)) Player.SendMessage(p, "> > Player is a &9Global Chat Moderator");

            if (p == null || (int)p.group.Permission <= CommandOtherPerms.GetPerm(this)) {
                if (Server.bannedIP.Contains(table.Rows[0]["IP"].ToString()))
                    table.Rows[0]["IP"] = "&8" + table.Rows[0]["IP"] + ", which is banned";
                Player.SendMessage(p, "> > the IP of " + table.Rows[0]["IP"]);
                if (Server.useWhitelist&& Server.whiteList.Contains(message))
                    Player.SendMessage(p, "> > Player is &fWhitelisted");
            }
            table.Dispose();
        }
        
        string TotalTime(string time) {
        	TimeSpan value = time.ParseDBTime();
            return "time spent on server: " + value.Days + " Days, " + value.Hours + " Hours, " + value.Minutes + " Minutes, " + value.Seconds + " Seconds.";
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/whowas <name> - Displays information about someone who left.");
        }
       
    }
}
