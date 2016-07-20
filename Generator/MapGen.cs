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

namespace MCGalaxy.Generator {
    
    public struct MapGenArgs {
        public Level Level;
        public string Type, RawArgs;
        public bool UseSeed;
        public int Seed;
    }
    
    public static class MapGen {
        public static bool IsRecognisedTheme(string s) {
            s = s.ToLower();
            return Array.IndexOf<string>(types, s) >= 0 || Array.IndexOf<string>(advTypes, s) >= 0;
        }
        
        static string[] types = { "island", "mountains", "forest", "ocean", "flat",
            "pixel", "empty", "desert", "space", "rainbow", "hell" };
        static string[] advTypes = { "billow", "perlin", "checkerboard", "spheres", "cylinders",
            "voronoi", "ridgedmultifractal", "perlin3d", "perlin3dyadjust" };

        public static void PrintThemes(Player p) {
            Player.Message(p, "Simple themes: " + String.Join(", ", types));
            Player.Message(p, "Advanced themes: " + String.Join(", ", advTypes));
        }
        
        public static bool OkayAxis(int len) {
            return len >= 16 && len <= 8192 && (len % 16) == 0;
        }

        public unsafe static void Generate(Level lvl, string type, string args) {
            MapGenArgs genArgs = new MapGenArgs();
            genArgs.Level = lvl; genArgs.Type = type;
            genArgs.RawArgs = args;
            
            genArgs.UseSeed = args != "";
            if (genArgs.UseSeed && !int.TryParse(args, out genArgs.Seed))
                genArgs.Seed = args.GetHashCode();
            
            if (!SimpleGen.Generate(genArgs))
                AdvNoiseGen.Generate(genArgs);
        }
    }
}
