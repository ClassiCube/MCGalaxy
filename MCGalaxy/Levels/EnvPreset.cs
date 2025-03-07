/*
    Copyright 2015-2024 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace MCGalaxy {
    public sealed class EnvPreset {

        const string FOLDER = "presets";
        const string FILE_EXTENSION = ".env";

        public readonly string Fog, Sky, Clouds, Sun, Shadow;
        public readonly string LavaLight = "", LampLight = "";
        
        public EnvPreset(string raw) {
            string[] args = raw.SplitSpaces();
            Fog = args[0]; Sky = args[1]; Clouds = args[2]; Sun = args[3]; Shadow = args[4];
            LavaLight = args.Length > 5 ? args[5] : "";
            LampLight = args.Length > 6 ? args[6] : "";
        }
            
        static Dictionary<string, string> Presets = new Dictionary<string, string>() {
                        //   fog   sky   clouds   sun   shadow
            { "Cartoon",  "00FFFF 1E90FF 00BFFF F5DEB3 F4A460" },
            { "Noir",     "000000 1F1F1F 000000 696969 1F1F1F" },
            { "Watery",   "5F9EA0 008080 008B8B E0FFFF 008B8B" },
            { "Misty",    "BBBBBB 657694 9494A5 696984 4E4E69" },
            { "Gloomy",   "6A80A5 405875 405875 444466 3B3B59" },
            { "Cloudy",   "AFAFAF 8E8E8E 8E8E8E 9B9B9B 8C8C8C" },
            { "Sunset",   "FFA322 836668 9A6551 7F6C60 46444C" },
            { "Dusk",     "D36538 836668 836668 525163 30304B" },
            { "Midnight", "131947 070A23 1E223A 181828 0F0F19" },
            { "Normal",   "    " },
        };

        public static EnvPreset Find(string value) {
            EnvPreset preset = FindDefault(value);
            if (preset != null) return preset;

            if (File.Exists(FOLDER + "/" + value.ToLower() + FILE_EXTENSION)) {
                string text = File.ReadAllText(FOLDER + "/" + value.ToLower() + FILE_EXTENSION);
                return new EnvPreset(text);
            }
            return null;
        }

        static EnvPreset FindDefault(string name) {
            foreach (var kvp in Presets) {
                if (kvp.Key.CaselessEq(name)) return new EnvPreset(kvp.Value);
            }
            return null;
        }

        public static void ListFor(Player p) {
            p.Message("&HPresets: &f{0}", Presets.Join(pr => pr.Key));

            string[] files = FileIO.TryGetFiles(FOLDER, "*" + FILE_EXTENSION);
            if (files == null) return;

            string all = files.Join(f => Path.GetFileNameWithoutExtension(f));
            if (all.Length > 0) p.Message("&HCustom presets: &f" + all);
        }
    }
}
