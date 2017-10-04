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
    
    /// <summary> Prints stats for an online player in /whois. </summary>
    public static class OnlineStat {

        /// <summary> List of stats that can be output to /whois. </summary>
        public static List<OnlineStatPrinter> Stats = new List<OnlineStatPrinter>() {
            OnlineCoreLine,
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
        
        static void OnlineCoreLine(Player p, Player who) {
            string prefix = who.title.Length == 0 ? "" : who.color + "[" + who.titlecolor + who.title + who.color + "] ";
            string fullName = prefix + who.ColoredName;
            CoreLine(p, fullName, who.name, who.group);
        }
        
        internal static void CoreLine(Player p, string fullName, string name, Group grp) {
            Player.Message(p, fullName + " %S(" + name + ") has:");
            Player.Message(p, "  Rank of " + grp.ColoredName);
        }
        
        internal static void MiscLine(Player p, string name, int deaths, int money) {
            if (Economy.Enabled) {
                Player.Message(p, "  &a{0} &cdeaths%S, &a{2} %S{3}, {1} %Sawards",
                               deaths, Awards.AwardAmount(name), money, ServerConfig.Currency);
            } else {
                Player.Message(p, "  &a{0} &cdeaths%S, {1} %Sawards",
                               deaths, Awards.AwardAmount(name));
            }
        }
        
        static void BlocksModifiedLine(Player p, Player who) {
            Player.Message(p, "  Modified &a{0} %Sblocks, &a{1} %Ssince login", who.TotalModified, who.SessionModified);
        }
        
        internal static void BlockStatsLine(Player p, long placed, long deleted, long drawn) {
            Player.Message(p, "    &a{0} %Splaced, &a{1} %Sdeleted, &a{2} %Sdrawn",
                           placed, deleted, drawn);
        }
        
        static void TimeSpentLine(Player p, Player who) {
            TimeSpan timeOnline = DateTime.UtcNow - who.SessionStartTime;
            Player.Message(p, "  Spent &a{0} %Son the server, &a{1} %Sthis session",
                           who.TotalTime.Shorten(), timeOnline.Shorten());
        }
        
        static void LoginLine(Player p, Player who) {
            Player.Message(p, "  First login &a{0}%S, and is currently &aonline",
                           who.FirstLogin.ToString("yyyy-MM-dd"));
        }
        
        internal static void LoginsLine(Player p, int logins, int kicks) {
            Player.Message(p, "  Logged in &a{0} %Stimes, &c{1} %Sof which ended in a kick", logins, kicks);
        }
        
        internal static void BanLine(Player p, string name) {
            if (!Group.BannedRank.Players.Contains(name)) return;            
            string banner, reason, prevRank;
            DateTime time;
            Ban.GetBanData(name, out banner, out reason, out time, out prevRank);
            
            if (banner != null) {
                Player.Message(p, "  Banned for {0} by {1}",
                               reason, PlayerInfo.GetColoredName(p, banner));
            } else {
                Player.Message(p, "  Is banned");
            }
        }
        
        internal static void SpecialGroupLine(Player p, string name) {
            if (Server.Devs.CaselessContains(name.RemoveLastPlus()))
                Player.Message(p, "  Player is an &9{0} Developer", Server.SoftwareName);
            if (Server.Mods.CaselessContains(name.RemoveLastPlus()))
                Player.Message(p, "  Player is an &9{0} Moderator", Server.SoftwareName);
            if (ServerConfig.OwnerName.CaselessEq(name))
                Player.Message(p, "  Player is the &cServer owner");
        }
        
        internal static void IPLine(Player p, string name, string ip) {
            LevelPermission seeIPPerm = CommandExtraPerms.MinPerm("whois");
            if (p != null && p.Rank < seeIPPerm) return;
            
            string ipMsg = ip;
            if (Server.bannedIP.Contains(ip)) ipMsg = "&8" + ip + ", which is banned";
            
            Player.Message(p, "  The IP of " + ipMsg);
            if (ServerConfig.WhitelistedOnly && Server.whiteList.Contains(name))
                Player.Message(p, "  Player is &fWhitelisted");
        }
                
        static void IdleLine(Player p, Player who) {
            TimeSpan idleTime = DateTime.UtcNow - who.LastAction;
            if (who.afkMessage != null) {
                Player.Message(p, "  Idle for {0} (AFK {1}%S)", idleTime.Shorten(), who.afkMessage);
            } else if (idleTime.TotalMinutes >= 1) {
                Player.Message(p, "  Idle for {0}", idleTime.Shorten());
            }
        }
        
        static void EntityLine(Player p, Player who) {
            bool hasSkin = !who.SkinName.CaselessEq(who.truename);
            bool hasModel = !(who.Model.CaselessEq("humanoid") || who.Model.CaselessEq("human"));
            
            if (hasSkin && hasModel) {
                Player.Message(p, "  Skin: &f{0}, %Smodel: &f{1}", who.SkinName, who.Model);
            } else if (hasSkin) {
                Player.Message(p, "  Skin: &f{0}", who.SkinName);
            } else if (hasModel) {
                Player.Message(p, "  Model: &f{0}", who.Model);
            }
        }
    }
}