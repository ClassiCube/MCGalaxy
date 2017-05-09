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
using MCGalaxy.DB;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdHide : Command {
        public override string name { get { return "hide"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Admin, "+ can hide/unhide without showing a message to ops") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("xhide", "silent") }; }
        }
        public CmdHide() { }

        public override void Use(Player p, string message)
        {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            if (message == "check") {
                string state = p.hidden ? "" : "not ";
                Player.Message(p, "You are " + state + "currently hidden!"); return;
            }
            if (message != "" && p.possess != "") {
                Player.Message(p, "Stop your current possession first."); return;
            }
            bool messageOps = true;
            if (message.CaselessEq("silent")) {
                if (!CheckExtraPerm(p)) { MessageNeedExtra(p, 1); return; }
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
                if (messageOps && !p.otherRankHidden)
                    Chat.MessageOps("To Ops -" + p.ColoredName + "%S- is now &finvisible%S.");
                
                string discMsg = PlayerDB.GetLogoutMessage(p);
                Chat.MessageGlobal(p, "&c- " + p.FullName + " %S" + discMsg, false);
                Server.IRC.Say(p.DisplayName + " %Sleft the game (disconnected%S)");
                if (messageOps && !p.opchat) opchat.Use(p, message);
                Server.hidden.AddIfNotExists(p.name);
            } else {
                Entities.GlobalSpawn(p, false);
                p.otherRankHidden = false;
                p.oHideRank = LevelPermission.Null;
                if (messageOps)
                    Chat.MessageAdmins("To Admins -" + p.ColoredName + "%S- is now &fvisible%S.");
                
                Chat.MessageGlobal(p, "&a+ " + p.FullName + " %S" + PlayerDB.GetLoginMessage(p), false);
                Server.IRC.Say(p.DisplayName + " %Sjoined the game");
                if (messageOps && p.opchat) opchat.Use(p, message);
                if (p.adminchat) adminchat.Use(p, message);
                Server.hidden.Remove(p.name);
            }
            
            TabList.Add(p, p, Entities.SelfID);
            Server.hidden.Save(false);
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/hide %H- Toggles your visibility to other players, also toggles opchat.");
            Player.Message(p, "%T/hide check %H- Checks your hidden status.");
            Player.Message(p, "%T/hide silent %H- hides without sending a message to other ops/admins.");
            Player.Message(p, "%HUse %T/ohide %Hto hide other players.");
        }
    }
}
