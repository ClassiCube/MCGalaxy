/*
    Copyright 2015 MCGalaxy
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using MCGalaxy.Commands;
using MCGalaxy.Eco;

namespace MCGalaxy.DB {

    public delegate void OnlineStatPrinter(Player p, Player who);
    
    /// <summary> Prints stats for an online player in /info. </summary>
    public static class OnlineStat {

        /// <summary> List of stats that can be output to /info. </summary>
        public static List<OnlineStatPrinter> Stats = new List<OnlineStatPrinter>() {
            CoreLine,
            (p, who) => MiscLine(p, who.name, who.TimesDied, who.money),
            BlocksModifiedLine,
            (p, who) => BlockStatsLine(p, who.TotalPlaced, who.TotalDeleted, who.TotalDrawn),
            TimeSpentLine,
            LoginLine,
            (p, who) => LoginsLine(p, who.TimesVisited, who.TimesBeenKicked),
            (p, who) => BanLine(p, who.name),
            (p, who) => SpecialGroupLine(p, who.name),
            (p, who) => IPLine(p, who.name, who.ip),
            IdleLine,
            EntityLine,
        };
        
        public static void CoreLine(Player p, Player who) {
            string prefix   = who.title.Length == 0 ? "" : who.MakeTitle(who.title, who.titlecolor);
            string fullName = prefix + who.ColoredName;
            CommonCoreLine(p, fullName, who.name, who.group, who.TotalMessagesSent);
        }
        
        internal static void CommonCoreLine(Player p, string fullName, string name, Group grp, int messages) {
            p.Message("{0} &S({1}) has:", fullName, name);
            p.Message("  Rank of {0}&S, wrote &a{1} &Smessages", grp.ColoredName, messages);
        }
        
        public static void MiscLine(Player p, string name, int deaths, int money) {
            if (Economy.Enabled) {
                p.Message("  &a{0} &cdeaths&S, &a{2} &S{3}, {1} &Sawards",
                               deaths, Awards.AwardAmount(name), money, Server.Config.Currency);
            } else {
                p.Message("  &a{0} &cdeaths&S, {1} &Sawards",
                               deaths, Awards.AwardAmount(name));
            }
        }
        
        public static void BlocksModifiedLine(Player p, Player who) {
            p.Message("  Modified &a{0} &Sblocks, &a{1} &Ssince login", who.TotalModified, who.SessionModified);
        }
        
        public static void BlockStatsLine(Player p, long placed, long deleted, long drawn) {
            p.Message("    &a{0} &Splaced, &a{1} &Sdeleted, &a{2} &Sdrawn",
                           placed, deleted, drawn);
        }
        
        public static void TimeSpentLine(Player p, Player who) {
            TimeSpan timeOnline = DateTime.UtcNow - who.SessionStartTime;
            p.Message("  Spent &a{0} &Son the server, &a{1} &Sthis session",
                           who.TotalTime.Shorten(), timeOnline.Shorten());
        }
        
        public static void LoginLine(Player p, Player who) {
            p.Message("  First login &a{0}&S, and is currently &aonline",
                           who.FirstLogin.ToString("yyyy-MM-dd"));
        }
        
        public static void LoginsLine(Player p, int logins, int kicks) {
            p.Message("  Logged in &a{0} &Stimes, &c{1} &Sof which ended in a kick", logins, kicks);
        }
        
        public static void BanLine(Player p, string name) {
            if (!Group.BannedRank.Players.Contains(name)) return;            
            string banner, reason, prevRank;
            DateTime time;
            Ban.GetBanData(name, out banner, out reason, out time, out prevRank);
            
            if (banner != null) {
                p.Message("  Banned for {0} by {1}", reason, p.FormatNick(banner));
            } else {
                p.Message("  Is banned");
            }
        }
        
        public static void SpecialGroupLine(Player p, string name) {
            if (Server.Devs.CaselessContains(name.RemoveLastPlus()))
                p.Message("  Player is an &9{0} Developer", Server.SoftwareName);
            if (Server.Config.OwnerName.CaselessEq(name))
                p.Message("  Player is the &cServer owner");
        }
        
        public static void IPLine(Player p, string name, string ip) {
            ItemPerms seeIpPerms = CommandExtraPerms.Find("WhoIs", 1);
            if (!seeIpPerms.UsableBy(p.Rank)) return;
            
            string ipMsg = ip;
            if (Server.bannedIP.Contains(ip)) ipMsg = "&8" + ip + ", which is banned";
            
            p.Message("  The IP of " + ipMsg);
            if (Server.Config.WhitelistedOnly && Server.whiteList.Contains(name))
                p.Message("  Player is &fWhitelisted");
        }
                
        public static void IdleLine(Player p, Player who) {
            TimeSpan idleTime = DateTime.UtcNow - who.LastAction;
            if (who.afkMessage != null) {
                p.Message("  Idle for {0} (AFK {1}&S)", idleTime.Shorten(), who.afkMessage);
            } else if (idleTime.TotalMinutes >= 1) {
                p.Message("  Idle for {0}", idleTime.Shorten());
            }
        }
        
        public static void EntityLine(Player p, Player who) {
            bool hasSkin = !who.SkinName.CaselessEq(who.truename);
            bool hasModel = !(who.Model.CaselessEq("humanoid") || who.Model.CaselessEq("human"));
            
            if (hasSkin && hasModel) {
                p.Message("  Skin: &f{0} &Smodel: &f{1}", who.SkinName, who.Model);
            } else if (hasSkin) {
                p.Message("  Skin: &f{0}", who.SkinName);
            } else if (hasModel) {
                p.Message("  Model: &f{0}", who.Model);
            }
        }
    }
}