/*
    Written by Jack1312
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
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdAllowGuns : Command
    {
        public override string name { get { return "allowguns"; } }
        public override string shortcut { get { return "ag"; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdAllowGuns() { }

        public override void Use(Player p, string message)
        {
            if (p == null) { Player.SendMessage(p, "This command can only be used in-game!"); return; }
            if (String.IsNullOrEmpty(message))
            {
                if (p.level.guns)
                {
                    p.level.guns = false;
                    Player.GlobalMessage("&9Gun usage has been disabled on &c" + p.level.name + "&9!");
                    Level.SaveSettings(p.level);

                    foreach (Player pl in Player.players)
                        if (pl.level == p.level)
                            pl.aiming = false;
                }
                else
                {
                    p.level.guns = true;
                    Player.GlobalMessage("&9Gun usage has been enabled on &c" + p.level.name + "&9!");
                    Level.SaveSettings(p.level);
                }
                return;
            }







            if (p != null)
            {
                Level foundLevel;
                if (message == "")
                {
                    if (p.level.guns)
                    {
                        p.level.guns = false;
                        Player.GlobalMessage("&9Gun usage has been disabled on &c" + p.level.name + "&9!");
                        Level.SaveSettings(p.level);
                        foreach (Player pl in Player.players)
                        {
                            if (pl.level.name.ToLower() == p.level.name.ToLower())
                            {
                                pl.aiming = false;
                                p.aiming = false;
                                return;
                            }
                            return;
                        }
                        return;
                    }
                    if (p.level.guns == false)
                    {
                        p.level.guns = true;
                        Player.GlobalMessage("&9Gun usage has been enabled on &c" + p.level.name + "&9!");
                        Level.SaveSettings(p.level);
                        return;
                    }
                }

                if (message != "")
                {
                    foundLevel = Level.Find(message);
                    if (!File.Exists("levels/" + message + ".lvl"))
                    {
                        Player.SendMessage(p, "&9The level, &c" + message + " &9does not exist!"); return;
                    }
                    if (foundLevel.guns)
                    {
                        foundLevel.guns = false;
                        Player.GlobalMessage("&9Gun usage has been disabled on &c" + message + "&9!");
                        Level.SaveSettings(foundLevel);
                        foreach (Player pl in Player.players)
                        {
                            if (pl.level.name.ToLower() == message.ToLower())
                            {
                                pl.aiming = false;
                            }
                            if (p.level.name.ToLower() == message.ToLower())
                            {
                                p.aiming = false;

                            }
                        }
                        return;
                    }
                    else
                    {
                        foundLevel.guns = true;
                        Player.GlobalMessage("&9Gun usage has been enabled on &c" + message + "&9!");
                        Level.SaveSettings(foundLevel);
                        return;
                    }
                }
            }
            if (p == null)
            {
                if (message == null)
                {
                    Player.SendMessage(p, "You must specify a level!");
                    return;
                }
                Level foundLevel;
                foundLevel = Level.Find(message);
                if (!File.Exists("levels/" + message + ".lvl"))
                {
                    Player.SendMessage(p, "The level, " + message + " does not exist!"); return;
                }
                if (foundLevel.guns)
                {
                    foundLevel.guns = false;
                    Player.GlobalMessage("&9Gun usage has been disabled on &c" + message + "&9!");
                    Level.SaveSettings(foundLevel);
                    Player.SendMessage(p, "Gun usage has been disabled on " + message + "!");
                    foreach (Player pl in Player.players)
                    {
                        if (pl.level.name.ToLower() == message.ToLower())
                        {
                            pl.aiming = false;
                            return;
                        }
                    }
                }
                foundLevel.guns = true;
                Player.GlobalMessage("&9Gun usage has been enabled on &c" + message + "&9!");
                Level.SaveSettings(foundLevel);
                Player.SendMessage(p, "Gun usage has been enabled on " + message + "!");
                return;
            }
        }



        public override void Help(Player p)
        {
            Player.SendMessage(p, "/allowguns - Allow/disallow guns and missiles on the specified level. If no message is given, the current level is taken.");
            Player.SendMessage(p, "Note: If guns are allowed on a map, and /allowguns is used, all guns and missiles will be disabled.");
        }
    }
}