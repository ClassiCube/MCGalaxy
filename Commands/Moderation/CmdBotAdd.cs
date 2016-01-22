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
namespace MCGalaxy.Commands
{
    public sealed class CmdBotAdd : Command
    {
        public override string name { get { return "botadd"; } }
        public override string shortcut { get { return ""; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdBotAdd() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            if (p != null)
            {
                if (!Player.ValidName(message)) { Player.SendMessage(p, "bot name " + message + " not valid!"); return; }
                PlayerBot bot = new PlayerBot(message, p.level, p.pos[0], p.pos[1], p.pos[2], p.rot[0], 0);
                PlayerBot.Add(bot);
                //who.SendMessage("You were summoned by " + p.color + p.name + "&e.");
                return;
            }
            Player.SendMessage(p, "This command can only be used in-game");
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/botadd <name> - Add a  new bot at your position.");
        }
    }
}
