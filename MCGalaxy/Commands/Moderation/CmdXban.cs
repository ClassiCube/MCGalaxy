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
    public sealed class CmdXban : Command2 {       
        public override string name { get { return "XBan"; } }
        public override string shortcut { get { return "BanX"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandAlias[] Aliases {
            get { return new [] { new CommandAlias("UBan", "-noip") }; }
        }
        
        public override void Use(Player p, string message, CommandData data) {
            bool banIP = true;
            if (message.CaselessStarts("-noip ")) {
                message = message.Substring("-noip ".Length);
                banIP = false;
            }            
            if (message.Length == 0) { Help(p); return; }

            string name = message.SplitSpaces()[0];
            Command.Find("UndoPlayer").Use(p, name + " all", data);
            if (banIP) Command.Find("BanIP").Use(p, "@" + name, data);
            Command.Find("Ban").Use(p, message, data);    
        }

        public override void Help(Player p) {
            p.Message("&T/XBan [player] <reason>");
            p.Message("&HBans, IP bans, undoes, and kicks the given player.");
            p.Message("&T/UBan [player] <reason>");
            p.Message("&HSame as &T/XBan&H, but does not ban the IP.");
        }
    }
}
