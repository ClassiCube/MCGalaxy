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
using System;
using System.Collections.Generic;
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdUnloaded : Command
    {
        public override string name { get { return "unloaded"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdUnloaded() { }

        public override void Use(Player p, string message)
        {
            try
            {
                List<string> levels = new List<string>(Server.levels.Count);

                string unloadedLevels = ""; int currentNum = 0; int maxMaps = 0;

                if (message != "")
                {
                    try {
                        int n = int.Parse(message);
                        if (n <= 0) { Help(p); return; }
                        maxMaps = n * 50;
                        currentNum = maxMaps - 50;
                    } catch { Help(p); return; }
                }

                DirectoryInfo di = new DirectoryInfo("levels/");
                FileInfo[] fi = di.GetFiles("*.lvl");
                foreach (Level l in Server.levels) { levels.Add(l.name.ToLower()); }

                if (maxMaps == 0)
                {
                    foreach (FileInfo file in fi)
                    {
                        if (!levels.Contains(file.Name.Replace(".lvl", "").ToLower()))
                        {
                            string level = file.Name.Replace(".lvl", "");
                            string visit = GetLoadOnGoto(level) && p.group.Permission >= GetPerVisitPermission(level) ? "%aYes" : "%cNo";
                            unloadedLevels += ", " + Group.findPerm(GetPerBuildPermission(level)).color + level + " &b[" + visit + "&b]";
                        }
                    }
                    if (unloadedLevels != "")
                    {
                        Player.SendMessage(p, "Unloaded levels [Accessible]: ");
                        Player.SendMessage(p, unloadedLevels.Remove(0, 2));
                        if (fi.Length > 50) { Player.SendMessage(p, "For a more structured list, use /unloaded <1/2/3/..>"); }
                    }
                    else Player.SendMessage(p, "No maps are unloaded");
                }
                else
                {
                    if (maxMaps > fi.Length) maxMaps = fi.Length;
                    if (currentNum > fi.Length) { Player.SendMessage(p, "No maps beyond number " + fi.Length); return; }

                    Player.SendMessage(p, "Unloaded levels [Accessible] (" + currentNum + " to " + maxMaps + "):");
                    for (int i = currentNum; i < maxMaps; i++)
                    {
                        if (!levels.Contains(fi[i].Name.Replace(".lvl", "").ToLower()))
                        {
                            string level = fi[i].Name.Replace(".lvl", "");
                            string visit = GetLoadOnGoto(level) && p.group.Permission >= GetPerVisitPermission(level) ? "%aYes" : "%cNo";
                            unloadedLevels += ", " + Group.findPerm(GetPerBuildPermission(level)).color + level + " &b[" + visit + "&b]";
                        }
                    }

                    if (unloadedLevels != "")
                    {
                        Player.SendMessage(p, unloadedLevels.Remove(0, 2));
                    }
                    else Player.SendMessage(p, "No maps are unloaded");
                }
            }
            catch (Exception e) { Server.ErrorLog(e); Player.SendMessage(p, "An error occured"); }
            //Exception catching since it needs to be tested on Ocean Flatgrass
        }

        private LevelPermission GetPerVisitPermission(string level) {
            string location = "levels/level properties/" + level + ".properties";
            LevelPermission lvlperm = LevelPermission.Guest;
            try {
                using (StreamReader reader = new StreamReader(location)) {
                    string line;
                    while ((line = reader.ReadLine()) != null) {
                        if (line.Split()[0].ToLower() == "pervisit") {
                            lvlperm = Group.Find(line.Split()[2]).Permission;
                            break;
                        }
                    }
                }
            } catch { return LevelPermission.Guest; }

            return lvlperm;
        }

        private LevelPermission GetPerBuildPermission(string level) {
            string location = "levels/level properties/" + level + ".properties";
            LevelPermission lvlperm = LevelPermission.Guest;
            try {
                using (StreamReader reader = new StreamReader(location)) {
                    string line;
                    while ((line = reader.ReadLine()) != null) {
                        if (line.Split()[0].ToLower() == "perbuild") {
                            lvlperm = Group.Find(line.Split()[2]).Permission;
                            break;
                        }
                    }
                }
            } catch { return LevelPermission.Guest; }

            return lvlperm;
        }

        private bool GetLoadOnGoto(string level) {
            string location = "levels/level properties/" + level + ".properties";
            bool loadOnGoto = false;
            try {
                using (StreamReader reader = new StreamReader(location)) {
                    string line;
                    while ((line = reader.ReadLine()) != null) {
                        if (line.Split()[0].ToLower() == "loadongoto") {
                            loadOnGoto = bool.Parse(line.Split()[2]);
                            break;
                        }
                    }
                }
            } catch { return false; }

            return loadOnGoto;
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "%f/unloaded " + Server.DefaultColor + "- Lists all unloaded levels and their accessible state.");
            Player.SendMessage(p, "%f/unloaded <1/2/3/..> " + Server.DefaultColor + "- Shows a compact list.");
        }
    }
}
