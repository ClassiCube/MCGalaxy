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
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdHide : Command
    {
        public override string name { get { return "hide"; } }
        public override string shortcut { get { return ""; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdHide() { }

        public override void Use(Player p, string message)
        {
            if (p == null) { Player.SendMessage(p, "This command can only be used in-game!"); return; }
            if (message == "check")
            {
                if (p.hidden)
                {
                    Player.SendMessage(p, "You are currently hidden!");
                    return;
                }
                else
                {
                    Player.SendMessage(p, "You are not currently hidden!");
                    return;
                }
            }
            else
                if (message != "")
                    if (p.possess != "")
                    {
                        Player.SendMessage(p, "Stop your current possession first.");
                return;
            }
            Command opchat = Command.all.Find("opchat");
            Command adminchat = Command.all.Find("adminchat");
            p.hidden = !p.hidden;
            if (p.hidden)
            {
                Player.GlobalDie(p, true);
                Chat.GlobalMessageOps("To Ops -" + p.color + p.DisplayName + Server.DefaultColor + "- is now &finvisible" + Server.DefaultColor + ".");
                Player.GlobalChat(p, "&c- " + p.color + p.prefix + p.DisplayName + Server.DefaultColor + " " + 
                                  (File.Exists("text/logout/" + p.name + ".txt") ? CP437Reader.ReadAllText("text/logout/" + p.name + ".txt") : "Disconnected."), false);
                Server.IRC.Say(p.DisplayName + " left the game (Disconnected.)");
                if (!p.opchat)
                {
                    opchat.Use(p, message);
                }
                //Player.SendMessage(p, "You're now &finvisible&e.");
            }
            else
            {
                Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
                Chat.GlobalMessageOps("To Ops -" + p.color + p.DisplayName + Server.DefaultColor + "- is now &8visible" + Server.DefaultColor + ".");
                Player.GlobalChat(p, "&a+ " + p.color + p.prefix + p.DisplayName + Server.DefaultColor + " " + 
                                  (File.Exists("text/login/" + p.name + ".txt") ? CP437Reader.ReadAllText("text/login/" + p.name + ".txt") : "joined the game."), false);
                Server.IRC.Say(p.DisplayName + " joined the game");
                if (p.opchat)
                {
                    opchat.Use(p, message);
                }
                if (p.adminchat)
                {
                    adminchat.Use(p, message);
                }
                //Player.SendMessage(p, "You're now &8visible&e.");
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/hide - Toggles your visibility to other players, also toggles opchat.");
        }
    }
}
