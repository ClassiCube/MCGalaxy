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
using MCGalaxy.Commands.Building;

namespace MCGalaxy.Drawing.Transforms {
    
    public sealed class RotateTransformFactory : TransformFactory {
        public override string Name { get { return "Rotate"; } }
        public override string[] Help { get { return HelpString; } }
        
        static string[] HelpString = new string[] {
            "%TArguments: [angleX] [angleY] [angleZ] <centre>",
            "%H[angle] values are values in degrees.",
            "%H[centre] if given, indicates to scale from the centre of a draw operation, " +
            "instead of outwards from the first mark. Recommended for cuboid and cylinder.",
        };
        
        public override Transform Construct(Player p, string message) {
            string[] args = message.SplitSpaces();
            if (args.Length < 3 || args.Length > 4) { Player.MessageLines(p, Help); return null; }
            float angleX, angleY, angleZ;
            RotateTransform rotater = new RotateTransform();
            
            if (!ParseAngle(p, args[0], out angleX)) return null;
            if (!ParseAngle(p, args[1], out angleY)) return null;
            if (!ParseAngle(p, args[2], out angleZ)) return null;
            rotater.SetAngles(angleX, angleY, angleZ);

            if (args.Length == 3) return rotater; // no centre argument
            if (!args[args.Length - 1].CaselessEq("centre")) {
                Player.Message(p, "The mode must be either \"centre\", or not given."); return null;
            }
            rotater.CentreOrigin = true;
            return rotater;
        }
        
        static bool ParseAngle(Player p, string input, out float angle) {
            if (!Utils.TryParseDecimal(input, out angle)) {
                Player.MessageLines(p, HelpString); return false;
            }            
            if (angle < -360 || angle > 360) { 
                Player.Message(p, "Angle must be between -360 and 360."); return false; 
            }
            return true;
        }
    }
}
