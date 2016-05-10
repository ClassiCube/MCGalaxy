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
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.Admin, 
                                                     "Lowest rank that can hide/unhide without showing a message to ops") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("xhide", "silent") }; }
        }        
        public CmdHide() { }

        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            if (message == "check") {
                string state = p.hidden ? "" : "not ";
                Player.Message(p, "You are " + state + "currently hidden!"); return;
            }
            if (message != "" && p.possess != "") {
                Player.Message(p, "Stop your current possession first."); return;
            }
            bool messageOps = true;
            if (message.CaselessEq("silent")) {
                if (!CheckAdditionalPerm(p)) { MessageNeedPerms(p, "can hide silently"); return; }
                messageOps = false;
            }
            
            Command opchat = Command.all.Find("opchat");
            Command adminchat = Command.all.Find("adminchat");
            p.hidden = !p.hidden;

            //Possible to use /hide myrank, but it accomplishes the same as regular /hide if you use it on yourself.
            if (message.CaselessEq("myrank")) {
                p.otherRankHidden = !p.otherRankHidden;
                p.hidden = p.otherRankHidden;
            }

            if (p.hidden) {
                Entities.GlobalDespawn(p, false);
                TabList.Add(p, p, 0xFF);
                if (messageOps && !p.otherRankHidden)
                    Chat.GlobalMessageOps("To Ops -" + p.ColoredName + "%S- is now &finvisible%S.");
                string discMsg = PlayerDB.GetLogoutMessage(p);
                Player.SendChatFrom(p, "&c- " + p.FullName + " %S" + discMsg, false);
                Server.IRC.Say(p.DisplayName + " left the game (" + discMsg + ")");
                if (messageOps && !p.opchat) opchat.Use(p, message);
            } else {
                Entities.GlobalSpawn(p, false);
                p.hidden = false;
                p.otherRankHidden = false;
                if (messageOps)
                    Chat.GlobalMessageAdmins("To Admins -" + p.ColoredName + "%S- is now &fvisible%S.");
                
                Player.SendChatFrom(p, "&a+ " + p.FullName + " %S" + PlayerDB.GetLoginMessage(p), false);
                Server.IRC.Say(p.DisplayName + " joined the game");
                if (messageOps && p.opchat) opchat.Use(p, message);
                if (p.adminchat) adminchat.Use(p, message);
            }
        }

        public override void Help(Player p) {
            Player.Message(p, "/hide - Toggles your visibility to other players, also toggles opchat.");
            Player.Message(p, "/hide check - Checks your hidden status.");
            Player.Message(p, "/hide silent - hides without sending a message to other ops/admins.");
            Player.Message(p, "Use /ohide to hide other players.");
        }
    }
}
