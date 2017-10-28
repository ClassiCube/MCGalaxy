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
using System.Drawing;
using System.Drawing.Imaging;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Drawing {
    
    public delegate Pixel PixelGet(int x, int y);
    public struct Pixel { public byte A, R, G, B; }
    
    public unsafe sealed class PixelGetter : IDisposable {
        
        Bitmap bmp;
        BitmapData data;
        byte* scan0;
        int stride;
        public PixelGet Get;
        public readonly int Width, Height;
        
        public PixelGetter(Bitmap bmp) { 
            this.bmp = bmp; 
            Width = bmp.Width; Height = bmp.Height;
        }
        
        public void Init() {
            bool fastPath = bmp.PixelFormat == PixelFormat.Format32bppRgb
                         || bmp.PixelFormat == PixelFormat.Format32bppArgb
                         || bmp.PixelFormat == PixelFormat.Format24bppRgb;
            if (!fastPath) { Get = GetGenericPixel; return; }
            // We can only use the fast path for 24bpp or 32bpp bitmaps
            
            Rectangle r = new Rectangle(0, 0, bmp.Width, bmp.Height);
            data = bmp.LockBits(r, ImageLockMode.ReadOnly, bmp.PixelFormat);
            scan0 = (byte*)data.Scan0;
            stride = data.Stride;
            
            if (bmp.PixelFormat == PixelFormat.Format24bppRgb) {
                Get = Get24BppPixel;
            } else {
                Get = Get32BppPixel;
            }
        }
        
        Pixel GetGenericPixel(int x, int y) {
            Pixel pixel;
            int argb = bmp.GetPixel(x, y).ToArgb(); // R/G/B properties incur overhead            
            pixel.A = (byte)(argb >> 24);
            pixel.R = (byte)(argb >> 16);
            pixel.G = (byte)(argb >> 8);
            pixel.B = (byte)argb;
            return pixel;
        }
        
        Pixel Get24BppPixel(int x, int y) {
            Pixel pixel;
            byte* ptr = (scan0 + y * stride) + (x * 3);
            pixel.B = ptr[0]; pixel.G = ptr[1]; pixel.R = ptr[2]; pixel.A = 255;
            return pixel;
        }
        
        Pixel Get32BppPixel(int x, int y) {
            Pixel pixel;
            byte* ptr = (scan0 + y * stride) + (x * 4);            
            pixel.B = ptr[0]; pixel.G = ptr[1]; pixel.R = ptr[2]; pixel.A = ptr[3];
            return pixel;
        }

        public void Dispose() {
            if (data != null) bmp.UnlockBits(data);
            data = null;
            bmp = null;
        }
    }
}
