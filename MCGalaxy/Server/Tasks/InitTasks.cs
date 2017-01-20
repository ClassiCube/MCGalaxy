/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.Net;

namespace MCGalaxy.Tasks {   
    internal static class InitTasks {

        const string staffUrl = "https://raw.githubusercontent.com/Hetal728/MCGalaxy/master/Uploads/devs.txt";       
        internal static void UpdateStaffList() {
            try {
                using (WebClient web = new WebClient()) {
                    string[] result = web.DownloadString(staffUrl).Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                    foreach (string line in result) {
                        string type = line.Split(':')[0].ToLower();
                        List<string> list = (type == "devs") ? Server.Devs : (type == "mods") ? Server.Mods : null;
                        foreach (string name in line.Split(':')[1].Split())
                            list.Add(name.TrimEnd('+'));
                    }
                }
            } catch (Exception e) {
                Server.ErrorLog(e);
                Server.s.Log("Failed to update MCGalaxy staff list.");
                Server.Devs.Clear();
                Server.Mods.Clear();
            }
        }
    }
}