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
            get { return new[] { new CommandPerm(LevelPermission.Admin, "can hide silently") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new CommandAlias[] { new CommandAlias("XHide", "silent") }; }
        }
        
        static void AnnounceOps(Player p, string msg) {
            LevelPermission hideRank = p.oHideRank == LevelPermission.Null ? p.Rank : p.oHideRank;
            ItemPerms perms = new ItemPerms(hideRank, null, null);
            Chat.MessageFrom(ChatScope.Perms, p, msg, perms, null, true);
        }

        public override void Use(Player p, string message) {
            if (message.Length > 0 && p.possess.Length > 0) {
                Player.Message(p, "Stop your current possession first."); return;
            }
            bool silent = false;
            if (message.CaselessEq("silent")) {
                if (!CheckExtraPerm(p, 1)) return;
                silent = true;
            }
            
            Command adminchat = Command.Find("AdminChat");
            Command opchat = Command.Find("OpChat");
            Entities.GlobalDespawn(p, false);
            
            p.hidden = !p.hidden;
            if (p.hidden) {
                AnnounceOps(p, "To Ops -λNICK%S- is now &finvisible");
                
                if (!silent) {
                    string leaveM = "&c- λFULL %S" + PlayerDB.GetLogoutMessage(p);
                    Chat.MessageFrom(p, leaveM, null, true);
                }
                
                if (!p.opchat) opchat.Use(p, "");
                Server.hidden.AddIfNotExists(p.name);
            } else {
                AnnounceOps(p, "To Ops -λNICK%S- is now &fvisible");
                p.oHideRank = LevelPermission.Null;
                
                if (!silent) {
                    string joinM = "&a+ λFULL %S" + PlayerDB.GetLoginMessage(p);
                    Chat.MessageFrom(p, joinM, null, true);
                }
                
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
            Player.Message(p, "%T/Hide silent %H- Hides without showing join/leave message");
            Player.Message(p, "%HUse %T/OHide %Hto hide other players.");
        }
    }
}
