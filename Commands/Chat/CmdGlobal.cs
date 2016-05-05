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
    public sealed class CmdGlobal : Command
    {
        public override string name { get { return "global"; } }
        public override string shortcut { get { return "gc"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdGlobal() { }
        //bla
        public override void Use(Player p, string message)
        {
            if (p != null && !p.verifiedName) { 
        	    Player.Message(p, "You cannot use global chat, because your name is not verified."); return; 
        	}

            if (String.IsNullOrEmpty(message)) { 
                p.InGlobalChat = !p.InGlobalChat;
                Player.Message(p, p.InGlobalChat ? "%aGlobal Chat enabled" : "%cGlobal Chat Disabled");
                return;
             }

            if (!Server.UseGlobalChat) { Player.Message(p, "Global Chat is disabled."); return; }
            if (p != null)
            {
            	if (p.muted) { Player.Message(p, "You are muted."); return; }
            	if (p.ignoreGlobal || p.ignoreAll) { Player.Message(p, "You cannot use Global Chat while you have it muted."); return; }
            	if (Server.chatmod && !p.voice) { Player.Message(p, "You cannot use Global Chat while in Chat Moderation!"); return; }
            	if (!Server.gcaccepted.Contains(p.name.ToLower())) { ShowRules(p, true); return; }
            }            //Server.GlobalChat.Say((p != null ? p.name + ": " : "Console: ") + message, p);
            Server.GlobalChat.Say(p == null ? "Console: " + message : p.name + ": " + message, p);
            Player.GlobalMessage("%G<[Global] " + (p != null ? p.name + ": " : "Console: ") + "&f" + (Server.profanityFilter ? ProfanityFilter.Parse(message) : message), true);

        }
        
        public static void ShowRules(Player p, bool showAccept) {
            Player.Message(p, "&cBy using the Global Chat you agree to the following rules:");
            Player.Message(p, "1. No Spamming");
            Player.Message(p, "2. No Advertising (Trying to get people to your server)");
            Player.Message(p, "3. No links");
            Player.Message(p, "4. No Excessive Cursing (You are allowed to curse, but not pointed at anybody)");
            Player.Message(p, "5. No use of $ Variables.");
            Player.Message(p, "6. English only. No exceptions.");
            Player.Message(p, "7. Be respectful");
            Player.Message(p, "8. Do not ask for ranks");
            Player.Message(p, "9. Do not ask for a server name");
            Player.Message(p, "10. Use common sense.");
            Player.Message(p, "11. Don't say any server name");
            if (showAccept)
                Player.Message(p, "&3Type /gcaccept to accept these rules");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/global [message] - Send a message to Global Chat.");
        }
    }
}
