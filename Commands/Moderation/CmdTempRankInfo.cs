/*
Copyright 2011 MCGalaxy
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
namespace MCGalaxy.Commands
{
    public sealed class CmdTempRankInfo : Command
    {
        public override string name { get { return "temprankinfo"; } }
        public override string shortcut { get { return "tri"; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdTempRankInfo() { }

        public override void Use(Player p, string message)
        {
            string alltext = File.ReadAllText("text/tempranks.txt");
            if (alltext.Contains(message) == false)
            {
                Player.SendMessage(p, "&cPlayer &a" + message + "&c Has not been assigned a temporary rank. Cannot unnasign.");
                return;
            }
            if (message == "")
            {
                Help(p);
                Player.SendMessage(p, "&cYou need to enter a player!");
                return;
            }

            foreach (string line in File.ReadAllLines("text/tempranks.txt")) {
                if (!line.Contains(message)) continue;
                
                string[] args = line.Split(' ');
                string temprank = args[1];
                string oldrank = args[2];
                string tempranker = args[9];
                int period = Convert.ToInt32(args[3]);
                int minutes = Convert.ToInt32(args[4]);
                int hours = Convert.ToInt32(args[5]);
                int days = Convert.ToInt32(args[6]);
                int months = Convert.ToInt32(args[7]);
                int years = Convert.ToInt32(args[8]);
                
                DateTime assignmentDate = new DateTime(years, months, days, hours, minutes, 0);
                DateTime expireDate = assignmentDate.AddHours(Convert.ToDouble(period));
                Player.SendMessage(p, "&1Temporary Rank Information of " + message);
                Player.SendMessage(p, "&aTemporary Rank: " + temprank);
                Player.SendMessage(p, "&aOld Rank: " + oldrank);
                Player.SendMessage(p, "&aDate of assignment: " + assignmentDate.ToString());
                Player.SendMessage(p, "&aDate of expiry: " + expireDate.ToString());
                Player.SendMessage(p, "&aTempranked by: " + tempranker);
            }
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "/TempRankInfo <player> - Lists the info of the Temporary rank of the given player");
        }
    }   
}
