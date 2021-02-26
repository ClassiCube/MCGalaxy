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
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using MCGalaxy.Network;
using MCGalaxy.Drawing;

namespace MCGalaxy.Generator {
    public static class HeightmapGen {
        
        static void OnDecodeError(Player p, Bitmap bmp) {
            if (bmp != null) bmp.Dispose();
            // TODO failed to decode the image. make sure you are using the URL of the image directly, not just the webpage it is hosted on              
            p.Message("&WThere was an error reading the downloaded image.");
            p.Message("&WThe url may need to end with its extension (such as .jpg).");
        }
        
        public static Bitmap DecodeImage(byte[] data, Player p) {
            Bitmap bmp = null;
            try {
                bmp = new Bitmap(new MemoryStream(data));
                int width = bmp.Width;
                // sometimes Mono will return an invalid bitmap instance that throws ArgumentNullException,
                // so we make sure to check for that here rather than later.
                return bmp;
            } catch (ArgumentException ex) {
                // GDI+ throws ArgumentException when data is not an image
                // This is a fairly expected error - e.g. when a user tries to /imgprint
                //   the webpage an image is hosted on, instead of the actual image itself. 
                // So don't bother logging a full error for this case
                Logger.Log(LogType.Warning, "Error decoding image: " + ex.Message);
                OnDecodeError(p, bmp);
                return null;
            } catch (Exception ex) {
                Logger.LogError("Error decoding image", ex);
                OnDecodeError(p, bmp);
                return null;
            }
        }
        
        public static bool Generate(Player p, Level lvl, string url) {
            if (url.Length == 0) { p.Message("You need to provide a url for the image."); return false; }
            
            byte[] data = HttpUtil.DownloadImage(url, p);
            if (data == null) return false;
            Bitmap bmp = DecodeImage(data, p);
            if (bmp == null) return false;
            
            int index = 0, oneY = lvl.Width * lvl.Length;
            try {
                if (lvl.Width != bmp.Width || lvl.Length != bmp.Height) {
                    p.Message("&cHeightmap size ({0}x{1}) does not match Width x Length ({2}x{3}) of the level",
                              bmp.Width, bmp.Height, lvl.Width, lvl.Length);
                    p.Message("&cAs such, the map may not look accurate.");
                    bmp = Resize(bmp, lvl.Width, lvl.Length);
                }
                
                using (PixelGetter pixels = new PixelGetter(bmp)) {
                    pixels.Init();
                    for (int z = 0; z < pixels.Height; z++)
                        for (int x = 0; x < pixels.Width; x++)
                    {
                        int height = pixels.Get(x, z).R;
                        byte layer = Block.Dirt, top = Block.Grass;
                        
                        if (
                            IsShorterBy(height, pixels, x - 1, z) ||
                            IsShorterBy(height, pixels, x + 1, z) ||
                            IsShorterBy(height, pixels, x, z - 1) ||
                            IsShorterBy(height, pixels, x, z + 1))
                        {
                            layer = Block.Stone; top = Block.Stone;
                        }
                        
                        height = height * lvl.Height / 255;
                        for (int y = 0; y < height - 1; y++)
                            lvl.blocks[index + oneY * y] = layer;
                        if (height > 0)
                            lvl.blocks[index + oneY * (height - 1)] = top;
                        index++;
                    }
                }
                // Cannot use using { } here because bmp may be reassigned
            } finally { bmp.Dispose(); }
            return true;
        }
        
        static Bitmap Resize(Bitmap bmp, int width, int height) {
            Bitmap resized = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(resized)) {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.SmoothingMode     = SmoothingMode.None;
                g.PixelOffsetMode   = PixelOffsetMode.None;
                g.DrawImage(bmp, 0, 0, width, height);
            }
            
            bmp.Dispose();
            return resized;
        }
        
        static bool IsShorterBy(int height, PixelGetter pixels, int x, int z) {
            if (x >= pixels.Width || x < 0 || z >= pixels.Height || z < 0) return false;
            int neighbourHeight = pixels.Get(x, z).R;
            return height >= neighbourHeight + 2;
        }
    }
}
