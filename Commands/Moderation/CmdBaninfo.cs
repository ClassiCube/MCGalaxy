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
    public sealed class CmdBaninfo : Command {
        public override string name { get { return "baninfo"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdBaninfo() { }

        public override void Use(Player p, string message) {
            if (message == "" || message.Length <= 3) { Help(p); return; }
            
            string[] data = Ban.GetBanData(message);
            if (data != null) {
                Player.Message(p, "&9User: &e" + message);
                Player.Message(p, "&9Banned by: &e" + data[0]);
                Player.Message(p, "&9Reason: &e" + data[1]);
                Player.Message(p, "&9Date and time: &e" + data[2]);
                Player.Message(p, "&9Old rank: &e" + data[3]);
                string stealth = data[4] == "true" ?  "&aYes" : "&cNo";
                Player.Message(p, "&9Stealth banned: " + stealth);
            } else if (!Group.findPerm(LevelPermission.Banned).playerList.Contains(message)) {
                Player.Message(p, "That player isn't banned");
            } else if (data == null) {
                Player.Message(p, "Couldn't find ban info about " + message + ".");
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/baninfo <player> - returns info about banned player.");
        }
    }
}
