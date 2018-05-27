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
using MCGalaxy.Events.PlayerEvents;

namespace MCGalaxy.Commands.Chatting {
    public sealed class CmdAfk : MessageCmd {
        public override string name { get { return "AFK"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message) { ToggleAfk(p, message); }
        internal static void ToggleAfk(Player p, string message) {
            if (p.joker) message = "";
            p.AutoAfk = false;
            p.IsAfk = !p.IsAfk;
            p.afkMessage = p.IsAfk ? message : null;
            TabList.Update(p, true);
            p.LastAction = DateTime.UtcNow;

            bool cantSend = p.muted || (Server.chatmod && !p.voice);
            if (p.IsAfk) {
                if (cantSend) {
                    Player.Message(p, "You are now marked as being AFK.");
                } else {
                    ShowMessage(p, "-" + p.ColoredName + "%S- is AFK " + message);
                    p.CheckForMessageSpam();
                }
                p.AFKCooldown = DateTime.UtcNow.AddSeconds(2);
            } else {
                if (cantSend) {
                    Player.Message(p, "You are no longer marked as being AFK.");
                } else {
                    ShowMessage(p, "-" + p.ColoredName + "%S- is no longer AFK");
                    p.CheckForMessageSpam();
                }
            }
        }
        
        static void ShowMessage(Player p, string message) {
            bool announce = !p.hidden && ServerConfig.IRCShowAFK;
            Chat.MessageGlobalOrLevel(p, message, Chat.FilterVisible(p), announce);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/AFK <reason>");
            Player.Message(p, "%HMarks yourself as AFK. Use again to mark yourself as back");
        }
    }
}
