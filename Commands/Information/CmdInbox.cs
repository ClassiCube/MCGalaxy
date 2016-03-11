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
using System.Data;
using MCGalaxy.SQL;
namespace MCGalaxy.Commands
{
    public sealed class CmdInbox : Command
    {
        public override string name { get { return "inbox"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdInbox() { }

        public override void Use(Player p, string message)
        {
            try
            {
                //safe against SQL injections because no user input is given here
                if (Server.useMySQL) Database.executeQuery("CREATE TABLE if not exists `Inbox" + p.name + "` (PlayerFrom CHAR(20), TimeSent DATETIME, Contents VARCHAR(255));"); 
                else Database.executeQuery("CREATE TABLE if not exists `Inbox" + p.name + "` (PlayerFrom TEXT, TimeSent DATETIME, Contents TEXT);");
                if (message == "")
                {
                    //safe against SQL injections because no user input is given here
                    DataTable Inbox = Database.fillData("SELECT * FROM `Inbox" + p.name + "` ORDER BY TimeSent");

                    if (Inbox.Rows.Count == 0) { Player.SendMessage(p, "No messages found."); Inbox.Dispose(); return; }

                    for (int i = 0; i < Inbox.Rows.Count; ++i)
                    {
                        Player.SendMessage(p, i + ": From &5" + Inbox.Rows[i]["PlayerFrom"].ToString() + Server.DefaultColor + " at &a" + Inbox.Rows[i]["TimeSent"].ToString());
                    }
                    Inbox.Dispose();
                }
                else if (message.Split(' ')[0].ToLower() == "del" || message.Split(' ')[0].ToLower() == "delete")
                {
                    int FoundRecord = -1;

                    if (message.Split(' ')[1].ToLower() != "all")
                    {
                        try
                        {
                            FoundRecord = int.Parse(message.Split(' ')[1]);
                        }
                        catch { Player.SendMessage(p, "Incorrect number given."); return; }

                        if (FoundRecord < 0) { Player.SendMessage(p, "Cannot delete records below 0"); return; }
                    }
                    //safe against SQL injections because no user input is given here
                    DataTable Inbox = Database.fillData("SELECT * FROM `Inbox" + p.name + "` ORDER BY TimeSent");

                    if (Inbox.Rows.Count - 1 < FoundRecord || Inbox.Rows.Count == 0)
                    {
                        Player.SendMessage(p, "\"" + FoundRecord + "\" does not exist."); Inbox.Dispose(); return;
                    }

                    DatabaseParameterisedQuery query = DatabaseParameterisedQuery.Create();
                    string queryString;
                    //safe against SQL injections because no user input is given here
                    if (FoundRecord == -1)
                        queryString = Server.useMySQL ? "TRUNCATE TABLE `Inbox" + p.name + "`" : "DELETE FROM `Inbox" + p.name + "`";
                    else {
                        query.AddParam("@From", Inbox.Rows[FoundRecord]["PlayerFrom"]);
                        query.AddParam("@Time", Convert.ToDateTime(Inbox.Rows[FoundRecord]["TimeSent"]).ToString("yyyy-MM-dd HH:mm:ss"));
                        queryString = "DELETE FROM `Inbox" + p.name + "` WHERE PlayerFrom=@FROM AND TimeSent=@Time";
                    }
                    Database.executeQuery(query, queryString);

                    if (FoundRecord == -1)
                        Player.SendMessage(p, "Deleted all messages.");
                    else
                        Player.SendMessage(p, "Deleted message.");

                    Inbox.Dispose();
                }
                else
                {
                    int FoundRecord;

                    try
                    {
                        FoundRecord = int.Parse(message);
                    }
                    catch { Player.SendMessage(p, "Incorrect number given."); return; }

                    if (FoundRecord < 0) { Player.SendMessage(p, "Cannot read records below 0"); return; }

                    //safe against SQL injections because no user input is given here
                    DataTable Inbox = Database.fillData("SELECT * FROM `Inbox" + p.name + "` ORDER BY TimeSent");

                    if (Inbox.Rows.Count - 1 < FoundRecord || Inbox.Rows.Count == 0)
                    {
                        Player.SendMessage(p, "\"" + FoundRecord + "\" does not exist."); Inbox.Dispose(); return;
                    }

                    Player.SendMessage(p, "Message from &5" + Inbox.Rows[FoundRecord]["PlayerFrom"] + Server.DefaultColor + " sent at &a" + Inbox.Rows[FoundRecord]["TimeSent"] + ":");
                    Player.SendMessage(p, Inbox.Rows[FoundRecord]["Contents"].ToString());
                    Inbox.Dispose();
                }
            }
            catch
            {
                Player.SendMessage(p, "Error accessing inbox. You may have no mail, try again.");
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/inbox - Displays all your messages.");
            Player.SendMessage(p, "/inbox [num] - Displays the message at [num]");
            Player.SendMessage(p, "/inbox <del> [\"all\"/num] - Deletes the message at Num or All if \"all\" is given.");
        }
    }
}
