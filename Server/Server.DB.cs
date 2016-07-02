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
using System.Linq;
using MCGalaxy.SQL;

namespace MCGalaxy {
    public sealed partial class Server {
        
        const string createPlayers =
            @"CREATE TABLE if not exists Players (
ID            INTEGER {0}{2}INCREMENT NOT NULL,
Name          TEXT, 
IP            CHAR(15), 
FirstLogin    DATETIME, 
LastLogin     DATETIME, 
totalLogin    MEDIUMINT,
Title         CHAR(20), 
TotalDeaths   SMALLINT, 
Money         MEDIUMINT UNSIGNED, 
totalBlocks   BIGINT,
totalCuboided BIGINT, 
totalKicked   MEDIUMINT, 
TimeSpent     VARCHAR(20), 
color         VARCHAR(6),
title_color   VARCHAR(6){1});";
        
        const string createOpstats = 
            @"CREATE TABLE if not exists Opstats (
ID     INTEGER {0}{2}INCREMENT NOT NULL,
Time   DATETIME, 
Name   TEXT, 
Cmd    VARCHAR(40), 
Cmdmsg VARCHAR(40){1});";
        
        const string insertSyntax = 
            @"INSERT INTO Opstats (Time, Name, Cmd, Cmdmsg) 
SELECT Time, Name, Cmd, Cmdmsg FROM Playercmds WHERE {0};";
        
        void InitDatabase() {
            try {
                if (Server.useMySQL)
                    Database.executeQuery("CREATE DATABASE if not exists `" + MySQLDatabaseName + "`", true);
            } catch (Exception e) {
                ErrorLog(e);
                s.Log("MySQL settings have not been set! Please Setup using the properties window.");
                return;
            }
            
            string prim1 = useMySQL ? "" : "PRIMARY KEY ";
            string prim2 = useMySQL ? ", PRIMARY KEY (ID)" : "";
            string autoI = useMySQL ? "AUTO_" : "AUTO";
            Database.executeQuery(string.Format(createPlayers, prim1, prim2, autoI));
            Database.executeQuery(string.Format(createOpstats, prim1, prim2, autoI));
            if (!File.Exists("extra/alter.txt") && useMySQL) {
                Database.executeQuery("ALTER TABLE Players MODIFY Name TEXT");
                Database.executeQuery("ALTER TABLE Opstats MODIFY Name TEXT");
                File.Create("extra/alter.txt");
            }
            
            //since 5.5.11 we are cleaning up the table Playercmds
            //if Playercmds exists copy-filter to Ostats and remove Playercmds
            if (Database.TableExists("Playercmds")) {
                foreach (string cmd in Server.Opstats)
                    Database.executeQuery(string.Format(insertSyntax, "cmd = '" + cmd + "'"));
                Database.executeQuery(string.Format(insertSyntax, "cmd = 'review' AND cmdmsg = 'next'"));
                Database.executeQuery("DROP TABLE Playercmds");
            }

            // Here, since SQLite is a NEW thing from 5.3.0.0, we do not have to check for existing tables in SQLite.
            if (!useMySQL) return;
            // Check if the color column exists.
            DataTable colorExists = Database.fillData("SHOW COLUMNS FROM Players WHERE `Field`='color'");
            if (colorExists.Rows.Count == 0)
                Database.executeQuery("ALTER TABLE Players ADD COLUMN color VARCHAR(6) AFTER totalKicked");
            colorExists.Dispose();

            DataTable tcolorExists = Database.fillData("SHOW COLUMNS FROM Players WHERE `Field`='title_color'");
            if (tcolorExists.Rows.Count == 0)
                Database.executeQuery("ALTER TABLE Players ADD COLUMN title_color VARCHAR(6) AFTER color");
            tcolorExists.Dispose();

            DataTable timespent = Database.fillData("SHOW COLUMNS FROM Players WHERE `Field`='TimeSpent'");
            if (timespent.Rows.Count == 0)
                Database.executeQuery("ALTER TABLE Players ADD COLUMN TimeSpent VARCHAR(20) AFTER totalKicked");
            timespent.Dispose();

            DataTable totalCuboided = Database.fillData("SHOW COLUMNS FROM Players WHERE `Field`='totalCuboided'");
            if (totalCuboided.Rows.Count == 0)
                Database.executeQuery("ALTER TABLE Players ADD COLUMN totalCuboided BIGINT AFTER totalBlocks");
            totalCuboided.Dispose();
        }
    }
}