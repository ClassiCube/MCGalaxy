/*
    Copyright 2015 MCGalaxy
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Data;
using System.Data.SQLite;

namespace MCGalaxy.SQL {

    public sealed class SQLiteBackend : IDatabaseBackend {

        public static IDatabaseBackend Instance = new SQLiteBackend();
        static ParameterisedQuery queryInstance = new SQLiteParameterisedQuery();
        
        static string connFormat = "Data Source =" + Server.apppath + "/MCGalaxy.db; Version =3; Pooling ={0}; Max Pool Size =300;";        
        public override string ConnectionString {
            get { return String.Format(connFormat, Server.DatabasePooling); }
        }
        public override bool EnforcesTextLength { get { return false; } }
        
        public override BulkTransaction CreateBulk() {
            return new SQLiteBulkTransaction(ConnectionString);
        }
        
        public override ParameterisedQuery CreateParameterised() {
            return new SQLiteParameterisedQuery();
        }
        
        internal override ParameterisedQuery GetStaticParameterised() {
            return queryInstance;
        }
        
        
        public override bool TableExists(string table) {
            using (DataTable results = GetRows("sqlite_master", "name",
        	                                   "WHERE type='table' AND name=@0", table)) {
                return results.Rows.Count > 0;
            }
        }
        
        public override void RenameTable(string srcTable, string dstTable) {
            string syntax = "ALTER TABLE `" + srcTable + "` RENAME TO `" + dstTable + "`";
            Database.Execute(syntax);
        }
        
        public override void ClearTable(string table) {
            string syntax = "DELETE FROM `" + table + "`";
            Database.Execute(syntax);
        }
    }
}
