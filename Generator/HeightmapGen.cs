/*
    Copyright 2015 MCGalaxy
    
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
using System.Net;

namespace MCGalaxy.Generator {
    public static class HeightmapGen {
        
        public static bool DownloadImage(string url, string dir, Player p) {
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                url = "http://" + url;
            
            try {
                using (WebClient client = new WebClient()) {
                    Player.Message(p, "Downloading file from: &f" + url);
                    client.DownloadFile(url, dir + "tempImage_" + p.name + ".bmp");
                }
                Player.Message(p, "Finished downloading image.");
                return true;
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                Player.Message(p, "&cFailed to download the image from the given url.");
                Player.Message(p, "&cThe url may need to end with its extension (such as .jpg).");
                return false;
            }
        }
    }
}
