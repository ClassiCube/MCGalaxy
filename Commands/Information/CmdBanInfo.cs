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
namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdBanInfo : Command {
        public override string name { get { return "baninfo"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdBanInfo() { }

        public override void Use(Player p, string message) {
            if (message == "") {
                if (p == null) { Player.Message(p, "Console must provide a player name."); return; }
                message = p.name;
            }
            bool banned = Group.IsBanned(message);
            string msg = message + (banned ? " is &CBANNED" : " is not banned");
            
            string[] data = Ban.GetBanData(message);
            if (data != null && banned) {
                Group grp = Group.Find(data[3]);
                string grpName = grp == null ? data[3] : grp.ColoredName;
                msg += " %S(Former rank: " + grpName + "%S)";
            }
            Player.Message(p, msg);
            
            if (data != null) {
                data[2] = Reformat(data[2]);
                Player.Message(p, "{0} on {1} by {2}", banned ? "Banned" : "Last banned", data[2], data[0]);
                Player.Message(p, "Reason: {0}", data[1]);
            } else {
                Player.Message(p, "No ban data found for " + message + ".");
            }
            
            data = Ban.GetUnbanData(message);
            if (data != null) {
                data[2] = Reformat(data[2]);
                Player.Message(p, "{0} on {1} by {2}", banned ? "Last unbanned" : "Ubanned", data[2], data[0]);
                Player.Message(p, "Reason: {0}", data[1]);
            }
        }
        
        static string Reformat(string data) {
            string[] date = data.Split(' ');
            data = date[1] + "-" + date[2] + "-" + date[3] + " at " + date[5];
            data = data.Replace(",", "");
            return data;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/baninfo <player> - returns info about banned player.");
        }
    }
}
