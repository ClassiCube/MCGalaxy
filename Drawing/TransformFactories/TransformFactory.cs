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
using MCGalaxy.Commands;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Drawing.Transforms {
    public abstract class TransformFactory {
        
        /// <summary> Human friendly name of this transform. </summary>
        public abstract string Name { get; }
        
        /// <summary> Description of the transform, in addition to its syntax. </summary>
        public abstract string[] Help { get; }
        
        /// <summary> Creates a transform from the given arguments, 
        /// returning null if invalid arguments are specified. </summary>
        public abstract Transform Construct(BrushArgs args);
        
        /// <summary> Validates the given arguments, returning false if they are invalid. </summary>
        public virtual bool Validate(BrushArgs args) { return Construct(args) != null; }
        
        public static List<TransformFactory> Transforms = new List<TransformFactory>() {
            new NoTransformFactory(),
        };
        
        public static string Available { get { return Transforms.Join(b => b.Name); } }
        
        public static TransformFactory Find(string name) {
            foreach (TransformFactory entry in Transforms) {
                if (entry.Name.CaselessEq(name)) return entry;
            }
            return null;
        }
    }
}
