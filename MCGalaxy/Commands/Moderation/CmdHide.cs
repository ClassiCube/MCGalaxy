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
using MCGalaxy.Events.PlayerEvents;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdHide : Command2 {
        public override string name { get { return "Hide"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override bool SuperUseable { get { return false; } }
        public override bool UpdatesLastCmd { get { return false; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Admin, "can hide silently") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new CommandAlias[] { new CommandAlias("XHide", "silent") }; }
        }
        
        static void AnnounceOps(Player p, string msg) {
            ItemPerms perms = new ItemPerms(p.hideRank);
            Chat.MessageFrom(ChatScope.Perms, p, msg, perms, null, true);
        }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length > 0 && p.possess.Length > 0) {
                p.Message("Stop your current possession first."); return;
            }
            bool silent = false;
            if (message.CaselessEq("silent")) {
                if (!CheckExtraPerm(p, data, 1)) return;
                silent = true;
            }
            
            Command adminchat = Command.Find("AdminChat");
            Command opchat = Command.Find("OpChat");
            Entities.GlobalDespawn(p, false);
            
            p.hidden = !p.hidden;
            if (p.hidden) {
                p.hideRank = data.Rank;
                AnnounceOps(p, "To Ops -λNICK&S- is now &finvisible");               
                
                if (!silent) {
                    string leaveMsg = "&c- λFULL &S" + PlayerDB.GetLogoutMessage(p);
                    Chat.MessageFrom(ChatScope.All, p, leaveMsg, null, null, true);
                }
                
                if (!p.opchat) opchat.Use(p, "", data);
                Server.hidden.Add(p.name);
                OnPlayerActionEvent.Call(p, PlayerAction.Hide);
            } else {
                AnnounceOps(p, "To Ops -λNICK&S- is now &fvisible");
                p.hideRank = LevelPermission.Banned;
                
                if (!silent) {
                    string joinMsg = "&a+ λFULL &S" + PlayerDB.GetLoginMessage(p);
                    Chat.MessageFrom(ChatScope.All, p, joinMsg, null, null, true);
                }
                
                if (p.opchat) opchat.Use(p, "", data);
                if (p.adminchat) adminchat.Use(p, "", data);
                Server.hidden.Remove(p.name);
                OnPlayerActionEvent.Call(p, PlayerAction.Unhide);
            }
            
            Entities.GlobalSpawn(p, false);
            TabList.Add(p, p, Entities.SelfID);
            Server.hidden.Save(false);
        }

        public override void Help(Player p) {
            p.Message("&T/Hide &H- Toggles your visibility to other players, also toggles opchat.");
            p.Message("&T/Hide silent &H- Hides without showing join/leave message");
            p.Message("&HUse &T/OHide &Hto hide other players.");
        }
    }
}
