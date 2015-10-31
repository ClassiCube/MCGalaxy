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
using MCGalaxy.SQL;
namespace MCGalaxy.Commands
{
    public sealed class CmdXColor : Command
    {
        public override string name { get { return "xcolor"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdXColor() { }

        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                Database.AddParams("@Name", p.name);
                Database.executeQuery("UPDATE Players SET color = '' WHERE name = @Name");
                Player.GlobalChat(p, p.color + "*" + p.DisplayName + Server.DefaultColor + "'s color reverted to " + p.group.color + "their group's default" + Server.DefaultColor + ".", false);
                p.color = p.group.color;

                Player.GlobalDie(p, false);
                Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
                p.SetPrefix();
                return;
            }
            string color = c.Parse(message);
            if (color == "") { Player.SendMessage(p, "There is no color \"" + message + "\"."); }
            else if (color == p.color) { Player.SendMessage(p, "You already have that color."); }
            else
            {
                Database.AddParams("@Color", c.Name(color));
                Database.AddParams("@Name", p.name);
                Database.executeQuery("UPDATE Players SET color = @Color WHERE name = @Name");

                Player.GlobalChat(p, p.color + "*" + p.DisplayName + Server.DefaultColor + "'s color changed to " + color + c.Name(color) + Server.DefaultColor + ".", false);
                p.color = color;

                Player.GlobalDie(p, false);
                Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
                p.SetPrefix();
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/xcolor [color]- Changes your nick color.");
            Player.SendMessage(p, "If no [color] is given, you will revert to your group's default.");
            Player.SendMessage(p, "&0black &1navy &2green &3teal &4maroon &5purple &6gold &7silver");
            Player.SendMessage(p, "&8gray &9blue &alime &baqua &cred &dpink &eyellow &fwhite");
        }
        static string Name(string name)
        {
            string ch = name[name.Length - 1].ToString().ToLower();
            if (ch == "s" || ch == "x") { return name + Server.DefaultColor + "'"; }
            else { return name + Server.DefaultColor + "'s"; }
        }
    }
}
