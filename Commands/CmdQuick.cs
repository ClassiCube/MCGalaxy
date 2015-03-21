/*
    Written By Jack1312
 
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
using System;
namespace MCGalaxy.Commands
{
    public sealed class CmdQuick : Command
    {
        public override string name { get { return "quick"; } }
        public override string shortcut { get { return "q"; } }
        public override string type { get { return "build"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdQuick() { }

        public override void Use(Player p, string message)
        {
            string msg = String.Empty;
            string type = String.Empty;
            if (message == "")
            {
                type = "cuboid";
            }
            else
            {
                type = message.Split(' ')[0];
            }
            try
            {
                msg = message.Replace(type + " ", "");
            }
            catch { }
            if (!p.group.CanExecute(Command.all.Find(type)))
            {
                Player.SendMessage(p, "You cannot execute the actual command, therefore you cannot use the quick version of it!");
                return;
            }
            if (p.level.Instant == false)
            {
                p.level.Instant = true;
            }
            if (type == "cuboid")
            {
                CmdCuboid.wait = 0;
                Command.all.Find(type).Use(p, msg);
                while (CmdCuboid.wait == 0)
                {
                }
                if (CmdCuboid.wait != 1)
                {
                    if (CmdCuboid.wait == 2)
                    {
                        Command.all.Find("reveal").Use(p, "all");
                        if (p.level.Instant == true)
                        {
                            p.level.Instant = false;
                        }
                    }
                }
                if (p.level.Instant == true)
                {
                    p.level.Instant = false;
                }
                return;
            }
            if (type == "replace")
            {
                CmdReplace.wait = 0;
                Command.all.Find(type).Use(p, msg);
                while (CmdReplace.wait == 0)
                {
                }
                if (CmdReplace.wait != 1)
                {
                    if (CmdReplace.wait == 2)
                    {
                        Command.all.Find("reveal").Use(p, "all");
                        if (p.level.Instant == true)
                        {
                            p.level.Instant = false;
                        }
                    }
                }
                if (p.level.Instant == true)
                {
                    p.level.Instant = false;
                }
                return;
            }
            if (type == "replaceall")
            {
                CmdReplaceAll.wait = 0;
                Command.all.Find(type).Use(p, msg);
                while (CmdReplaceAll.wait == 0)
                {
                }
                if (CmdReplaceAll.wait != 1)
                {
                    if (CmdReplaceAll.wait == 2)
                    {
                        Command.all.Find("reveal").Use(p, "all");
                        if (p.level.Instant == true)
                        {
                            p.level.Instant = false;
                        }
                    }
                }
                if (p.level.Instant == true)
                {
                    p.level.Instant = false;
                }
                return;
            }
            if (type == "replacenot")
            {
                CmdReplaceNot.wait = 0;
                Command.all.Find(type).Use(p, msg);
                while (CmdReplaceNot.wait == 0)
                {
                }
                if (CmdReplaceNot.wait != 1)
                {
                    if (CmdReplaceNot.wait == 2)
                    {
                        Command.all.Find("reveal").Use(p, "all");
                        if (p.level.Instant == true)
                        {
                            p.level.Instant = false;
                        }
                    }
                }
                if (p.level.Instant == true)
                {
                    p.level.Instant = false;
                }
                return;
            }
            if (type == "spheroid")
            {
                CmdSpheroid.wait = 0;
                Command.all.Find(type).Use(p, msg);
                while (CmdSpheroid.wait == 0)
                {
                }
                if (CmdSpheroid.wait != 1)
                {
                    if (CmdSpheroid.wait == 2)
                    {
                        Command.all.Find("reveal").Use(p, "all");
                        if (p.level.Instant == true)
                        {
                            p.level.Instant = false;
                        }
                    }
                }
                if (p.level.Instant == true)
                {
                    p.level.Instant = false;
                }
                return;
            }
            if (type == "pyramid")
            {
                CmdPyramid.wait = 0;
                Command.all.Find(type).Use(p, msg);
                while (CmdPyramid.wait == 0)
                {
                }
                if (CmdPyramid.wait != 1)
                {
                    if (CmdPyramid.wait == 2)
                    {
                        Command.all.Find("reveal").Use(p, "all");
                        if (p.level.Instant == true)
                        {
                            p.level.Instant = false;
                        }
                    }
                }
                if (p.level.Instant == true)
                {
                    p.level.Instant = false;
                }
                return;
            }
            Player.SendMessage(p, "No such quickcuboid type!");
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/quick [Function] [Block] [Type] - Cuboids the selected function instantly.");
            Player.SendMessage(p, "Functions: cuboid, pyramid, replace, replaceall, replacenot, spheroid.");
            Player.SendMessage(p, "if none is specified, quick cuboid will be used! Shortcut: /q");
        }
    }
}