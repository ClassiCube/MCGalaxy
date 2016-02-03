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
    public sealed class CmdUnbanip : Command
    {
        public override string name { get { return "unbanip"; } }
        public override string shortcut { get { return ""; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdUnbanip() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            if (message[0] == '@')
            {
                message = message.Remove(0, 1).Trim();
                Player who = PlayerInfo.Find(message);
                if (who == null)
                {
                    DataTable ip;
                    int tryCounter = 0;
             rerun: try
                    {
                        Database.AddParams("@Name", message);
                        ip = Database.fillData("SELECT IP FROM Players WHERE Name = @Name");
                    }
                    catch (Exception e)
                    {
                        tryCounter++;
                        if (tryCounter < 10)
                        {
                            goto rerun;
                        }
                        else
                        {
                            Server.ErrorLog(e);
                            Player.SendMessage(p, "There was a database error fetching the IP address.  It has been logged.");
                            return;
                        }
                    }
                    if (ip.Rows.Count > 0)
                    {
                        message = ip.Rows[0]["IP"].ToString();
                    }
                    else
                    {
                        Player.SendMessage(p, "Unable to find an IP address for that user.");
                        return;
                    }
                    ip.Dispose();
                }
                else
                {
                    message = who.ip;
                }
            }

            if (message.IndexOf('.') == -1) { Player.SendMessage(p, "Not a valid ip!"); return; }
            if (p != null) if (p.ip == message) { Player.SendMessage(p, "You shouldn't be able to use this command..."); return; }
            if (!Server.bannedIP.Contains(message)) { Player.SendMessage(p, message + " doesn't seem to be banned..."); return; }
            Server.bannedIP.Remove(message); Server.bannedIP.Save("banned-ip.txt", false);

            if (p != null)
            {
                Server.IRC.Say(message.ToLower() + " was un-ip-banned by " + p.name + ".");
                Server.s.Log("IP-UNBANNED: " + message.ToLower() + " by " + p.name + ".");
                Player.GlobalMessage(message + " was &8un-ip-banned" + Server.DefaultColor + " by " + p.color + p.name + Server.DefaultColor + ".");
            }
            else
            {
                Server.IRC.Say(message.ToLower() + " was un-ip-banned by console.");
                Server.s.Log("IP-UNBANNED: " + message.ToLower() + " by console.");
                Player.GlobalMessage(message + " was &8un-ip-banned" + Server.DefaultColor + " by console.");
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/unbanip <ip/player> - Un-bans an ip.  Also accepts a player name when you use @ before the name.");
        }
    }
}
