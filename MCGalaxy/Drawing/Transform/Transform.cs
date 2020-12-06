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
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;

namespace MCGalaxy.Drawing.Transforms {
    public abstract class Transform {
        public abstract string Name { get; }
        public virtual void Configure(DrawOp op, Player p) { }
        
        /// <summary> Estimates the total number of blocks that the drawing commands affects,
        /// after this transformation (e.g. scaling) has been applied to it. </summary>
        public virtual void GetBlocksAffected(ref long affected) { }
        
        public abstract void Perform(Vec3S32[] marks, DrawOp op, Brush brush, DrawOpOutput output);
    }
}
