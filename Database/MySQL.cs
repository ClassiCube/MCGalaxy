/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
namespace MCGalaxy
{
    namespace SQL
    {
        public static class MySQL //: Database //Extending for future improvement (Making it object oriented later)
        {
            private static string connStringFormat = "Data Source={0};Port={1};User ID={2};Password={3};Pooling={4}";
            private static MySqlParameterCollection parameters = new MySqlCommand().Parameters;

            public static string connString { get { return String.Format(connStringFormat, Server.MySQLHost, Server.MySQLPort, Server.MySQLUsername, Server.MySQLPassword, Server.DatabasePooling); } }
            [Obsolete("Preferably use Database.executeQuery instead")]
            public static void executeQuery(string queryString, bool createDB = false)
            {
                Database.executeQuery(queryString, createDB);
            }
            [Obsolete("Preferably use Database.executeQuery instead")]
            public static DataTable fillData(string queryString, bool skipError = false)
            {
                return Database.fillData(queryString, skipError);
            }

            /// <summary>
            /// Adds a parameter to the parameterized MySQL query.
            /// Use this before executing the query.
            /// </summary>
            /// <param name="name">The name of the parameter</param>
            /// <param name="param">The value of the parameter</param>
            public static void AddParams(string name, object param) {
                parameters.AddWithValue(name, param);
            }
            /// <summary>
            /// Clears the parameters added with <see cref="MCGalaxy.SQL.MySQL.AddParams(System.string, System.string)"/>
            /// <seealso cref="MCGalaxy.SQL.MySQL"/>
            /// </summary>
            public static void ClearParams() {
                parameters.Clear();
            }
            private static void AddMySQLParameters(MySqlCommand command) {
                foreach (MySqlParameter param in parameters)
                    command.Parameters.Add(param);
            }
            private static void AddMySQLParameters(MySqlDataAdapter dAdapter) {
                foreach (MySqlParameter param in parameters)
                    dAdapter.SelectCommand.Parameters.Add(param);
            }

            internal static void execute(string queryString, bool createDB = false) {
                using (var conn = new MySqlConnection(connString)) {
                    conn.Open();
                    if (!createDB) {
                        conn.ChangeDatabase(Server.MySQLDatabaseName);
                    }
                    using (MySqlCommand cmd = new MySqlCommand(queryString, conn)) {
                        AddMySQLParameters(cmd);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }

            internal static void fill(string queryString, DataTable toReturn) {
                using (var conn = new MySqlConnection(connString)) {
                    conn.Open();
                    conn.ChangeDatabase(Server.MySQLDatabaseName);
                    using (MySqlDataAdapter da = new MySqlDataAdapter(queryString, conn)) {
                        AddMySQLParameters(da);
                        da.Fill(toReturn);
                    }
                    conn.Close();
                }
            }
        }
    }
}

