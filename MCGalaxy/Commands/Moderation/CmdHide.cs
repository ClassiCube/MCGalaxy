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
        public override string name { get { return "Hide"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Admin, "+ can hide silently") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new CommandAlias[] { new CommandAlias("XHide", "silent") }; }
        }

        public override void Use(Player p, string message) {
            if (message.Length > 0 && p.possess.Length > 0) {
                Player.Message(p, "Stop your current possession first."); return;
            }
            bool announceToOps = true;
            if (message.CaselessEq("silent")) {
                if (!CheckExtraPerm(p, 1)) return;
                announceToOps = false;
            }
            
            Command adminchat = Command.all.FindByName("AdminChat");
            Command opchat = Command.all.FindByName("OpChat");
            Entities.GlobalDespawn(p, false);
            
            p.hidden = !p.hidden;
            //Possible to use /hide myrank, but it accomplishes the same as regular /hide if you use it on yourself.
            if (message.CaselessEq("myrank")) {
                p.otherRankHidden = !p.otherRankHidden;
                p.hidden = p.otherRankHidden;
            }

            if (p.hidden) {                
            	if (announceToOps && !p.otherRankHidden) {
                    Chat.MessageOps("To Ops -" + p.ColoredName + "%S- is now &finvisible%S.");
            	}
                
                string discMsg = PlayerDB.GetLogoutMessage(p);
                Chat.MessageGlobal(p, "&c- " + p.FullName + " %S" + discMsg, false);
                Server.IRC.Say(p.DisplayName + " %Sleft the game (disconnected%S)");
                if (announceToOps && !p.opchat) opchat.Use(p, "");
                Server.hidden.AddIfNotExists(p.name);
            } else {                
                p.otherRankHidden = false;
                p.oHideRank = LevelPermission.Null;
                if (announceToOps) {
                    Chat.MessageOps("To Ops -" + p.ColoredName + "%S- is now &fvisible%S.");
                }
                
                Chat.MessageGlobal(p, "&a+ " + p.FullName + " %S" + PlayerDB.GetLoginMessage(p), false);
                Server.IRC.Say(p.DisplayName + " %Sjoined the game");
                if (p.opchat) opchat.Use(p, "");
                if (p.adminchat) adminchat.Use(p, "");
                Server.hidden.Remove(p.name);
            }
            
            Entities.GlobalSpawn(p, false);
            TabList.Add(p, p, Entities.SelfID);
            Server.hidden.Save(false);
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/Hide %H- Toggles your visibility to other players, also toggles opchat.");
            Player.Message(p, "%T/Hide silent %H- Hides without sending a message to opchat");
            Player.Message(p, "%HUse %T/OHide %Hto hide other players.");
        }
    }
}
