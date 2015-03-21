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
    public sealed class CmdImport : Command
    {
        public override string name { get { return "import"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdImport() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            string fileName;
            fileName = "extra/import/" + message + ".dat";

            if (!Directory.Exists("extra/import")) Directory.CreateDirectory("extra/import");
            if (!File.Exists(fileName))
            {
                Player.SendMessage(p, "Could not find .dat file");
                return;
            }

			using (FileStream fs = File.OpenRead(fileName))
			{
				if (ConvertDat.Load(fs, message) != null)
				{
					Player.SendMessage(p, "Converted map!");
				}
				else
				{
					Player.SendMessage(p, "The map conversion failed.");
					return;
				}
			}

            Command.all.Find("load").Use(p, message);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/import [.dat file] - Imports the .dat file given");
            Player.SendMessage(p, ".dat files should be located in the /extra/import/ folder");
        }
    }
}