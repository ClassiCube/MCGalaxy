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
    
    public sealed class RotateTransform : Transform {
        
        public override string Name { get { return "Rotate"; } }
        public bool CentreOrigin;
        double cosX, cosY, cosZ, sinX, sinY, sinZ;
        Vec3S32 P;
        
        public void SetAngles(double xDeg, double yDeg, double zDeg) {
            cosX = Math.Cos(xDeg * Math.PI / 180.0);
            cosY = Math.Cos(yDeg * Math.PI / 180.0);
            cosZ = Math.Cos(zDeg * Math.PI / 180.0);
            
            sinX = Math.Sin(xDeg * Math.PI / 180.0);
            sinY = Math.Sin(yDeg * Math.PI / 180.0);
            sinZ = Math.Sin(zDeg * Math.PI / 180.0);            
        }
        
        public override void Perform(Vec3S32[] marks, DrawOp op, Brush brush, DrawOpOutput output) {
            P = (op.Min + op.Max) / 2;
            if (!CentreOrigin) P = op.Origin;
            op.Perform(marks, brush, b => OutputBlock(b, output));
        }
        
        void OutputBlock(DrawOpBlock b, DrawOpOutput output) {
            double dx = b.X - P.X, dy = b.Y - P.Y, dz = b.Z - P.Z;
            double rotX = 0, rotY = 0, rotZ = 0;
            
            // Rotate X
            rotY = cosX * dy - sinX * dz;
            rotZ = sinX * dy + cosX * dz;
            dy = rotY; dz = rotZ;
            
            // Rotate Y
            rotX = cosY * dx + sinY * dz;
            rotZ = -sinY * dx + cosY * dz;
            dx = rotX; dz = rotZ;
            
            // Rotate Z
            rotX = cosZ * dx - sinZ * dy;
            rotY = sinZ * dx + cosZ * dy;
            dx = rotX; dy = rotY;
            
            b.X = (ushort)(dx + P.X + ((dx % 1) >= 0.5 ? 1 : 0));
            b.Y = (ushort)(dy + P.Y + ((dy % 1) >= 0.5 ? 1 : 0));
            b.Z = (ushort)(dz + P.Z + ((dz % 1) >= 0.5 ? 1 : 0));
            output(b);
        }
    }
}
