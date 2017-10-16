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
    public sealed class CmdPing : Command {
        public override string name { get { return "Ping"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can see ping of all players") }; }
        }
        
        public override void Use(Player p, string message) {
            if (!message.CaselessEq("all")) {
                if (Player.IsSuper(p)) { Player.Message(p, "Super users cannot measure their own ping."); return; }
                
                if (!p.hasTwoWayPing) {
                    Player.Message(p, "Your client does not support measuring ping. You may need to update it.");
                } else {
                     Player.Message(p, p.Ping.Format());
                }
            } else {
                if (!CheckExtraPerm(p, 1)) return;            
                Player[] players = PlayerInfo.Online.Items;
                Player.Message(p, "Ping/latency list for online players:");
                
                foreach (Player pl in players) {
                    if (!Entities.CanSee(p, pl)) continue;
                    if (pl.Ping.AveragePingMilliseconds() == 0) continue;
                    Player.Message(p, pl.ColoredName + " %S- " + pl.Ping.Format());
                }
            }
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/Ping %H- Outputs details about your ping to the server.");
            Player.Message(p, "%T/Ping all %H- Outputs ping details for all players.");
            Player.Message(p, "&cNOTE: %HNot all clients support measuring ping.");
        }
    }
}
