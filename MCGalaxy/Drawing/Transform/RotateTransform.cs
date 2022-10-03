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

namespace MCGalaxy.Drawing.Transforms 
{
    public sealed class RotateTransform : Transform 
    {        
        public override string Name { get { return "Rotate"; } }
        public bool CentreOrigin;
        double alphaX, betaX, alphaY, betaY, alphaZ, betaZ;
        Vec3S32 P;
        
        // https://silmon.github.io/arbitrary-image-rotation-using-shearing.html
        // http://delta.cs.cinvestav.mx/~mcintosh/newweb/camex/node37.html
        // https://www.ocf.berkeley.edu/~fricke/projects/israel/paeth/rotation_by_shearing.html
        
        public void SetAngles(double xDeg, double yDeg, double zDeg) {
            CalcShear2D(xDeg, out alphaX, out betaX);
            CalcShear2D(yDeg, out alphaY, out betaY);
            CalcShear2D(zDeg, out alphaZ, out betaZ);         
        }
        
        void CalcShear2D(double angle, out double alpha, out double beta) {
            angle *= -Math.PI / 180.0; // degrees -> radians, and - so same direction as old RotateTransform
            alpha = -Math.Tan(angle / 2);
            beta  = Math.Sin(angle);
        }
        
        void Shear2D(ref int x, ref int y, double alpha, double beta) {
            int x_ = (int)(x  + alpha * (y  + 0.5)); // shear #1
            int y_ = (int)(y  + beta  * (x_ + 0.5)); // shear #2
            int X_ = (int)(x_ + alpha * (y_ + 0.5)); // shear #3
            
            x = X_; y = y_;
        }
        
        public override void Perform(Vec3S32[] marks, DrawOp op, Brush brush, DrawOpOutput output) {
            P = (op.Min + op.Max) / 2;
            if (!CentreOrigin) P = op.Origin;
            op.Perform(marks, brush, b => OutputBlock(b, output));
        }
        
        void OutputBlock(DrawOpBlock b, DrawOpOutput output) {
            int dx = b.X - P.X, dy = b.Y - P.Y, dz = b.Z - P.Z;
            
            Shear2D(ref dy, ref dz, alphaX, betaX);
            Shear2D(ref dx, ref dz, alphaY, betaY);
            Shear2D(ref dx, ref dy, alphaZ, betaZ);
            
            b.X = (ushort)(P.X + dx);
            b.Y = (ushort)(P.Y + dy);
            b.Z = (ushort)(P.Z + dz);
            output(b);
        }
    }
}
