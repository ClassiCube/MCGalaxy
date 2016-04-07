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

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            Player who = PlayerInfo.FindOrShowMatches(p, message);
            if (who == null) return;
            if (p == who) { Player.SendMessage(p, "Cannot freeze yourself."); return; }
            if (p != null && who.group.Permission >= p.group.Permission) { 
                MessageTooHighRank(p, "freeze", false); return; 
            }
            
            string frozenby = (p == null) ? "(console)" : p.ColoredName;
            if (!who.frozen) {
                Player.SendChatFrom(who, who.ColoredName + " %Shas been &bfrozen %Sby " + frozenby + "%S.", false);
            	Server.s.Log(who.name + " has been frozen by " + frozenby);
            } else {
                Player.SendChatFrom(who, who.ColoredName + " %Shas been &adefrosted %Sby " + frozenby + "%S.", false);
                Server.s.Log(who.name + " has been defrosted by " + frozenby);
            }
            who.frozen = !who.frozen;
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/freeze <name> - Stops <name> from moving until unfrozen.");
        }
    }
}
