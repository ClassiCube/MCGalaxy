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
using System;
using MCGalaxy.Games;

namespace MCGalaxy {

    /// <summary> Contains methods related to the management of tab list of player names. </summary>
    public static class TabList {
        
        // Want nobody to be first, banned to be last
        const LevelPermission offset = LevelPermission.Nobody;
        internal static void Add(Player dst, Player p, byte id) {
            byte grpPerm = (byte)(offset - p.group.Permission);
            if (!Server.zombie.Running || !p.Game.Infected) {
                string col = Entities.GetSupportedCol(dst, p.color);
                string group = p.Game.Referee ? "&2Referees" : "&fPlayers";
                
                if (dst.hasExtList)
                    dst.SendExtAddPlayerName(id, p.truename, col + p.truename, group, grpPerm);
                return;
            }
            
            string name = p.truename;
            if (ZombieGame.ZombieName != "" && !dst.Game.Aka)
                name = ZombieGame.ZombieName;           
            if (dst.hasExtList)
                dst.SendExtAddPlayerName(id, p.truename, Colors.red + name, "&cZombies", grpPerm);
        }
        
        internal static void Add(Player dst, PlayerBot b) {
            if (dst.hasExtList)
                dst.SendExtAddPlayerName(b.id, b.skinName, b.color + b.name, "Bots", 0);
        }
        
        internal static void Remove(Player dst, byte id) {
            if (dst.hasExtList)
                dst.SendExtRemovePlayerName(id);
        }
    }
}
