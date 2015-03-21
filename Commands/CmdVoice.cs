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
namespace MCGalaxy.Commands
{
    public sealed class CmdVoice : Command
    {
        public override string name { get { return "voice"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdVoice() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            Player who = Player.Find(message);
            if (who != null)
            {
                if (who.voice)
                {
                    who.voice = false;
                    Player.SendMessage(p, "Removing voice status from " + who.name);
                    who.SendMessage("Your voice status has been revoked.");
                    who.voicestring = "";
                }
                else
                {
                    who.voice = true;
                    Player.SendMessage(p, "Giving voice status to " + who.name);
                    who.SendMessage("You have received voice status.");
                    who.voicestring = "&f+";
                }
            }
            else
            {
                Player.SendMessage(p, "There is no player online named \"" + message + "\"");
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/voice <name> - Toggles voice status on or off for specified player.");
        }
    }
}