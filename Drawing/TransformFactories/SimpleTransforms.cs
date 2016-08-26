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
    public sealed class NoTransformFactory : TransformFactory {        
        public override string Name { get { return "None"; } }
        public override string[] Help { get { return HelpString; } }
        
        static string[] HelpString = new [] {
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
        
        static string[] HelpString = new [] {
            "%TArguments: [scaleX] [scaleY] [scaleZ] <centre>",
            "%TAlternatively: [scale] <centre>",            
            "%H[scale] values can be either an integer or a fraction (e.g. 2 or 1/2).",
            "%H[centre] if given, indicates to scale from the centre of a draw operation, " +
            "instead of outwards from the first mark. Recommended for cuboid and cylinder.",
        };
        
        public override Transform Construct(Player p, string message) {
// TODO: actually parse the arguments        	
        	return new ScaleTransform() { XMul = 2, XDiv = 2, YMul = 1, YDiv = 2, ZMul = 2, ZDiv = 1 };
        }
    }
}
