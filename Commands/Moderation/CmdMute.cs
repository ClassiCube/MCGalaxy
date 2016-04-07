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
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdMute : Command
    {
        public override string name { get { return "mute"; } }
        public override string shortcut { get { return ""; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdMute() { }

        public override void Use(Player p, string message)
        {
            if (message == "" || message.Split(' ').Length > 2) { Help(p); return; }
            Player who = PlayerInfo.FindOrShowMatches(p, message);
            if (who == null) {
                if (Server.muted.Contains(message)) {
                    Server.muted.Remove(message);
                    Player.GlobalMessage(message + Server.DefaultColor + " is not online but is now &bun-muted");
                    Extensions.DeleteLineWord("ranks/muted.txt", who.name.ToLower());
                }
                return;
            }

            if (who == p) { Player.SendMessage(p, "You cannot mute or unmute yourself."); return; }

            if (who.muted) {
                who.muted = false;
                Player.SendChatFrom(who, who.color + who.DisplayName + " %Shas been &bun-muted", false);
                Extensions.DeleteLineWord("ranks/muted.txt", who.name.ToLower());
            } else  {
                if (p != null && who.group.Permission >= p.group.Permission) { 
                    MessageTooHighRank(p, "mute", false); return;
                }
                who.muted = true;
                Player.SendChatFrom(who, who.color + who.DisplayName + " %Shas been &8muted", false);
                using (StreamWriter writer = new StreamWriter("ranks/muted.txt", true)) {
                    writer.WriteLine(who.name.ToLower());
                }
                Server.s.Log("SAVED: ranks/muted.txt");
            }
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/mute <player> - Mutes or unmutes the player.");
        }
    }
}
