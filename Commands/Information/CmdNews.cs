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
using System.Collections.Generic;
using System.IO;

namespace MCGalaxy.Commands
{
    public sealed class CmdNews : Command
    {
        public override string name { get { return "news"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can send the news to everyone") }; }
        }
        
        const string newsFile = "text/news.txt";
        public override void Use(Player p, string message) {
            if (!File.Exists(newsFile)) {
                CP437Writer.WriteAllText(newsFile, "News have not been created. Put News in '" + newsFile + "'."); return;
            }
            
            List<string> lines = CP437Reader.ReadAllLines(newsFile);
            if (message == "") {
                foreach (string line in lines)
                    Player.Message(p, line);
                return;
            }
            
            string[] args = message.Split(' ');
            if (args[0] == "all") {
            	if (!CheckExtraPerm(p)) { MessageNeedPerms(p, "can send the server news to all players."); return; }
                foreach (string line in lines)
                    Player.GlobalMessage(line);
                return;
            }
            
            Player who = PlayerInfo.FindMatches(p, args[0]);
            if (who == null) return;
            foreach (string line in lines)
                Player.Message(who, line);
            Player.Message(p, "The News were successfully sent to " + who.name + ".");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/news - Shows server news.");
            Player.Message(p, "/news <player> - Sends the News to <player>.");
            Player.Message(p, "/news all - Sends the News to everyone.");
        }
    }
}