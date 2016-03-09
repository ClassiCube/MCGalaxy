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
namespace MCGalaxy.Commands
{
    public sealed class CmdXundo : Command
    {
        public override string name { get { return "xundo"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public CmdXundo() { }
        public override void Use(Player p, string message)
        {

            if (message == "") { Help(p); return; }
            int number = message.Split(' ').Length;
            if (number != 1) { Help(p); return; }

            Player who = PlayerInfo.Find(message);

            string error = "You are not allowed to undo this player";

            if (who == null || p == null || !(who.group.Permission >= LevelPermission.Operator && p.group.Permission < LevelPermission.Operator))
            {
                //This executes if who doesn't exist, if who is lower than Operator, or if the user is an op+.
                //It also executes on any combination of the three
                Command.all.Find("undo").Use(p, ((who == null) ? message : who.name) + " all"); //Who null check
                return;
            }
            Player.SendMessage(p, error);
        }


        public override void Help(Player p)
        {
            Player.SendMessage(p, "/xundo [name]  -  works as 'undo [name] all' but now anyone can use it (up to their undo limit)");
        }
    }
}
