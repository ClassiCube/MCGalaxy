/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using MySql.Data.MySqlClient;

namespace MCGalaxy.SQL {
    
    public sealed class MySQLParameterisedQuery : ParameterisedQuery {

        public override void Execute(string queryString, string connString, bool createDB = false) {
            using (var conn = new MySqlConnection(connString)) {
                conn.Open();
                if (!createDB) conn.ChangeDatabase(Server.MySQLDatabaseName);
                
                using (MySqlCommand cmd = new MySqlCommand(queryString, conn)) {
                    foreach (var param in parameters)
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public override void Fill(string queryString, string connString, DataTable toReturn) {
            using (var conn = new MySqlConnection(connString)) {
                conn.Open();
                conn.ChangeDatabase(Server.MySQLDatabaseName);
                using (MySqlDataAdapter da = new MySqlDataAdapter(queryString, conn)) {
                    foreach (var param in parameters)
                        da.SelectCommand.Parameters.AddWithValue(param.Key, param.Value);
                    da.Fill(toReturn);
                    da.SelectCommand.Dispose();
                }
                conn.Close();
            }
        }
    }
}
