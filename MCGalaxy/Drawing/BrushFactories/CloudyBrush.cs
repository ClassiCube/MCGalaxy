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
using MCGalaxy.Commands;
using BlockID = System.UInt16;

namespace MCGalaxy.Drawing.Brushes {
    public sealed class CloudyBrushFactory : BrushFactory {       
        public override string Name { get { return "Cloudy"; } }        
        public override string[] Help { get { return HelpString; } }
        
        static string[] HelpString = new string[] {
            "&TArguments: [block1/frequency] [block2] <args>..",
            "&HDraws by selecting blocks from the given [blocks] using perlin noise.",
            "&Hfrequency is optional (defaults to 1), and specifies the number of times " +
                "the block should appear (as a fraction of the total of all the frequencies).",
            "&HOptional args format: &T<first letter of argument>=<value>",
            "&HArguments: &Ta&Hmplitude, &Tf&Hrequency (scale), &Ts&Heed, " +
            "&To&Hctaves, &Tp&Hersistence (turbulence), &Tl&Hacunarity",
        };
        
        public override Brush Construct(BrushArgs args) {
            NoiseArgs n = default(NoiseArgs);
            // Constants borrowed from fCraft to match it
            n.Amplitude = 1;
            n.Frequency = 0.08f;
            n.Octaves = 3;
            n.Seed = int.MinValue;
            n.Persistence = 0.75f;
            n.Lacunarity = 2;
            
            int[] count;
            BlockID[] toAffect = FrequencyBrush.GetBlocks(args, out count,
                                            Filter, arg => Handler(arg, args.Player, ref n));
            
            if (toAffect == null) return null;
            return new CloudyBrush(toAffect, count, n);
        }
        
        // Only want to handle non block options.
        static bool Filter(string arg) { return arg.Length >= 2 && (arg[1] == '_' || arg[1] == '='); }
        
        static bool Handler(string arg, Player p, ref NoiseArgs args) {
            char opt = arg[0];
            arg = arg.Substring(2); // get part after _ or =
            
            if (opt == 'l') return ParseDecimal(p, arg, ref args.Lacunarity, 2.00f);
            if (opt == 'a') return ParseDecimal(p, arg, ref args.Amplitude, 1.00f);
            if (opt == 'f') return ParseDecimal(p, arg, ref args.Frequency, 0.08f);
            if (opt == 'p') return ParseDecimal(p, arg, ref args.Persistence, 0.75f);
            
            if (opt == 'o') {
                if (!CommandParser.GetInt(p, arg, "Octaves", ref args.Octaves, 1, 16)) return false;
            } else if (opt == 's') {
                if (!CommandParser.GetInt(p, arg, "Seed", ref args.Seed)) return false;
            } else {
                p.Message("\"{0}\" was not a valid argument name.", opt);
                return false;
            }
            return true;
        }
        
        static bool ParseDecimal(Player p, string arg, ref float target, float scale) {
            if (!CommandParser.GetReal(p, arg, "Value", ref target)) return false;           
            target *= scale; return true;
        }
    }
    
    public struct NoiseArgs {
        public int Octaves, Seed;
        public float Frequency, Amplitude, Persistence, Lacunarity;
    }
}
