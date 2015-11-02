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
    public sealed class CmdInfo : Command
    {
        public override string name { get { return "info"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdInfo() { }

        public override void Use(Player p, string message)
        {
            if (message != "")
            {
                Help(p);
            }
            else
            {
                Player.SendMessage(p, "This server's name is &b" + Server.name + Server.DefaultColor + ".");
                Player.SendMessage(p, "There are currently " + Player.number + " players on this server");
                Player.SendMessage(p, "This server currently has $banned people that are &8banned" + Server.DefaultColor + ".");
                Player.SendMessage(p, "This server currently has " + Server.levels.Count + " levels loaded.");
                Player.SendMessage(p, "This server's currency is: " + Server.moneys);
                Player.SendMessage(p, "This server runs on &bMCGalaxy" + Server.DefaultColor + ", which is based on &bMCLawl" + Server.DefaultColor + ".");
                Player.SendMessage(p, "This server's version: &a" + Server.VersionString);
                Command.all.Find("devs").Use(p, "");
                TimeSpan up = DateTime.Now - Server.timeOnline;
                string upTime = "Time online: &b";
                if (up.Days == 1) upTime += up.Days + " day, ";
                else if (up.Days > 0) upTime += up.Days + " days, ";
                if (up.Hours == 1) upTime += up.Hours + " hour, ";
                else if (up.Days > 0 || up.Hours > 0) upTime += up.Hours + " hours, ";
                if (up.Minutes == 1) upTime += up.Minutes + " minute and ";
                else if (up.Hours > 0 || up.Days > 0 || up.Minutes > 0) upTime += up.Minutes + " minutes and ";
                if (up.Seconds == 1) upTime += up.Seconds + " second";
                else upTime += up.Seconds + " seconds";
                Player.SendMessage(p, upTime);
                if (Server.updateTimer.Interval > 1000) Player.SendMessage(p, "This server is currently in &5Low Lag" + Server.DefaultColor + " mode.");
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/info - Displays the server information.");
        }
    }
}
