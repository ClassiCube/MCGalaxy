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

namespace MCGalaxy.Commands {
    public sealed class CmdSend : Command {
        public override string name { get { return "send"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdSend() { }

        public override void Use(Player p, string message) {
            string[] parts = message.SplitSpaces(2);
            if (message == "" || parts.Length == 1) { Help(p); return; }

            string receiverName = PlayerInfo.FindMatchesPreferOnline(p, parts[0]);
            if (receiverName == null) return;
            string senderName = p == null ? "(console)" : p.name;
            string senderNick = p == null ? "(console)" : p.ColoredName;

            message = parts[1];
            //DB
            if (message.Length >= 256 && Database.Backend.EnforcesTextLength) {
                Player.Message(p, "Message was too long. It has been trimmed to:");
                Player.Message(p, message.Substring(0, 255));
                message = message.Substring(0, 255);
            }
            
            //safe against SQL injections because whoTo is checked for illegal characters
            Database.Backend.CreateTable("Inbox" + receiverName, createInbox);
            Database.Backend.AddRow("Inbox" + receiverName, "PlayerFrom, TimeSent, Contents",
                                    senderName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message);

            Player receiver = PlayerInfo.FindExact(receiverName);
            Player.Message(p, "Message sent to {0}%S.", 
                           PlayerInfo.GetColoredName(p, receiverName));
            if (receiver == null) return;
            
            if (Player.IsSuper(p)) {
                receiver.SendMessage("Message recieved from " + senderNick + "%S.");
            } else {
                p.MessageTo(receiver, "Message recieved from " + senderNick + "%S.");
            }
        }
        
        static ColumnDesc[] createInbox = {
            new ColumnDesc("PlayerFrom", ColumnType.Char, 20),
            new ColumnDesc("TimeSent", ColumnType.DateTime),
            new ColumnDesc("Contents", ColumnType.VarChar, 255),
        };
        
        public override void Help(Player p) {
            Player.Message(p, "%T/send [name] [message]");
            Player.Message(p, "%HSends [message] to [name], which can be read with /inbox");
        }
    }
}
