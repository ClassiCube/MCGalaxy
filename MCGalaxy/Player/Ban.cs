/*
 Copyright 2011 MCForge
        
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
using System.IO;
using System.Text;

namespace MCGalaxy {
    
    /// <summary> Can check the info about someone's ban, find out if there's info about someone,
    /// and add / remove someone to the baninfo (NOT THE BANNED.TXT !) </summary>
    public static class Ban {
        
        static PlayerMetaList bans = new PlayerMetaList("text/bans.txt");
        static PlayerMetaList unbans = new PlayerMetaList("text/unbans.txt");
        
        public static void EnsureExists() {
            bans.EnsureExists();
            unbans.EnsureExists();
        }
        
        public static string FormatBan(string banner, string reason) {
            return "Banned by " + banner + ": " + reason;
        }        
        
        public static string PackTempBanData(string reason, string banner, DateTime expiry) {
            if (reason == null) reason = "-";
            return banner + " " + expiry.Ticks + " " + reason;
        }
        
        public static void UnpackTempBanData(string line, out string reason, out string banner, out DateTime expiry) {
            string[] parts = line.SplitSpaces(3);
            banner = parts[0];
            expiry = new DateTime(long.Parse(parts[1]), DateTimeKind.Utc);
            reason = parts.Length > 2 ? parts[2] : "";
        }
        
        
        /// <summary> Adds a ban entry for the given user, and who banned them and why they were banned. </summary>
        public static void BanPlayer(Player banner, string target, string reason, bool stealth, string oldrank) {
            if (reason == "") reason = Server.defaultBanMessage;
            reason = reason.Replace(" ", "%20");
            
            string player = banner == null ? "(console)" : banner.truename;
            AddBanEntry(player, target.ToLower(), reason, stealth.ToString(), FormatDate(), oldrank);
        }
        
        /// <summary> Adds a ban entry for the given user, and who banned them and why they were banned. </summary>
        public static void UnbanPlayer(Player unbanner, string target, string reason) {
            if (reason == "") reason = "(none given)";
            reason = reason.Replace(" ", "%20");
            
            string player = unbanner == null ? "(console)" : unbanner.truename;
            AddUnbanEntry(player, target.ToLower(), reason, FormatDate());
        }
        
        static string FormatDate() {
            DateTime now = DateTime.Now;
            return now.DayOfWeek + "%20" + now.Day + "%20" + now.Month + 
                "%20" + now.Year + ",%20at%20" + now.Hour + ":" + now.Minute;
        }
        
        static void AddBanEntry(string pl, string who, string reason, 
                                string stealth, string datetime, string oldrank) {
            string data = pl + " " + who + " " + reason + " " + stealth + " " + datetime + " " + oldrank;
            bans.Append(data);
        }
        
        static void AddUnbanEntry(string pl, string who, string reason, string datetime) {
            string data = pl + " " + who + " " + reason + " " + datetime;
            unbans.Append(data);
        }
        
        
        /// <summary> Returns info about the current or last ban of a user, as a string array of
        /// {banned by, ban reason, date and time, previous rank, stealth},
        /// or null if no ban data was found. </summary>
        public static string[] GetBanData(string who) {
            who = who.ToLower();
            foreach (string line in File.ReadAllLines(bans.file)) {
                string[] parts = line.SplitSpaces();
                if (parts.Length <= 5 || parts[1] != who) continue;
                
                parts[2] = parts[2].Replace("%20", " ");
                parts[4] = parts[4].Replace("%20", " ");
                return new string[] { parts[0], parts[2], parts[4], parts[5], parts[3] };
            }
            return null;
        }
        
        /// <summary> Returns info about the last unban of a user, as a string array of
        /// {banned by, ban reason, date and time}, or null if no unban data was found. </summary>
        public static string[] GetUnbanData(string who) {
            who = who.ToLower();
            foreach (string line in File.ReadAllLines(unbans.file)) {
                string[] parts = line.SplitSpaces();
                if (parts.Length <= 3 || parts[1] != who) continue;
                
                parts[2] = parts[2].Replace("%20", " ");
                parts[3] = parts[3].Replace("%20", " ");
                return new string[] { parts[0], parts[2], parts[3] };
            }
            return null;
        }
        
        
        /// <summary> Deletes the ban information about the user. </summary>
        public static bool DeleteBan(string name) { return DeleteInfo(name, bans); }
        
        /// <summary> Deletes the unban information about the user. </summary>
        public static bool DeleteUnban(string name) { return DeleteInfo(name, unbans); }
        
        static bool DeleteInfo(string name, PlayerMetaList list) {
            name = name.ToLower();
            bool success = false;
            StringBuilder sb = new StringBuilder();
            
            foreach (string line in File.ReadAllLines(list.file)) {
                string[] parts = line.SplitSpaces();
                if (parts.Length <= 1 || parts[1] != name)
                    sb.AppendLine(line);
                else
                    success = true;
            }
            File.WriteAllText(list.file, sb.ToString());
            return success;
        }
        
        
        /// <summary> Change the ban reason for the given user. </summary>
        public static bool ChangeBanReason(string who, string reason) {
            return ChangeReason(who, reason, bans);
        }
        
        /// <summary> Change the unban reason for the given user. </summary>
        public static bool ChangeUnbanReason(string who, string reason) {
            return ChangeReason(who, reason, unbans);
        }
        
        static bool ChangeReason(string who, string reason, PlayerMetaList list) {
            who = who.ToLower();
            reason = reason.Replace(" ", "%20");
            bool success = false;
            StringBuilder sb = new StringBuilder();
            
            foreach (string line in File.ReadAllLines(list.file)) {
                string[] parts = line.SplitSpaces();
                if (parts.Length > 2 && parts[1] == who) {
                    success = true;
                    sb.AppendLine(String.Join(" ", parts));
                } else {
                    sb.AppendLine(line);
                }
            }
            
            if (success)
                File.WriteAllText(list.file, sb.ToString());
            return success;
        }
    }
}