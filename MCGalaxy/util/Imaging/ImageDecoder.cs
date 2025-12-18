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
using System.IO.Compression;
using MCGalaxy.Util;

namespace MCGalaxy.Util.Imaging
{
    public sealed class SimpleBitmap //: IBitmap2D
    {
        public int Width, Height;
        public Pixel[] pixels;
        
        public void AllocatePixels() {
            pixels = new Pixel[Width * Height];
        }
        
        public SimpleBitmap ResizeBilinear(int dstWidth, int dstHeight) {
            SimpleBitmap dst = new SimpleBitmap();
            dst.Width  = dstWidth;
            dst.Height = dstHeight;
            dst.AllocatePixels();
            
            float scaleX = (float)Width  / dstWidth;
            float scaleY = (float)Height / dstHeight;
            
            int maxX = Width  - 1;
            int maxY = Height - 1;
            
            Pixel[] pixels = this.pixels;
            int stride = this.Width;
            int dstI = 0;
            
            for (int y = 0; y < dstHeight; y++)
            {
                float sy = (y + 0.5f) * scaleY;
                int  sy0 = (int)sy; if (sy0 >= maxY) sy0 = maxY;
                int  sy1 = sy0 + 1; if (sy1 >= maxY) sy1 = maxY;
                float dy = sy - sy0;
                
                for (int x = 0; x < dstWidth; x++)
                {
                    float sx = (x + 0.5f) * scaleX;
                    int  sx0 = (int)sx; if (sx0 >= maxX) sx0 = maxX;
                    int  sx1 = sx0 + 1; if (sx1 >= maxX) sx1 = maxX;
                    float dx = sx - sx0;
                    
                    Pixel p00 = pixels[sy0 * stride + sx0];
                    Pixel p10 = pixels[sy0 * stride + sx1];
                    Pixel p01 = pixels[sy1 * stride + sx0];
                    Pixel p11 = pixels[sy1 * stride + sx1];
                    
                    Pixel p;
                    p.R = (byte)((p00.R * (1-dx) * (1-dy)) + (p10.R * dx * (1-dy)) + (p01.R * (1-dx) * dy) + (p11.R * dx * dy));
                    p.G = (byte)((p00.G * (1-dx) * (1-dy)) + (p10.G * dx * (1-dy)) + (p01.G * (1-dx) * dy) + (p11.G * dx * dy));
                    p.B = (byte)((p00.B * (1-dx) * (1-dy)) + (p10.B * dx * (1-dy)) + (p01.B * (1-dx) * dy) + (p11.B * dx * dy));
                    p.A = (byte)((p00.A * (1-dx) * (1-dy)) + (p10.A * dx * (1-dy)) + (p01.A * (1-dx) * dy) + (p11.A * dx * dy));
                    dst.pixels[dstI++] = p;
                }
            }
            return dst;
        }
    }
    
    public abstract class ImageDecoder
    {
        protected byte[] buf_data;
        protected int buf_offset, buf_length;
        
        /// <summary> Attempts to advance next read offset by 'amount', then returns current read offset </summary>
        protected int AdvanceOffset(int amount) {
            int offset = buf_offset;
            
            buf_offset += amount;
            if (buf_offset > buf_length)
                throw new EndOfStreamException("End of stream reading data");
            return offset;
        }
        
        protected void SetBuffer(byte[] src) {
            buf_data   = src;
            buf_offset = 0;
            buf_length = src.Length;
        }
        
        
        protected static void Fail(string reason) {
            throw new InvalidDataException(reason);
        }
        
        /// <summary> Checks if starting bytes of data match given signature </summary>
        /// <remarks> Ignores parts of sig that are &lt; 0 values </remarks>
        protected static bool MatchesSignature(byte[] data, byte[] sig) {
            if (data.Length < sig.Length) return false;
            
            for (int i = 0; i < sig.Length; i++)
            {
                if (data[i] != sig[i]) return false;
            }
            return true;
        }
        
        
        public static SimpleBitmap DecodeFrom(byte[] src) {
            ImageDecoder decoder = DetectFrom(src);
            if (decoder != null) return decoder.Decode(src);
            
            throw new InvalidDataException("Unsupported or invalid image format");
        }
        
        static ImageDecoder DetectFrom(byte[] src) {
            if ( PngDecoder.DetectHeader(src)) return new PngDecoder();
            if ( GifDecoder.DetectHeader(src)) return new GifDecoder();
            if (JpegDecoder.DetectHeader(src)) return new JpegDecoder();
            
            return null;
        }
        
        public abstract SimpleBitmap Decode(byte[] src);
    }
}