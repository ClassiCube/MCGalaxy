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
namespace MCGalaxy.Commands.Moderation {   
    public sealed class CmdXban : Command {       
        public override string name { get { return "xban"; } }
        public override string shortcut { get { return "banx"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandAlias[] Aliases {
            get { return new [] { new CommandAlias("uban", "-noip") }; }
        }
        
        public override void Use(Player p, string message) {
            bool banIP = true;
            if (message.CaselessStarts("-noip ")) {
                message = message.Substring("-noip ".Length);
                banIP = false;
            }            
            if (message == "") { Help(p); return; }

            string name = message.SplitSpaces()[0];
            Command.all.Find("undoplayer").Use(p, name + " all");
            if (banIP) Command.all.Find("banip").Use(p, "@" + name);
            Command.all.Find("kickban").Use(p, message);    
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/xban [name] [message]");
            Player.Message(p, "%HBans, IP bans, undoes, and kicks [name] with [message], if specified.");
            Player.Message(p, "%T/uban [name] [message]");
            Player.Message(p, "%HSame as /xban, but does not ban the IP.");
        }
    }
}
