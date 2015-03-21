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
using System.IO;

namespace MCGalaxy
{
    public static class Updater
    {
        /// <summary>
        /// Loads updater properties from given file
        /// </summary>
        /// <param name="givenPath">File path relative to server to load properties from</param>
        public static void Load(string givenPath)
        {
            if (File.Exists(givenPath))
            {
                string[] lines = File.ReadAllLines(givenPath);

                foreach (string line in lines)
                {
                    if (line != "" && line[0] != '#')
                    {
                        string key = line.Split('=')[0].Trim();
                        string value = line.Split('=')[1].Trim();

                        switch (key.ToLower())
                        {
                            case "autoupdate":
                                Server.autoupdate = (value.ToLower() == "true") ? true : false;
                                break;
                            case "notify":
                                Server.notifyPlayers = (value.ToLower() == "true") ? true : false;
                                break;
                            case "restartcountdown":
                                Server.restartcountdown = value;
                                break;

                        }
                    }
                }
            }
        }

    }
}
