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
using System.IO;

namespace MCGalaxy.Commands.Moderation {   
    public sealed class CmdTempRankList : Command {        
        public override string name { get { return "tempranklist"; } }
        public override string shortcut { get { return "trl"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message) {
            if (message != "") { Help(p); return; }

            int count = 0;
            foreach (string line in File.ReadAllLines("text/tempranks.txt")) {
                if (count == 0)
                    Player.Message(p, "&ePlayers with a temporary rank assigned:");
                CmdTempRankInfo.PrintTempRankInfo(p, line);
                count++;
            }
            if (count == 0)
                Player.Message(p, "&cThere are no players with a temporary rank assigned.");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/trl - Lists every user with a temporary rank");
        }
    }
}
