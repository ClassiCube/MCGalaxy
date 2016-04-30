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

        public override void Use(Player p, string message) {
        	if (message != "") { Help(p); return; }
        	
        	Player.SendMessage(p, "This server's name is &b" + Server.name + "%S.");
        	Player.SendMessage(p, "&a" + Player.number + " %Splayers online, &8" 
        	                   + Player.GetBannedCount() + " banned%S players total.");
        	Player.SendMessage(p, "&a" + LevelInfo.Loaded.Count + " %Slevels currently loaded.");
        	Player.SendMessage(p, "This server's currency is: " + Server.moneys);
        	Player.SendMessage(p, "This server runs &bMCGalaxy &a" + Server.VersionString + 
        	                   "%S, which is based on &bMCForge %Sand &bMCLawl%S.");
        	Command.all.Find("devs").Use(p, "");
        	
        	TimeSpan up = DateTime.UtcNow - Server.StartTime;
        	Player.SendMessage(p, "Time online: &b" + WhoInfo.Shorten(up, true));
        	Player.SendMessage(p, "Player positions are updated every &b" 
        	                   + Server.updateTimer.Interval + " %Smilliseconds.");
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/info - Displays the server information.");
        }
    }
}
