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
        Shear2D shearX, shearY, shearZ;
        Vec3S32 P;

        struct Shear2D {
            public int xMulX, xMulY, yMulX, yMulY;
            public double alpha, beta;
        };
        
        // https://silmon.github.io/arbitrary-image-rotation-using-shearing.html
        // http://delta.cs.cinvestav.mx/~mcintosh/newweb/camex/node37.html
        // https://www.ocf.berkeley.edu/~fricke/projects/israel/paeth/rotation_by_shearing.html
        public void SetAngles(double xDeg, double yDeg, double zDeg) {
            CalcShear2D(xDeg, ref shearX);
            CalcShear2D(yDeg, ref shearY);
            CalcShear2D(zDeg, ref shearZ);         
        }
        
        void CalcShear2D(double angle, ref Shear2D shear) {
            angle %= 360.0;
            if (angle < 0) angle += 360.0;

            // trying to use shear with angles close to 180 tends to cause issues
            //  (due to Math.Tan(angle / 2) approaching tan(45 degrees))
            // so avoid this by reducing the angles to 0-90 degrees,
            //  then rotating the output appropriately afterwards
            if (angle >= 0 && angle <= 90) {
                shear.xMulX =  1;
                shear.yMulY =  1;
            } else if (angle > 90 && angle <= 180) {
                angle -= 90;
                shear.xMulY =  1;
                shear.yMulX = -1;
            } else if (angle > 180 && angle <= 270) {
                angle -= 180;
                shear.xMulX = -1;
                shear.yMulY = -1;
            } else {
                angle -= 270;
                shear.xMulY = -1;
                shear.yMulX =  1;
            }

            // TODO angle %= 90; instead?? and integer angles instead???
            angle = -angle; // same output as old Rotate Transform

            angle *= Math.PI / 180.0; // degrees -> radians;
            shear.alpha = -Math.Tan(angle / 2);
            shear.beta  = Math.Sin(angle);
        }
        
        void DoShear2D(ref int x, ref int y, ref Shear2D shear) {
            int X_ = (int)(x  + shear.alpha * (y  + 0.5)); // shear #1
            int Y_ = (int)(y  + shear.beta  * (X_ + 0.5)); // shear #2
            X_     = (int)(X_ + shear.alpha * (Y_ + 0.5)); // shear #3

            // rotate by quadrant the angle was originally in
            x = shear.xMulX * X_ + shear.xMulY * Y_;
            y = shear.yMulX * X_ + shear.yMulY * Y_;
        }
        
        public override void Perform(Vec3S32[] marks, DrawOp op, Brush brush, DrawOpOutput output) {
            P = (op.Min + op.Max) / 2;
            if (!CentreOrigin) P = op.Origin;
            op.Perform(marks, brush, b => OutputBlock(b, output));
        }
        
        void OutputBlock(DrawOpBlock b, DrawOpOutput output) {
            int dx = b.X - P.X, dy = b.Y - P.Y, dz = b.Z - P.Z;
            
            DoShear2D(ref dy, ref dz, ref shearX);
            DoShear2D(ref dx, ref dz, ref shearY);
            DoShear2D(ref dx, ref dy, ref shearZ);
            
            b.X = (ushort)(P.X + dx);
            b.Y = (ushort)(P.Y + dy);
            b.Z = (ushort)(P.Z + dz);
            output(b);
        }
    }
}