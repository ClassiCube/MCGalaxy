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
namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdVoice : Command2 {        
        public override string name { get { return "Voice"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0 && p.IsSuper) { SuperRequiresArgs(p, "player name"); return; }
            Player target = message.Length == 0 ? p : PlayerInfo.FindMatches(p, message);
            if (target == null) return;
            if (!CheckRank(p, data, target, "voice", true)) return;
            
            if (target.voice) {
                p.Message("Removing voice status from " + p.FormatNick(target));
                target.Message("Your voice status has been revoked.");
            } else {
                p.Message("Giving voice status to " + p.FormatNick(target));
                target.Message("You have received voice status.");
            }
            target.voice = !target.voice;
        }
        
        public override void Help(Player p) {
            p.Message("&T/Voice [name]");
            p.Message("&HToggles voice status on or off for the given player.");
            p.Message("&HIf no name is given, toggles your own voice status.");
        }
    }
}
