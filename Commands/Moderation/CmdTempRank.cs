/*
Copyright 2011 MCForge
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
using System.IO;

namespace MCGalaxy.Commands {
    
    public sealed class CmdTempRank : Command {
        
        public override string name { get { return "temprank"; } }
        public override string shortcut { get { return "tr"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        const StringComparison comp = StringComparison.OrdinalIgnoreCase;

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length < 3) { Help(p); return; }
            string player = args[0], rank = args[1], period = args[2];
            Player who = PlayerInfo.Find(player);
            if (who == null) {
                OfflinePlayer pInfo = PlayerInfo.FindOffline(player);
                if (pInfo == null) { Player.SendMessage(p, "&cPlayer &a" + player + "&c not found."); return; }
                player = pInfo.name;
            } else {
                player = who.name;
            }
            
            Group group = Group.Find(rank);
            if (group == null) { Player.SendMessage(p, "&cRank &a" + rank + "&c does not exist."); return; }
            int periodTime;
            if (!Int32.TryParse(period, out periodTime)) {
                Player.SendMessage(p, "&cThe period needs to be a number."); return;
            }

            foreach (string line in File.ReadAllLines("text/tempranks.txt")) {
                if (line.StartsWith(player, comp)) {
                    Player.SendMessage(p, "&cThe player already has a temporary rank assigned!"); return;
                }
            }
            
            if (p != null && who != null && p == who) {
                Player.SendMessage(p, "&cYou cannot assign yourself a temporary rank."); return;
            }
            Group pGroup = who != null ? who.group : Group.findPlayerGroup(player);
            if (p != null && pGroup.Permission >= p.group.Permission) {
                Player.SendMessage(p, "Cannot change the temporary rank of someone equal or higher to yourself."); return;
            }
            if (p != null && group.Permission >= p.group.Permission) {
                Player.SendMessage(p, "Cannot change the temporary rank to a higher rank than yourself."); return;
            }

            DateTime now = DateTime.Now;
            string assigner = p == null ? "Console" : p.name;
            using (StreamWriter sw = new StreamWriter("text/tempranks.txt", true))
                sw.WriteLine(player + " " + rank + " " + pGroup.name + " " + period + " " + now.Minute + " " +
                             now.Hour + " " + now.Day + " " + now.Month + " " + now.Year + " " + assigner);
            
            Command.all.Find("setrank").Use(null, who.name + " " + group.name + " assigning temp rank");
            Player.SendMessage(p, "Temporary rank (" + rank + ") assigned succesfully to " + player + " for " + period + " hours");
            Player.SendMessage(who, "Your Temporary rank (" + rank + ") is assigned succesfully for " + period + " hours");
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/temprank <player> <rank> <period(hours)> - Sets a temporary rank for the specified player.");
        }
    }
}
