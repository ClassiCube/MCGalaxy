/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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

namespace MCGalaxy.Commands {
    
    public sealed class CmdWhois : Command {
        public override string name { get { return "whois"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.AdvBuilder, "The lowest rank which can see a player's ip and if they are whitelisted") }; }
        }

        public override void Use(Player p, string message) {
            Player who = message == "" ? p : PlayerInfo.Find(message);
            if (message == "") message = p.name;
            if (who == null || !Entities.CanSee(p, who)) {
                Player.SendMessage(p, "\"" + message + "\" is offline! Using /whowas instead.");
                Command.all.Find("whowas").Use(p, message); return;
            }
            
            WhoInfo info = new WhoInfo();
            string prefix = who.title == "" ? "" : "[" + who.titlecolor + who.title + who.color + "] ";            
            info.FullName = prefix + who.DisplayName; 
            info.Name = who.name;
            info.Group = who.group;
            info.Money = who.money; info.Deaths = who.overallDeath;
            info.TotalBlocks = who.overallBlocks; info.LoginBlocks = who.loginBlocks;
            info.TimeSpent = who.time; info.TimeOnline = DateTime.Now - who.timeLogged;
            info.First = who.firstLogin;
            info.Logins = who.totalLogins; info.Kicks = who.totalKicked;
            info.IP = who.ip;
            WhoInfo.Output(p, info, CheckAdditionalPerm(p));
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/whois [player] - Displays information about someone.");
        }
    }
}
