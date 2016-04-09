/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
    public sealed class CmdHide : Command
    {
        public override string name { get { return "hide"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdHide() { }

        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            DoHide(p, message, true);
        }
        
        internal static void DoHide(Player p, string message, bool messageOps) {
            if (message == "check") {
                string state = p.hidden ? "" : "not ";
                Player.SendMessage(p, "You are " + state + "currently hidden!"); return;
            }
            if (message != "" && p.possess != "") {
                Player.SendMessage(p, "Stop your current possession first."); return;
            }            
            Command opchat = Command.all.Find("opchat");
            Command adminchat = Command.all.Find("adminchat");
            p.hidden = !p.hidden;
            
            if (p.hidden) {
                Player.GlobalDespawn(p, true);
                if (messageOps)
                    Chat.GlobalMessageOps("To Ops -" + p.color + p.DisplayName + "%S- is now &finvisible%S.");
                
                string discMsg = PlayerDB.GetLogoutMessage(p);
                Player.SendChatFrom(p, "&c- " + p.FullName + " %S" + discMsg, false);
                Server.IRC.Say(p.DisplayName + " left the game (" + discMsg + ")");
                if (messageOps && !p.opchat) opchat.Use(p, message);
            } else {
                Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
                if (messageOps)
                    Chat.GlobalMessageOps("To Ops -" + p.color + p.DisplayName + "%S- is now &8visible%S.");
                
                Player.SendChatFrom(p, "&a+ " + p.FullName + " %S" + PlayerDB.GetLoginMessage(p), false);
                Server.IRC.Say(p.DisplayName + " joined the game");
                if (messageOps && p.opchat) opchat.Use(p, message);
                if (p.adminchat) adminchat.Use(p, message);
            }
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/hide - Toggles your visibility to other players, also toggles opchat.");
        }
    }
    
    public sealed class CmdXhide : Command {
        public override string name { get { return "xhide"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            CmdHide.DoHide(p, message, false);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/xhide - like /hide, only it doesn't send a message to ops.");
        }
    }
}
