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
    public sealed class CmdLowlag : Command
    {
        public override string name { get { return "lowlag"; } }
        public override string shortcut { get { return ""; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdLowlag() { }

        public override void Use(Player p, string message)
        {
            if (message != "") { Help(p); return; }

            if (Server.updateTimer.Interval > 1000)
            {
                Server.updateTimer.Interval = 100;
                Player.GlobalMessage("&dLow lag " + Server.DefaultColor + "mode was turned &cOFF" + Server.DefaultColor + ".");
            }
            else
            {
                Server.updateTimer.Interval = 10000;
                Player.GlobalMessage("&dLow lag " + Server.DefaultColor + "mode was turned &aON" + Server.DefaultColor + ".");
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/lowlag - Turns lowlag mode on or off");
        }
    }
}
