/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the Educational Community License, Version 2.0 and
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
    public sealed class CmdAfk : Command
    {
        public override string name { get { return "afk"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public static string keywords { get { return ""; } }
        public CmdAfk() { }

        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            if (Server.chatmod) {
                Player.SendMessage(p, "You cannot use /afk while chat moderation is enabled");
                return;
            }
            if (p != null && p.muted) { Player.SendMessage(p, "Cannot use /afk while muted."); return; }

            if (message == "list") {
                foreach (string s in Server.afkset) {
            	    Player pl = PlayerInfo.FindExact(s);
            	    if (pl == null || !Entities.CanSee(p, pl)) continue;
                    Player.SendMessage(p, s);
                }
                return;
            }
            
            if (p.joker)
                message = "";
            if (!Server.afkset.Contains(p.name)) {
                p.afkStart = DateTime.Now;
                Server.afkset.Add(p.name);
                if (p.muted)
                    message = "";
                Player.GlobalMessage("-" + p.ColoredName + "%S- is AFK " + message);
                //IRCBot.Say(p.name + " is AFK " + message);
                Server.IRC.Say(p.DisplayName + " is AFK " + message);
            } else {
                Server.afkset.Remove(p.name);
                Player.GlobalMessage("-" + p.ColoredName + "%S- is no longer AFK");
                //IRCBot.Say(p.name + " is no longer AFK");
                Server.IRC.Say(p.DisplayName + " is no longer AFK");
            }
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/afk <reason> - mark yourself as AFK. Use again to mark yourself as back");
        }
    }
}
