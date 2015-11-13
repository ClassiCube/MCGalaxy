/*
	Copyright 2011 MCGalaxy
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace MCGalaxy
{
    namespace SQL
    {
        public sealed class MySQLTransactionHelper : DatabaseTransactionHelper
        {
            private MySqlConnection connection = null;
            private MySqlTransaction transaction = null;

            public MySQLTransactionHelper() {
                init(MySQL.connString);
            }

            public MySQLTransactionHelper(string connString)
            {
                init(connString);
            }

            private void init(string connString) {
                connection = new MySqlConnection(connString);
                connection.Open();
                connection.ChangeDatabase(Server.MySQLDatabaseName);

                transaction = connection.BeginTransaction();
            }

            public static DatabaseTransactionHelper Create() {
                return Create(MySQL.connString);
            }

            public static DatabaseTransactionHelper Create(string connString)
            {
                try
                {
                    return new MySQLTransactionHelper(connString);
                }
                catch (Exception ex)
                {
                    Server.ErrorLog(ex);
                    return null;
                }
            }

            public override void Execute(string query)
            {
                try {
                    using (MySqlCommand cmd = new MySqlCommand(query, connection, transaction)) {
                        cmd.ExecuteNonQuery();
                    }
                } catch (Exception e) {
                    System.IO.File.AppendAllText("MySQL_error.log", DateTime.Now + " " + query + "\r\n");
                    Server.ErrorLog(e);
                }
            }

            public override void Commit()
            {
                try
                {
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Server.ErrorLog(ex);
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        Server.ErrorLog(ex2);
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

            public override void Dispose()
            {
                transaction.Dispose();
                connection.Dispose();
                transaction = null;
                connection = null;
            }
        }
    }
}
