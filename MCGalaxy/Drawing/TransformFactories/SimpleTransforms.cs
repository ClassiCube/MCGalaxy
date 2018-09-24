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
using MCGalaxy.Commands.Building;

namespace MCGalaxy.Drawing.Transforms {
    public sealed class NoTransformFactory : TransformFactory {
        public override string Name { get { return "None"; } }
        public override string[] Help { get { return HelpString; } }
        
        static string[] HelpString = new string[] {
            "%TArguments: none",
            "%HDoes not affect the output of draw operations.",
        };
        
        public override Transform Construct(Player p, string message) {
            return NoTransform.Instance;
        }
    }
    
    public sealed class ScaleTransformFactory : TransformFactory {
        public override string Name { get { return "Scale"; } }
        public override string[] Help { get { return HelpString; } }
        
        static string[] HelpString = new string[] {
            "%TArguments: [scaleX] [scaleY] [scaleZ] <centre>",
            "%TAlternatively: [scale] <centre>",
            "%H[scale] values can be an integer or a fraction (e.g. 2 or 1/2).",
            "%H[centre] if given, indicates to scale from the centre of a draw operation, " +
                "instead of outwards from the first mark. Recommended for cuboid and cylinder.",
        };
        
        public override Transform Construct(Player p, string message) {
            string[] args = message.SplitSpaces();
            if (message.Length == 0 || args.Length > 4) { p.MessageLines(Help); return null; }
            int mul = 0, div = 0;
            ScaleTransform scaler = new ScaleTransform();
            
            if (args.Length <= 2) {
                if (!ParseFraction(p, args[0], "Scale", out mul, out div)) return null;
                scaler.XMul = mul; scaler.XDiv = div;
                scaler.YMul = mul; scaler.YDiv = div;
                scaler.ZMul = mul; scaler.ZDiv = div;
            } else {
                if (!ParseFraction(p, args[0], "X scale", out mul, out div)) return null;
                scaler.XMul = mul; scaler.XDiv = div;
                if (!ParseFraction(p, args[1], "Y scale", out mul, out div)) return null;
                scaler.YMul = mul; scaler.YDiv = div;
                if (!ParseFraction(p, args[2], "Z scale", out mul, out div)) return null;
                scaler.ZMul = mul; scaler.ZDiv = div;
            }

            if ((args.Length % 2) != 0) return scaler; // no centre argument
            if (!args[args.Length - 1].CaselessEq("centre")) {
                p.Message("The mode must be either \"centre\", or not given."); return null;
            }
            scaler.CentreOrigin = true;
            return scaler;
        }
        
        static bool ParseFraction(Player p, string input, string argName, out int mul, out int div) {
            int sep = input.IndexOf('/');
            div = 1; mul = 1;
            
            if (sep == -1) { // single whole number
                return CommandParser.GetInt(p, input, argName, ref mul, -32, 32);
            }
            
            string top = input.Substring(0, sep), bottom = input.Substring(sep + 1);
            if (!CommandParser.GetInt(p, top,    argName + " (numerator)",   ref mul, -32768, 32768)) return false;
            if (!CommandParser.GetInt(p, bottom, argName + " (denominator)", ref div, -32768, 32768)) return false;
            
            if (div == 0) { p.Message("%WCannot divide by 0."); return false; }
            float fract = mul / (float)div;
            if (Math.Abs(fract) > 32) { p.Message(argName + " must be between -32 and 32."); return false; }
            return true;
        }
    }
}
