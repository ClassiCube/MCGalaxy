/*
	Copyright 2015 MCGalaxy team
	
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
using System.Globalization;
using System.Text.RegularExpressions;
using MCGalaxy.SQL;
namespace MCGalaxy.Commands
{
    public sealed class CmdPlayerEditDB : Command
    {
        public override string name { get { return "playeredit"; } }
        public override string shortcut { get { return "pe"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdPlayerEditDB() { }

        public override void Use(Player p, string message)
        {
            string[] split = message.Split(' ');
            if (split[0] == "") { Help(p); return; }

            Player who = Player.Find(split[0]);
            if (who != null) { Player.SendMessage(p, "You cannot edit online players!"); return; }

            string query;
            string syntax;
            Database.AddParams("@Name", split[0]);
            if (Server.useMySQL) { syntax = "SELECT * FROM players WHERE Name=@Name COLLATE utf8_general_ci"; }
            else { syntax = "SELECT * FROM Players WHERE Name=@Name COLLATE NOCASE"; }
            DataTable playerDb = Database.fillData(syntax);
            if (playerDb.Rows.Count == 0) { Player.SendMessage(p, "Player &b" + split[0] + Server.DefaultColor + " could not be found."); return; }
            playerDb.Dispose();

            if (split.Length == 1) 
            { 
                Player.SendMessage(p, Colors.red + "You must specify a type.");
                Player.SendMessage(p, "Valid Types: FirstLogin, LastLogin, TotalLogins, Title, TotalDeaths, Money, " +
                    "TotalBlocks, TotalCuboid, TotalKicked, TimeSpent, Color, TitleColor ");
                return; 
            }
            else
            {
                if (split.Length < 2)
                {
                    Player.SendMessage(p, Colors.red + "Invalid type.");
                    Player.SendMessage(p, "Valid Types: FirstLogin, LastLogin, TotalLogins, Title, TotalDeaths, Money, " +
                        "TotalBlocks, TotalCuboid, TotalKicked, TimeSpent, Color, TitleColor ");
                    return;
                }
                switch (split[1].ToLower())
                {

                    case "firstlogin":
                        if (split.Length < 3)
                        {
                            Player.SendMessage(p, "FirstLogin Format: yyyy-mm-dd_hh:mm:ss");
                            Player.SendMessage(p, "Do not include spaces or other special characters other than what you see above.");
                        }
                        else
                        {
                            string firstTime = "";
                            string loginDate = split[2].Replace('_', ' ');
                            if (!Regex.IsMatch(loginDate, @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}"))
                            {
                                Player.SendMessage(p, "Invalid format!");
                                Player.SendMessage(p, "FirstLogin Format: yyyy-mm-dd_hh:mm:ss");
                                return;
                            }
                            else
                            {
                                try
                                {
                                    DateTime localDateTime = DateTime.Parse(loginDate);
                                    firstTime = loginDate;
                                }
                                catch (FormatException)
                                {
                                    Player.SendMessage(p, "Invalid format.");
                                    Player.SendMessage(p, "Your date must be possible via the Western Calender");
                                    Player.SendMessage(p, "Time cannot exceede increments of 23h, 59m, 59s");
                                    return;
                                }
                                query = "UPDATE Players SET FirstLogin=@Date WHERE Name=@Name";
                                Database.AddParams("@Date", firstTime);
                                Database.AddParams("@Name", split[0]);
                                Database.executeQuery(query);
                                string dateTimeMsg = String.Format("The {0} data for &b{1} " + Server.DefaultColor + "has been updated to &a{2}" + Server.DefaultColor + "!", split[1], split[0], firstTime);
                                Player.SendMessage(p, dateTimeMsg);
                                return;
                            }
                        }
                        break;
                    case "lastlogin":
                        if (split.Length < 3)
                        {
                            Player.SendMessage(p, "LastLogin Format: yyyy-mm-dd_hh:mm:ss");
                            Player.SendMessage(p, "Do not include spaces or other special characters other than what you see above.");
                        }
                        else
                        {
                            string lastTime = "";
                            string lastDate = split[2].Replace('_', ' ');
                            if (!Regex.IsMatch(lastDate, @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}"))
                            {
                                Player.SendMessage(p, "Invalid format!");
                                Player.SendMessage(p, "LastLogin Format: yyyy-mm-dd_hh:mm:ss");
                                return;
                            }
                            else
                            {
                                try
                                {
                                    DateTime localDateTime = DateTime.Parse(lastDate);
                                    lastTime = lastDate;
                                }
                                catch (FormatException)
                                {
                                    Player.SendMessage(p, "Invalid format.");
                                    Player.SendMessage(p, "Your date must be possible via the Western Calender");
                                    Player.SendMessage(p, "Time cannot exceede increments of 23h, 59m, 59s");
                                    return;
                                }
                                query = "UPDATE Players SET LastLogin=@Date WHERE Name=@Name";
                                Database.AddParams("@Date", lastTime);
                                Database.AddParams("@Name", split[0]);
                                Database.executeQuery(query);
                                string dateTimeMsg = String.Format("The {0} data for &b{1} " + Server.DefaultColor + "has been updated to &a{2}" + Server.DefaultColor + "!", split[1], split[0], lastTime);
                                Player.SendMessage(p, dateTimeMsg);
                                return;
                            }
                        }
                        break;
                    case "totallogin":
                    case "totallogins":
                        if (split.Length < 3)
                        {
                            Player.SendMessage(p, "TotalLogin Format: #");
                            Player.SendMessage(p, "This may be up to 9 numbers in length.");
                        }
                        else
                        {
                            string totalLogins = "";
                            totalLogins = split[2];
                            int logins = Int32.Parse(totalLogins);
                            if (logins < 1 || logins > 999999999) { Player.SendMessage(p, "Error: TotalLogins count must be more than zero and less than one billion."); return; }
                            query = "UPDATE Players SET totalLogin=@Int WHERE Name=@Name";
                            Database.AddParams("@Int", totalLogins);
                            Database.AddParams("@Name", split[0]);
                            Database.executeQuery(query);
                            string intMsg = String.Format("The {0} data for &b{1} " + Server.DefaultColor + "has been updated to &a{2}" + Server.DefaultColor + "!", split[1], split[0], totalLogins);
                            Player.SendMessage(p, intMsg);
                            return;
                        }
                        break;
                    case "title":
                        if (split.Length < 3)
                        {
                            Player.SendMessage(p, "Title Format: TitleName");
                            Player.SendMessage(p, "This may be up to 20 characters in length, but may NOT include spaces.");
                        }
                        else
                        {
                            string newTitle = "";
                            string titleMsg = "";
                            if (split[2].Length > 17) { Player.SendMessage(p, "Title must be under 17 letters."); return; }
                            newTitle = split[2];
                            if (split[2] == "null")
                            {
                                query = "UPDATE Players SET Title='' WHERE Name=@Name";
                            }
                            else
                            {
                                query = "UPDATE Players SET Title=@Title WHERE Name=@Name";
                            }
                            Database.AddParams("@Title", newTitle);
                            Database.AddParams("@Name", split[0]);
                            Database.executeQuery(query);
                            if (split[2] == "null")
                            {
                                titleMsg = String.Format("The {0} for &b{1} " + Server.DefaultColor + "has been removed!", split[1], split[0]);
                            }
                            else
                            {
                                titleMsg = String.Format("The {0} for &b{1} " + Server.DefaultColor + "has been updated to &a{2}" + Server.DefaultColor + "!", split[1], split[0], newTitle);
                            }
                            Player.SendMessage(p, titleMsg);

                        }
                        break;
                    case "totaldeaths":
                        if (split.Length < 3)
                        {
                            Player.SendMessage(p, "TotalDeaths Format: #");
                            Player.SendMessage(p, "This may be up to 6 numbers in length");
                        }
                        else
                        {
                            string totalDeaths = "";
                            totalDeaths = split[2];
                            int deaths = Int32.Parse(totalDeaths);
                            if (deaths < 0 || deaths > 999999) { Player.SendMessage(p, "Error: TotalDeaths count must be less than one million and cannot be negative."); return; }
                            query = "UPDATE Players SET TotalDeaths=@Int WHERE Name=@Name";
                            Database.AddParams("@Int", totalDeaths);
                            Database.AddParams("@Name", split[0]);
                            Database.executeQuery(query);
                            string intMsg = String.Format("The {0} data for &b{1} " + Server.DefaultColor + "has been updated to &a{2}" + Server.DefaultColor + "!", split[1], split[0], totalDeaths);
                            Player.SendMessage(p, intMsg);
                            return;
                        }
                        break;
                    case "money":
                        if (split.Length < 3)
                        {
                            Player.SendMessage(p, "Money Format: #");
                            Player.SendMessage(p, "This may be up to 8 numbers in length");
                        }
                        else
                        {
                            string money = "";
                            money = split[2];
                            int moneys = Int32.Parse(money);
                            if (moneys < 0 || moneys > 99999999) { Player.SendMessage(p, "Error: Money count must be less than one hundred million and cannot be negative."); return; }
                            query = "UPDATE Players SET Money=@Int WHERE Name=@Name";
                            Database.AddParams("@Int", money);
                            Database.AddParams("@Name", split[0]);
                            Database.executeQuery(query);
                            string intMsg = String.Format("The {0} data for &b{1} " + Server.DefaultColor + "has been updated to &a{2}" + Server.DefaultColor + "!", split[1], split[0], money);
                            Player.SendMessage(p, intMsg);
                            return;
                        }
                        break;
                    case "totalblocks":
                        if (split.Length < 3)
                        {
                            Player.SendMessage(p, "TotalBlocks Format: #");
                            Player.SendMessage(p, "This may be up to 20 numbers in length but less than 2147483647.");
                        }
                        else
                        {
                            string totalBlocks = "";
                            totalBlocks = split[2];
                            int blocks = Int32.Parse(totalBlocks);
                            if (blocks < 0 || blocks > 2147483647) { Player.SendMessage(p, "Error: TotalBlocks count must be less than 2147483647 and cannot be negative."); return; }
                            query = "UPDATE Players SET totalBlocks=@Int WHERE Name=@Name";
                            Database.AddParams("@Int", totalBlocks);
                            Database.AddParams("@Name", split[0]);
                            Database.executeQuery(query);
                            string intMsg = String.Format("The {0} data for &b{1} " + Server.DefaultColor + "has been updated to &a{2}" + Server.DefaultColor + "!", split[1], split[0], totalBlocks);
                            Player.SendMessage(p, intMsg);
                            return;
                        }
                        break;
                    case "totalcuboided":
                    case "totalcuboid":
                        if (split.Length < 3)
                        {
                            Player.SendMessage(p, "TotalCuboid Format: #");
                            Player.SendMessage(p, "This may be up to 20 numbers in length but less than 2147483647.");
                        }
                        else
                        {
                            string totalCuboided = "";
                            totalCuboided = split[2];
                            int cuboid = Int32.Parse(totalCuboided);
                            if (cuboid < 0 || cuboid > 2147483647) { Player.SendMessage(p, "Error: TotalCuboid count must be less than 2147483647 and cannot be negative."); return; }
                            query = "UPDATE Players SET totalCuboided=@Int WHERE Name=@Name";
                            Database.AddParams("@Int", totalCuboided);
                            Database.AddParams("@Name", split[0]);
                            Database.executeQuery(query);
                            string intMsg = String.Format("The {0} data for &b{1} " + Server.DefaultColor + "has been updated to &a{2}" + Server.DefaultColor + "!", split[1], split[0], totalCuboided);
                            Player.SendMessage(p, intMsg);
                            return;
                        }
                        break;
                    case "totalkicked":
                        if (split.Length < 3)
                        {
                            Player.SendMessage(p, "TotalKicked Format: #");
                            Player.SendMessage(p, "This may be up to 9 numbers in length.");
                        }
                        else
                        {
                            string totalKicked = "";
                            totalKicked = split[2];
                            int kicked = Int32.Parse(totalKicked);
                            if (kicked < 0 || kicked > 999999999) { Player.SendMessage(p, "Error: TotalKicked count must be less than one billion and cannot be negative."); return; }
                            query = "UPDATE Players SET totalKicked=@Int WHERE Name=@Name";
                            Database.AddParams("@Int", totalKicked);
                            Database.AddParams("@Name", split[0]);
                            Database.executeQuery(query);
                            string intMsg = String.Format("The {0} data for &b{1} " + Server.DefaultColor + "has been updated to &a{2}" + Server.DefaultColor + "!", split[1], split[0], totalKicked);
                            Player.SendMessage(p, intMsg);
                            return;
                        }
                        break;
                    case "timespent":
                        if (split.Length < 3)
                        {
                            Player.SendMessage(p, "TimeSpent Format: dd:hh:mm:ss");
                            Player.SendMessage(p, "Do not include spaces or other special characters other than what you see above.");
                        }
                        else
                        {
                            string timeSpent = "";
                            string timespentstr = split[2].Replace(':', ' ');
                            if (!Regex.IsMatch(timespentstr, @"\d{2}\s\d{2}\s\d{2}\s\d{2}"))
                            {
                                Player.SendMessage(p, "Invalid format!");
                                Player.SendMessage(p, "TimeSpent Format: dd:hh:mm:ss");
                                Player.SendMessage(p, "If your number needs to be a single digit place a 0 in front.");
                                return;
                            }
                            else
                            {
                                try
                                {
                                    string onlyTime = split[2].Substring(3, 8);
                                    DateTime timeFrame = DateTime.ParseExact(onlyTime, "HH:mm:ss", new CultureInfo("en-US") );
                                    timeSpent = timespentstr;
                                }
                                catch (FormatException)
                                {
                                    Player.SendMessage(p, "Invalid format.");
                                    Player.SendMessage(p, "TimeSpent Format: dd:hh:mm:ss");
                                    Player.SendMessage(p, "Hour range(0-23), Minute and Second range(0-59)");
                                    return;
                                }
                                query = "UPDATE Players SET TimeSpent=@Spent WHERE Name=@Name";
                                Database.AddParams("@Spent", timeSpent);
                                Database.AddParams("@Name", split[0]);
                                Database.executeQuery(query);
                                string timeSpentMsg = String.Format("The {0} data for &b{1} " + Server.DefaultColor + "has been updated to &a{2}" + Server.DefaultColor + "!", split[1], split[0], timeSpent);
                                Player.SendMessage(p, timeSpentMsg);
                                return;
                            }
                        }
                        break;
                    case "color":
                        if (split.Length < 3)
                        {
                            Player.SendMessage(p, "Color Format: ColorName");
                            Player.SendMessage(p, "Use the word \"null\" to reset to default color");
                        }
                        else
                        {
                            string color = "";
                            string titleMsg = "";
                            color = split[2];
                            if (color == "null") { query = "UPDATE Players SET color='' WHERE Name=@Name"; }
                            string trueColor = Colors.Parse(color);
                            if (trueColor == "" && color != "null") { Player.SendMessage(p, "There is no color \"" + color + "\"."); return; }
                            else
                            {
                                query = "UPDATE Players SET color=@Color WHERE Name=@Name";
                            }
                            Database.AddParams("@Color", color);
                            Database.AddParams("@Name", split[0]);
                            Database.executeQuery(query);
                            if (split[2] == "null")
                            {
                                titleMsg = String.Format("The {0} for &b{1} " + Server.DefaultColor + "has been removed!", split[1], split[0]);
                            }
                            else
                            {
                                titleMsg = String.Format("The {0} for &b{1} " + Server.DefaultColor + "has been updated to {2}{3}" + Server.DefaultColor + "!", split[1], split[0], trueColor, split[2]);
                            }
                            Player.SendMessage(p, titleMsg);

                        }
                        break;
                    case "titlecolor":
                        if (split.Length < 3)
                        {
                            Player.SendMessage(p, "TitleColor Format: ColorName");
                            Player.SendMessage(p, "Use the word \"null\" to reset to default color");
                        }
                        else
                        {
                            string titleColor = "";
                            string titleMsg = "";
                            titleColor = split[2];
                            if (titleColor == "null") { query = "UPDATE Players SET title_color='' WHERE Name=@Name"; }
                            string trueTitleColor = Colors.Parse(titleColor);
                            if (trueTitleColor == "" && titleColor != "null") { Player.SendMessage(p, "There is no color \"" + titleColor + "\"."); return; }
                            else
                            {
                                query = "UPDATE Players SET title_color=@Color WHERE Name=@Name";
                            }
                            Database.AddParams("@Color", titleColor);
                            Database.AddParams("@Name", split[0]);
                            Database.executeQuery(query);
                            if (split[2] == "null")
                            {
                                titleMsg = String.Format("The {0} for &b{1} " + Server.DefaultColor + "has been removed!", split[1], split[0]);
                            }
                            else
                            {
                                titleMsg = String.Format("The {0} for &b{1} " + Server.DefaultColor + "has been updated to {2}{3}" + Server.DefaultColor + "!", split[1], split[0], trueTitleColor, split[2]);
                            }
                            Player.SendMessage(p, titleMsg);

                        }
                        break;
                    default:
                        {
                            Player.SendMessage(p, Colors.red + "Invalid type.");
                            Player.SendMessage(p, "Valid Types: FirstLogin, LastLogin, TotalLogins, Title, TotalDeaths, Money, " +
                                "TotalBlocks, TotalCuboid, TotalKicked, TimeSpent, Color, TitleColor ");
                        }
                        break;
                }
            }
        }


        public override void Help(Player p)
        {
            Player.SendMessage(p, "/pe <username> <type> <value>");
            Player.SendMessage(p, "Used to edit an offline player's information while in-game. Use with caution!");
            Player.SendMessage(p, "Types: FirstLogin, LastLogin, TotalLogins, Title, TotalDeaths, Money, " +
                "TotalBlocks, TotalCuboid, TotalKicked, TimeSpent, Color, TitleColor ");
            Player.SendMessage(p, "To see Value parameters, leave <value> blank when selecting a type.");
        }
    }
}
