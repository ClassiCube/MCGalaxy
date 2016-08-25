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
using MCGalaxy.Drawing.Brushes;

namespace MCGalaxy.Drawing.Transforms {
    public sealed class NoTransformFactory : TransformFactory {        
        public override string Name { get { return "None"; } }
        public override string[] Help { get { return HelpString; } }
        
        static string[] HelpString = new [] {
            "%TArguments: none",
            "%HDoes not affect the output of draw operations.",
        };
        
        public override Transform Construct(BrushArgs args) {
            return new NoTransform();
        }
        
        public override bool Validate(BrushArgs args) { return true; }
    }
}
