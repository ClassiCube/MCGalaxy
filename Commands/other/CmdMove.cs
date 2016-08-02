/*
	Copyright 2011 MCForge
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
    public sealed class CmdMove : Command {
        public override string name { get { return "move"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdMove() { }

        public override void Use(Player p, string message) {
            // /move name map
            // /move x y z
            // /move name x y z

            string[] args = message.Split(' ');
            if (args.Length < 1 || args.Length > 4) { Help(p); return; }

            // /move name
            if (args.Length == 1) {
                // Use main world by default
                // Add the world name to the 2nd param so that the IF block below is used
                args = new string[] { args[0], Server.mainLevel.name };
            }

            if (args.Length == 2) {    // /move name map
                Player who = PlayerInfo.FindMatches(p, args[0]);
                Level where = LevelInfo.FindMatches(p, args[1]);
                if (who == null || where == null) return;
                if (p != null && who.Rank > p.Rank) { 
                    MessageTooHighRank(p, "move", true); return;
                }

                PlayerActions.ChangeMap(who, where.name);
                if (who.level == where)
                    Player.Message(p, "Sent " + who.ColoredName + " %Sto " + where.name);
                else
                    Player.Message(p, where.name + " is not loaded");
            } else {
                // /move name x y z
                // /move x y z

                Player who;
                int offset = 0;
                if (args.Length == 4) {
                    who = PlayerInfo.FindMatches(p, args[0]);
                    if (who == null) return;
                    if (p != null && who.Rank > p.Rank) { 
                        MessageTooHighRank(p, "move", true); return; 
                    }
                    offset = 1;
                } else {
                    who = p;
                }

                try {
                    ushort x = ushort.Parse(args[offset + 0]);
                    ushort y = ushort.Parse(args[offset + 1]);
                    ushort z = ushort.Parse(args[offset + 2]);
                    PlayerActions.MoveCoords(who, x, y, z);
                    if (p != who) Player.Message(p, "Moved " + who.color + who.name);
                }
                catch { Player.Message(p, "Invalid co-ordinates"); }
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/move <player> <map> <x> <y> <z>");
            Player.Message(p, "%HMove <player> to <map> or given coordinates");
            Player.Message(p, "%H<map> must be blank if x, y or z is used and vice versa");
            Player.Message(p, "%HIf <map> is empty, the main level be assumed");
        }
    }
}
