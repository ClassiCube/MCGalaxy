/*
	Copyright 2011 MCGalaxy
	
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
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdGcaccept : Command
    {
        public override string name { get { return "gcaccept"; } }
        public override string shortcut { get { return "gca"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdGcaccept() { }

        public override void Use(Player p, string message)
        {
            if (Server.gcaccepted.Contains(p.name.ToLower())) { Player.SendMessage(p, "You already accepted the global chat rules!"); return; }
            Server.gcaccepted.Add(p.name.ToLower());
            File.WriteAllLines("text/gcaccepted.txt", Server.gcaccepted.ToArray());
            Player.SendMessage(p, "Congratulations! You can now use the Global Chat");
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/gcaccept - Accept the global chat rules.");
        }
    }
}
