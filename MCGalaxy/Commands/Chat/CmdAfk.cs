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
namespace MCGalaxy.Commands.Chatting {
    public sealed class CmdAfk : MessageCmd {
        public override string name { get { return "afk"; } }
        public override string type { get { return CommandTypes.Information; } }
        public CmdAfk() { }

        public override void Use(Player p, string message)
        {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            if (message == "list") {
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player pl in players) {
                    if (!Entities.CanSee(p, pl) || !pl.IsAfk) continue;
                    Player.Message(p, p.name);
                }
                return;
            }
            ToggleAfk(p, message);
        }
        
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
                    Chat.MessageGlobal(p, "-" + p.ColoredName + "%S- is AFK " + message, false, true);
                    Player.RaisePlayerAction(p, PlayerAction.AFK, message);
                    p.CheckForMessageSpam();
                }
                
                p.AFKCooldown = DateTime.UtcNow.AddSeconds(2);
                p.RaiseONAFK();
                Player.RaiseAFK(p);
                OnPlayerAFKEvent.Call(p);
            } else {
                if (cantSend) {
                    Player.Message(p, "You are no longer marked as being AFK.");
                } else {
                    Chat.MessageGlobal(p, "-" + p.ColoredName + "%S- is no longer AFK", false, true);
                    Player.RaisePlayerAction(p, PlayerAction.UnAFK, message);
                    p.CheckForMessageSpam();
                }
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/afk <reason>");
            Player.Message(p, "%HMarks yourself as AFK. Use again to mark yourself as back");
        }
    }
}
