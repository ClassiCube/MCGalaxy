/*
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
//Thanks to the /nyan creators...
using System.IO;
using System.Net;
namespace MCGalaxy.Commands
{
    public sealed class CmdPumpkin : Command
    {
        public override string name { get { return "pumpkin"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override void Use(Player p, string message)
        {
            if (p == null) { Player.SendMessage(p, "This command can only be used in-game!"); return; }
            if (!Directory.Exists("extra/copy"))
                Directory.CreateDirectory("extra/copy");

            if (!File.Exists("extra/copy/pumpkin.copy"))
            {
                Player.SendMessage(p, "Pumpkin copy doesn't exist. Downloading...");
                try
                {
                    using (WebClient WEB = new WebClient())
                        WEB.DownloadFile("http://www.comingsoon.tk/uploads/copy/pumpkin.copy", "extra/copy/pumpkin.copy");
                }
                catch
                {
                    Player.SendMessage(p, "Sorry, downloading failed. Please try again later.");
                    return;
                }
            }
            Command.all.Find("retrieve").Use(p, "pumpkin");
            Command.all.Find("paste").Use(p, "");
            ushort[] loc = p.getLoc(false);
            ushort loc0 = (ushort)(loc[0] + 3);
            ushort loc2 = (ushort)(loc[2] - 8);
            Command.all.Find("click").Use(p, loc0 + " " + loc[1] + " " + loc2);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/pumpkin - places a pumpkin at your location!");
            Player.SendMessage(p, "&4Warning! &cThis will go through other blocks, and replace them!");
        }
    }
}