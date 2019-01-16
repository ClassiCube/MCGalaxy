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
using System.Collections.Generic;
using System.Data;
using MCGalaxy.Maths;
using MCGalaxy.SQL;

namespace MCGalaxy.Blocks.Extended {
    public class PortalExit { public string Map; public ushort X, Y, Z; }
    
    public static class Portal {
        
        public static bool Handle(Player p, ushort x, ushort y, ushort z) {
            if (!p.level.hasPortals) return false;
            
            PortalExit exit = Get(p.level.MapName, x, y, z);
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
                if (!changedMap) { p.Message("Unable to use this portal, as this portal goes to that map."); return true; }
                p.BlockUntilLoad(10);
            }

            Position pos = Position.FromFeetBlockCoords(exit.X, exit.Y, exit.Z);
            p.SendPos(Entities.SelfID, pos, rot);
            return true;
        }
        
        
        internal static object ReadCoords(IDataRecord record, object arg) {
            Vec3U16 pos;
            pos.X = (ushort)record.GetInt32(0);
            pos.Y = (ushort)record.GetInt32(1);
            pos.Z = (ushort)record.GetInt32(2);
            
            ((List<Vec3U16>)arg).Add(pos);
            return arg;
        }
        
        static object ReadExit(IDataRecord record, object arg) { return ParseExit(record); }
        static PortalExit ParseExit(IDataRecord record) {
            PortalExit data = new PortalExit();
            data.Map = record.GetText(0).Cp437ToUnicode();
            
            data.X = (ushort)record.GetInt32(1);
            data.Y = (ushort)record.GetInt32(2);
            data.Z = (ushort)record.GetInt32(3);
            return data;
        }
        
        static object ReadAllExits(IDataRecord record, object arg) {
            ((List<PortalExit>)arg).Add(ParseExit(record));
            return arg;
        }
        
        
        internal static List<Vec3U16> GetAllCoords(string map) {
            List<Vec3U16> coords = new List<Vec3U16>();
            Database.Backend.ReadRows("Portals" + map, "EntryX,EntryY,EntryZ", coords, ReadCoords);
            return coords;
        }

        public static List<PortalExit> GetAll(string map) {
            List<PortalExit> exits = new List<PortalExit>();
            Database.Backend.ReadRows("Portals" + map, "ExitMap,ExitX,ExitY,ExitZ", exits, ReadAllExits);
            return exits;
        }
        
        public static PortalExit Get(string map, ushort x, ushort y, ushort z) {
            object raw = Database.Backend.ReadRows("Portals" + map, "ExitMap,ExitX,ExitY,ExitZ",
                                                   null, ReadExit,
                                                   "WHERE EntryX=@0 AND EntryY=@1 AND EntryZ=@2", x, y, z);
            return (PortalExit)raw;
        }
        
        public static void Delete(string map, ushort x, ushort y, ushort z) {
            Database.Backend.DeleteRows("Portals" + map,
                                        "WHERE EntryX=@0 AND EntryY=@1 AND EntryZ=@2", x, y, z);
        }
        
        public static void Set(string map, ushort x, ushort y, ushort z, 
                               ushort exitX, ushort exitY, ushort exitZ, string exitMap) {
            Database.Backend.CreateTable("Portals" + map, LevelDB.createPortals);
            int count = Database.CountRows("Portals" + map,
                                           "WHERE EntryX=@0 AND EntryY=@1 AND EntryZ=@2", x, y, z);
            
            if (count == 0) {
                Database.Backend.AddRow("Portals" + map, "EntryX, EntryY, EntryZ, ExitX, ExitY, ExitZ, ExitMap",
                                        x, y, z, exitX, exitY, exitZ, exitMap);
            } else {
                Database.Backend.UpdateRows("Portals" + map, "ExitMap=@6, ExitX=@3, ExitY=@4, ExitZ=@5",
                                            "WHERE EntryX=@0 AND EntryY=@1 AND EntryZ=@2", x, y, z,
                                            exitX, exitY, exitZ, exitMap);
            }
            
            Level lvl = LevelInfo.FindExact(map);
            if (lvl != null) lvl.hasPortals = true;
        }
    }
}