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
        public long TotalBlocks, LoginBlocks;
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
                Player.Message(p, "  &a" + who.Deaths + " &cdeaths%S, &a" + who.Money +
                                   " %S" + Server.moneys +", " + Awards.AwardAmount(who.Name) + " awards");
            else
                Player.Message(p, "  &a" + who.Deaths + " &cdeaths%S, " + Awards.AwardAmount(who.Name) + " awards");
            
            if (who.LoginBlocks >= 0)
                Player.Message(p, "  &bModified &a{0} &eblocks, &a{1} &esince login", who.TotalBlocks, who.LoginBlocks);
            else
                Player.Message(p, "  &bModified &a{0} &eblocks", who.TotalBlocks);

            if (who.TimeOnline.Ticks > 0)
            	Player.Message(p, "  Spent {0} on the server, {1} this session", who.TimeSpent.Shorten(), who.TimeOnline.Shorten());
            else
            	Player.Message(p, "  Spent {0} on the server", who.TimeSpent.Shorten());
            
            if (who.Last.Ticks > 0)
                Player.Message(p, "  First login &a" + who.First.ToString("yyyy-MM-dd")
                                   + "%S, last login &a" + who.Last.ToString("yyyy-MM-dd"));
            else
                Player.Message(p, "  First login on &a" + who.First.ToString("yyyy-MM-dd")
                                   + "%S, and is currently &aonline");
            
            Player.Message(p, "  Logged in &a{0} %Stimes, &c{1} %Sof which ended in a kick", who.Logins, who.Kicks);        
            if (who.Group.Permission == LevelPermission.Banned) {
                string[] data = Ban.GetBanData(who.Name);
                if (data != null)
                    Player.Message(p, "  is banned for " + data[1] + " by " + data[0]);
                else
                    Player.Message(p, "  is banned");
            }                

            if (Server.Devs.CaselessContains(who.Name.TrimEnd('+')))
                Player.Message(p, "  Player is an &9MCGalaxy Developer");
            if (Server.Mods.CaselessContains(who.Name.TrimEnd('+')))
                Player.Message(p, "  Player is an &9MCGalaxy Moderator");

            if (canSeeIP) {
                string ipMsg = who.IP;
                if (Server.bannedIP.Contains(who.IP)) ipMsg = "&8" + who.IP + ", which is banned";
                Player.Message(p, "  The IP of " + ipMsg);
                if (Server.useWhitelist&& Server.whiteList.Contains(who.Name))
                    Player.Message(p, "  Player is &fWhitelisted");
            }
            if (who.AfkMessage != null)
                Player.Message(p, "  Idle for {0} (AFK {1}%S)", who.IdleTime.Shorten(), who.AfkMessage);
            else if (who.IdleTime.TotalMinutes >= 1)
            	Player.Message(p, "  Idle for {0}", who.IdleTime.Shorten());
            
            if (!Server.zombie.Running) return;
            Player.Message(p, "  Survived &a" + who.RoundsTotal +
                               " %Srounds total, most in a row was &e" + who.RoundsMax);
            Player.Message(p, "  Infected &a" + who.InfectedTotal +
                               " %Splayers total, most in a round was &e" + who.InfectedMax);
        }
    }
}
