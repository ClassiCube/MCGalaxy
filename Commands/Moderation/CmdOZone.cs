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
    public sealed class CmdOZone : Command
    {
        public override string name { get { return "ozone"; } }
        public override string shortcut { get { return "oz"; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override bool museumUsable { get { return false; } }
        public override void Help(Player p)
        {
            Player.Message(p, "/ozone <rank/player> - Zones the entire map to <rank/player>");
            Player.Message(p, "To delete a zone, just use /zone del anywhere on the map");
        }
        public override void Use(Player p, string message)
        {
            if (message == "") { this.Help(p); }
            else
            {
                int x2 = p.level.Width - 1;
                int y2 = p.level.Height - 1;
                int z2 = p.level.Length - 1;
                Command zone = Command.all.Find("zone");
                Command click = Command.all.Find("click");
                zone.Use(p, "add " + message);
                click.Use(p, 0 + " " + 0 + " " + 0);
                click.Use(p, x2 + " " + y2 + " " + z2);
            }
        }
    }
}
