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
using System.Collections.Generic;
using System.IO;

namespace MCGalaxy.Commands
{
    public sealed class CmdNews : Command
    {
        public override string name { get { return "news"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "The lowest rank that can send the news to everyone") }; }
        }
        
        public override void Use(Player p, string message)
        {
            string newsFile = "text/news.txt";
            if (!File.Exists(newsFile))
            {
            	CP437Writer.WriteAllText(newsFile, "News have not been created. Put News in '" + newsFile + "'.");
                return;
            }
            
            List<string> lines = CP437Reader.ReadAllLines(newsFile);
            if (message == "")
            {
                foreach (string t in lines)
                {
                    Player.SendMessage(p, t);
                }
            }
            else
            {
                string[] split = message.Split(' ');
                if (split[0] == "all") { 
                	if ((int)p.group.Permission < CommandOtherPerms.GetPerm(this)) {
                		Player.SendMessage(p, "You must be at least " + Group.findPermInt(CommandOtherPerms.GetPerm(this)).name + " to send this to all players."); 
                		return; 
                	} 
                	for (int k = 0; k < lines.Count; k++) { 
                		Player.GlobalMessage(lines[k]); 
                	} 
                	return; 
                }
                
                Player player = PlayerInfo.Find(split[0]);
                if (player == null) { Player.SendMessage(p, "Could not find player \"" + split[0] + "\"!"); return; }
                foreach (string t in lines)
                {
                    Player.SendMessage(player, t);
                }
                Player.SendMessage(p, "The News were successfully sent to " + player.name + ".");
               
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/news - Shows server news.");
            Player.SendMessage(p, "/news <player> - Sends the News to <player>.");
            Player.SendMessage(p, "/news all - Sends the News to everyone.");
        }
    }
}
