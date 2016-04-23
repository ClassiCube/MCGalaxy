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
        public DateTime FirstLogin, LastLogin;
        public int Logins, Kicks;
        public string IP;
        
        public static void Output(Player p, WhoInfo who, bool canSeeIP) {
            Player.SendMessage(p, who.FullName + " %S(" + who.Name + ") has:");
            Player.SendMessage(p, ">> Rank of " + who.Group.ColoredName);
            
            if (Economy.Enabled)
                Player.SendMessage(p, ">> &a" + who.Deaths + " &cdeaths%S, &a" + who.Money + 
                                   " %S" + Server.moneys +", " + Awards.AwardAmount(who.Name) + " awards");
            else
                Player.SendMessage(p, ">> &a" + who.Deaths + " &cdeaths%s, " + Awards.AwardAmount(who.Name) + " awards");
            
            if (who.LoginBlocks >= 0)
                Player.SendMessage(p, ">> &bModified &a" + who.TotalBlocks + " &eblocks, &a" + who.LoginBlocks + " &echanged since login.");
            else
                Player.SendMessage(p, ">> &bModified &a" + who.TotalBlocks + " &eblocks");
            
            //string storedTime = Convert.ToDateTime(DateTime.Now.Subtract(who.timeLogged).ToString()).ToString("HH:mm:ss");
           // TimeSpan time = who.time;
           // Player.SendMessage(p, "> time spent on server: " + time.Days + " Days, " + time.Hours + " Hours, " + time.Minutes + " Minutes, " + time.Seconds + " Seconds.");
           // Player.SendMessage(p, "> been logged in for &a" + storedTime);
           // Player.SendMessage(p, "> first logged into the server on &a" + who.firstLogin.ToString("yyyy-MM-dd") + " at " + who.firstLogin.ToString("HH:mm:ss"));
            Player.SendMessage(p, ">> Logged in &a" + who.Logins + " %Stimes, &c" + who.Kicks + " %Sof which ended in a kick.");
            
            string[] data = Ban.GetBanData(who.Name);
            if (data != null)               
                Player.SendMessage(p, ">> is banned for " + data[1] + " by " + data[0]);

            if (Server.Devs.CaselessContains(who.Name))
                Player.SendMessage(p, ">> Player is a &9Developer");
            if (Server.Mods.CaselessContains(who.Name))
                Player.SendMessage(p, ">> Player is a &9MCGalaxy Moderator");

            if (!canSeeIP) return;
            string ipMsg = who.IP;
            if (Server.bannedIP.Contains(who.IP)) ipMsg = "&8" + who.IP + ", which is banned";
            Player.SendMessage(p, ">> The IP of " + ipMsg);
            if (Server.useWhitelist&& Server.whiteList.Contains(who.Name))
                Player.SendMessage(p, ">> Player is &fWhitelisted");
        }
    }
}
