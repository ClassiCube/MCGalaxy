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
using System;

namespace MCGalaxy.Commands {
    
    public sealed class CmdAka : Command {
        
        public override string name { get { return "aka"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        
        public override void Use(Player p, string message) {
            p.Game.Aka = !p.Game.Aka;
            Player[] players = PlayerInfo.Online.Items;
            Player.SendMessage(p, "AKA mode is now: " + (p.Game.Aka ? "&aon" : "&coff"));
            
            foreach (Player pl in players) {
                if (pl.level != p.level || p == pl || !Player.CanSee(p, pl) || pl.Game.Referee) continue;                
                p.SendDespawn(pl.id);
                p.SpawnEntity(pl, pl.id, pl.pos[0], pl.pos[1], pl.pos[2], pl.rot[0], pl.rot[1], "");
            }
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/aka");
            Player.SendMessage(p, "%HToggles whether infected players show their actual names.");
        }
    }
}
