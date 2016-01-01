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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using MCGalaxy.SQL;
namespace MCGalaxy.Commands {
    public sealed class CmdBanip : Command {
        public override string name { get { return "banip"; } }
        public override string shortcut { get { return "bi"; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdBanip() { }

        public override void Use(Player p, string message) {
            if (String.IsNullOrEmpty(message.Trim())) { Help(p); return; }
            message = Chat.EscapeColours(message);
            string name = "";
            if (message[0] == '@') {
                message = message.Remove(0, 1).Trim();
                Player who = Player.Find(message);

                if (who == null) {
                    DataTable ip;
                    int tryCounter = 0;
                rerun: try {
                        Database.AddParams("@Name", message);
                        ip = Database.fillData("SELECT IP FROM Players WHERE Name = @Name");
                    } catch (Exception e) {
                        tryCounter++;
                        if (tryCounter < 10)
                            goto rerun;
                        else {
                            Server.ErrorLog(e);
                            return;
                        }
                    }
                    if (ip.Rows.Count > 0) {
                        name = message.ToLower();
                        message = ip.Rows[0]["IP"].ToString();
                    } else {
                        Player.SendMessage(p, "Unable to find an IP address for that user.");
                        return;
                    }
                    ip.Dispose();
                } else {
                    name = who.name.ToLower();
                    message = who.ip;
                }
            } else {
                Player who = Player.Find(message);
                if (who != null) {
                    name = who.name.ToLower();
                    message = who.ip;
                }
            }

            if (message.Equals("127.0.0.1")) { Player.SendMessage(p, "You can't ip-ban the server!"); return; }
            if (message.IndexOf('.') == -1) { Player.SendMessage(p, "Invalid IP!"); return; }
            if (message.Split('.').Length != 4) { Player.SendMessage(p, "Invalid IP!"); return; }
            if (p != null && p.ip == message) { Player.SendMessage(p, "You can't ip-ban yourself.!"); return; }
            if (Server.bannedIP.Contains(message)) { Player.SendMessage(p, message + " is already ip-banned."); return; }

            // Check if IP belongs to an op+
            // First get names of active ops+ with that ip
            List<string> opNamesWithThatIP = (from pl in Player.players where (pl.ip == message && pl.@group.Permission >= LevelPermission.Operator) select pl.name).ToList();
            // Next, add names from the database
            Database.AddParams("@IP", message);
            DataTable dbnames = Database.fillData("SELECT Name FROM Players WHERE IP = @IP");

            foreach (DataRow row in dbnames.Rows) {
                opNamesWithThatIP.Add(row[0].ToString());
            }
            dbnames.Dispose();

            if (opNamesWithThatIP != null && opNamesWithThatIP.Count > 0) {
                // We have at least one op+ with a matching IP
                // Check permissions of everybody who matched that IP
                foreach (string opname in opNamesWithThatIP) {
                    // Console can ban anybody else, so skip this section
                    if (p != null) {
                        // If one of these guys matches a player with a higher rank don't allow the ipban to proceed! 
                        Group grp = Group.findPlayerGroup(opname);
                        if (grp != null) {
                            if (grp.Permission >= p.group.Permission) {
                                Player.SendMessage(p, "You can only ipban IPs used by players with a lower rank.");
                                Player.SendMessage(p, Server.DefaultColor + opname + "(" + grp.color + grp.name + Server.DefaultColor + ") uses that IP.");
                                Server.s.Log(p.name + "failed to ipban " + message + " - IP is also used by: " + opname + "(" + grp.name + ")");
                                return;
                            }
                        }
                    }
                }
            }



            if (p != null) {
                Server.IRC.Say(message.ToLower() + " was ip-banned by " + p.name + ".");
                Server.s.Log("IP-BANNED: " + message.ToLower() + " by " + p.name + ".");
                Player.GlobalMessage(message + " was &8ip-banned" + Server.DefaultColor + " by " + p.color + p.name + Server.DefaultColor + ".");
            } else {
                Server.IRC.Say(message.ToLower() + " was ip-banned by console.");
                Server.s.Log("IP-BANNED: " + message.ToLower() + " by console.");
                Player.GlobalMessage(message + " was &8ip-banned" + Server.DefaultColor + " by console.");
            }
            Server.bannedIP.Add(message);
            Server.bannedIP.Save("banned-ip.txt", false);

            /*
            foreach (Player pl in Player.players) {
                if (message == pl.ip) { pl.Kick("Kicked by ipban"); }
            }*/
        }
        public override void Help(Player p) {
            Player.SendMessage(p, "/banip <ip/name> - Bans an ip. Also accepts a player name when you use @ before the name.");
        }
    }
}
