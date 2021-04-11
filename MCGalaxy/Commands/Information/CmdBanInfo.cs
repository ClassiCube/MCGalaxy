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
using MCGalaxy.DB;

namespace MCGalaxy.Commands.Info {
    public sealed class CmdBanInfo : Command2 {
        public override string name { get { return "BanInfo"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool UseableWhenFrozen { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            if (CheckSuper(p, message, "player name")) return;
            if (message.Length == 0) message = p.name;
            
            string plName = PlayerInfo.FindMatchesPreferOnline(p, message);
            if (plName == null) return;
            string nick = p.FormatNick(plName);
            
            string tempData = Server.tempBans.FindData(plName);
            string tempBanner = null, tempReason = null;
            DateTime tempExpiry = DateTime.MinValue;
            if (tempData != null) {
                Ban.UnpackTempBanData(tempData, out tempReason, out tempBanner, out tempExpiry);
            }
            
            bool permaBanned = Group.BannedRank.Players.Contains(plName);
            bool isBanned = permaBanned || tempExpiry >= DateTime.UtcNow;
            string msg = nick;
            string ip  = PlayerDB.FindIP(plName);
            bool ipBanned = ip != null && Server.bannedIP.Contains(ip);
            
            if (!ipBanned && isBanned) msg += " &Sis &cBANNED";
            else if (!ipBanned && !isBanned) msg += " &Sis not banned";
            else if (ipBanned && isBanned) msg += " &Sand their IP are &cBANNED";
            else msg += " &Sis not banned, but their IP is &cBANNED";
            
            string banner, reason, prevRank;
            DateTime time;
            Ban.GetBanData(plName, out banner, out reason, out time, out prevRank);
            if (banner != null && permaBanned) {
                string grpName = Group.GetColoredName(prevRank);
                msg += " &S(Former rank: " + grpName + "&S)";
            }
            p.Message(msg);
            
            if (tempExpiry >= DateTime.UtcNow) {
                TimeSpan delta = tempExpiry - DateTime.UtcNow;
                p.Message("Temp-banned &S by {1} &Sfor another {0}",
                               delta.Shorten(), GetName(p, tempBanner));
                if (tempReason.Length > 0) {
                    p.Message("Reason: {0}",tempReason);
                }
            }
            
            if (banner != null) {
                DisplayDetails(p, banner, reason, time, permaBanned ? "Banned" : "Last banned");
            } else {
                p.Message("No previous bans recorded for {0}&S.", nick);
            }            
            Ban.GetUnbanData(plName, out banner, out reason, out time);
            DisplayDetails(p, banner, reason, time, permaBanned ? "Last unbanned" : "Unbanned");
        }
        
        static void DisplayDetails(Player p, string banner, string reason, DateTime time, string type) {
            if (banner == null) return;
            
            TimeSpan delta = DateTime.UtcNow - time;
            p.Message("{0} {1} ago by {2}",
                           type, delta.Shorten(), GetName(p, banner));
            p.Message("Reason: {0}", reason);
        }
        
        static string GetName(Player p, string user) {
            // ban/unban uses truename
            if (Server.Config.ClassicubeAccountPlus && !user.EndsWith("+")) user += "+";
            return p.FormatNick(user);
        }
        
        public override void Help(Player p) {
            p.Message("&T/BanInfo [player]");
            p.Message("&HOutputs information about current and/or previous ban/unban for that player.");
        }
    }
}
