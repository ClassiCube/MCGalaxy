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
        public string Theme, RawArgs;
        public bool UseSeed;
        public int Seed;
    }
    
    public static class MapGen {
        
        public static bool IsRecognisedTheme(string s) {
            s = s.ToLower();
            return simpleGens.ContainsKey(s) || advGens.ContainsKey(s);
        }
        
        public static void PrintThemes(Player p) {
            Player.Message(p, "Simple themes: " + simpleGens.Keys.Concatenate(", "));
            Player.Message(p, "Advanced themes: " + advGens.Keys.Concatenate(", "));
        }
        
        public static bool OkayAxis(int len) {
            return len >= 16 && len <= 8192 && (len % 16) == 0;
        }

        public static bool Generate(Level lvl, string theme, string args) {
            MapGenArgs genArgs = new MapGenArgs();
            genArgs.Level = lvl; genArgs.Theme = theme;
            genArgs.RawArgs = args;
            
            genArgs.UseSeed = args != "";
            if (genArgs.UseSeed && !int.TryParse(args, out genArgs.Seed))
                genArgs.Seed = args.GetHashCode();
            Action<MapGenArgs> generator = null;
            
            simpleGens.TryGetValue(theme, out generator);
            if (generator != null) { generator(genArgs); return true; }
            advGens.TryGetValue(theme, out generator);
            if (generator != null) { generator(genArgs); return true; }
            return false;
        }
        
        
        static Dictionary<string, Action<MapGenArgs>> simpleGens, advGens;        
        public static void RegisterSimpleGen(string theme, Action<MapGenArgs> gen) {
            simpleGens[theme.ToLower()] = gen;
        }
        
        public static void RegisterAdvancedGen(string theme, Action<MapGenArgs> gen) {
            advGens[theme.ToLower()] = gen;
        }
        
        static MapGen() {
            simpleGens = new Dictionary<string, Action<MapGenArgs>>();
            advGens = new Dictionary<string, Action<MapGenArgs>>();
            SimpleGen.RegisterGenerators();
            AdvNoiseGen.RegisterGenerators();
        }
    }
}
