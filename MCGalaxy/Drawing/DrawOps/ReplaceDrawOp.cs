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
using MCGalaxy.DB;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Maths;

namespace MCGalaxy.Drawing.Ops {
    
    public class ReplaceDrawOp : DrawOp {
        
        public ExtBlock Include;
        
        public ReplaceDrawOp(ExtBlock include) {
            Include = include;
            Flags = BlockDBFlags.Replaced;
        }
        
        public override string Name { get { return "Replace"; } }
        public override bool AffectedByTransform { get { return false; } }        
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            return (Max.X - Min.X + 1) * (Max.Y - Min.Y + 1) * (Max.Z - Min.Z + 1);
        }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            Vec3U16 p1 = Clamp(Min), p2 = Clamp(Max);
            for (ushort y = p1.Y; y <= p2.Y; y++)
                for (ushort z = p1.Z; z <= p2.Z; z++)
                    for (ushort x = p1.X; x <= p2.X; x++)
            {
                ExtBlock block = Level.GetBlock(x, y, z);
                if (block == Include) output(Place(x, y, z, brush));
            }
        }
    }
    
    public class ReplaceNotDrawOp : DrawOp {
        
        public ExtBlock Exclude;
        
        public ReplaceNotDrawOp(ExtBlock exclude) {
            Exclude = exclude;
            Flags = BlockDBFlags.Replaced;
        }
        
        public override string Name { get { return "ReplaceNot"; } }
        public override bool AffectedByTransform { get { return false; } }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            return (Max.X - Min.X + 1) * (Max.Y - Min.Y + 1) * (Max.Z - Min.Z + 1);
        }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            Vec3U16 p1 = Clamp(Min), p2 = Clamp(Max);
            for (ushort y = p1.Y; y <= p2.Y; y++)
                for (ushort z = p1.Z; z <= p2.Z; z++)
                    for (ushort x = p1.X; x <= p2.X; x++)
            {
                ExtBlock block = Level.GetBlock(x, y, z);
                if (block != Exclude) output(Place(x, y, z, brush));
            }
        }
    }
}