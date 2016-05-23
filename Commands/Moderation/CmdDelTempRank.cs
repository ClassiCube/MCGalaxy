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
using System.Text;

namespace MCGalaxy.Commands.Moderation {    
    public sealed class CmdDelTempRank : Command {
        public override string name { get { return "deltemprank"; } }
        public override string shortcut { get { return "dtr"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        const StringComparison comp = StringComparison.OrdinalIgnoreCase;

        public override void Use(Player p, string message) {
            bool assigned = false;
            StringBuilder all = new StringBuilder();
            Player who = PlayerInfo.Find(message);
            
            foreach (string line in File.ReadAllLines("text/tempranks.txt")) {
                if (!line.StartsWith(message, comp)) { all.AppendLine(line); continue; }
                
                string[] parts = line.Split(' ');
                Group newgroup = Group.Find(parts[2]);
                Command.all.Find("setrank").Use(null, message + " " + newgroup.name + " temp rank unassigned");
                Player.Message(p, "&eTemp rank of &a" + message + "&e has been unassigned");
                if (who != null)
                    Player.Message(who, "&eYour temp rank has been unassigned");
                assigned = true;
            }
            
            if (!assigned) {
                Player.Message(p, "&a" + message + "&c has not been assigned a temp rank."); return;
            }
            File.WriteAllText("text/tempranks.txt", all.ToString());
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/dtr <player> - Deletes that player's temp rank");
        }
    }
}
