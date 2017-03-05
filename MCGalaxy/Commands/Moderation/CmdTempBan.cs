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

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.SplitSpaces(3);
            string reason = args.Length > 2 ? args[2] : "";
            
            string target = ModActionCmd.FindName(p, "temp ban", "tempban",
                                                 args.Length == 1 ? "" : " " + args[1],
                                                 args[0], ref reason);
            if (target == null) return;
            Player who = PlayerInfo.FindExact(target);
            
            Group grp = who == null ? PlayerInfo.GetGroup(target) : who.group;
            if (p != null && grp.Permission >= p.Rank) {
                MessageTooHighRank(p, "temp ban", false); return;
            }
            
            TimeSpan time = TimeSpan.FromHours(1);
            if (args.Length > 1 && !args[1].TryParseShort(p, 'm', "temp ban for", out time)) return;
            if (time.TotalSeconds < 1) { Player.Message(p, "Cannot temp ban someone for less than a second."); return; }
            
            reason = ModActionCmd.ExpandReason(p, reason);
            if (reason == null) return;            
            string banner = p == null ? "(console)" : p.truename;
            
            Server.tempBans.AddOrReplace(target,
                                         Ban.PackTempBanData(reason, banner, DateTime.UtcNow.Add(time)));
            Server.tempBans.Save();
                                                                     
            if (who != null) {
                string kickReason = reason == "" ? "" : " - (" + reason + ")";
                who.Kick("Banned for " + time.Shorten(true) + "." + kickReason);
            }
            
            Player.Message(p, "Temp banned " + target + " for " + time.Shorten(true) + ".");
            if (args.Length <= 2) Player.AddNote(target, p, "T");
            else Player.AddNote(target, p, "T", reason);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/tempban [name] [timespan] <reason>");
            Player.Message(p, "%HBans [name] for [timespan]. Default is 1 hour.");
            Player.Message(p, "%H e.g. to tempban for 90 minutes, [timespan] would be %S1h30m");
            Player.Message(p, "%HFor <reason>, @number can be used as a shortcut for that rule.");
        }
    }
}
