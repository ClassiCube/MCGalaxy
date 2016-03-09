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
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdXhide : Command
    {
        public override string name { get { return "xhide"; } }
        public override string shortcut { get { return ""; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message)
        {
            if (p == null) { MessageInGameOnly(p); return; }
            if (message != "") { Help(p); return; }
            if (p.possess != "")
            {
                Player.SendMessage(p, "Stop your current possession first.");
                return;
            }
            Command adminchat = Command.all.Find("adminchat");
            p.hidden = !p.hidden;
            if (p.hidden)
            {
                Player.GlobalDespawn(p, true);
                Player.SendChatFrom(p, "&c- " + p.color + p.prefix + p.DisplayName + Server.DefaultColor + " " + 
                                  (File.Exists("text/logout/" + p.name + ".txt") ? CP437Reader.ReadAllText("text/logout/" + p.name + ".txt") : "Disconnected."), false);
                Server.IRC.Say(p.name + " left the game (Disconnected.)");
                if (!p.adminchat)
                {
                    adminchat.Use(p, message);
                }

            }
            else
            {
                Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false, "");
                Player.SendChatFrom(p, "&a+ " + p.color + p.prefix + p.DisplayName + Server.DefaultColor + " " + 
                                  (File.Exists("text/login/" + p.name + ".txt") ? CP437Reader.ReadAllText("text/login/" + p.name + ".txt") : "joined the game."), false);
                Server.IRC.Say(p.name + " joined the game");
                if (p.adminchat)
                {
                    adminchat.Use(p, message);
                }
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/xhide - like /hide, only it doesn't send a message to ops.");
        }
    }
}

