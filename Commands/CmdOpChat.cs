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
    public sealed class CmdOpChat : Command
    {
        public override string name { get { return "opchat"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdOpChat() { }

        public override void Use(Player p, string message)
        {
            p.opchat = !p.opchat;
            if (p.opchat) Player.SendMessage(p, "All messages will now be sent to OPs only");
            else Player.SendMessage(p, "OP chat turned off");
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/opchat - Makes all messages sent go to OPs by default");
        }
    }
}