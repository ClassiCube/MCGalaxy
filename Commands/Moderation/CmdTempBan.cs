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

namespace MCGalaxy.Commands {
    
    public sealed class CmdTempBan : Command {
        
        public override string name { get { return "tempban"; } }
        public override string shortcut { get { return "tb"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.Split(' ');
            Player who = PlayerInfo.Find(args[0]);
            
            string target = who == null ? args[0] : who.name;
            if (!Player.ValidName(target)) { Player.SendMessage(p, "Invalid name \"" + target + "\"."); return; }
            Group grp = who == null ? PlayerInfo.GetGroup(target) : who.group;
            if (p != null && grp.Permission >= p.group.Permission) {
                Player.SendMessage(p, "Cannot temp ban someone of the same or higher rank."); return;
            }
            
            int minutes = 60;
            if (args.Length > 1 && !int.TryParse(args[1], out minutes)) {
                Player.SendMessage(p, "Invalid minutes given."); return;
            }
            if (minutes > 1440) { Player.SendMessage(p, "Cannot temp ban for more than a day"); return; }
            if (minutes < 1) { Player.SendMessage(p, "Cannot temp ban someone for less than a minute"); return; }
            
            Server.TempBan tBan;
            tBan.name = target;
            tBan.expiryTime = DateTime.Now.AddMinutes(minutes);
            Server.tempBans.Add(tBan);
            if (who != null)
                who.Kick("Banned for " + minutes + " minutes!");
            Player.SendMessage(p, "Temp banned " + target + " for " + minutes + " minutes.");
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/tempban <name> <minutes> - Bans <name> for <minutes>");
            Player.SendMessage(p, "Max time is 1440 (1 day). Default is 60");
            Player.SendMessage(p, "Temp bans will reset on server restart");
        }
    }
}
