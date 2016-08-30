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
using System.IO;
using Draw = System.Drawing;
using MCGalaxy.Drawing.Brushes;

namespace MCGalaxy.Drawing.Ops {

    public class ImagePrintDrawOp : DrawOp {
        public override string Name { get { return "ImagePrint"; } }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            return Source.Width * Source.Height;
        }
        
        internal Draw.Bitmap Source;
        internal int Mode, Direction;
        internal bool Layer;
        internal string Filename;
        
        public override void Perform(Vec3S32[] marks, Player p, Level lvl, Brush brush, Action<DrawOpBlock> output) {
            Vec3U16 p0 = Clamp(marks[0]);
            PaletteEntry[] palette = ImagePalette.GetPalette(Mode);
            PaletteEntry cur = default(PaletteEntry);
            
            IPalette selector = null;
            if (Mode == 6) selector = new GrayscalePalette();
            else selector = new RgbPalette();
            selector.SetAvailableBlocks(palette);
            
            Vec3S32 dx, dy, adj;
            CalcMul(Layer, Direction, out dx, out dy, out adj);

            for (int yy = 0; yy < Source.Height; yy++)
                for (int xx = 0; xx < Source.Width; xx++)
            {
                ushort X = (ushort)(p0.X + dx.X * xx + dy.X * yy);
                ushort Y = (ushort)(p0.Y + dx.Y * xx + dy.Y * yy);
                ushort Z = (ushort)(p0.Z + dx.Z * xx + dy.Z * yy);

                Draw.Color col = Source.GetPixel(xx, yy);
                cur.R = col.R; cur.G = col.G; cur.B = col.B;
                int position;
                cur.Block = selector.BestMatch(cur, out position);
                if (Mode == 1 || Mode == 3) {
                    int threshold = Mode == 1 ? 20 : 3;
                    // Back layer block
                    if (position <= threshold) {
                        X = (ushort)(X + adj.X);
                        Z = (ushort)(Z + adj.Z);
                    }
                }

                if (col.A < 20) cur.Block = Block.air;
                output(Place(X, Y, Z, cur.Block, 0));
            }
            
            Source.Dispose();
            Source = null;
            if (Filename == "tempImage_" + p.name)
                File.Delete("extra/images/tempImage_" + p.name + ".bmp");
            Player.Message(p, "Finished printing image using " + ImagePalette.Names[Mode]);
        }
        
        void CalcMul(bool layer, int dir,
                     out Vec3S32 signX, out Vec3S32 signY, out Vec3S32 adj) {
            signX = default(Vec3S32); signY = default(Vec3S32); adj = default(Vec3S32);
            
            // Calculate back layer offset
            if (dir == 0) adj.Z = 1;
            if (dir == 1) adj.Z = -1;
            if (dir == 2) adj.X = -1;
            if (dir == 3) adj.X = 1;
            
            if (layer) {
                if (dir == 0) { signX.X = 1; signY.Z = -1; }
                if (dir == 1) { signX.X = -1; signY.Z = 1; }
                if (dir == 2) { signX.Z = 1; signY.X = 1; }
                if (dir == 3) { signX.Z = -1; signY.X = -1; }
            } else {
                signY.Y = 1; // Oriented upwards
                if (dir == 0) signX.X = 1;
                if (dir == 1) signX.X = -1;
                if (dir == 2) signX.Z = 1;
                if (dir == 3) signX.Z = -1;
            }
        }
    }
}
