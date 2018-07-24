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

namespace MCGalaxy.Generator {
    
    /// <summary> Holds arguments for a map generator. </summary>
    public struct MapGenArgs {
        public Player Player;
        public Level Level;
        public string Theme, Args;
        public bool UseSeed;
        public int Seed;
    }
    
    /// <summary> Represents a map generator, returning whether map generation succeeded or not. </summary>
    public delegate bool MapGenerator(MapGenArgs args);
    
    /// <summary> Maintains a list of map generator instances. </summary>
    public static class MapGen {
        
        /// <summary> Returns whether the given map generation theme name is recognised. </summary>
        public static bool IsRecognisedTheme(string s) {
            s = s.ToLower();
            return simpleGens.ContainsKey(s) || advGens.ContainsKey(s);
        }

        public static bool IsSimpleTheme(string s) {
            return simpleGens.ContainsKey(s.ToLower());
        }
        
        public static void PrintThemes(Player p) {
            p.Message("Simple themes: " + simpleGens.Keys.Join(", "));
            p.Message("Advanced themes: " + advGens.Keys.Join(", "));
        }
        
        public static IEnumerable<string> SimpleThemeNames { get { return simpleGens.Keys; } }
        
        public static IEnumerable<string> AdvancedThemeNames { get { return advGens.Keys; } }
        
        
        public static bool OkayAxis(int len) {
            return len >= 16 && len <= 8192 && (len % 16) == 0;
        }

        public static bool Generate(Level lvl, string theme, string seed, Player p) {
            MapGenArgs genArgs = new MapGenArgs();
            genArgs.Level = lvl; genArgs.Player = p;
            genArgs.Theme = theme; lvl.Config.Theme = theme;
            genArgs.Args = seed;   lvl.Config.Seed  = seed;
            
            genArgs.UseSeed = seed.Length > 0;
            if (genArgs.UseSeed && !int.TryParse(seed, out genArgs.Seed))
                genArgs.Seed = seed.GetHashCode();
            MapGenerator generator = null;
            
            simpleGens.TryGetValue(theme, out generator);
            if (generator != null) return generator(genArgs);
            advGens.TryGetValue(theme, out generator);
            if (generator != null) return generator(genArgs);
            return false;
        }
        
        
        static Dictionary<string, MapGenerator> simpleGens, advGens;
        public static void RegisterSimpleGen(string theme, MapGenerator gen) {
            simpleGens[theme.ToLower()] = gen;
        }
        
        public static void RegisterAdvancedGen(string theme, MapGenerator gen) {
            advGens[theme.ToLower()] = gen;
        }
        
        static MapGen() {
            simpleGens = new Dictionary<string, MapGenerator>();
            advGens = new Dictionary<string, MapGenerator>();
            SimpleGen.RegisterGenerators();
            fCraftMapGenerator.RegisterGenerators();
            
            AdvNoiseGen.RegisterGenerators();
            RegisterAdvancedGen("heightmap", HeightmapGen.Generate);
        }
    }
}
