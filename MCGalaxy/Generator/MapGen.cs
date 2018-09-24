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
    public delegate bool MapGenFunc(Player p, Level lvl, string seed);
    public enum GenType { Simple, fCraft, Advanced };
    
    /// <summary> Maintains a list of map generator instances. </summary>
    public sealed class MapGen {
        public string Theme, Desc;
        public GenType Type;
        public MapGenFunc GenFunc;
        
        public bool Generate(Player p, Level lvl, string seed) {
            lvl.Config.Theme = Theme;
            lvl.Config.Seed  = seed;
            return GenFunc(p, lvl, seed);
        }
        
        
        public static Random MakeRng(string seed) {
            if (seed.Length == 0) return new Random();
            return new Random(MakeInt(seed));
        }
        
        public static int MakeInt(string seed) {
            if (seed.Length == 0) return new Random().Next();
            
            int value;
            if (!int.TryParse(seed, out value)) value = seed.GetHashCode();
            return value;
        }

        
        public static List<MapGen> Generators = new List<MapGen>();
        public static MapGen Find(string theme) {
            foreach (MapGen gen in Generators) {
                if (gen.Theme.CaselessEq(theme)) return gen;
            }
            return null;
        }
        
        static string FilterThemes(GenType type) { 
            return Generators.Join(g => g.Type == type ? g.Theme : null); 
        }
        public static void PrintThemes(Player p) {
            p.Message("%HSimple themes: &f"   + FilterThemes(GenType.Simple));
            p.Message("%HfCraft themes: &f"   + FilterThemes(GenType.fCraft));
            p.Message("%HAdvanced themes: &f" + FilterThemes(GenType.Advanced));
        }
        
        public static void Register(string theme, GenType type, MapGenFunc func, string desc) {
            MapGen gen = new MapGen() { Theme = theme, GenFunc = func, Desc = desc, Type = type };
            Generators.Add(gen);
        }
        
        static MapGen() {
            SimpleGen.RegisterGenerators();
            fCraftMapGen.RegisterGenerators();
            AdvNoiseGen.RegisterGenerators();
            Register("Heightmap", GenType.Advanced, HeightmapGen.Generate,
                     "%HSeed specifies the URL of the heightmap image");
        }
    }
}
