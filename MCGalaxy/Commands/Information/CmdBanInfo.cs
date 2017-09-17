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

namespace MCGalaxy.Commands.Info {
    public sealed class CmdBanInfo : Command {
        public override string name { get { return "BanInfo"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }

        public override void Use(Player p, string message) {
            if (CheckSuper(p, message, "player name")) return;
            if (message.Length == 0) message = p.name;
            
            string plName = PlayerInfo.FindMatchesPreferOnline(p, message);
            if (plName == null) return;
            string colName = PlayerInfo.GetColoredName(p, plName);
            
            bool permaBanned = Group.BannedRank.Players.Contains(plName);
            bool banned = permaBanned || Server.tempBans.Contains(plName);
            string msg = colName;
            string ip = PlayerInfo.FindIP(plName);
            bool ipBanned = ip != null && Server.bannedIP.Contains(ip);
            
            if (!ipBanned && banned) msg += " %Sis &CBANNED";
            else if (!ipBanned && !banned) msg += " %Sis not banned";
            else if (ipBanned && banned) msg += " %Sand their IP are &CBANNED";
            else msg += " %Sis not banned, but their IP is &CBANNED";          
            
            string banner, reason, prevRank;
            DateTime time;
            Ban.GetBanData(plName, out banner, out reason, out time, out prevRank);
            if (banner != null && permaBanned) {
                string grpName = Group.GetColoredName(prevRank);
                msg += " %S(Former rank: " + grpName + "%S)";
            }
            Player.Message(p, msg);
            DisplayTempbanDetails(p, plName);
            
            if (banner != null) {
                DisplayDetails(p, banner, reason, time, permaBanned ? "Banned" : "Last banned");
            } else {
                Player.Message(p, "No previous bans recorded for {0}%S.", colName);
            }
            
            Ban.GetUnbanData(plName, out banner, out reason, out time);
            DisplayDetails(p, banner, reason, time, permaBanned ? "Last unbanned" : "Unbanned");
        }
        
        static void DisplayTempbanDetails(Player p, string target) {
            string data = Server.tempBans.FindData(target);
            if (data == null) return;
            
            string banner, reason;
            DateTime expiry;
            Ban.UnpackTempBanData(data, out reason, out banner, out expiry);        
            if (expiry < DateTime.UtcNow) return;
            
            TimeSpan delta = expiry - DateTime.UtcNow;        
            Player.Message(p, "Temp-banned %S by {1} %Sfor another {0}", 
                           delta.Shorten(), GetName(p, banner));
            if (reason != "") Player.Message(p, "Reason: {0}", reason);
        }
        
        static void DisplayDetails(Player p, string banner, string reason, DateTime time, string type) {
            if (banner == null) return;
            
            TimeSpan delta = DateTime.UtcNow - time;
            Player.Message(p, "{0} {1} ago by {2}", 
                           type, delta.Shorten(), GetName(p, banner));
            Player.Message(p, "Reason: {0}", reason);
        }
        
        static string GetName(Player p, string user) {
            // ban/unban uses truename
            if (ServerConfig.ClassicubeAccountPlus && !user.EndsWith("+")) user += "+";
            return PlayerInfo.GetColoredName(p, user);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/BanInfo [player]");
            Player.Message(p, "%HOutputs information about current and/or previous ban/unban for that player.");
        }
    }
}
