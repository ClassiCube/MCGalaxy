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
            data.Map = record.GetText(0);
            
            data.X = (ushort)record.GetInt32(1);
            data.Y = (ushort)record.GetInt32(2);
            data.Z = (ushort)record.GetInt32(3);
            return data;
        }
        
        static object ReadAllExits(IDataRecord record, object arg) {
            ((List<PortalExit>)arg).Add(ParseExit(record));
            return arg;
        }
        
        
        /// <summary> Returns whether a Portals table for the given map exists in the DB. </summary>
        public static bool ExistsInDB(string map) { return Database.TableExists("Portals" + map); }
        
        /// <summary> Returns the coordinates for all portals in the given map. </summary>
        public static List<Vec3U16> GetAllCoords(string map) {
            List<Vec3U16> coords = new List<Vec3U16>();
            if (!ExistsInDB(map)) return coords;
            
            Database.ReadRows("Portals" + map, "EntryX,EntryY,EntryZ", coords, ReadCoords);
            return coords;
        }

        /// <summary> Returns the exit details associated with each portal in the given map. </summary>
        public static List<PortalExit> GetAllExits(string map) {
            List<PortalExit> exits = new List<PortalExit>();
            if (!ExistsInDB(map)) return exits;
            
            Database.ReadRows("Portals" + map, "ExitMap,ExitX,ExitY,ExitZ", exits, ReadAllExits);
            return exits;
        }
        
        /// <summary> Deletes all portals for the given map. </summary>
        public static void DeleteAll(string map) {
            if (!ExistsInDB(map)) return;
            Database.DeleteTable("Portals" + map);
        }
        
        /// <summary> Copies all portals from the given map to another map. </summary>
        public static void CopyAll(string src, string dst) {
            if (!ExistsInDB(src)) return;
            Database.CreateTable("Portals" + dst, LevelDB.createPortals);
            Database.CopyAllRows("Portals" + src, "Portals" + dst);
            // Fixup portal exists that go to the same map
            Database.UpdateRows("Portals" + dst, "ExitMap=@1", "WHERE ExitMap=@0", src, dst);
        }
        
        /// <summary> Moves all portals from the given map to another map. </summary>
        public static void MoveAll(string src, string dst) {
            if (!ExistsInDB(src)) return;
            Database.RenameTable("Portals" + src, "Portals" + dst);
        }
        
        
        /// <summary> Returns the exit details for the given portal in the given map. </summary>
        /// <remarks> Returns null if the given portal does not actually exist. </remarks>
        public static PortalExit Get(string map, ushort x, ushort y, ushort z) {
            object raw = Database.ReadRows("Portals" + map, "ExitMap,ExitX,ExitY,ExitZ", null, ReadExit,
        	                               "WHERE EntryX=@0 AND EntryY=@1 AND EntryZ=@2", x, y, z);
            return (PortalExit)raw;
        }
        
        /// <summary> Deletes the given portal from the given map. </summary>
        public static void Delete(string map, ushort x, ushort y, ushort z) {
            Database.DeleteRows("Portals" + map,
        	                    "WHERE EntryX=@0 AND EntryY=@1 AND EntryZ=@2", x, y, z);
        }
        
        /// <summary> Creates or updates the given portal in the given map. </summary>
        public static void Set(string map, ushort x, ushort y, ushort z,
                               ushort exitX, ushort exitY, ushort exitZ, string exitMap) {
            Database.CreateTable("Portals" + map, LevelDB.createPortals);
            int count = Database.CountRows("Portals" + map,
                                           "WHERE EntryX=@0 AND EntryY=@1 AND EntryZ=@2", x, y, z);
            
            if (count == 0) {
                Database.AddRow("Portals" + map, "EntryX, EntryY, EntryZ, ExitX, ExitY, ExitZ, ExitMap",
            	                x, y, z, exitX, exitY, exitZ, exitMap);
            } else {
                Database.UpdateRows("Portals" + map, "ExitMap=@6, ExitX=@3, ExitY=@4, ExitZ=@5",
            	                    "WHERE EntryX=@0 AND EntryY=@1 AND EntryZ=@2", x, y, z,
            	                    exitX, exitY, exitZ, exitMap);
            }
            
            Level lvl = LevelInfo.FindExact(map);
            if (lvl != null) lvl.hasPortals = true;
        }
    }
}