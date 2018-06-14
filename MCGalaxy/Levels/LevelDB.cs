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
                    Logger.Log(LogType.Warning, "Couldn't accquire BlockDB write lock on {0}, skipping save", lvl.name);
                    return;
                }
                lvl.BlockDB.FlushCache();
            }
            Logger.Log(LogType.BackgroundActivity, "Saved BlockDB changes for: {0}", lvl.name);
        }

        internal static void LoadZones(Level level, string name) {
            if (!Database.TableExists("Zone" + name)) return;
            int id = 0;
            bool changedPerbuild = false;
            
            using (DataTable table = Database.Backend.GetRows("Zone" + name, "*")) {
                foreach (DataRow row in table.Rows) {
                    Zone z = new Zone(level);
                    z.MinX = ushort.Parse(row["SmallX"].ToString());
                    z.MinY = ushort.Parse(row["SmallY"].ToString());
                    z.MinZ = ushort.Parse(row["SmallZ"].ToString());
                    z.MaxX = ushort.Parse(row["BigX"].ToString());
                    z.MaxY = ushort.Parse(row["BigY"].ToString());
                    z.MaxZ = ushort.Parse(row["BigZ"].ToString());
                    
                    string owner = row["Owner"].ToString();
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
                    
                    z.Config.Name = "Zone" + id;
                    id++;
                    z.AddTo(level);
                }
            }
            
            if (changedPerbuild) Level.SaveSettings(level);
            if (level.Zones.Count > 0 && !level.Save(true)) return;
            Database.Backend.DeleteTable("Zone" + name);
            Logger.Log(LogType.SystemActivity, "Upgraded zones for map " + name);
        }
        
        internal static void LoadPortals(Level level, string name) {
            level.hasPortals = Database.TableExists("Portals" + name);
            if (!level.hasPortals) return;
            
            using (DataTable table = Database.Backend.GetRows("Portals" + name, "*")) {
                foreach (DataRow row in table.Rows) {
                    ushort x = ushort.Parse(row["EntryX"].ToString());
                    ushort y = ushort.Parse(row["EntryY"].ToString());
                    ushort z = ushort.Parse(row["EntryZ"].ToString());
                    
                    BlockID block = level.GetBlock(x, y, z);
                    if (level.Props[block].IsPortal) continue;
                    
                    Database.Backend.DeleteRows("Portals" + name, "WHERE EntryX=@0 AND EntryY=@1 AND EntryZ=@2", x, y, z);
                }
            }
        }
        
        internal static void LoadMessages(Level level, string name) {
            level.hasMessageBlocks = Database.TableExists("Messages" + name);
            if (!level.hasMessageBlocks) return;            
            List<Vec3U16> coords = MessageBlock.GetAll(name);
            
            foreach (Vec3U16 p in coords) {
                BlockID block = level.GetBlock(p.X, p.Y, p.Z);
                if (level.Props[block].IsMessageBlock) continue;
                MessageBlock.Delete(name, p.X, p.Y, p.Z);
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