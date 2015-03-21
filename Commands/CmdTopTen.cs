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
using System.Data;
using MCGalaxy.SQL;
namespace MCGalaxy.Commands
{
    public sealed class CmdTopTen : Command
    {
        public override string name { get { return "topten"; } }
        public override string shortcut { get { return "10"; } }
        public override string type { get { return "information"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdTopTen() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }

            if (String.Compare(message, "1", true) == 0)
            {
                DataTable playerDb = Database.fillData("SELECT distinct name, totallogin FROM Players order by totallogin desc limit 10");

                Player.SendMessage(p, "TOP TEN NUMBER OF LOGINS:");
                for (int i = 0; i < playerDb.Rows.Count; i++)
                {
                    Player.SendMessage(p, (i + 1) + ") " + playerDb.Rows[i]["Name"] + " - [" + playerDb.Rows[i]["TotalLogin"] + "]");
                }

                playerDb.Dispose();
            }
            if (String.Compare(message, "2", true) == 0)
            {
                DataTable playerDb = Database.fillData("SELECT distinct name, totaldeaths FROM Players order by totaldeaths desc limit 10");

                Player.SendMessage(p, "TOP TEN NUMBER OF DEATHS:");
                for (int i = 0; i < playerDb.Rows.Count; i++)
                {
                    Player.SendMessage(p, (i + 1) + ") " + playerDb.Rows[i]["Name"] + " - [" + playerDb.Rows[i]["TotalDeaths"] + "]");
                }

                playerDb.Dispose();
            }
            if (String.Compare(message, "3", true) == 0)
            {
                DataTable playerDb = Database.fillData("SELECT distinct player, money FROM Economy order by money desc limit 10");

                Player.SendMessage(p, "TOP TEN AMOUNTS OF MONEY:");
                for (int i = 0; i < playerDb.Rows.Count; i++)
                {
                    Player.SendMessage(p, (i + 1) + ") " + playerDb.Rows[i]["player"] + " - [" + playerDb.Rows[i]["money"] + "]");
                }

                playerDb.Dispose();
            }
            if (String.Compare(message, "4", true) == 0)
            {
                DataTable playerDb = Database.fillData("SELECT distinct name, firstlogin FROM Players order by firstlogin asc limit 10");

                Player.SendMessage(p, "FIRST PLAYERS:");
                for (int i = 0; i < playerDb.Rows.Count; i++)
                {
                    Player.SendMessage(p, (i + 1) + ") " + playerDb.Rows[i]["Name"] + " - [" + playerDb.Rows[i]["firstlogin"] + "]");
                }

                playerDb.Dispose();
            }
            if (String.Compare(message, "5", true) == 0)
            {
                DataTable playerDb = Database.fillData("SELECT distinct name, lastlogin  FROM Players order by lastlogin desc limit 10");

                Player.SendMessage(p, "MOST RECENT PLAYERS:");
                for (int i = 0; i < playerDb.Rows.Count; i++)
                {
                    Player.SendMessage(p, (i + 1) + ") " + playerDb.Rows[i]["Name"] + " - [" + playerDb.Rows[i]["lastlogin"] + "]");
                }

                playerDb.Dispose();
            }
            if (String.Compare(message, "6", true) == 0)
            {
                DataTable playerDb = Database.fillData("SELECT distinct name, totalblocks FROM Players order by totalblocks desc limit 10");

                Player.SendMessage(p, "TOP TEN NUMBER OF BLOCKS MODIFIED:");
                for (int i = 0; i < playerDb.Rows.Count; i++)
                {
                    Player.SendMessage(p, (i + 1) + ") " + playerDb.Rows[i]["Name"] + " - [" + playerDb.Rows[i]["TotalBlocks"] + "]");
                }

                playerDb.Dispose();
            }
            if (String.Compare(message, "7", true) == 0)
            {
                DataTable playerDb = Database.fillData("SELECT distinct name, totalkicked FROM Players order by totalkicked desc limit 10");

                Player.SendMessage(p, "TOP TEN NUMBER OF KICKS:");
                for (int i = 0; i < playerDb.Rows.Count; i++)
                {
                    Player.SendMessage(p, (i + 1) + ") " + playerDb.Rows[i]["Name"] + " - [" + playerDb.Rows[i]["TotalKicked"] + "]");
                }

                playerDb.Dispose();
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "%2/topten [#] - Prints a particular top ten list");
            Player.SendMessage(p, "1) Number of Logins");
            Player.SendMessage(p, "2) Number of Deaths");
            Player.SendMessage(p, "3) Money");
            Player.SendMessage(p, "4) First Players");
            Player.SendMessage(p, "5) Recent Players");
            Player.SendMessage(p, "6) Blocks Modified");
            Player.SendMessage(p, "7) Number of Kicks");
        }
    }
}