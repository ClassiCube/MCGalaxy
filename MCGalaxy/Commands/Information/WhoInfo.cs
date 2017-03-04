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
    
    class WhoInfo {
        public string FullName, Name;
        public Group Group;
        public int Money, Deaths;
        public long TotalBlocks, LoginBlocks, TotalDrawn, TotalPlaced, TotalDeleted;
        public TimeSpan TimeSpent, TimeOnline;
        public DateTime First, Last;
        public int Logins, Kicks;
        public string IP;
        public int RoundsTotal, RoundsMax;
        public int InfectedTotal, InfectedMax;
        public TimeSpan IdleTime;
        public string AfkMessage;
        
        public static void Output(Player p, WhoInfo who, bool canSeeIP) {
            Player.Message(p, who.FullName + " %S(" + who.Name + ") has:");
            Player.Message(p, "  Rank of " + who.Group.ColoredName);
            
            if (Economy.Enabled)
                Player.Message(p, "  &a{0} &cdeaths%S, &a{2} %S{3}, {1} %Sawards", 
                               who.Deaths, Awards.AwardAmount(who.Name), who.Money, Server.moneys);
            else
                Player.Message(p, "  &a{0} &cdeaths%S, {1} %Sawards", 
                               who.Deaths, Awards.AwardAmount(who.Name));
            
            if (who.LoginBlocks >= 0)
                Player.Message(p, "  Modified &a{0} %Sblocks, &a{1} %Ssince login", who.TotalBlocks, who.LoginBlocks);
            else
                Player.Message(p, "  Modified &a{0} %Sblocks", who.TotalBlocks);
            Player.Message(p, "    &a{0} %Splaced, &a{1} %Sdeleted, &a{2} %Sdrawn", 
                           who.TotalPlaced, who.TotalDeleted, who.TotalDrawn);

            if (who.TimeOnline.Ticks > 0)
                Player.Message(p, "  Spent &a{0} %Son the server, &a{1} %Sthis session", 
                               who.TimeSpent.Shorten(), who.TimeOnline.Shorten());
            else
                Player.Message(p, "  Spent &a{0} %Son the server", who.TimeSpent.Shorten());
            
            if (who.Last.Ticks > 0)
                Player.Message(p, "  First login &a" + who.First.ToString("yyyy-MM-dd")
                                   + "%S, last login &a" + who.Last.ToString("yyyy-MM-dd"));
            else
                Player.Message(p, "  First login on &a" + who.First.ToString("yyyy-MM-dd")
                                   + "%S, and is currently &aonline");
            
            Player.Message(p, "  Logged in &a{0} %Stimes, &c{1} %Sof which ended in a kick", who.Logins, who.Kicks);        
            if (who.Group.Permission == LevelPermission.Banned) {
                string[] data = Ban.GetBanData(who.Name);
                if (data != null) {
                    Player.Message(p, "  Banned for {0} by {1}", 
                                   data[1], PlayerInfo.GetColoredName(p, data[0]));
                } else {
                    Player.Message(p, "  Is banned");
                }
            }                

            if (Server.Devs.CaselessContains(who.Name.TrimEnd('+')))
                Player.Message(p, "  Player is an &9{0} Developer", Server.SoftwareName);
            if (Server.Mods.CaselessContains(who.Name.TrimEnd('+')))
                Player.Message(p, "  Player is an &9{1} Moderator", Server.SoftwareName);

            if (canSeeIP) {
                string ipMsg = who.IP;
                if (Server.bannedIP.Contains(who.IP)) ipMsg = "&8" + who.IP + ", which is banned";
                Player.Message(p, "  The IP of " + ipMsg);
                if (Server.useWhitelist && Server.whiteList.Contains(who.Name))
                    Player.Message(p, "  Player is &fWhitelisted");
            }
            
            if (who.AfkMessage != null) {
                Player.Message(p, "  Idle for {0} (AFK {1}%S)", who.IdleTime.Shorten(), who.AfkMessage);
            } else if (who.IdleTime.TotalMinutes >= 1) {
                Player.Message(p, "  Idle for {0}", who.IdleTime.Shorten());
            }
            
            if (!Server.zombie.Running) return;
            Player.Message(p, "  Survived &a{0} %Srounds (max &e{1}%S)", 
                           who.RoundsTotal, who.RoundsMax);
            Player.Message(p, "  Infected &a{0} %Splayers (max &e{1}%S)",
                           who.InfectedTotal, who.InfectedMax);
        }
    }
}
