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
            
            //Possible to use /hide myrank, but it accomplishes the same as regular /hide if you use it on yourself.
            if (message.ToLower() == "myrank")
            {
                p.otherRankHidden = !p.otherRankHidden;
                p.hidden = p.otherRankHidden;
            }

            if (p.hidden)
            {
                Player.GlobalDespawn(p, true);
                if (messageOps && !p.otherRankHidden)
                    Chat.GlobalMessageOps("To Ops -" + p.color + p.DisplayName + "%S- is now &finvisible%S.");
                string discMsg = PlayerDB.GetLogoutMessage(p);
                Player.SendChatFrom(p, "&c- " + p.FullName + " %S" + discMsg, false);
                Server.IRC.Say(p.DisplayName + " left the game (" + discMsg + ")");
                if (messageOps && !p.opchat) opchat.Use(p, message);
            }
            else
            {
                Player.GlobalSpawn(p, false);
                p.hidden = false;
                p.otherRankHidden = false;
                if(messageOps)
                    Chat.GlobalMessageAdmins("To Admins -" + p.color + p.DisplayName + "%S- is now &fvisible%S.");
                
                Player.SendChatFrom(p, "&a+ " + p.FullName + " %S" + PlayerDB.GetLoginMessage(p), false);
                Server.IRC.Say(p.DisplayName + " joined the game");
                if (messageOps && p.opchat) opchat.Use(p, message);
                if (p.adminchat) adminchat.Use(p, message);
            }
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/hide - Toggles your visibility to other players, also toggles opchat.");
            Player.SendMessage(p, "/hide check - Checks your hidden status.");
            Player.SendMessage(p, "Use /xhide to hide without sending a message to other ops/admins.");
            Player.SendMessage(p, "Use /ohide to hide other players.");
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
