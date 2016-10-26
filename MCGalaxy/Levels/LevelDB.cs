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
using MCGalaxy.SQL;

namespace MCGalaxy {
    public static class LevelDB {
        
        public unsafe static void SaveBlockDB(Level lvl) {
            if (lvl.blockCache.Count == 0) return;
            if (!lvl.UseBlockDB) { lvl.blockCache.Clear(); return; }
            List<Level.BlockPos> tempCache = lvl.blockCache;
            string date = new String('-', 19); //yyyy-mm-dd hh:mm:ss
            
            fixed (char* ptr = date) {
                ptr[4] = '-'; ptr[7] = '-'; ptr[10] = ' '; ptr[13] = ':'; ptr[16] = ':';
                using (BulkTransaction bulk = BulkTransaction.Create())
                    DoSaveChanges(tempCache, ptr, lvl, date, bulk);
            }
            tempCache.Clear();
            lvl.blockCache = new List<Level.BlockPos>();
            Server.s.Log("Saved BlockDB changes for:" + lvl.name, true);
        }
        
        unsafe static bool DoSaveChanges(List<Level.BlockPos> tempCache, char* ptr,
                                         Level lvl, string date, BulkTransaction transaction) {
            string template = "INSERT INTO `Block" + lvl.name +
                "` (Username, TimePerformed, X, Y, Z, type, deleted) VALUES (@0, @1, @2, @3, @4, @5, @6)";
            ushort x, y, z;
            
            IDbCommand cmd = BulkTransaction.CreateCommand(template, transaction);
            if (cmd == null) return false;
            
            IDataParameter nameP = transaction.CreateParam("@0", DbType.AnsiStringFixedLength); cmd.Parameters.Add(nameP);
            IDataParameter timeP = transaction.CreateParam("@1", DbType.AnsiStringFixedLength); cmd.Parameters.Add(timeP);
            IDataParameter xP = transaction.CreateParam("@2", DbType.UInt16); cmd.Parameters.Add(xP);
            IDataParameter yP = transaction.CreateParam("@3", DbType.UInt16); cmd.Parameters.Add(yP);
            IDataParameter zP = transaction.CreateParam("@4", DbType.UInt16); cmd.Parameters.Add(zP);
            IDataParameter tileP = transaction.CreateParam("@5", DbType.Byte); cmd.Parameters.Add(tileP);
            IDataParameter delP = transaction.CreateParam("@6", DbType.Byte); cmd.Parameters.Add(delP);
            
            for (int i = 0; i < tempCache.Count; i++) {
                Level.BlockPos bP = tempCache[i];
                lvl.IntToPos(bP.index, out x, out y, out z);
                DateTime time = Server.StartTimeLocal.AddTicks((bP.flags >> 2) * TimeSpan.TicksPerSecond);
                MakeInt(time.Year, 4, 0, ptr); MakeInt(time.Month, 2, 5, ptr); MakeInt(time.Day, 2, 8, ptr);
                MakeInt(time.Hour, 2, 11, ptr); MakeInt(time.Minute, 2, 14, ptr); MakeInt(time.Second, 2, 17, ptr);
                
                nameP.Value = bP.name;
                timeP.Value = date;
                xP.Value = x; yP.Value = y; zP.Value = z;
                tileP.Value = bP.rawBlock;
                delP.Value = (byte)(bP.flags & 3);

                if (!BulkTransaction.Execute(template, cmd)) {
                    cmd.Dispose();
                    cmd.Parameters.Clear();
                    transaction.Rollback(); return false;
                }
            }
            cmd.Dispose();
            cmd.Parameters.Clear();
            transaction.Commit();
            return true;
        }
        
        unsafe static void MakeInt(int value, int chars, int offset, char* ptr) {
            for (int i = 0; i < chars; i++, value /= 10) {
                char c = (char)('0' + (value % 10));
                ptr[offset + (chars - 1 - i)] = c;
            }
        }
        
        public static void CreateTables(string givenName) {
            Database.Backend.CreateTable("Block" + givenName, LevelDB.createBlock);
        }
        
        internal static void LoadZones(Level level, string name) {
            if (!Database.TableExists("Zone" + name)) return;
            using (DataTable table = Database.Backend.GetRows("Zone" + name, "*")) {
                Level.Zone Zn;
                foreach (DataRow row in table.Rows) {
                    Zn.smallX = ushort.Parse(row["SmallX"].ToString());
                    Zn.smallY = ushort.Parse(row["SmallY"].ToString());
                    Zn.smallZ = ushort.Parse(row["SmallZ"].ToString());
                    Zn.bigX = ushort.Parse(row["BigX"].ToString());
                    Zn.bigY = ushort.Parse(row["BigY"].ToString());
                    Zn.bigZ = ushort.Parse(row["BigZ"].ToString());
                    Zn.Owner = row["Owner"].ToString();
                    level.ZoneList.Add(Zn);
                }
            }
        }
        
        internal static void LoadPortals(Level level, string name) {
            level.hasPortals = Database.TableExists("Portals" + name);
            if (!level.hasPortals) return;
            
            using (DataTable table = Database.Backend.GetRows("Portals" + name, "*")) {
                foreach (DataRow row in table.Rows) {
                    ushort x = ushort.Parse(row["EntryX"].ToString());
                    ushort y = ushort.Parse(row["EntryY"].ToString());
                    ushort z = ushort.Parse(row["EntryZ"].ToString());
                    
                    byte block = level.GetTile(x, y, z);
                    if (block == Block.custom_block) {
                        block = level.GetExtTile(x, y, z);
                        if (level.CustomBlockProps[block].IsPortal) continue;
                    } else {
                        if (Block.Props[block].IsPortal) continue;
                    }
                    
                    Database.Backend.DeleteRows("Portals" + name, "WHERE EntryX=@0 AND EntryY=@1 AND EntryZ=@2", x, y, z);
                }
            }
        }
        
        internal static void LoadMessages(Level level, string name) {
            level.hasMessageBlocks = Database.TableExists("Messages" + name);
            if (!level.hasMessageBlocks) return;
            
            using (DataTable table = Database.Backend.GetRows("Messages" + name, "*")) {
                foreach (DataRow row in table.Rows) {
                    ushort x = ushort.Parse(row["X"].ToString());
                    ushort y = ushort.Parse(row["Y"].ToString());
                    ushort z = ushort.Parse(row["Z"].ToString());
                    
                    byte block = level.GetTile(x, y, z);
                    if (block == Block.custom_block) {
                        block = level.GetExtTile(x, y, z);
                        if (level.CustomBlockProps[block].IsMessageBlock) continue;
                    } else {
                        if (Block.Props[block].IsMessageBlock) continue;
                    }

                    Database.Backend.DeleteRows("Messages" + name, "WHERE X=@0 AND Y=@1 AND Z=@2", x, y, z);
                }
            }
        }
        
        public static void DeleteZone(string level, Level.Zone zn) {
            object locker = ThreadSafeCache.DBCache.Get(level);
            lock (locker) {
                if (!Database.TableExists("Zone" + level)) return;
                Database.Backend.DeleteRows("Zone" + level, "WHERE Owner=@0 AND SmallX=@1 AND SMALLY=@2 " +
                                            "AND SMALLZ=@3 AND BIGX=@4 AND BIGY=@5 AND BIGZ=@6",
                                            zn.Owner, zn.smallX, zn.smallY, zn.smallZ, zn.bigX, zn.bigY, zn.bigZ);
            }
        }
        
        public static void CreateZone(string level, Level.Zone zn) {
            object locker = ThreadSafeCache.DBCache.Get(level);
            lock (locker) {
                Database.Backend.CreateTable("Zone" + level, LevelDB.createZones);
                Database.Backend.AddRow("Zone" + level, "Owner, SmallX, SmallY, SmallZ, BigX, BigY, BigZ",
                                        zn.Owner, zn.smallX, zn.smallY, zn.smallZ, zn.bigX, zn.bigY, zn.bigZ);
            }
        }

        
        internal static ColumnDesc[] createBlock = {
            new ColumnDesc("Username", ColumnType.Char, 20),
            new ColumnDesc("TimePerformed", ColumnType.DateTime),
            new ColumnDesc("X", ColumnType.UInt16),
            new ColumnDesc("Y", ColumnType.UInt16),
            new ColumnDesc("Z", ColumnType.UInt16),
            new ColumnDesc("Type", ColumnType.UInt8),
            new ColumnDesc("Deleted", ColumnType.Bool),
        };
        
        internal static ColumnDesc[] createPortals = {
            new ColumnDesc("EntryX", ColumnType.UInt16),
            new ColumnDesc("EntryY", ColumnType.UInt16),
            new ColumnDesc("EntryZ", ColumnType.UInt16),
            new ColumnDesc("ExitMap", ColumnType.Char, 20),
            new ColumnDesc("ExitX", ColumnType.UInt16),
            new ColumnDesc("ExitY", ColumnType.UInt16),
            new ColumnDesc("ExitZ", ColumnType.UInt16),
        };
        
        internal static ColumnDesc[] createMessages = {
            new ColumnDesc("X", ColumnType.UInt16),
            new ColumnDesc("Y", ColumnType.UInt16),
            new ColumnDesc("Z", ColumnType.UInt16),
            new ColumnDesc("Message", ColumnType.Char, 255),
        };

        internal static ColumnDesc[] createZones = {
            new ColumnDesc("SmallX", ColumnType.UInt16),
            new ColumnDesc("SmallY", ColumnType.UInt16),
            new ColumnDesc("SmallZ", ColumnType.UInt16),
            new ColumnDesc("BigX", ColumnType.UInt16),
            new ColumnDesc("BigY", ColumnType.UInt16),
            new ColumnDesc("BigZ", ColumnType.UInt16),
            new ColumnDesc("Owner", ColumnType.VarChar, 20),
        };
    }
}