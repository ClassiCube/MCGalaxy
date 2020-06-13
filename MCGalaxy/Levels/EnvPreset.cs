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
using System.Collections.Generic;

namespace MCGalaxy {
    internal sealed class EnvPreset {
        public string Fog, Sky, Clouds, Sun, Shadow;
        
        public EnvPreset(string raw) {
            string[] args = raw.SplitSpaces();
            Fog = args[0]; Sky = args[1]; Clouds = args[2]; Sun = args[3]; Shadow = args[4];
        }
            
        public static Dictionary<string, string> Presets = new Dictionary<string, string>() {
                        //   fog   sky   clouds   sun   shadow
            { "Cartoon",  "00FFFF 1E90FF 00BFFF F5DEB3 F4A460" },
            { "Noir",     "000000 1F1F1F 000000 696969 1F1F1F" },
            { "Trippy",   "4B0082 FFD700 006400 7CFC00 B22222" },
            { "Watery",   "5F9EA0 008080 008B8B E0FFFF 008B8B" },
            { "Gloomy",   "6A80A5 405875 405875 444466 3B3B59" },
            { "Cloudy",   "AFAFAF 8E8E8E 8E8E8E 9B9B9B 8C8C8C" },
            { "Sunset",   "FFA322 836668 9A6551 7F6C60 46444C" },
            { "Midnight", "131947 070A23 1E223A 181828 0F0F19" },
            { "Normal",   "    " },
        };
        
        public static EnvPreset Find(string name) {
            foreach (var kvp in Presets) {
                if (kvp.Key.CaselessEq(name)) return new EnvPreset(kvp.Value);
            }
            return null;
        }
    }
}
