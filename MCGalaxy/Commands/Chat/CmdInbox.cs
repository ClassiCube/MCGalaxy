/*
    Copyright 2011 MCForge
        
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
using MCGalaxy.SQL;

namespace MCGalaxy.Commands {
    public sealed class CmdInbox : Command {
        public override string name { get { return "inbox"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdInbox() { }

        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); }
            string[] parts = message.ToLower().SplitSpaces(2);
            if (!Database.TableExists("Inbox" + p.name)) {
                Player.Message(p, "Your inbox is empty."); return;
            }

            if (message == "") {
                using (DataTable Inbox = Database.Backend.GetRows("Inbox" + p.name, "*", "ORDER BY TimeSent")) {
                    if (Inbox.Rows.Count == 0) { Player.Message(p, "No messages found."); return; }
                    foreach (DataRow row in Inbox.Rows) {
                        OutputMessage(p, row);
                    }
                }
            } else if (parts[0] == "del" || parts[0] == "delete") {
                int num = -1;
                if (parts.Length == 1) {
                    Player.Message(p, "You need to provide either \"all\" or a number."); return;
                }
                if (parts[1] != "all" && !int.TryParse(parts[1], out num)) {
                    Player.Message(p, "Incorrect number given."); return;
                }
                if (parts[1] != "all" && num < 0) {
                    Player.Message(p, "Message number must be greater than or equal to 0."); return;
                }
                
                using (DataTable Inbox = Database.Backend.GetRows("Inbox" + p.name, "*", "ORDER BY TimeSent")) {
                    if (num != -1 && num >= Inbox.Rows.Count) {
                        Player.Message(p, "\"" + num + "\" does not exist."); return;
                    }

                    //safe against SQL injections because no user input is given here
                    if (num == -1) {
                        Database.Backend.ClearTable("Inbox" + p.name);
                    } else {
                        DataRow row = Inbox.Rows[num];
                        string time = Convert.ToDateTime(row["TimeSent"]).ToString("yyyy-MM-dd HH:mm:ss");
                        Database.Backend.DeleteRows("Inbox" + p.name, 
                                                    "WHERE PlayerFrom=@0 AND TimeSent=@1", row["PlayerFrom"], time);
                    }

                    if (num == -1) Player.Message(p, "Deleted all messages.");
                    else Player.Message(p, "Deleted message.");
                }
            } else {
                int num;
                if (!int.TryParse(message, out num)) { Player.Message(p, "Incorrect number given."); return; }
                if (num < 0) { Player.Message(p, "Message number must be greater than or equal to 0."); return; }

                using (DataTable Inbox = Database.Backend.GetRows("Inbox" + p.name, "*", "ORDER BY TimeSent")) {
                    if (num >= Inbox.Rows.Count) {
                        Player.Message(p, "Message number \"" + num + "\" does not exist."); return;
                    }
                    OutputMessage(p, Inbox.Rows[num]);
                }
            }
        }
        
        static void OutputMessage(Player p, DataRow row) {
            DateTime time = Convert.ToDateTime(row["TimeSent"]);
            TimeSpan delta = DateTime.Now - time;
            Player.Message(p, "From &5{0} &a{1} ago:", row["PlayerFrom"], delta.Shorten());
            Player.Message(p, row["Contents"].ToString());
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/inbox");
            Player.Message(p, "%HDisplays all your messages.");
            Player.Message(p, "%T/inbox [num]");
            Player.Message(p, "%HDisplays the message at [num]");
            Player.Message(p, "%T/inbox del [num]/all");
            Player.Message(p, "%HDeletes the message at [num], deletes all messages if \"all\".");
        }
    }
}
