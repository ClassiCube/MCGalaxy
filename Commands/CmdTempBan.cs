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
namespace MCGalaxy.Commands
{
    public sealed class CmdTempBan : Command
    {
        public override string name { get { return "tempban"; } }
        public override string shortcut { get { return "tb"; } }
        public override string type { get { return "moderation"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdTempBan() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            if (message.IndexOf(' ') == -1) message = message + " 60";

            Player who = Player.Find(message.Split(' ')[0]);
            if (who == null) { Player.SendMessage(p, "Could not find player"); return; }
            if (p != null && who.group.Permission >= p.group.Permission) { Player.SendMessage(p, "Cannot ban someone of the same rank"); return; }
            int minutes;
            try
            {
                minutes = int.Parse(message.Split(' ')[1]);
            } catch { Player.SendMessage(p, "Invalid minutes"); return; }
            if (minutes > 1440) { Player.SendMessage(p, "Cannot ban for more than a day"); return; }
            if (minutes < 1) { Player.SendMessage(p, "Cannot ban someone for less than a minute"); return; }
            
            Server.TempBan tBan;
            tBan.name = who.name;
            tBan.allowedJoin = DateTime.Now.AddMinutes(minutes);
            Server.tempBans.Add(tBan);
            who.Kick("Banned for " + minutes + " minutes!");
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/tempban <name> <minutes> - Bans <name> for <minutes>");
            Player.SendMessage(p, "Max time is 1440 (1 day). Default is 60");
            Player.SendMessage(p, "Temp bans will reset on server restart");
        }
    }
}