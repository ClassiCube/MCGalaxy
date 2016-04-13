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
namespace MCGalaxy.Commands
{
    public sealed class CmdViewRanks : Command
    {
        public override string name { get { return "viewranks"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("ops", "operator"), 
                    new CommandAlias("admins", "superop"), new CommandAlias("banned", "banned") }; }
        }
        public CmdViewRanks() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }

            Group foundGroup = Group.Find(message);
            if (foundGroup == null)
            {
                Player.SendMessage(p, "Could not find group");
                return;
            }


            string totalList = "";
            foreach (string s in foundGroup.playerList.All())
            {
                totalList += ", " + s;
            }

            if (totalList == "")
            {
                Player.SendMessage(p, "No one has the rank of " + foundGroup.color + foundGroup.name);
                return;
            }
            
            Player.SendMessage(p, "People with the rank of " + foundGroup.color + foundGroup.name + ":");
            Player.SendMessage(p, totalList.Remove(0, 2));
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/viewranks [rank] - Shows all users who have [rank]");
            Player.SendMessage(p, "Available ranks: " + Group.concatList());
        }
    }
}
