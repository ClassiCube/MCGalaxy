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

namespace MCGalaxy.Commands {    
    public sealed class CmdReferee : Command {
        public override string name { get { return "ref"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandEnable Enabled { get { return CommandEnable.Zombie; } }   
        public CmdReferee() { }
        
        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }

            if (p.Game.Referee) {
                Player.SendChatFrom(p, p.ColoredName + " %Sis no longer a referee", false);
                p.Game.Referee = !p.Game.Referee;
                if (p.level == Server.zombie.CurLevel)
                    Server.zombie.PlayerJoinedLevel(p, Server.zombie.CurLevel, Server.zombie.CurLevel);
                
                if (p.HasCpeExt(CpeExt.HackControl))
                    p.Send(Hacks.MakeHackControl(p));
                Command.all.Find("spawn").Use(p, "");
            } else {
                Player.SendChatFrom(p, p.ColoredName + " %Sis now a referee", false);               
                Server.zombie.PlayerLeftServer(p);
                Entities.GlobalDespawn(p, false, true);
                p.Game.Referee = !p.Game.Referee;
                
                if (p.HasCpeExt(CpeExt.HackControl))
                    p.Send(Packet.MakeHackControl(true, true, true, true, true, -1));
            }
            
            Entities.GlobalSpawn(p, false, "");
            TabList.Add(p, p, 0xFF);
            p.SetPrefix();
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/referee");
            Player.Message(p, "%HTurns referee mode on/off.");
            Player.Message(p, "%H  Note that leaving referee mode sends you back to spawn.");
        }
    }
}
