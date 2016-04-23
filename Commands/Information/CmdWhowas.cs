/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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

namespace MCGalaxy.Commands {
    
    public sealed class CmdWhowas : Command {        
        public override string name { get { return "whowas"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.AdvBuilder, "The lowest rank which can see a player's ip and if they are whitelisted") }; }
        }
        
        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            int matches;
            Player pl = PlayerInfo.FindOrShowMatches(p, message, out matches);
            if (matches > 1) return;
            if (matches == 1) {
                Player.SendMessage(p, pl.color + pl.name + " %Sis online, using /whois instead.");
                Command.all.Find("whois").Use(p, message); return;
            }

            if (!Player.ValidName(message)) { Player.SendMessage(p, "\"" + message + "\" is not a valid player name."); return; }
            OfflinePlayer target = PlayerInfo.FindOffline(message, true);
            if (target == null) { 
                Player.SendMessage(p, "\"" + message + "\" was not found in the database."); 
                Player.SendMessage(p, "Note you must use a player's full account name."); return; 
            }
            
            Group group = Group.Find(Group.findPlayer(message));
            string color = target.color == "" ? group.color : target.color;
            string prefix = target.title == "" ? "" : "[" + target.titleColor + target.title + color + "] ";
            
            WhoInfo info = new WhoInfo();
            info.FullName = prefix + target.name.TrimEnd('+'); 
            info.Name = target.name;
            info.Group = group;
            info.Money = int.Parse(target.money); info.Deaths = int.Parse(target.deaths);
            info.TotalBlocks = long.Parse(target.blocks); info.LoginBlocks = -1;
            info.TimeSpent = target.totalTime.ParseDBTime();
            info.First = DateTime.Parse(target.firstLogin); 
            info.Last = DateTime.Parse(target.lastLogin);
            info.Logins = int.Parse(target.logins); info.Kicks = int.Parse(target.kicks);
            info.IP = target.ip;
            WhoInfo.Output(p, info, CheckAdditionalPerm(p));
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "/whowas <name> - Displays information about an offline player.");
        }
    }
}
