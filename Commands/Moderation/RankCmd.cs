/*
    Copyright 2015 MCGalaxy team
    
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
using System.IO;

namespace MCGalaxy.Commands.Moderation {
    internal static class RankCmd {
        
        internal static void ChangeRank(string name, Group oldRank, Group newRank,
                                        Player who, bool saveToNewRank = true) {
            Server.reviewlist.Remove(name);
            oldRank.playerList.Remove(name);
            oldRank.playerList.Save();
            
            if (saveToNewRank) {
                newRank.playerList.Add(name);
                newRank.playerList.Save();
            }
            if (who == null) return;
            
            Entities.DespawnEntities(who, false);
            if (who.color == "" || who.color == who.group.color)
                who.color = newRank.color;
            who.group = newRank;
            
            who.SetPrefix();
            who.Send(Packet.MakeUserType(who));
            Entities.SpawnEntities(who, false);
        }
    }
}
