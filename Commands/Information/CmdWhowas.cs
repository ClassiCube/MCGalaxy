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
        public CmdWhowas() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            Player pl = PlayerInfo.Find(message);
            if (pl != null && !pl.hidden)
            {
                Player.SendMessage(p, pl.color + pl.name + Server.DefaultColor + " is online, using /whois instead.");
                Command.all.Find("whois").Use(p, message);
                return;
            }

            if (message.IndexOf("'") != -1) { Player.SendMessage(p, "Cannot parse request."); return; }
            string syntax = Server.useMySQL ? "SELECT * FROM Players WHERE Name=@Name COLLATE utf8_general_ci" :
            	"SELECT * FROM Players WHERE Name=@Name COLLATE NOCASE";           
            Database.AddParams("@Name", message);
            DataTable playerDb = Database.fillData(syntax);
            
            string FoundRank = Group.findPlayer(message.ToLower());
            if (playerDb.Rows.Count == 0) { Player.SendMessage(p, Group.Find(FoundRank).color + message + Server.DefaultColor + " has the rank of " + Group.Find(FoundRank).color + FoundRank); return; }
            string title = playerDb.Rows[0]["Title"].ToString();
            string color = Colors.Parse(playerDb.Rows[0]["color"].ToString().Trim());
            if (color == "" || color == null || String.IsNullOrEmpty(color)) color = Group.Find(FoundRank).color;
            string tcolor = Colors.Parse(playerDb.Rows[0]["title_color"].ToString().Trim());
            if (title == "" || title == null || String.IsNullOrEmpty(title))
                Player.SendMessage(p, color + message + Server.DefaultColor + " has :");
            else
                Player.SendMessage(p, color + "[" + tcolor + playerDb.Rows[0]["Title"] + color + "] " + message + Server.DefaultColor + " has :");
            Player.SendMessage(p, "> > the rank of " + Group.Find(FoundRank).color + FoundRank);
            try
            {
                if (!Group.Find("Nobody").commands.Contains("pay") && !Group.Find("Nobody").commands.Contains("give") && !Group.Find("Nobody").commands.Contains("take")) Player.SendMessage(p, "> > &a" + playerDb.Rows[0]["Money"] + Server.DefaultColor + " " + Server.moneys);
            }
            catch { }
            Player.SendMessage(p, "> > &cdied &a" + playerDb.Rows[0]["TotalDeaths"] + Server.DefaultColor + " times");
            Player.SendMessage(p, "> > &bmodified &a" + playerDb.Rows[0]["totalBlocks"] + " &eblocks.");
            Player.SendMessage(p, "> > was last seen on &a" + playerDb.Rows[0]["LastLogin"]);
            Player.SendMessage(p, "> > " + TotalTime(playerDb.Rows[0]["TimeSpent"].ToString()));
            Player.SendMessage(p, "> > first logged into the server on &a" + playerDb.Rows[0]["FirstLogin"]);
            Player.SendMessage(p, "> > logged in &a" + playerDb.Rows[0]["totalLogin"] + Server.DefaultColor + " times, &c" + playerDb.Rows[0]["totalKicked"] + Server.DefaultColor + " of which ended in a kick.");
            Player.SendMessage(p, "> > " + Awards.awardAmount(message) + " awards");
            if (Ban.IsBanned(message))
            {
                string[] data = Ban.GetBanData(message);
                Player.SendMessage(p, "> > was banned by " + data[0] + " for " + data[1] + " on " + data[2]);
            }

            if (Server.Devs.ContainsInsensitive(message)) {
                Player.SendMessage(p, Server.DefaultColor + "> > Player is a &9Developer");
            } else if (Server.Mods.ContainsInsensitive(message)) {
                Player.SendMessage(p, Server.DefaultColor + "> > Player is a &9MCGalaxy Moderator");
            } else if (Server.GCmods.ContainsInsensitive(message)) {
                Player.SendMessage(p, Server.DefaultColor + "> > Player is a &9Global Chat Moderator");
            }

            if (!(p != null && (int)p.group.Permission <= CommandOtherPerms.GetPerm(this)))
            {
                if (Server.bannedIP.Contains(playerDb.Rows[0]["IP"].ToString()))
                    playerDb.Rows[0]["IP"] = "&8" + playerDb.Rows[0]["IP"] + ", which is banned";
                Player.SendMessage(p, "> > the IP of " + playerDb.Rows[0]["IP"]);
                if (Server.useWhitelist)
                {
                    if (Server.whiteList.Contains(message.ToLower()))
                    {
                        Player.SendMessage(p, "> > Player is &fWhitelisted");
                    }
                }
            }
            playerDb.Dispose();
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/whowas <name> - Displays information about someone who left.");
        }
        public string TotalTime(string time)
        {
            return "time spent on server: " + time.Split(' ')[0] + " Days, " + time.Split(' ')[1] + " Hours, " + time.Split(' ')[2] + " Minutes, " + time.Split(' ')[3] + " Seconds.";
        }
    }
}
