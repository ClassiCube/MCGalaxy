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

namespace MCGalaxy.SQL.Native {
    
    public sealed class NativeParameterisedQuery : ParameterisedQuery {

        public override void Execute(string query, bool createDB = false) {
            using (var conn = new NativeConnection()) {
				conn.ConnectionString = SQLite.connString;
                conn.Open();
                using (NativeCommand cmd = new NativeCommand()) {
                	cmd.CommandText = query;
                	cmd.Connection = conn;
                    foreach (var param in parameters)
                        cmd.args.AddWithValue(param.Key, param.Value);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public unsafe override void Fill(string query, DataTable results) {
			using (var conn = new NativeConnection()) {
				conn.ConnectionString = SQLite.connString;
                conn.Open();
                using (NativeCommand cmd = new NativeCommand()) {
                	cmd.CommandText = query;
                	cmd.Connection = conn;
                    foreach (var param in parameters)
                        cmd.args.AddWithValue(param.Key, param.Value);
                    cmd.Prepare();
                    
                    NativeReader reader = new NativeReader();
                    reader.ReadColumns(cmd, results);
                    reader.ReadRows(cmd, results);
                }
                conn.Close();
            }
        }
    }
}