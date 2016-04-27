/*
	Copyright 2011 MCForge
	
	Written by Valek / MCLawl team
		
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
    public sealed class CmdOHide : Command
    {
        public override string name { get { return "ohide"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            
            string[] args = message.Split(' ');           
            Player who = PlayerInfo.FindOrShowMatches(p, args[0]);
            if (who == null) return;
            if (who.group.Permission >= p.group.Permission) {
                MessageTooHighRank(p, "hide", false); return;
            }
            
            if (args.Length >= 2 && args[1].CaselessEq("myrank")) {
                who.oHideRank = p.group.Permission;
                Command.all.Find("hide").Use(who, "myrank");
                Player.SendMessage(p, "Used /hide myrank on " + who.ColoredName + "%S.");
            } else {
                Command.all.Find("hide").Use(who, "");
                Player.SendMessage(p, "Used /hide on " + who.ColoredName + "%S.");
            }
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "/ohide <player> - Hides/unhides the player specified.");
            Player.SendMessage(p, "/ohide <player> myrank - Hides/unhides the player specified to players below your rank.");
            Player.SendMessage(p, "Only works on players of lower rank.");
        }
    }
}
