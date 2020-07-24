/*
    Copyright 2015 MCGalaxy
        
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
using MCGalaxy.Network;

namespace MCGalaxy.Commands.Chatting {
    public sealed class CmdPing : Command2 {
        public override string name { get { return "Ping"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can see ping of all players") }; }
        }
        
        public override void Use(Player p, string message, CommandData data) {
            if (!message.CaselessEq("all")) {
                if (p.IsSuper) { p.Message("Super users cannot measure their own ping."); return; }
                
                if (!p.hasTwoWayPing) {
                    p.Message("Your client does not support measuring ping.");
                } else if (p.Ping.Measures() == 0) {
                    p.Message("No ping measurements yet. Try again in a bit.");
                } else {
                    p.Message(p.Ping.Format());
                }
            } else {
                if (!CheckExtraPerm(p, data, 1)) return;
                Player[] players = PlayerInfo.Online.Items;
                p.Message("Ping/latency list of online players: (&ALo%S:&7Avg%S:&CHi%S)ms");
                
                foreach (Player pl in players) {
                    if (!Entities.CanSee(data, p, pl)) continue;
                    if (pl.Ping.Measures() == 0) continue;
                    p.Message(pl.Ping.FormatAll() + " %S- " + pl.ColoredName);
                }
            }
        }

        public override void Help(Player p) {
            p.Message("%T/Ping %H- Outputs details about your ping to the server.");
            p.Message("%T/Ping all %H- Outputs ping details for all players.");
            p.Message("&cNOTE: %HNot all clients support measuring ping.");
        }
    }
}
