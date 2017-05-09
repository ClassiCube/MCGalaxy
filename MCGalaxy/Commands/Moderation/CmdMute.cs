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
using System;
using System.IO;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdMute : Command {
        public override string name { get { return "mute"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdMute() { }

        public override void Use(Player p, string message)
        {
            if (message == "" || message.SplitSpaces().Length > 2) { Help(p); return; }
            Player who = PlayerInfo.FindMatches(p, message);
            if (who == null) {
                if (Server.muted.Contains(message)) {
                    Server.muted.Remove(message);
                    Server.muted.Save();
                    Chat.MessageGlobal("{0} %Sis not online but is now &bun-muted", message);
                }
                return;
            }
            if (who == p) { Player.Message(p, "You cannot mute or unmute yourself."); return; }

            if (who.muted) {
                who.muted = false;
                Chat.MessageGlobal(who, who.ColoredName + " %Swas &bun-muted", false);
                Server.muted.Remove(who.name);
            } else  {
                if (p != null && who.Rank >= p.Rank) { 
                    MessageTooHighRank(p, "mute", false); return;
                }
                who.muted = true;
                Chat.MessageGlobal(who, who.ColoredName + " %Swas &8muted", false);
                Server.muted.AddIfNotExists(who.name);
                Player.AddNote(who.name, p, "M");
            }
            Server.muted.Save();
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/mute [player]");
            Player.Message(p, "%HMutes or unmutes that player.");
        }
    }
}
