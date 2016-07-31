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
using System.Collections.Generic;
using MCGalaxy.Games;

namespace MCGalaxy.Commands {
    
    public sealed class CmdWhois : Command {
        public override string name { get { return "whois"; } }
        public override string shortcut { get { return "whowas"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.AdvBuilder, "+ can see IPs and if a player is whitelisted") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("info"), new CommandAlias("i") }; }
        }
        
        public override void Use(Player p, string message) {
            if (message == "") message = p.name;
            int matches;
            Player pl = PlayerInfo.FindMatches(p, message, out matches);
            if (matches > 1) return;
            
            WhoInfo info;
            if (matches == 1) {
                info = FromOnline(pl);
            } else {
                if (!ValidName(p, message, "player")) return;
                Player.Message(p, "Searching database for the player..");           
                PlayerData target = PlayerInfo.FindOfflineMatches(p, message);
                if (target == null) return;
                info = FromOffline(target, message);
            }
            WhoInfo.Output(p, info, CheckExtraPerm(p));
        }
        
        WhoInfo FromOnline(Player who) {
            WhoInfo info = new WhoInfo();
            string prefix = who.title == "" ? "" : who.color + "[" + who.titlecolor + who.title + who.color + "] ";
            info.FullName = prefix + who.ColoredName;
            info.Name = who.name;
            info.Group = who.group;
            info.Money = who.money; info.Deaths = who.overallDeath;
            info.TotalBlocks = who.overallBlocks; info.TotalDrawn = who.TotalBlocksDrawn;            
            info.LoginBlocks = who.loginBlocks;
            
            info.TimeSpent = who.time; info.TimeOnline = DateTime.Now - who.timeLogged;
            info.First = who.firstLogin;
            info.Logins = who.totalLogins; info.Kicks = who.totalKicked;
            info.IP = who.ip; info.AfkMessage = who.afkMessage;
            info.IdleTime = DateTime.UtcNow - who.LastAction;
            
            info.RoundsTotal = who.Game.TotalRoundsSurvived;
            info.RoundsMax = who.Game.MaxRoundsSurvived;
            info.InfectedTotal = who.Game.TotalInfected;
            info.InfectedMax = who.Game.MaxInfected;
            return info;
        }
        
        WhoInfo FromOffline(PlayerData target, string message) {
            Group group = Group.findPlayerGroup(target.Name);
            string color = target.Color == "" ? group.color : target.Color;
            string prefix = target.Title == "" ? "" : color + "[" + target.TitleColor + target.Title + color + "] ";
            
            WhoInfo info = new WhoInfo();
            info.FullName = prefix + color + target.Name.TrimEnd('+');
            info.Name = target.Name;
            info.Group = group;
            info.Money = int.Parse(target.Money); info.Deaths = int.Parse(target.Deaths);
            info.TotalBlocks = long.Parse(target.Blocks); info.LoginBlocks = -1;
            info.TotalDrawn = long.Parse(target.Cuboided);
            
            info.TimeSpent = target.TotalTime.ParseDBTime();
            info.First = DateTime.Parse(target.FirstLogin);
            info.Last = DateTime.Parse(target.LastLogin);
            info.Logins = int.Parse(target.Logins); info.Kicks = int.Parse(target.Kicks);
            info.IP = target.IP;
            
            if (Server.zombie.Running) {
                ZombieStats stats = Server.zombie.LoadZombieStats(target.Name);
                info.RoundsTotal = stats.TotalRounds; info.InfectedTotal = stats.TotalInfected;
                info.RoundsMax = stats.MaxRounds; info.InfectedMax = stats.MaxInfected;
            }
            return info;
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/whois [name]");
            Player.Message(p, "%HDisplays information about that player.");
            Player.Message(p, "%HNote: Works for both online and offline players.");
        }
    }
}
