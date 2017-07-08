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
        public override string name { get { return "baninfo"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }

        public override void Use(Player p, string message) {
            if (CheckSuper(p, message, "player name")) return;
            if (message == "") message = p.name;
            
            string plName = PlayerInfo.FindMatchesPreferOnline(p, message);
            if (plName == null) return;
            string colName = PlayerInfo.GetColoredName(p, plName);
                        
            bool banned = Group.BannedRank.Players.Contains(plName);
            string msg = colName;
            string ip = PlayerInfo.FindIP(plName);
            bool ipBanned = ip != null && Server.bannedIP.Contains(ip);
            
            if (!ipBanned && banned) msg += " %Sis &CBANNED";
            else if (!ipBanned && !banned) msg += " %Sis not banned";
            else if (ipBanned && banned) msg += " %Sand their IP are &CBANNED";
            else msg += " %Sis not banned, but their IP is &CBANNED";
            
            string[] data = Ban.GetBanData(plName);
            if (data != null && banned) {
                string grpName = Group.GetColoredName(data[3]);
                msg += " %S(Former rank: " + grpName + "%S)";
            }
            Player.Message(p, msg);
            
            if (data != null) {
                TimeSpan delta = GetDelta(data[2]);
                Player.Message(p, "{0} {1} ago by {2}", banned ? "Banned" : "Last banned", 
                               delta.Shorten(), GetName(p, data[0]));
                Player.Message(p, "Reason: {0}", data[1]);
            } else {
                Player.Message(p, "No ban data found for {0}%S.", colName);
            }
            
            data = Ban.GetUnbanData(plName);
            if (data != null) {
                TimeSpan delta = GetDelta(data[2]);
                Player.Message(p, "{0} {1} ago by {2}", banned ? "Last unbanned" : "Unbanned", 
                               delta.Shorten(), GetName(p, data[0]));
                Player.Message(p, "Reason: {0}", data[1]);
            }
        }
        
        static string GetName(Player p, string user) {
            // ban/unban uses truename
            if (ServerConfig.ClassicubeAccountPlus && !user.EndsWith("+")) user += "+";
            return PlayerInfo.GetColoredName(p, user);
        }
        
        static TimeSpan GetDelta(string data) {
            data = data.Replace(",", "");
            string[] date = data.SplitSpaces();
            string[] minuteHour = date[5].Split(':');
            
            int hour = int.Parse(minuteHour[0]), minute = int.Parse(minuteHour[1]);
            int day = int.Parse(date[1]), month = int.Parse(date[2]), year = int.Parse(date[3]);
            DateTime time = new DateTime(year, month, day, hour, minute, 0);
            return DateTime.Now - time;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/baninfo [player]");
            Player.Message(p, "%HOutputs information about current and/or previous ban/unban for that player.");
        }
    }
}
