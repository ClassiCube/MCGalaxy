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
    public sealed class CmdTempRankInfo : Command {
        
        public override string name { get { return "temprankinfo"; } }
        public override string shortcut { get { return "tri"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }

            foreach (string line in File.ReadAllLines("text/tempranks.txt")) {
                if (!line.Contains(message)) continue;
                PrintTempRankInfo(p, line); return;
            }
            Player.Message(p, "&cPlayer &a" + message + "&chas not been assigned a temporary rank.");
        }
        
        internal static void PrintTempRankInfo(Player p, string line) {
            string[] args = line.Split(' ');
            string temprank = args[1], oldrank = args[2], tempranker = args[9];
            int minutes = Convert.ToInt32(args[4]), hours = Convert.ToInt32(args[5]);
            int days = Convert.ToInt32(args[6]), months = Convert.ToInt32(args[7]);
            int years = Convert.ToInt32(args[8]);
            
            int period = Convert.ToInt32(args[3]);
            Group oldGrp = Group.Find(oldrank), tempGrp = Group.Find(temprank);
            string oldCol = oldGrp == null ? "" : oldGrp.color;
            string tempCol = tempGrp == null ? "" : tempGrp.color;
            
            DateTime assignmentDate = new DateTime(years, months, days, hours, minutes, 0);
            DateTime expireDate = assignmentDate.AddHours(Convert.ToDouble(period));
            Player.Message(p, "%STemp rank information for " + args[0] + ":");
            Player.Message(p, "%sFrom " + oldCol + oldrank + " %sto "
                               + tempCol + temprank + "%s, by " + tempranker);
            Player.Message(p, "&SRanked on &a" + assignmentDate.ToString() +
                               "%S, expires &a" + expireDate.ToString());
        }

        public override void Help(Player p) {
            Player.Message(p, "/tri <player> - Lists the info about the temp rank of the given player");
        }
    }
}
