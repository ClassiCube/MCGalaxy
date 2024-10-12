/*
    Copyright 2015 MCGalaxy
    
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
using System.Collections.Generic;
using MCGalaxy;

namespace MCGalaxy.Commands.Info {
	public sealed class CmdStaff : Command2 {
		public override string name { get { return "Staff"; } }
		public override string type { get { return "information"; } }
        
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "are staff"),
                    new CommandPerm(LevelPermission.Operator, "can read staff messages") }; }
        }
		
      public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces(2);
            
            if (args[0].Length > 0) {
                ChatModes.MessageStaff(p, args[0]);
            } else {
                ItemPerms perms = CommandExtraPerms.Find("Staff", 1);

                foreach (Group grp in Group.GroupList) {
                    if (grp.Permission < perms.MinRank) continue;
                    if (grp.Permission == LevelPermission.Nobody) continue;

                    List<string> players = grp.Players.All();
                    if (players.Count == 0) continue; 

                    p.Message(grp.ColoredName + "%H: " + players.Join());

                }
            }
        }
		
		public override void Help(Player p) {
            p.Message("%T/Staff %H- Shows a list of current staff.");
            p.Message("%T/Staff [message] %H- Sends a message to online staff.");
		}
	}
}