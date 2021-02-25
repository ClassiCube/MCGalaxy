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
    public sealed class CmdInbox : Command2 {
        public override string name { get { return "Inbox"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool SuperUseable { get { return false; } }
        const int i_text = 0, i_sent = 1, i_from = 2;
        
        public override void Use(Player p, string message, CommandData data) {
            if (!Database.TableExists("Inbox" + p.name)) {
                p.Message("Your inbox is empty."); return;
            }
            
            List<string[]> entries = Database.GetRows("Inbox" + p.name, "Contents,TimeSent,PlayerFrom", 
                                                      "ORDER BY TimeSent");
            if (entries.Count == 0) {
                p.Message("Your inbox is empty."); return;
            }

            string[] args = message.SplitSpaces(2);
            if (message.Length == 0) {
                foreach (string[] entry in entries) { Output(p, entry); }
            } else if (IsDeleteCommand(args[0])) {
                if (args.Length == 1) {
                    p.Message("You need to provide either \"all\" or a number."); return;
                } else if (args[1].CaselessEq("all")) {
                    Database.DeleteRows("Inbox" + p.name);
                    p.Message("Deleted all messages.");
                } else {
                    DeleteByID(p, args[1], entries);
                }
            } else {
                OutputByID(p, message, entries);
            }
        }
        
        static void DeleteByID(Player p, string value, List<string[]> entries) {
            int num = 1;
            if (!CommandParser.GetInt(p, value, "Message number", ref num, 1)) return;
            
            if (num > entries.Count) {
                p.Message("Message #{0} does not exist.", num);
            } else {
                string[] entry = entries[num - 1];
                Database.DeleteRows("Inbox" + p.name,
                                    "WHERE PlayerFrom=@0 AND TimeSent=@1", entry[i_from], entry[i_sent]);
                p.Message("Deleted message #{0}", num);
            }
        }
        
        static void OutputByID(Player p, string value, List<string[]> entries) {
            int num = 1;
            if (!CommandParser.GetInt(p, value, "Message number", ref num, 1)) return;
            
            if (num > entries.Count) {
                p.Message("Message #{0} does not exist.", num);
            } else {
                Output(p, entries[num - 1]);
            }
        }
        
        static void Output(Player p, string[] entry) {
            DateTime time = entry[i_sent].ParseDBDate();
            TimeSpan delta = DateTime.Now - time;
            string sender = p.FormatNick(entry[i_from]);
            
            p.Message("From {0} &a{1} ago:", sender, delta.Shorten());
            p.Message(entry[i_text]);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Inbox");
            p.Message("&HDisplays all your messages.");
            p.Message("&T/Inbox [num]");
            p.Message("&HDisplays the message at [num]");
            p.Message("&T/Inbox del [num]/all");
            p.Message("&HDeletes the message at [num], deletes all messages if \"all\"");
            p.Message("  &HUse &T/Send &Hto reply to a message");
        }
    }
}
