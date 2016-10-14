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
                "` (Username, TimePerformed, X, Y, Z, type, deleted) VALUES (@Name, @Time, @X, @Y, @Z, @Tile, @Del)";
            ushort x, y, z;
            
            IDbCommand cmd = BulkTransaction.CreateCommand(template, transaction);
            if (cmd == null) return false;
            
            IDataParameter nameP = transaction.CreateParam("@Name", DbType.AnsiStringFixedLength); cmd.Parameters.Add(nameP);
            IDataParameter timeP = transaction.CreateParam("@Time", DbType.AnsiStringFixedLength); cmd.Parameters.Add(timeP);
            IDataParameter xP = transaction.CreateParam("@X", DbType.UInt16); cmd.Parameters.Add(xP);
            IDataParameter yP = transaction.CreateParam("@Y", DbType.UInt16); cmd.Parameters.Add(yP);
            IDataParameter zP = transaction.CreateParam("@Z", DbType.UInt16); cmd.Parameters.Add(zP);
            IDataParameter tileP = transaction.CreateParam("@Tile", DbType.Byte); cmd.Parameters.Add(tileP);
            IDataParameter delP = transaction.CreateParam("@Del", DbType.Byte); cmd.Parameters.Add(delP);
            
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
            if (!Database.TableExists("Portals" + name)) return;
            using (DataTable table = Database.Backend.GetRows("Portals" + name, "*")) {
                foreach (DataRow row in table.Rows) {
                    byte tile = level.GetTile(ushort.Parse(row["EntryX"].ToString()),
                                              ushort.Parse(row["EntryY"].ToString()),
                                              ushort.Parse(row["EntryZ"].ToString()));
                    if (Block.portal(tile)) continue;
                    
                    Database.Execute("DELETE FROM `Portals" + name + "` WHERE EntryX=@0 AND EntryY=@1 AND EntryZ=@2",
                                     row["EntryX"], row["EntryY"], row["EntryZ"]);
                }
            }
        }
        
        internal static void LoadMessages(Level level, string name) {
            if (!Database.TableExists("Messages" + name)) return;
            using (DataTable table = Database.Backend.GetRows("Messages" + name, "*")) {
                foreach (DataRow row in table.Rows) {
                    byte tile = level.GetTile(ushort.Parse(row["X"].ToString()),
                                              ushort.Parse(row["Y"].ToString()),
                                              ushort.Parse(row["Z"].ToString()));
                    if (Block.mb(tile)) continue;
                    
                    //givenName is safe against SQL injections, it gets checked in CmdLoad.cs
                    Database.Execute("DELETE FROM `Messages" + name + "` WHERE X=@0 AND Y=@1 AND Z=@2",
                                     row["X"], row["Y"], row["Z"]);
                }
            }
        }
        
        public static void DeleteZone(string level, Level.Zone zn) {
            object locker = ThreadSafeCache.DBCache.Get(level);
            lock (locker) {
                if (!Database.TableExists("Zone" + level)) return;
                Database.Execute("DELETE FROM `Zone" + level + "` WHERE Owner=@0 AND SmallX=@1 AND " +
                                 "SMALLY=@2 AND SMALLZ=@3 AND BIGX=@4 AND BIGY=@5 AND BIGZ=@6",
                                 zn.Owner, zn.smallX, zn.smallY, zn.smallZ, zn.bigX, zn.bigY, zn.bigZ);
            }
        }
        
        public static void CreateZone(string level, Level.Zone zn) {
            object locker = ThreadSafeCache.DBCache.Get(level);
            lock (locker) {
                Database.Backend.CreateTable("Zone" + level, LevelDB.createZones);
                Database.Execute("INSERT INTO `Zone" + level +  "` (Owner, SmallX, SmallY, " +
                                 "SmallZ, BigX, BigY, BigZ) VALUES (@0, @1, @2, @3, @4, @5, @6)",
                                 zn.Owner, zn.smallX, zn.smallY, zn.smallZ, zn.bigX, zn.bigY, zn.bigZ);
            }
        }

        
        internal static ColumnParams[] createBlock = {
            new ColumnParams("Username", ColumnType.Char, 20),
            new ColumnParams("TimePerformed", ColumnType.DateTime),
            new ColumnParams("X", ColumnType.UInt16),
            new ColumnParams("Y", ColumnType.UInt16),
            new ColumnParams("Z", ColumnType.UInt16),
            new ColumnParams("Type", ColumnType.UInt8),
            new ColumnParams("Deleted", ColumnType.Bool),
        };
        
        internal static ColumnParams[] createPortals = {
            new ColumnParams("EntryX", ColumnType.UInt16),
            new ColumnParams("EntryY", ColumnType.UInt16),
            new ColumnParams("EntryZ", ColumnType.UInt16),
            new ColumnParams("ExitMap", ColumnType.Char, 20),
            new ColumnParams("ExitX", ColumnType.UInt16),
            new ColumnParams("ExitY", ColumnType.UInt16),
            new ColumnParams("ExitZ", ColumnType.UInt16),
        };
        
        internal static ColumnParams[] createMessages = {
            new ColumnParams("X", ColumnType.UInt16),
            new ColumnParams("Y", ColumnType.UInt16),
            new ColumnParams("Z", ColumnType.UInt16),
            new ColumnParams("Message", ColumnType.Char, 255),
        };

        internal static ColumnParams[] createZones = {
            new ColumnParams("SmallX", ColumnType.UInt16),
            new ColumnParams("SmallY", ColumnType.UInt16),
            new ColumnParams("SmallZ", ColumnType.UInt16),
            new ColumnParams("BigX", ColumnType.UInt16),
            new ColumnParams("BigY", ColumnType.UInt16),
            new ColumnParams("BigZ", ColumnType.UInt16),
            new ColumnParams("Owner", ColumnType.VarChar, 20),
        };
    }
}