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
using MCGalaxy.Games;

namespace MCGalaxy.Commands {
    
    public sealed class CmdWhois : Command {
        public override string name { get { return "whois"; } }
        public override string shortcut { get { return "whowas"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.AdvBuilder, "Lowest rank which can see IPs and if whitelisted") }; }
        }
        
        public override void Use(Player p, string message) {
            if (message == "") message = p.name;
            int matches;
            Player pl = PlayerInfo.FindOrShowMatches(p, message, out matches);
            if (matches > 1) return;
            
            WhoInfo info;
            if (matches == 1) {
                info = FromOnline(pl);
            } else {            
                if (!Player.ValidName(message)) { Player.SendMessage(p, "\"" + message + "\" is not a valid player name."); return; }
                
                Player.SendMessage(p, "Searching the database for the player..");
                OfflinePlayer target = PlayerInfo.FindOffline(message, true);
                if (target == null) {
                    Player.SendMessage(p, "\"" + message + "\" was not found in the database.");
                    Player.SendMessage(p, "Note you must use a player's full account name."); return;
                }
                info = FromOffline(target, message);
            }
            WhoInfo.Output(p, info, CheckAdditionalPerm(p));
        }
        
        WhoInfo FromOnline(Player who) {
            WhoInfo info = new WhoInfo();
            string prefix = who.title == "" ? "" : who.color + "[" + who.titlecolor + who.title + who.color + "] ";
            info.FullName = prefix + who.ColoredName;
            info.Name = who.name;
            info.Group = who.group;
            info.Money = who.money; info.Deaths = who.overallDeath;
            info.TotalBlocks = who.overallBlocks; info.LoginBlocks = who.loginBlocks;
            info.TimeSpent = who.time; info.TimeOnline = DateTime.Now - who.timeLogged;
            info.First = who.firstLogin;
            info.Logins = who.totalLogins; info.Kicks = who.totalKicked;
            info.IP = who.ip; info.AfkMessage = who.afkMessage;
            
            info.RoundsTotal = who.Game.TotalRoundsSurvived;
            info.RoundsMax = who.Game.MaxRoundsSurvived;
            info.InfectedTotal = who.Game.TotalInfected;
            info.InfectedMax = who.Game.MaxInfected;
            return info;
        }
        
        WhoInfo FromOffline(OfflinePlayer target, string message) {
            Group group = Group.Find(Group.findPlayer(message));
            string color = target.color == "" ? group.color : target.color;
            string prefix = target.title == "" ? "" : color + "[" + target.titleColor + target.title + color + "] ";
            
            WhoInfo info = new WhoInfo();
            info.FullName = prefix + color + target.name.TrimEnd('+');
            info.Name = target.name;
            info.Group = group;
            info.Money = int.Parse(target.money); info.Deaths = int.Parse(target.deaths);
            info.TotalBlocks = long.Parse(target.blocks); info.LoginBlocks = -1;
            info.TimeSpent = target.totalTime.ParseDBTime();
            info.First = DateTime.Parse(target.firstLogin);
            info.Last = DateTime.Parse(target.lastLogin);
            info.Logins = int.Parse(target.logins); info.Kicks = int.Parse(target.kicks);
            info.IP = target.ip;
            
            if (Server.zombie.Running) {
                ZombieStats stats = Server.zombie.LoadZombieStats(target.name);
                info.RoundsTotal = stats.TotalRounds; info.InfectedTotal = stats.TotalInfected;
                info.RoundsMax = stats.MaxRounds; info.InfectedMax = stats.MaxInfected;
            }
            return info;
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "%T/whois [name] %H- Displays information about that player.");
            Player.SendMessage(p, "%HNote this works for both online and offline players.");
        }
    }
}
