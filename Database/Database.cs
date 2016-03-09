/*
	Copyright 2011 MCForge
		
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
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
namespace MCGalaxy
{
    namespace SQL
    {
        public sealed class Database
        {
            public static void CopyDatabase(StreamWriter sql)
            {
                //We technically know all tables in the DB...  But since this is MySQL, we can also get them all with a MySQL command
                //So we show the tables, and store the result.
                //Also output information data (Same format as phpMyAdmin's dump)

                //Important note:  This does NOT account for foreign keys, BLOB's etc.  It only works for what we actually put in the db.

                sql.WriteLine("-- MCGalaxy SQL Database Dump");
                sql.WriteLine("-- version 1.5");
                sql.WriteLine("-- http://mcgalaxy.ml");
                sql.WriteLine("--");
                sql.WriteLine("-- Host: {0}", Server.MySQLHost);
                sql.WriteLine("-- Generation Time: {0} at {1}", DateTime.Now.Date, DateTime.Now.TimeOfDay);
                sql.WriteLine("-- MCGalaxy Version: {0}", Server.Version);
                sql.WriteLine();
                //Extra stuff goes here
                sql.WriteLine();
                //database here
                List<String> sqlTables = (Database.getTables());
                foreach (string tableName in sqlTables)
                {
                    //For each table, we iterate through all rows, (and save them)
                    sql.WriteLine("-- --------------------------------------------------------");
                    sql.WriteLine();
                    sql.WriteLine("--");
                    sql.WriteLine("-- Table structure for table `{0}`", tableName);
                    sql.WriteLine("--");
                    sql.WriteLine();
                    List<string[]> tableSchema = new List<string[]>();
                    if (Server.useMySQL)
                    {
                        string[] rowParams;
                        string pri;
                        sql.WriteLine("CREATE TABLE IF NOT EXISTS `{0}` (", tableName);
                        using (DataTable tableRowSchema = Database.fillData("DESCRIBE " + tableName))
                        {
                            rowParams = new string[tableRowSchema.Columns.Count];
                            pri = "";
                            foreach (DataRow row in tableRowSchema.Rows)
                            {
                                //Save the info contained to file
                                List<string> tmp = new List<string>();
                                for (int col = 0; col < tableRowSchema.Columns.Count; col++)
                                {
                                    tmp.Add(row.Field<string>(col));
                                }// end:for(col)
                                rowParams = tmp.ToArray<string>();
                                rowParams[2] = (rowParams[2].ToLower().Equals("no") ? "NOT " : "DEFAULT ") + "NULL";
                                pri += (rowParams[3].ToLower().Equals("pri") ? rowParams[0] + ";" : "");
                                sql.WriteLine("`{0}` {1} {2}" + (rowParams[5].Equals("") ? "" : " {5}") + (pri.Equals("") && row == tableRowSchema.Rows[tableRowSchema.Rows.Count - 1] ? "" : ","), rowParams);
                                tableSchema.Add(rowParams);
                            }// end:foreach(DataRow row)
                        }
                        if (!pri.Equals(""))
                        {
                            string[] tmp = pri.Substring(0, pri.Length - 1).Split(';');
                            sql.Write("PRIMARY KEY (`");
                            foreach (string prim in tmp)
                            {
                                sql.Write(prim);
                                sql.Write("`" + (tmp[tmp.Count() - 1].Equals(prim) ? ")" : ", `"));
                            }
                        } /*else {
                            sql.Flush();
                            sql.BaseStream.Seek(-3, SeekOrigin.Current);
                        }*/
                        sql.WriteLine(");");
                    }
                    else
                    {
                        using (DataTable tableSQL = Database.fillData("SELECT sql FROM" +
                                                                            "   (SELECT * FROM sqlite_master UNION ALL" +
                                                                            "    SELECT * FROM sqlite_temp_master)" +
                                                                            "WHERE tbl_name LIKE '" + tableName + "'" +
                                                                            "  AND type!='meta' AND sql NOT NULL AND name NOT LIKE 'sqlite_%'" +
                                                                            "ORDER BY substr(type,2,1), name"))
                        {
                            //just print out the data in the table.
                            foreach (DataRow row in tableSQL.Rows)
                            {
                                string tableSQLString = row.Field<string>(0);
                                sql.WriteLine(tableSQLString.Replace(" " + tableName, " `" + tableName + "`").Replace("CREATE TABLE `" + tableName + "`", "CREATE TABLE IF NOT EXISTS `" + tableName + "`") + ";");
                                //We parse this ourselves to find the actual types.
                                tableSchema = getSchema(tableSQLString);

                            }
                        }
                    }
                    sql.WriteLine();
                    using (DataTable tableRowData = Database.fillData("SELECT * FROM  " + tableName))
                    {
                        if (tableRowData.Rows.Count > 0)
                        {
                            sql.WriteLine("--");
                            sql.WriteLine("-- Dumping data for table `{0}`", tableName);
                            sql.WriteLine("--");
                            sql.WriteLine();
                            List<DataColumn> allCols = new List<DataColumn>();
                            foreach (DataColumn col in tableRowData.Columns)
                            {
                                allCols.Add(col);
                            }
                            foreach (DataRow row in tableRowData.Rows)
                            { //We rely on the correct datatype being given here.
                                sql.WriteLine();
                                sql.Write("INSERT INTO `{0}` (`", tableName);
                                foreach (string[] rParams in tableSchema)
                                {
                                    sql.Write(rParams[0]);
                                    sql.Write((tableSchema.ElementAt<string[]>(tableSchema.Count() - 1).Equals(rParams) ? "`) VALUES" : "`, `"));
                                }
                                //Save the info contained to file
                                sql.WriteLine();
                                sql.Write("(");
                                for (int col = 0; col < row.ItemArray.Length; col++)
                                {
                                    //The values themselves can be integers or strings, or null
                                    Type eleType = allCols[col].DataType;
                                    if (row.IsNull(col))
                                    {
                                        sql.Write("NULL");

                                    }
                                    else if (eleType.Name.Equals("DateTime"))
                                    { // special format
                                        DateTime dt = row.Field<DateTime>(col);
                                        sql.Write("'{0:yyyy-MM-dd HH:mm:ss.ffff}'", dt);

                                    }
                                    else if (eleType.Name.Equals("Boolean"))
                                    {
                                        sql.Write(row.Field<Boolean>(col) ? "1" : "0");

                                    }
                                    else if (eleType.Name.Equals("String"))
                                    { // Requires ''
                                        sql.Write("'{0}'", row.Field<string>(col));

                                    }
                                    else
                                    {
                                        sql.Write(row.Field<Object>(col)); // We assume all other data is left as-is
                                        //This includes numbers, blobs, etc.  (As well as objects, but we don't save them into the database)

                                    }
                                    sql.Write((col < row.ItemArray.Length - 1 ? ", " : ");"));
                                }// end:for(col)

                            }// end:foreach(DataRow row)
                        }
                        else
                        {
                            sql.WriteLine("-- No data in table `{0}`!", tableName);
                        }
                        sql.WriteLine();
                    }

                }// end:foreach(DataRow sqlTablesRow)

            }

            private static List<string[]> getSchema(string tableSQLString)
            {
                // All SQL for creating tables looks like "CREATE TABLE [IF NOT EXISTS] <TableName> (<ColumnDef>[, ... [, PRIMARY KEY (<ColumnName>[, ...])]])
                // <ColumnDef> = <name> <type> [[NOT|DEFAULT] NULL] [PRIMARY KEY] [AUTO_INCREMENT]
                List<string[]> schema = new List<string[]>();
                int foundStart = tableSQLString.IndexOf("(") + 1;
                int foundLength = tableSQLString.LastIndexOf(")") - foundStart;
                tableSQLString = tableSQLString.Substring(foundStart, foundLength);
                // Now we have everything inside the parenthisies.
                string[] column = tableSQLString.Split(',');
                foreach (string col in column)
                {
                    if (!col.ToUpper().StartsWith("PRIMARY KEY"))
                    {
                        string[] split = col.TrimStart('\n', '\r', '\t').Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        //Just to make it the same as the MySQL schema.
                        schema.Add(new string[] { split[0].Trim('`'), split[1].Trim('\t', '`'),
                                              ( split.Count() > 2 ? (split[2].Trim('\t', '`').ToUpper() == "NOT" ? "NOT NULL" : "DEFAULT NULL") : ""),
                                              ( split.Count() > 2 ? (split[split.Count() - 2].Trim('\t', '`').ToUpper() == "PRIMARY" && split[split.Count() - 1].Trim('\t', '`').ToUpper() == "KEY" ? "PRI" : "") : ""),
                                              "NULL",
                                              (split.Contains("AUTO_INCREMENT") || split.Contains("AUTOINCREMENT") ? "AUTO_INCREMENT" : "")});
                    }
                }
                return schema;
            }

            private static List<string> getTables()
            {
                List<string> tableNames = new List<string>();
                using (DataTable tables = fillData((Server.useMySQL ? "SHOW TABLES" : "SELECT * FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'")))
                {
                    foreach (DataRow row in tables.Rows)
                    {
                        string tableName = row.Field<string>((Server.useMySQL ? 0 : 1));
                        tableNames.Add(tableName);
                    }
                }
                return tableNames;
            }// end:CopyDatabase()

            /// <summary>
            /// Adds a parameter to the parameterized SQL query.
            /// Use this before executing the query.
            /// </summary>
            /// <param name="name">The name of the parameter</param>
            /// <param name="param">The value of the parameter</param>
            public static void AddParams(string name, object param)
            {
                if (Server.useMySQL)
                    MySQL.AddParams(name, param);
                else
                    SQLite.AddParams(name, param);
            }

            public static void executeQuery(string queryString, bool createDB = false)
            {
                int totalCount = 0;
            retry: try
                {
                    if (Server.useMySQL)
                    {
                        MySQL.execute(queryString, createDB);
                    }
                    else
                    {
                        if (!createDB) // Databases do not need to be created in SQLite.
                            SQLite.execute(queryString);
                    }
                }
                catch (Exception e)
                {
                    if (!createDB || !Server.useMySQL)
                    {
                        totalCount++;
                        if (totalCount > 10)
                        {
                            File.AppendAllText("MySQL_error.log", DateTime.Now + " " + queryString + "\r\n");
                            Server.ErrorLog(e);
                        }
                        else
                        {
                            goto retry;
                        }
                    }
                    else
                    {
                        throw e;
                    }
                }
                finally
                {
                    if (Server.useMySQL)
                        MySQL.ClearParams();
                    else
                        SQLite.ClearParams();
                }
            }

            public static DataTable fillData(string queryString, bool skipError = false)
            {
                int totalCount = 0;
                using (DataTable toReturn = new DataTable("toReturn"))
                {
                retry: try
                    {
                        if (Server.useMySQL)
                        {
                            MySQL.fill(queryString, toReturn);
                        }
                        else
                        {
                            SQLite.fill(queryString, toReturn);
                        }
                    }
                    catch (Exception e)
                    {
                        totalCount++;
                        if (totalCount > 10)
                        {
                            if (!skipError)
                            {
                                File.AppendAllText("MySQL_error.log", DateTime.Now + " " + queryString + "\r\n");
                                Server.ErrorLog(e);
                            }
                        }
                        else
                            goto retry;
                    }
                    finally
                    {
                        if (Server.useMySQL)
                            MySQL.ClearParams();
                        else
                            SQLite.ClearParams();
                    }
                    return toReturn;
                }
            }

            internal static void fillDatabase(Stream stream)
            {
                //Backup
                using (FileStream backup = File.Create("backup.sql"))
                {
                    CopyDatabase(new StreamWriter(backup));
                }
                //Delete old
                List<string> tables = getTables();
                foreach (string name in tables)
                {
                    executeQuery(String.Format("DROP TABLE `{0}`", name));
                }
                //Make new
                string script = new StreamReader(stream).ReadToEnd();
                string[] cmds = script.Split(';');
                StringBuilder sb = new StringBuilder();

                using (DatabaseTransactionHelper helper = DatabaseTransactionHelper.Create())
                {

                    foreach (string cmd in cmds)
                    {
                        String newCmd = cmd.Trim(" \r\n\t".ToCharArray());
                        int index = newCmd.ToUpper().IndexOf("CREATE TABLE");
                        if (index > -1)
                        {
                            newCmd = newCmd.Remove(0, index);
                            //Do we have a primary key?
                            try
                            {
                                if (Server.useMySQL)
                                {
                                    int priIndex = newCmd.ToUpper().IndexOf(" PRIMARY KEY AUTOINCREMENT");
                                    int priCount = " PRIMARY KEY AUTOINCREMENT".Length;
                                    int attIdx = newCmd.Substring(0, newCmd.Substring(0, priIndex - 1).LastIndexOfAny("` \n".ToCharArray())).LastIndexOfAny("` \n".ToCharArray()) + 1;
                                    int attIdxEnd = newCmd.IndexOfAny("` \n".ToCharArray(), attIdx) - 1;
                                    string attName = newCmd.Substring(attIdx, attIdxEnd - attIdx + 1).Trim(' ', '\n', '`', '\r');
                                    //For speed, we just delete this, and add it to the attribute name, then delete the auto_increment clause.
                                    newCmd = newCmd.Remove(priIndex, priCount);
                                    newCmd = newCmd.Insert(newCmd.LastIndexOf(")"), ", PRIMARY KEY (`" + attName + "`)");
                                    newCmd = newCmd.Insert(newCmd.IndexOf(',', priIndex), " AUTO_INCREMENT");

                                }
                                else
                                {
                                    int priIndex = newCmd.ToUpper().IndexOf(",\r\nPRIMARY KEY");
                                    int priIndexEnd = newCmd.ToUpper().IndexOf(')', priIndex);
                                    int attIdx = newCmd.IndexOf("(", priIndex) + 1;
                                    int attIdxEnd = priIndexEnd - 1;
                                    string attName = newCmd.Substring(attIdx, attIdxEnd - attIdx + 1);
                                    newCmd = newCmd.Remove(priIndex, priIndexEnd - priIndex + 1);
                                    int start = newCmd.IndexOf(attName) + attName.Length;
                                    int end = newCmd.IndexOf(',');
                                    newCmd = newCmd.Remove(start, end - start);
                                    newCmd = newCmd.Insert(newCmd.IndexOf(attName) + attName.Length, " INTEGER PRIMARY KEY AUTOINCREMENT");
                                    newCmd = newCmd.Replace(" auto_increment", "").Replace(" AUTO_INCREMENT", "").Replace("True", "1").Replace("False", "0");
                                }
                            }
                            catch (ArgumentOutOfRangeException) { } // If we don't, just ignore it.
                        }
                        //Run the command in the transaction.
                        helper.Execute(newCmd.Replace(" unsigned", "").Replace(" UNSIGNED", "") + ";");
                        //                        sb.Append(newCmd).Append(";\n");
                    }
                    helper.Commit();



                }
                //Not sure if order matters.
                //AUTO_INCREMENT is changed to AUTOINCREMENT for MySQL -> SQLite
                //AUTOINCREMENT is changed to AUTO_INCREMENT for SQLite -> MySQL
                // All we should have in the script file is CREATE TABLE and INSERT INTO commands.
                //executeQuery(sb.ToString().Replace(" unsigned", "").Replace(" UNSIGNED", ""));
            }
        }
    }
}