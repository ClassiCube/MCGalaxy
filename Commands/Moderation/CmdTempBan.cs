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
using System;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdTempBan : Command {       
        public override string name { get { return "tempban"; } }
        public override string shortcut { get { return "tb"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        static char[] trimChars = {' '};

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.Split(trimChars, 3);
            Player who = PlayerInfo.Find(args[0]);
            
            string target = who == null ? args[0] : who.name;
            if (!Player.ValidName(target)) { Player.Message(p, "Invalid name \"" + target + "\"."); return; }
            Group grp = who == null ? PlayerInfo.GetGroup(target) : who.group;
            if (p != null && grp.Permission >= p.group.Permission) {
                MessageTooHighRank(p, "temp ban", false); return;
            }
            
            int minutes = 60;
            if (args.Length > 1 && !int.TryParse(args[1], out minutes)) {
                Player.Message(p, "Invalid minutes given."); return;
            }
            if (minutes > 1440) { Player.Message(p, "Cannot temp ban for more than a day"); return; }
            if (minutes < 1) { Player.Message(p, "Cannot temp ban someone for less than a minute"); return; }
            
            Server.TempBan tBan;
            tBan.name = target;
            tBan.reason = args.Length > 2 ? args[2] : "";
            tBan.expiryTime = DateTime.UtcNow.AddMinutes(minutes);
            AddTempban(tBan);
            
            if (who != null) {
                string reason = String.IsNullOrEmpty(tBan.reason) ? ""
                    : " - (" + tBan.reason + ")";
                who.Kick("Banned for " + minutes + " minutes!" + reason);
            }
            
            Player.Message(p, "Temp banned " + target + " for " + minutes + " minutes.");
            if (args.Length <= 2) Player.AddNote(target, p, "T");
            else Player.AddNote(target, p, "T", args[2]);
        }
        
        void AddTempban(Server.TempBan tBan) {
            for (int i = 0; i < Server.tempBans.Count; i++) {
                if (!Server.tempBans[i].name.CaselessEq(tBan.name)) continue;
                Server.tempBans[i] = tBan;
                return;
            }            
            Server.tempBans.Add(tBan);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/tempban <name> <minutes> [reason] - Bans <name> for <minutes>");
            Player.Message(p, "Max time is 1440 (1 day). Default is 60");
            Player.Message(p, "Temp bans will reset on server restart");
        }
    }
}
