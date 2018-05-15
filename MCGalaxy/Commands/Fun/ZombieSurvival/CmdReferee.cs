/*
    Copyright 2011 MCForge
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using MCGalaxy.Games;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Network;

namespace MCGalaxy.Commands.Fun {
    public sealed class CmdReferee : Command {
        public override string name { get { return "Ref"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandEnable Enabled { get { return CommandEnable.Zombie; } }
        public override bool SuperUseable { get { return false; } }
        
        public override void Use(Player p, string message) {
            if (p.Game.Referee) {
                Chat.MessageGlobal(p, p.ColoredName + " %Sis no longer a referee", false);
                OnPlayerActionEvent.Call(p, PlayerAction.UnReferee);
                p.Game.Referee = false;
            } else {
                Chat.MessageGlobal(p, p.ColoredName + " %Sis now a referee", false);
                OnPlayerActionEvent.Call(p, PlayerAction.Referee);
                p.Game.Referee = true;
            }
            
            if (p.Supports(CpeExt.InstantMOTD)) {
                p.SendMapMotd();
            } else if (p.Supports(CpeExt.HackControl)) {
                if (p.Game.Referee) {
                    p.Send(Packet.HackControl(true, true, true, true, true, -1));
                } else {
                    p.Send(Hacks.MakeHackControl(p, p.level.GetMotd(p)));
                }
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Referee");
            Player.Message(p, "%HTurns referee mode on/off.");
            Player.Message(p, "%H  Note that leaving referee mode sends you back to spawn.");
        }
    }
}
