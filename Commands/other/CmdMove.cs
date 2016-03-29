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
using System;
namespace MCGalaxy.Commands
{
    public sealed class CmdMove : Command
    {
        public override string name { get { return "move"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdMove() { }

        public override void Use(Player p, string message)
        {
            // /move name map
            // /move x y z
            // /move name x y z

            string[] param = message.Split(' ');

            if (param.Length < 1 || param.Length > 4) { Help(p); return; }

            // /move name
            if (param.Length == 1)
            {
                // Use main world by default
                // Add the world name to the 2nd param so that the IF block below is used
                param = new string[] { param[0], Server.mainLevel.name };
            }

            if (param.Length == 2)     // /move name map
            {
                Player who = PlayerInfo.FindOrShowMatches(p, param[0]);
                Level where = LevelInfo.Find(param[1]);
                if (who == null) return;
                if (where == null) { Player.SendMessage(p, "Could not find level specified"); return; }
                if (p != null && who.group.Permission > p.group.Permission) { Player.SendMessage(p, "Cannot move someone of greater rank"); return; }

                Command.all.Find("goto").Use(who, where.name);
                if (who.level == where)
                    Player.SendMessage(p, "Sent " + who.color + who.name + Server.DefaultColor + " to " + where.name);
                else
                    Player.SendMessage(p, where.name + " is not loaded");
            }
            else
            {
                // /move name x y z
                // /move x y z

                Player who;

                if (param.Length == 4)
                {
                    who = PlayerInfo.FindOrShowMatches(p, param[0]);
                    if (who == null) return;
                    if (p != null && who.group.Permission > p.group.Permission) { Player.SendMessage(p, "Cannot move someone of greater rank"); return; }
                    message = message.Substring(message.IndexOf(' ') + 1);
                }
                else
                {
                    who = p;
                }

                try
                {
                    ushort x = System.Convert.ToUInt16(message.Split(' ')[0]);
                    ushort y = System.Convert.ToUInt16(message.Split(' ')[1]);
                    ushort z = System.Convert.ToUInt16(message.Split(' ')[2]);
                    x *= 32; x += 16;
                    y *= 32; y += 32;
                    z *= 32; z += 16;
                    who.SendPos(0xFF, x, y, z, p.rot[0], p.rot[1]);
                    if (p != who) Player.SendMessage(p, "Moved " + who.color + who.name);
                }
                catch { Player.SendMessage(p, "Invalid co-ordinates"); }
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/move <player> <map> <x> <y> <z> - Move <player> to <map> or given coordinates");
            Player.SendMessage(p, "<map> must be blank if x, y or z is used and vice versa");
            Player.SendMessage(p, "If <map> is empty, the main level be assumed");
        }
    }
}
