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
        
        public class PortalExitData {
            public string Map;
            public int X, Y, Z;
        }
        
        static object IteratePortalExit(IDataRecord record, object arg) {
            PortalExitData data = new PortalExitData();
            data.Map = record.GetString("ExitMap");
            data.X   = record.GetInt32("ExitX");
            data.Y   = record.GetInt32("ExitY");
            data.Z   = record.GetInt32("ExitZ");
            
            data.Map = data.Map.Cp437ToUnicode();
            return data;
        }
        
        public static PortalExitData Get(string map, ushort x, ushort y, ushort z) {
            object raw = Database.Backend.IterateRows("Portals" + map, "*", null, IteratePortalExit,
			                                          "WHERE EntryX=@0 AND EntryY=@1 AND EntryZ=@2", x, y, z);
		    return (PortalExitData)raw;
        }
        
        public static bool Handle(Player p, ushort x, ushort y, ushort z) {
            if (!p.level.hasPortals) return false;
            
            PortalExitData exit = Get(p.level.MapName, x, y, z);
            if (exit == null) return false;
            Orientation rot = p.Rot;
            
            if (p.level.name != exit.Map) {
                Level curLevel = p.level;
                p.summonedMap = exit.Map;
                bool changedMap = false;
                
                try {
                    changedMap = PlayerActions.ChangeMap(p, exit.Map);
                } catch (Exception ex) {
                    Logger.LogError(ex);
                    changedMap = false;
                }
                
                p.summonedMap = null;                
                if (!changedMap) { Player.Message(p, "Unable to use this portal, as this portal goes to that map."); return true; }
                p.BlockUntilLoad(10);
            }

            Position pos = Position.FromFeetBlockCoords(exit.X, exit.Y, exit.Z);
            p.SendPos(Entities.SelfID, pos, rot);
            return true;
        }
    }
}