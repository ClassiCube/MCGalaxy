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
using System.Collections.Generic;
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdOpRules : Command
    {
        public override string name { get { return "oprules"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdOpRules() { }

        public override void Use(Player p, string message)
        {
            //Do you really need a list for this?
            List<string> oprules = new List<string>();
            if (!File.Exists("text/oprules.txt"))
            {
                File.WriteAllText("text/oprules.txt", "No oprules entered yet!");
            }

			using (StreamReader r = File.OpenText("text/oprules.txt"))
			{
				while (!r.EndOfStream)
					oprules.Add(r.ReadLine());
			}

            Player who = null;
            if (message != "")
            {
                who = Player.Find(message);
                 if (p.group.Permission < who.group.Permission) { Player.SendMessage(p, "You cant send /oprules to another player!"); return; }
            }
            else
            {
                who = p;
            }

            if (who != null)
            {
                //if (who.level == Server.mainLevel && Server.mainLevel.permissionbuild == LevelPermission.Guest) { who.SendMessage("You are currently on the guest map where anyone can build"); }
                who.SendMessage("Server OPRules:");
                foreach (string s in oprules)
                    who.SendMessage(s);
            }
            else
            {
                Player.SendMessage(p, "There is no player \"" + message + "\"!");
            }
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/oprules [player]- Displays server oprules to a player");
        }
    }
}
