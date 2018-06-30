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
using MCGalaxy.Network;

namespace MCGalaxy.Tasks {   
    internal static class InitTasks {

        const string staffUrl = Updater.BaseURL + "Uploads/devs.txt";       
        internal static void UpdateStaffList(SchedulerTask task) {
            try {
                using (WebClient client = HttpUtil.CreateWebClient()) {
                    string raw = client.DownloadString(staffUrl);
                    string[] list = raw.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                    
                    foreach (string line in list) {
                        string[] bits = line.Split(':');
                        List<string> group = null;
                        if (bits[0].CaselessEq("devs")) group = Server.Devs;
                        if (bits[0].CaselessEq("mods")) group = Server.Mods;
                        
                        if (group == null) continue;
                        foreach (string name in bits[1].SplitSpaces()) {
                            group.Add(name.RemoveLastPlus());
                        }
                    }
                }
            } catch (Exception ex) {
                Logger.LogError("Error updating " + Server.SoftwareName + " staff list", ex);
                Server.Devs.Clear();
                Server.Mods.Clear();
            }
        }
    }
}