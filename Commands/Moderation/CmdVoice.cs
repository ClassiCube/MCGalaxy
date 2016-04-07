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
namespace MCGalaxy.Commands {
    
    public sealed class CmdVoice : Command {        
        public override string name { get { return "voice"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message) {
            if (message == "" && p == null) { Help(p); return; }
            Player who = message == "" ? p : PlayerInfo.FindOrShowMatches(p, message);
            if (who == null) return;
            if (p != null && who.group.Permission > p.group.Permission) {
                MessageTooHighRank(p, "voice", true); return;
            }
            
            if (who.voice) {
                Player.SendMessage(p, "Removing voice status from " + who.color + who.DisplayName);
                who.SendMessage("Your voice status has been revoked.");
            } else {
                Player.SendMessage(p, "Giving voice status to " + who.color + who.DisplayName);
                who.SendMessage("You have received voice status.");
            }
            who.voice = !who.voice;
            who.voicestring = who.voice ? "&f+" : "";
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/voice [name]");
            Player.SendMessage(p, "%HToggles voice status on or off for the given player.");
            Player.SendMessage(p, "%HIf no name is given, toggles your own voice status.");
        }
    }
}
