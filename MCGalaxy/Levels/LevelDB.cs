/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using MCGalaxy.Blocks.Extended;
using MCGalaxy.Maths;
using MCGalaxy.SQL;
using MCGalaxy.Util;
using BlockID = System.UInt16;

namespace MCGalaxy {
    public static class LevelDB {
        
        public unsafe static void SaveBlockDB(Level lvl) {
            if (lvl.BlockDB.Cache.Head == null) return;
            if (!lvl.Config.UseBlockDB) { lvl.BlockDB.Cache.Clear(); return; }

            using (IDisposable wLock = lvl.BlockDB.Locker.AccquireWrite(60 * 1000)) {
                if (wLock == null) {
                    Logger.Log(LogType.Warning, "&WCouldn't accquire BlockDB write lock on {0}, skipping save", lvl.name);
                    return;
                }
                lvl.BlockDB.FlushCache();
            }
            Logger.Log(LogType.BackgroundActivity, "Saved BlockDB changes for: {0}", lvl.name);
        }

        static object ListZones(IDataRecord record, object arg) {
            Zone z = new Zone();
            z.MinX = (ushort)record.GetInt("SmallX");
            z.MinY = (ushort)record.GetInt("SmallY");
            z.MinX = (ushort)record.GetInt("SmallZ");
            
            z.MaxX = (ushort)record.GetInt("BigX");
            z.MaxY = (ushort)record.GetInt("BigY");
            z.MaxX = (ushort)record.GetInt("BigZ");
            z.Config.Name = record.GetText("Owner");
            
            ((List<Zone>)arg).Add(z);
            return arg;
        }
        
        internal static void LoadZones(Level level, string map) {
            if (!Database.TableExists("Zone" + map)) return;
            
            List<Zone> zones = new List<Zone>();
            Database.ReadRows("Zone" + map, "*", zones, ListZones);
            
            bool changedPerbuild = false;
            for (int i = 0; i < zones.Count; i++) {
                Zone z = zones[i];
                string owner = z.Config.Name;
                
                if (owner.StartsWith("grp")) {
                    Group grp = Group.Find(owner.Substring(3));
                    if (grp != null) z.Access.Min = grp.Permission;
                } else if (z.CoversMap(level)) {
                    level.BuildAccess.Whitelisted.Add(owner);
                    changedPerbuild = true;
                    continue;
                } else {
                    z.Access.Whitelisted.Add(owner);
                    z.Access.Min = LevelPermission.Admin;
                }
                
                z.Config.Name = "Zone" + i;
                z.AddTo(level);
            }
            
            if (changedPerbuild) level.SaveSettings();
            if (level.Zones.Count > 0 && !level.Save(true)) return;
            
            Database.DeleteTable("Zone" + map);
            Logger.Log(LogType.SystemActivity, "Upgraded zones for map " + map);
        }
        
        internal static void LoadPortals(Level level, string map) {
            List<Vec3U16> coords = Portal.GetAllCoords(map);
            level.hasPortals     = coords.Count > 0;
            if (!level.hasPortals) return;
            
            foreach (Vec3U16 p in coords) {
                BlockID block = level.GetBlock(p.X, p.Y, p.Z);
                if (level.Props[block].IsPortal) continue;
                Portal.Delete(map, p.X, p.Y, p.Z);
            }
        }
        
        internal static void LoadMessages(Level level, string map) {
            List<Vec3U16> coords   = MessageBlock.GetAllCoords(map);
            level.hasMessageBlocks = coords.Count > 0;
            if (!level.hasMessageBlocks) return;
            
            foreach (Vec3U16 p in coords) {
                BlockID block = level.GetBlock(p.X, p.Y, p.Z);
                if (level.Props[block].IsMessageBlock) continue;
                MessageBlock.Delete(map, p.X, p.Y, p.Z);
            }
        }
        
        internal static ColumnDesc[] createPortals = new ColumnDesc[] {
            new ColumnDesc("EntryX", ColumnType.UInt16),
            new ColumnDesc("EntryY", ColumnType.UInt16),
            new ColumnDesc("EntryZ", ColumnType.UInt16),
            new ColumnDesc("ExitMap", ColumnType.Char, 20),
            new ColumnDesc("ExitX", ColumnType.UInt16),
            new ColumnDesc("ExitY", ColumnType.UInt16),
            new ColumnDesc("ExitZ", ColumnType.UInt16),
        };
        
        internal static ColumnDesc[] createMessages = new ColumnDesc[] {
            new ColumnDesc("X", ColumnType.UInt16),
            new ColumnDesc("Y", ColumnType.UInt16),
            new ColumnDesc("Z", ColumnType.UInt16),
            new ColumnDesc("Message", ColumnType.Char, 255),
        };
    }
}