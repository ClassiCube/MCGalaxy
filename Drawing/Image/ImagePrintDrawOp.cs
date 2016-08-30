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
        Vec3S32 dx, dy, adj;
        
        public override void Perform(Vec3S32[] marks, Player p, Level lvl, Brush brush, Action<DrawOpBlock> output) {
            Vec3U16 p0 = Clamp(marks[0]);
            PaletteEntry[] palette = ImagePalette.GetPalette(Mode);
            IPalette selector = null;
            if (Mode == 6) selector = new GrayscalePalette();
            else selector = new RgbPalette();
            selector.SetAvailableBlocks(palette);
            CalcDirectionVectors(Direction);

            using (PixelGetter getter = new PixelGetter(Source)) {
                getter.Init();
                getter.Iterate(P => OutputPixel(P, selector, output));
            }
            
            Source.Dispose();
            Source = null;
            if (Filename == "tempImage_" + p.name)
                File.Delete("extra/images/tempImage_" + p.name + ".bmp");
            Player.Message(p, "Finished printing image using " + ImagePalette.Names[Mode]);
        }
        
        void OutputPixel(Pixel P, IPalette selector, Action<DrawOpBlock> output) {
            ushort x = (ushort)(Origin.X + dx.X * P.X + dy.X * P.Y);
            ushort y = (ushort)(Origin.Y + dx.Y * P.X + dy.Y * P.Y);
            ushort z = (ushort)(Origin.Z + dx.Z * P.X + dy.Z * P.Y);
            if (P.A < 20) { output(Place(x, y, z, Block.air, 0)); return; }
            
            int position;
            byte block = selector.BestMatch(P.R, P.G, P.B, out position);
            if (Mode == 1 || Mode == 3) {
                int threshold = Mode == 1 ? 20 : 3;
                // Back layer block
                if (position <= threshold) {
                    x = (ushort)(x + adj.X);
                    z = (ushort)(z + adj.Z);
                }
            }
            output(Place(x, y, z, block, 0));
        }
        
        void CalcDirectionVectors(int dir) {
            dx = default(Vec3S32); dy = default(Vec3S32); adj = default(Vec3S32);
            
            // Calculate back layer offset
            if (dir == 0) adj.Z = 1;
            if (dir == 1) adj.Z = -1;
            if (dir == 2) adj.X = -1;
            if (dir == 3) adj.X = 1;
            
            if (Layer) {
                if (dir == 0) { dx.X = 1; dy.Z = -1; }
                if (dir == 1) { dx.X = -1; dy.Z = 1; }
                if (dir == 2) { dx.Z = 1; dy.X = 1; }
                if (dir == 3) { dx.Z = -1; dy.X = -1; }
            } else {
                dy.Y = 1; // Oriented upwards
                if (dir == 0) dx.X = 1;
                if (dir == 1) dx.X = -1;
                if (dir == 2) dx.Z = 1;
                if (dir == 3) dx.Z = -1;
            }
        }
    }
}
