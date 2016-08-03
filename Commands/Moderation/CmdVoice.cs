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
            Player who = message == "" ? p : PlayerInfo.FindMatches(p, message);
            if (who == null) return;
            if (p != null && who.Rank > p.Rank) {
                MessageTooHighRank(p, "voice", true); return;
            }
            
            if (who.voice) {
                Player.Message(p, "Removing voice status from " + who.ColoredName);
                Player.Message(who, "Your voice status has been revoked.");
            } else {
                Player.Message(p, "Giving voice status to " + who.ColoredName);
                Player.Message(who, "You have received voice status.");
            }
            who.voice = !who.voice;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/voice [name]");
            Player.Message(p, "%HToggles voice status on or off for the given player.");
            Player.Message(p, "%HIf no name is given, toggles your own voice status.");
        }
    }
}
