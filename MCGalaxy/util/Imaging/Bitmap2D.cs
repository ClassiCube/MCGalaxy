/*
    Copyright 2015-2024 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.IO;

namespace MCGalaxy.Util.Imaging
{
    public delegate Pixel PixelGet(int x, int y);
    public delegate void  PixelSet(int x, int y, Pixel pixel);
    
    public struct Pixel
    {
        public byte A, R, G, B;
        
        public Pixel(byte r, byte g, byte b, byte a) {
            R = r; G = g; B = b; A = a;
        }
        
        public static Pixel BLACK = new Pixel(0, 0, 0, 255);
        
        public override string ToString() {
            return string.Format("R={0}, G={1}, B={2}, A={3}", R, G, B, A);
        }
    }

    /// <summary> Represents a 2D image </summary>
    public sealed class Bitmap2D
    {
        public int Width, Height;
        public Pixel[] Pixels;
        
        public void AllocatePixels() {
            Pixels = new Pixel[Width * Height];
        }
        
        public Pixel Get(int x, int y) {
            return Pixels[y * Width + x];
        }
    }
}
