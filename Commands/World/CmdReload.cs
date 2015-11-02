/*
	Copyright 2011 MCGalaxy
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.osedu.org/licenses/ECL-2.0
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdReload : Command
    {
        public override string name { get { return "reload"; } }
        public override string shortcut { get { return "rd"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdReload() { }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/reload <map> - Reloads the specified map. Uses the current map if no message is given.");
        }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            {
                if (!File.Exists("levels/" + message + ".lvl"))
                {
                    Player.SendMessage(p, "The specified level does not exist!");
                    return;
                }
                if (Server.mainLevel.name == message)
                {
                    Player.SendMessage(p, "You cannot reload the main level!");
                    return;
                }
                if (p == null)
                {
                    {
                        foreach (Player pl in Player.players)
                        {
                            if (pl.level.name.ToLower() == message.ToLower())
                            {
                                Command.all.Find("unload").Use(p, message);
                                Command.all.Find("load").Use(p, message);
                                Command.all.Find("goto").Use(pl, message);
                            }
                        }
                        Player.GlobalMessage("&cThe map, " + message + " has been reloaded!");
                        //IRCBot.Say("The map, " + message + " has been reloaded.");
                        Server.IRC.Say("The map, " + message + " has been reloaded.");
                        Server.s.Log("The map, " + message + " was reloaded by the console");
                        return;
                    }
                }
                if (p != null)
                {
                    {
                        foreach (Player pl in Player.players)
                        {
                            if (pl.level.name.ToLower() == message.ToLower())
                            {
                                p.ignorePermission = true;
                                Command.all.Find("unload").Use(p, message);
                                Command.all.Find("load").Use(p, message);
                                Command.all.Find("goto").Use(pl, message);
                            }
                        }
                        Player.GlobalMessage("&cThe map, " + message + " has been reloaded!");
						//IRCBot.Say("The map, " + message + " has been reloaded.");
                        Server.IRC.Say("The map, " + message + " has been reloaded.");
						Server.s.Log("The map, " + message + " was reloaded by " + p.name);
						p.ignorePermission = false;
						return;
					}
				}
			}
		}
	}
}
