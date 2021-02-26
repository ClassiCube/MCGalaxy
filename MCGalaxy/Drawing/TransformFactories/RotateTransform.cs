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
    
    public sealed class RotateTransformFactory : TransformFactory {
        public override string Name { get { return "Rotate"; } }
        public override string[] Help { get { return HelpString; } }
        
        static string[] HelpString = new string[] {
            "&TArguments: [angleX] [angleY] [angleZ] <centre>",
            "&H[angle] values are values in degrees.",
            "&H[centre] if given, indicates to scale from the centre of a draw operation, " +
            "instead of outwards from the first mark. Recommended for cuboid and cylinder.",
        };
        
        public override Transform Construct(Player p, string message) {
            string[] args = message.SplitSpaces();
            if (args.Length < 3 || args.Length > 4) { p.MessageLines(Help); return null; }
            float angleX = 0, angleY = 0, angleZ = 0;
            RotateTransform rotater = new RotateTransform();
            
            if (!ParseAngle(p, args[0], ref angleX)) return null;
            if (!ParseAngle(p, args[1], ref angleY)) return null;
            if (!ParseAngle(p, args[2], ref angleZ)) return null;
            rotater.SetAngles(angleX, angleY, angleZ);

            if (args.Length == 3) return rotater; // no centre argument
            if (!args[args.Length - 1].CaselessEq("centre")) {
                p.Message("The mode must be either \"centre\", or not given."); return null;
            }
            rotater.CentreOrigin = true;
            return rotater;
        }
        
        static bool ParseAngle(Player p, string input, ref float angle) {
            if (!CommandParser.GetReal(p, input, "Angle", ref angle, -360, 360)) {
                p.MessageLines(HelpString); return false;
        	}
            return true;
        }
    }
}
