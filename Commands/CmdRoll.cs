/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
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
namespace MCGalaxy.Commands
{
    public sealed class CmdRoll : Command
    {
        public override string name { get { return "roll"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdRoll() { }

        public override void Use(Player p, string message)
        {
            int min, max; Random rand = new Random();
            try { min = int.Parse(message.Split(' ')[0]); }
            catch { min = 1; }
            try { max = int.Parse(message.Split(' ')[1]); }
            catch { max = 7; }

            Player.GlobalMessage(p.color + p.name + Server.DefaultColor + " rolled a &a" + rand.Next(Math.Min(min, max), Math.Max(min, max) + 1).ToString() + Server.DefaultColor + " (" + Math.Min(min, max) + "|" + Math.Max(min, max) + ")");
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/roll [min] [max] - Rolls a random number between [min] and [max].");
        }
    }
}