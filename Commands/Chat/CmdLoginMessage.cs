/*
    Written By Jack1312

    Copyright 2011 MCForge
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System.IO;

namespace MCGalaxy.Commands {
    
    public sealed class CmdLoginMessage : Command {
        
        public override string name { get { return "loginmessage"; } }
        public override string shortcut { get { return "loginmsg"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdLoginMessage() { }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.SplitSpaces(2);
            if (args.Length < 2) { Help(p); return; }
            string target = PlayerInfo.FindMatchesPreferOnline(p, args[0]);
            if (target == null) return;
            
            PlayerDB.SetLoginMessage(target, args[1]);
            Player.Message(p, "The login message of {0} %Shas been changed to: {1}",
                           PlayerInfo.GetColoredName(p, target), args[1]);
            string changer = p == null ? "(console)" : p.name;
            Server.s.Log(changer + " changed " + target + "'s login message to: " + args[1]);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/loginmessage [Player] [Message]");
            Player.Message(p, "%HSets the login message shown for that player.");
        }
    }
}
