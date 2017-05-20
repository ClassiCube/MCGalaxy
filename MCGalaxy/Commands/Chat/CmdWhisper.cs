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
namespace MCGalaxy.Commands.Chatting {
    public sealed class CmdWhisper : Command {
        public override string name { get { return "whisper"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message) {
            if (message == "") {
                p.whisper = !p.whisper; p.whisperTo = "";
                if (p.whisper) Player.Message(p, "All messages sent will now auto-whisper");
                else Player.Message(p, "Whisper chat turned off");
            } else {
                Player who = PlayerInfo.FindMatches(p, message);
                if (who == null) { p.whisperTo = ""; p.whisper = false; return; }

                p.whisper = true;
                p.whisperTo = who.name;
                Player.Message(p, "Auto-whisper enabled. All messages will now be sent to " + who.DisplayName + ".");
            }
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/whisper [name]");
            Player.Message(p, "%HMakes all messages act like whispers");
        }
    }
}
