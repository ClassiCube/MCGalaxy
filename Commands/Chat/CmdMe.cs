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
using System;
namespace MCGalaxy.Commands
{
    public sealed class CmdMe : Command
    {
        public override string name { get { return "me"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdMe() { }
        public override void Use(Player p, string message)
        {
            if (message == "") { Player.SendMessage(p, "You"); return; }
            if (p == null) { Player.SendMessage(p, "This command can only be used in-game!"); return; }

            if (p.muted) { Player.SendMessage(p, "You are currently muted and cannot use this command."); return; }
            if (Server.chatmod && !p.voice) { Player.SendMessage(p, "Chat moderation is on, you cannot emote."); return; }

            if (Server.worldChat)
            {
                Player.GlobalChat(p, p.color + "*" + Chat.StripColours(p.DisplayName) + p.color + " " + message, false);
            }
            else
            {
                Chat.GlobalChatLevel(p, p.color + "*" + Chat.StripColours(p.DisplayName) + p.color + " " + message, false);
            }
            //IRCBot.Say("*" + p.name + " " + message);
            Server.IRC.Say("*" + p.DisplayName + " " + message);


        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "What do you need help with, m'boy?! Are you stuck down a well?!");
        }
    }
}
