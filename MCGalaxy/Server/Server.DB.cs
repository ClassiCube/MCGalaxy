/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.IO;
using MCGalaxy.SQL;

namespace MCGalaxy {
    public sealed partial class Server {
        
        static ColumnParams[] createPlayers = {
            new ColumnParams("ID", ColumnType.Integer, priKey: true, autoInc: true, notNull: true),
            new ColumnParams("Name", ColumnType.Text),
            new ColumnParams("IP", ColumnType.Char, 15),
            new ColumnParams("FirstLogin", ColumnType.DateTime),
            new ColumnParams("LastLogin", ColumnType.DateTime),
            new ColumnParams("totalLogin", ColumnType.Int24),
            new ColumnParams("Title", ColumnType.Char, 20),
            new ColumnParams("TotalDeaths", ColumnType.Int16),
            new ColumnParams("Money", ColumnType.UInt24),
            new ColumnParams("totalBlocks", ColumnType.Int64),
            new ColumnParams("totalCuboided", ColumnType.Int64),
            new ColumnParams("totalKicked", ColumnType.Int24),
            new ColumnParams("TimeSpent", ColumnType.VarChar, 20),
            new ColumnParams("color", ColumnType.VarChar, 6),
            new ColumnParams("title_color", ColumnType.VarChar, 6),
        };
        
        static ColumnParams[] createOpstats = {
            new ColumnParams("ID", ColumnType.Integer, priKey: true, autoInc: true, notNull: true),
            new ColumnParams("Time", ColumnType.DateTime),
            new ColumnParams("Cmd", ColumnType.VarChar, 40),
            new ColumnParams("Cmdmsg", ColumnType.VarChar, 40),
        };
        
        const string insertSyntax = 
            @"INSERT INTO Opstats (Time, Name, Cmd, Cmdmsg) 
SELECT Time, Name, Cmd, Cmdmsg FROM Playercmds WHERE {0};";
        
        void InitDatabase() {
            try {
                Database.Backend.CreateDatabase();
            } catch (Exception e) {
                ErrorLog(e);
                s.Log("MySQL settings have not been set! Please Setup using the properties window.");
                return;
            }

            Database.Backend.CreateTable("Opstats", createOpstats);
            Database.Backend.CreateTable("Players", createPlayers);
            if (!File.Exists("extra/alter.txt") && useMySQL) {
                Database.Execute("ALTER TABLE Players MODIFY Name TEXT");
                Database.Execute("ALTER TABLE Opstats MODIFY Name TEXT");
                File.Create("extra/alter.txt");
            }
            
            //since 5.5.11 we are cleaning up the table Playercmds
            //if Playercmds exists copy-filter to Opstats and remove Playercmds
            if (Database.TableExists("Playercmds")) {
                foreach (string cmd in Server.Opstats)
                    Database.Execute(string.Format(insertSyntax, "cmd = '" + cmd + "'"));
                Database.Execute(string.Format(insertSyntax, "cmd = 'review' AND cmdmsg = 'next'"));
                Database.Backend.DeleteTable("Playercmds");
            }

            // Here, since SQLite is a NEW thing from 5.3.0.0, we do not have to check for existing tables in SQLite.
            if (!useMySQL) return;
            // Check if the color column exists.
            DataTable colorExists = Database.Fill("SHOW COLUMNS FROM Players WHERE `Field`='color'");
            if (colorExists.Rows.Count == 0)
                Database.Backend.AddColumn("Players", "color", "VARCHAR(6)", "totalKicked");
            colorExists.Dispose();

            DataTable tcolorExists = Database.Fill("SHOW COLUMNS FROM Players WHERE `Field`='title_color'");
            if (tcolorExists.Rows.Count == 0)
                Database.Backend.AddColumn("Players", "title_color", "VARCHAR(6)", "color");
            tcolorExists.Dispose();

            DataTable timespent = Database.Fill("SHOW COLUMNS FROM Players WHERE `Field`='TimeSpent'");
            if (timespent.Rows.Count == 0)
                Database.Backend.AddColumn("Players", "TimeSpent", "VARCHAR(20)", "totalKicked");
            timespent.Dispose();

            DataTable totalCuboided = Database.Fill("SHOW COLUMNS FROM Players WHERE `Field`='totalCuboided'");
            if (totalCuboided.Rows.Count == 0)
                Database.Backend.AddColumn("Players", "totalCuboided", "BIGINT", "totalBlocks");
            totalCuboided.Dispose();
        }
    }
}