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
    public static class ImageUtils
    {
        public static Bitmap2D DecodeImage(byte[] data, Player p) {
            try {
                return ImageDecoder.DecodeFrom(data);
            } catch (UnknownImageFormatException ex) {
                // This is a fairly expected error - e.g. when a user tries to /imgprint
                //   the webpage an image is hosted on, instead of the actual image itself.
                // So don't bother logging a full error for this case
                Logger.Log(LogType.Warning, "Error decoding image: " + ex.Message);
                OnDecodeError(p);
                return null;
            } catch (Exception ex) {
                Logger.LogError("Error decoding image", ex);
                OnDecodeError(p);
                return null;
            }
        }

        static void OnDecodeError(Player p) {
            // TODO failed to decode the image. make sure you are using the URL of the image directly, not just the webpage it is hosted on
            p.Message("&WThere was an error reading the downloaded image.");
            p.Message("&WThe url may need to end with its extension (such as .jpg).");
        }
        
        
        public static Bitmap2D ResizeBilinear(Bitmap2D src, int dstWidth, int dstHeight) {
            Bitmap2D dst = new Bitmap2D();
            dst.Width  = dstWidth;
            dst.Height = dstHeight;
            dst.AllocatePixels();
            
            float scaleX = (float)src.Width  / dstWidth;
            float scaleY = (float)src.Height / dstHeight;
            
            int maxX = src.Width  - 1;
            int maxY = src.Height - 1;
            
            Pixel[] pixels = src.Pixels;
            int stride = src.Width;
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
                    dst.Pixels[dstI++] = p;
                }
            }
            return dst;
        }
    }
}