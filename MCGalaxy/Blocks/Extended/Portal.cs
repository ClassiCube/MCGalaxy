/*
    Copyright 2011 MCForge
        
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
using System.Data;
using MCGalaxy.SQL;

namespace MCGalaxy.Blocks.Extended {
    public static class Portal {
        
        public static bool Handle(Player p, ushort x, ushort y, ushort z) {
            p.RevertBlock(x, y, z);
            if (!p.level.hasPortals) return false;
            
            try {
                DataTable Portals = Database.Backend.GetRows("Portals" + p.level.name, "*",
                                                             "WHERE EntryX=@0 AND EntryY=@1 AND EntryZ=@2", x, y, z);
                int last = Portals.Rows.Count - 1;
                if (last == -1) { Portals.Dispose(); return false; }
                byte rotX = p.rot[0], rotY = p.rot[1];
                
                DataRow row = Portals.Rows[last];
                string map = row["ExitMap"].ToString();
                map.Cp437ToUnicodeInPlace();
                
                if (p.level.name != map) {
                    Level curLevel = p.level;
                    PlayerActions.ChangeMap(p, map, true);
                    if (curLevel == p.level) { Player.Message(p, "The map the portal goes to isn't loaded."); return true; }
                    p.BlockUntilLoad(10);
                }
                
                x = ushort.Parse(row["ExitX"].ToString());
                y = ushort.Parse(row["ExitY"].ToString());
                z = ushort.Parse(row["ExitZ"].ToString());
                PlayerActions.MoveCoords(p, x, y, z, rotX, rotY);
                Portals.Dispose();
                return true;
            } catch {
                return false;
            }
        }
    }
}