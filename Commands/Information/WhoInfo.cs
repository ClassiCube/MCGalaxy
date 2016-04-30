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
        
        public static void Output(Player p, WhoInfo who, bool canSeeIP) {
            Player.SendMessage(p, who.FullName + " %S(" + who.Name + ") has:");
            Player.SendMessage(p, ">> Rank of " + who.Group.ColoredName);
            
            if (Economy.Enabled)
                Player.SendMessage(p, ">> &a" + who.Deaths + " &cdeaths%S, &a" + who.Money +
                                   " %S" + Server.moneys +", " + Awards.AwardAmount(who.Name) + " awards");
            else
                Player.SendMessage(p, ">> &a" + who.Deaths + " &cdeaths%s, " + Awards.AwardAmount(who.Name) + " awards");
            
            if (who.LoginBlocks >= 0)
                Player.SendMessage(p, ">> &bModified &a" + who.TotalBlocks + " &eblocks, &a" + who.LoginBlocks + " &esince login");
            else
                Player.SendMessage(p, ">> &bModified &a" + who.TotalBlocks + " &eblocks");

            if (who.TimeOnline.Ticks > 0)
                Player.SendMessage(p, ">> Spent " + Shorten(who.TimeSpent) + " on the server, " + Shorten(who.TimeOnline) + " this session");
            else
                Player.SendMessage(p, ">> Spent " + Shorten(who.TimeSpent) + " on the server");
            
            if (who.Last.Ticks > 0)
                Player.SendMessage(p, ">> First login &a" + who.First.ToString("yyyy-MM-dd")
                                   + "%S, last login &a" + who.Last.ToString("yyyy-MM-dd"));
            else
                Player.SendMessage(p, ">> First login on &a" + who.First.ToString("yyyy-MM-dd")
                                   + "%S, and is currently &aonline");
            
            Player.SendMessage(p, ">> Logged in &a" + who.Logins + " %Stimes, &c" + who.Kicks + " %Sof which ended in a kick");        
            if (who.Group.Permission == LevelPermission.Banned) {
                string[] data = Ban.GetBanData(who.Name);
                if (data != null)
                    Player.SendMessage(p, ">> is banned for " + data[1] + " by " + data[0]);
                else
                    Player.SendMessage(p, ">> is banned");
            }                

            if (Server.Devs.CaselessContains(who.Name))
                Player.SendMessage(p, ">> Player is a &9Developer");
            if (Server.Mods.CaselessContains(who.Name))
                Player.SendMessage(p, ">> Player is a &9MCGalaxy Moderator");

            if (canSeeIP) {
                string ipMsg = who.IP;
                if (Server.bannedIP.Contains(who.IP)) ipMsg = "&8" + who.IP + ", which is banned";
                Player.SendMessage(p, ">> The IP of " + ipMsg);
                if (Server.useWhitelist&& Server.whiteList.Contains(who.Name))
                    Player.SendMessage(p, ">> Player is &fWhitelisted");
            }
            
            if (!Server.zombie.Running) return;
            Player.SendMessage(p, ">> Survived &a" + who.RoundsTotal +
                               " %Srounds total, most in a row was &e" + who.RoundsMax);
            Player.SendMessage(p, ">> Infected &a" + who.InfectedTotal +
                               " %Splayers total, most in a round was &e" + who.InfectedMax);
        }
        
        public static string Shorten(TimeSpan value, bool seconds = false) {
            string time = "";
            if (value.Days >= 1) time = value.Days + "d " + value.Hours + "h " + value.Minutes + "m";
            if (value.Hours >= 1) time = value.Hours + "h " + value.Minutes + "m";
            else time = value.Minutes + "m";
            if (seconds) time += " " + value.Seconds + "s";
            return time;
        }
    }
}
