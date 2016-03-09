/*
	Copyright 2011 MCForge
	
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
    public sealed class CmdFreeze : Command
    {
        public override string name { get { return "freeze"; } }
        public override string shortcut { get { return "fz"; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdFreeze() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            Player who = PlayerInfo.Find(message);
            if (who == null) { Player.SendMessage(p, "Could not find player."); return; }
            else if (who == p) { Player.SendMessage(p, "Cannot freeze yourself."); return; }
            else if (p != null) { if (who.group.Permission >= p.group.Permission) { Player.SendMessage(p, "Cannot freeze someone of equal or greater rank."); return; } }
            string frozenby = (p == null) ? "<CONSOLE>" : p.color + p.DisplayName;
            
            if (!who.frozen)
            {
                who.frozen = true;
                Player.SendChatFrom(who, who.color + who.DisplayName + Server.DefaultColor + " has been &bfrozen" + Server.DefaultColor + " by " + frozenby + Server.DefaultColor + ".", false);
            	Server.s.Log(who.name + " has been frozen by " + frozenby);
            }
            else
            {
                who.frozen = false;
                Player.SendChatFrom(who, who.color + who.DisplayName + Server.DefaultColor + " has been &adefrosted" + Server.DefaultColor + " by " + frozenby + Server.DefaultColor + ".", false);
                Server.s.Log(who.name + " has been defrosted by " + frozenby);
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/freeze <name> - Stops <name> from moving until unfrozen.");
        }
    }
}
