/*
    Copyright 2011 MCForge
        
    Dual-licensed under the Educational Community License, Version 2.0 and
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
using System.Text.RegularExpressions;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands.Chatting {
    public sealed class CmdSend : Command2 {
        public override string name { get { return "Send"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public override bool UseableWhenFrozen { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            string[] parts = message.SplitSpaces(2);
            if (message.Length == 0 || parts.Length == 1) { Help(p); return; }
            if (!MessageCmd.CanSpeak(p, name)) return;

            string receiverName = PlayerInfo.FindMatchesPreferOnline(p, parts[0]);
            if (receiverName == null) return;

            message = parts[1];
            //DB
            if (message.Length >= 256 && Database.Backend.EnforcesTextLength) {
                p.Message("Message was too long. It has been trimmed to:");
                p.Message(message.Substring(0, 255));
                message = message.Substring(0, 255);
            }
            
            Database.Backend.CreateTable("Inbox" + receiverName, createInbox);
            Database.Backend.AddRow("Inbox" + receiverName, "PlayerFrom, TimeSent, Contents",
                                    p.name, DateTime.Now.ToString(Database.DateFormat), message);

            Player receiver = PlayerInfo.FindExact(receiverName);
            p.Message("Message sent to {0}%S.", PlayerInfo.GetColoredName(p, receiverName));
            if (receiver == null) return;
            
            if (!Chat.Ignoring(receiver, p)) {
                receiver.Message("Message recieved from {0}%S. Check %T/inbox", p.ColoredName);
            }
        }
        
        static ColumnDesc[] createInbox = new ColumnDesc[] {
            new ColumnDesc("PlayerFrom", ColumnType.Char, 20),
            new ColumnDesc("TimeSent", ColumnType.DateTime),
            new ColumnDesc("Contents", ColumnType.VarChar, 255),
        };
        
        public override void Help(Player p) {
            p.Message("%T/Send [name] [message]");
            p.Message("%HSends [message] to [name], which can be read with %T/inbox");
        }
    }
}
