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
using System.IO;
using System.Threading;
namespace MCGalaxy.Commands
{
    public sealed class CmdLockdown : Command
    {
        public override string name { get { return "lockdown"; } }
        public override string shortcut { get { return "ld"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message)
        {
            if (!Directory.Exists("text/lockdown"))
            {
                Player.SendMessage(p, "Could not locate the folder creating one now.");
                Directory.CreateDirectory("text/lockdown");
                Directory.CreateDirectory("text/lockdown/map");
                Player.SendMessage(p, "Added the settings for the command");
            }

            string[] param = message.Split(' ');


            if (param.Length == 2 && (param[0] == "map" || param[0] == "player"))
            {
                if (param[0] == "map")
                {
                    if (!Directory.Exists("text/lockdown/map"))
                    {
                        p.SendMessage("Could not locate the map folder, creating one now.");
                        Directory.CreateDirectory("text/lockdown/map");
                        p.SendMessage("Added the map settings Directory within 'text/lockdown'!");
                    }

                    string filepath = "text/lockdown/map/" + param[1] + "";
                    bool mapIsLockedDown = File.Exists(filepath);

                    if (!mapIsLockedDown)
                    {
                        File.Create(filepath).Dispose();
                        Player.GlobalMessage("The map " + param[1] + " has been locked");
                        Chat.GlobalMessageOps("Locked by: " + ((p == null) ? "Console" : p.name));
                    }
                    else
                    {
                        File.Delete(filepath);
                        Player.GlobalMessage("The map " + param[1] + " has been unlocked");
                        Chat.GlobalMessageOps("Unlocked by: " + ((p == null) ? "Console" : p.name));
                    }
                }

                if (param[0] == "player")
                {
                    Player who = Player.Find(param[1]);

                    if (who == null)
                    {
                        Player.SendMessage(p, "There is no player with such name online");
                        return;
                    }

                    if (!who.jailed)
                    {
                        if (p != null)
                        {
                            if (who.group.Permission >= p.group.Permission)
                            {
                                Player.SendMessage(p, "Cannot lock down someone of equal or greater rank.");
                                return;
                            }
                        }
                        if (p != null && who.level != p.level)
                        {
                            Player.SendMessage(p, "Moving player to your map...");
                            Command.all.Find("goto").Use(who, p.level.name);
                            int waits = 0;
                            while (who.Loading)
                            {
                                Thread.Sleep(500);
                                // If they don't load in 10 seconds, eff it.
                                if (waits++ == 20)
                                    break;
                            }
                        }
                        who.jailed = true;
                        Player.GlobalMessage(who.color + who.DisplayName + Server.DefaultColor + " has been locked down!");
                        Chat.GlobalMessageOps("Locked by: " + ((p == null) ? "Console" : p.name));
                        return;
                    }
                    else
                    {
                        who.jailed = false;
                        Player.GlobalMessage(who.color + who.DisplayName + Server.DefaultColor + " has been unlocked.");
                        Chat.GlobalMessageOps("Unlocked by: " + ((p == null) ? "Console" : p.name));
                        return;
                    }
                }
            }
            else
            {
                Help(p);
                return;
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, " use /lockdown map <mapname> to lock it down.");
            Player.SendMessage(p, " use /lockdown player <playername> to lock down player."); return;
        }
    }
}
