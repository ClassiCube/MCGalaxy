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
using MCGalaxy.Maths;
using MCGalaxy.Util;
using BlockID = System.UInt16;

namespace MCGalaxy.Drawing.Ops 
{
    public class ImagePrintDrawOp : DrawOp 
    {
        public override string Name { get { return "ImagePrint"; } }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) {
            return Source.Width * Source.Height;
        }
        
        internal IBitmap2D Source;
        internal bool DualLayer, LayerMode;
        public ImagePalette Palette;
        
        internal Vec3S32 dx, dy, adj;
        protected IPaletteMatcher selector;
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) {
            selector = new RgbPaletteMatcher();
            CalcLayerColors();

            try {
                Source.LockBits();
                OutputPixels(output);
            } finally {
                Source.UnlockBits();
            }
            selector = null;
            
            // Put all the blocks in shadow
            if (DualLayer) {
                ushort y = (ushort)Math.Min(Origin.Y + Source.Height, Level.Height-1);
                for (int i = 0; i < Source.Width; i++) {
                    ushort x = (ushort)(Origin.X + dx.X * i);
                    ushort z = (ushort)(Origin.Z + dx.Z * i);
                    output(Place(x, y, z, Block.Stone));
                    
                    x = (ushort)(x - adj.X); z = (ushort)(z - adj.Z);
                    output(Place(x, y, z, Block.Stone));
                }
            }
            
            Source.Dispose();
            Source = null;
            Player.Message("Finished printing image using {0} palette.", Palette.Name);
        }
        
        void CalcLayerColors() {
            PaletteEntry[] front = new PaletteEntry[Palette.Entries.Length];
            PaletteEntry[] back  = new PaletteEntry[Palette.Entries.Length];
            
            ColorDesc sun, dark, bright;
            if (!Colors.TryParseHex(Level.Config.LightColor, out sun)) {
                sun = Colors.ParseHex("FFFFFF");
            }
            if (!Colors.TryParseHex(Level.Config.ShadowColor, out dark)) {
                dark = Colors.ParseHex("9B9B9B");
            }
            bright = Colors.ParseHex("FFFFFF");
            
            for (int i = 0; i < Palette.Entries.Length; i++) {
                PaletteEntry entry = Palette.Entries[i];
                BlockDefinition def = Level.GetBlockDef(entry.Block);
                
                if (def != null && def.FullBright) {
                    front[i] = Multiply(entry, bright);
                    back[i]  = Multiply(entry, bright);
                } else {
                    front[i] = Multiply(entry,  sun);
                    back[i]  = Multiply(entry, dark);
                }
            }
            selector.SetPalette(front, back);
        }
        
        static PaletteEntry Multiply(PaletteEntry entry, ColorDesc rgb) {
            entry.R = (byte)(entry.R * rgb.R / 255);
            entry.G = (byte)(entry.G * rgb.G / 255);
            entry.B = (byte)(entry.B * rgb.B / 255);
            return entry;
        }
        
        protected virtual void OutputPixels(DrawOpOutput output) {
            int width = Source.Width, height = Source.Height;
            int srcY = height - 1; // need to flip coords in bitmap vertically
            
            for (int yy = 0; yy < height; yy++, srcY--)
                for (int xx = 0; xx < width; xx++) 
            {
                Pixel P = Source.Get(xx, srcY);
                ushort x = (ushort)(Origin.X + dx.X * xx + dy.X * yy);
                ushort y = (ushort)(Origin.Y + dx.Y * xx + dy.Y * yy);
                ushort z = (ushort)(Origin.Z + dx.Z * xx + dy.Z * yy);
                if (P.A < 20) { output(Place(x, y, z, Block.Air)); continue; }
                
                BlockID block;
                if (!DualLayer) {
                    block = selector.BestMatch(ref P);
                } else {
                    bool backLayer;
                    block = selector.BestMatch(P.R, P.G, P.B, out backLayer);                    
                    if (!backLayer) {
                        x = (ushort)(x - adj.X);
                        z = (ushort)(z - adj.Z);
                    }
                }
                output(Place(x, y, z, block));
            }
        }
        
        public void CalcState(Vec3S32[] m) {
            dx = default(Vec3S32); dy = default(Vec3S32); adj = default(Vec3S32);
            DualLayer = DualLayer && !LayerMode;
            
            int dir;
            if (Math.Abs(m[1].X - m[0].X) > Math.Abs(m[1].Z - m[0].Z)) {
                dir = m[1].X <= m[0].X ? 1 : 0;
            } else {
                dir = m[1].Z <= m[0].Z ? 3 : 2;
            }
            
            // TODO: Rewrite to use dirX/dirZ instead
            // Calculate back layer offset
            if (dir == 0) adj.Z = -1;
            if (dir == 1) adj.Z = +1;
            if (dir == 2) adj.X = +1;
            if (dir == 3) adj.X = -1;
            
            if (LayerMode) {
                if (dir == 0) { dx.X = +1; dy.Z = -1; }
                if (dir == 1) { dx.X = -1; dy.Z = +1; }
                if (dir == 2) { dx.Z = +1; dy.X = +1; }
                if (dir == 3) { dx.Z = -1; dy.X = -1; }
            } else {
                dy.Y = 1; // Oriented upwards
                if (dir == 0) dx.X = +1;
                if (dir == 1) dx.X = -1;
                if (dir == 2) dx.Z = +1;
                if (dir == 3) dx.Z = -1;
            }
        }
    }
    
    public class ImagePrintDitheredDrawOp : ImagePrintDrawOp
    {
        protected override void OutputPixels(DrawOpOutput output) {
            int width = Source.Width, height = Source.Height;
            int srcY = height - 1; // need to flip coords in bitmap vertically

             Vec3F32[,] errors = new Vec3F32[width, height];
            // Floyd steinberg dithering
            
            // TODO: just use two 'error rows' in memory, instead of entire image
            // error_cur, error_next
            // then after processing current row
            // i) swap error_cur and error_next
            // ii) clear error_next to 0
            
            for (int yy = 0; yy < height; yy++, srcY--)
            {
                for (int xx = 0; xx < width; xx++) 
                {
                    Pixel P  = Source.Get(xx, srcY);
                    ushort x = (ushort)(Origin.X + dx.X * xx + dy.X * yy);
                    ushort y = (ushort)(Origin.Y + dx.Y * xx + dy.Y * yy);
                    ushort z = (ushort)(Origin.Z + dx.Z * xx + dy.Z * yy);
                    if (P.A < 20) { output(Place(x, y, z, Block.Air)); continue; }
                	
                    Vec3F32 oldPixel = new Vec3F32(P.R, P.G, P.B) + errors[xx, yy];
                    // No Clamp for float?
                    if (oldPixel.X > 255) { oldPixel.X = 255; } if (oldPixel.X < 0) { oldPixel.X = 0; }
                    if (oldPixel.Y > 255) { oldPixel.Y = 255; } if (oldPixel.Y < 0) { oldPixel.Y = 0; }
                    if (oldPixel.Z > 255) { oldPixel.Z = 255; } if (oldPixel.Z < 0) { oldPixel.Z = 0; }      

                    P.R = (byte)oldPixel.X;
                    P.G = (byte)oldPixel.Y;
                    P.B = (byte)oldPixel.Z;
                    
                    BlockID block = selector.BestMatch(ref P);
                    output(Place(x, y, z, block));
                        
                    Vec3F32 newPixel = new Vec3F32(P.R, P.G, P.B);
                    errors[xx, yy]   = newPixel;
                    
                    Vec3F32 quantError = oldPixel - newPixel;
                    if (xx + 1 < width                  )  errors[xx + 1, yy    ] += (7.0f / 16.0f) * quantError;
                    if (xx - 1 > 0     && yy + 1 < height) errors[xx - 1, yy + 1] += (3.0f / 16.0f) * quantError;
                    if (yy + 1 < height                 )  errors[xx,     yy + 1] += (5.0f / 16.0f) * quantError;
                    if (xx + 1 < width && yy + 1 < height) errors[xx + 1, yy + 1] += (1.0f / 16.0f) * quantError;
                }
            }
        }
    }
}
