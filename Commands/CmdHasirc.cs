/*
	Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
 
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
namespace MCGalaxy.Commands
{
    public sealed class CmdHasirc : Command
    {
        public override string name { get { return "hasirc"; } }
        public override string shortcut { get { return "irc"; } }
        public override string type { get { return "information"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdHasirc() { }

        public override void Use(Player p, string message)
        {
            if (message != "")
            {
                Help(p);
                return;
            }
            else
            {
                string hasirc;
                string ircdetails = "";
                if (Server.irc)
                {
                    hasirc = "&aEnabled" + Server.DefaultColor + ".";
                    ircdetails = Server.ircServer + " > " + Server.ircChannel;
                }
                else
                {
                    hasirc = "&cDisabled" + Server.DefaultColor + ".";
                }
                Player.SendMessage(p, "IRC is " + hasirc);
                if (ircdetails != "")
                {
                    Player.SendMessage(p, "Location: " + ircdetails);
                }
            }
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/hasirc - Denotes whether or not the server has IRC active.");
            Player.SendMessage(p, "If IRC is active, server and channel are also displayed.");
        }
    }
}
