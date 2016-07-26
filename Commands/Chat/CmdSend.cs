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

            Player who = PlayerInfo.Find(parts[0]);
            string whoTo = who == null ? parts[0] : who.name;
            string fromname = p == null ? "(console)" : p.name;
            if (!Player.ValidName(whoTo)) {
                Player.Message(p, "%cIllegal name!"); return;
            }

            message = parts[1];
            //DB
            if (message.Length >= 256 && Server.useMySQL) { 
                Player.Message(p, "Message was too long. It has been trimmed to:"); 
                Player.Message(p, message.Substring(0, 255)); 
                message = message.Substring(0, 255);
            }
            //safe against SQL injections because whoTo is checked for illegal characters
            Database.Execute("CREATE TABLE if not exists `Inbox" + whoTo + 
                             "` (PlayerFrom CHAR(20), TimeSent DATETIME, Contents VARCHAR(255));");
            Database.Execute("INSERT INTO `Inbox" + whoTo + "` (PlayerFrom, TimeSent, Contents) VALUES (@0, @1, @2)",
                             fromname, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message);

            Player.Message(p, "Message sent to &5" + whoTo + ".");
            if (who != null) who.SendMessage("Message recieved from &5" + fromname + "%S.");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/send [name] <message>");
            Player.Message(p, "%HSends <message> to [name].");
        }
    }
}
