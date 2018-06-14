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
using System.Collections.Generic;
using System.Data;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands.Chatting {
    public sealed class CmdInbox : Command {
        public override string name { get { return "Inbox"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool SuperUseable { get { return false; } }

        class MailEntry { public string Contents, Timestamp, From; }
        static object ReadInbox(IDataRecord record, object arg) {
            MailEntry e = new MailEntry();
            e.Contents  = record.GetString("Contents");
            e.Timestamp = record.GetString("TimeSent");
            e.From      = record.GetString("PlayerFrom");
            
            ((List<MailEntry>)arg).Add(e); return arg;
        }
        
        public override void Use(Player p, string message) {
            List<MailEntry> entries = new List<MailEntry>();
            Database.Backend.ReadRows("Inbox" + p.name, "*",
                                      entries, ReadInbox, "ORDER BY TimeSent");
            if (entries.Count == 0) {
                Player.Message(p, "Your inbox is empty."); return;
            }

            string[] args = message.SplitSpaces(2);
            if (message.Length == 0) {
                foreach (MailEntry entry in entries) { Output(p, entry); }
            } else if (IsDeleteCommand(args[0])) {
                if (args.Length == 1) {
                    Player.Message(p, "You need to provide either \"all\" or a number."); return;
                } else if (args[1].CaselessEq("all")) {
                    Database.Backend.ClearTable("Inbox" + p.name);
                    Player.Message(p, "Deleted all messages.");
                } else {
                    DeleteByID(p, args[1], entries);
                }
            } else {
                OutputByID(p, message, entries);
            }
        }
        
        static void DeleteByID(Player p, string value, List<MailEntry> entries) {
            int num = 1;
            if (!CommandParser.GetInt(p, value, "Message number", ref num, 1)) return;
            
            if (num > entries.Count) {
                Player.Message(p, "Message #{0} does not exist.", num);
            } else {
                MailEntry entry = entries[num - 1];
                Database.Backend.DeleteRows("Inbox" + p.name,
                                            "WHERE PlayerFrom=@0 AND TimeSent=@1", entry.From, entry.Timestamp);
                Player.Message(p, "Deleted message #{0}", num);
            }
        }
        
        static void OutputByID(Player p, string value, List<MailEntry> entries) {
            int num = 1;
            if (!CommandParser.GetInt(p, value, "Message number", ref num, 1)) return;
            
            if (num > entries.Count) {
                Player.Message(p, "Message #{0} does not exist.", num);
            } else {
                Output(p, entries[num - 1]);
            }
        }
        
        static void Output(Player p, MailEntry entry) {
            TimeSpan delta = DateTime.Now - DateTime.Parse(entry.Timestamp);
            string sender = PlayerInfo.GetColoredName(p, entry.From);
            
            Player.Message(p, "From {0} &a{1} ago:", sender, delta.Shorten());
            Player.Message(p, entry.Contents);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Inbox");
            Player.Message(p, "%HDisplays all your messages.");
            Player.Message(p, "%T/Inbox [num]");
            Player.Message(p, "%HDisplays the message at [num]");
            Player.Message(p, "%T/Inbox del [num]/all");
            Player.Message(p, "%HDeletes the message at [num], deletes all messages if \"all\"");
            Player.Message(p, "  %HUse %T/Send %Hto reply to a message");
        }
    }
}
