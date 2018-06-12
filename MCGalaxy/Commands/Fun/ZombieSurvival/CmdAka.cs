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
using MCGalaxy.Games;

namespace MCGalaxy.Commands.Fun {
    
    public sealed class CmdAka : Command {
        public override string name { get { return "AKA"; } }
        public override string type { get { return CommandTypes.Games; } }
        
        public override void Use(Player p, string message) {
            ZSData data = ZSGame.Get(p);
            data.AkaMode = !data.AkaMode;
            Player[] players = PlayerInfo.Online.Items;
            Player.Message(p, "AKA mode is now: " + (data.AkaMode ? "&aOn" : "&cOff"));
            
            foreach (Player other in players) {
                if (other.level != p.level || p == other || !p.CanSeeEntity(other)) continue;
                
                Entities.Despawn(p, other);
                Entities.Spawn(p, other);
            }
            TabList.Add(p, p, Entities.SelfID);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/AKA");
            Player.Message(p, "%HToggles whether infected players show their actual names.");
        }
    }
}