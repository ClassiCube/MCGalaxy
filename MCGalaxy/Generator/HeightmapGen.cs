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
using MCGalaxy.Network;
using MCGalaxy.Util;

namespace MCGalaxy.Generator 
{
    public static class HeightmapGen 
    {       
        static void OnDecodeError(Player p, IBitmap2D bmp) {
            if (bmp != null) bmp.Dispose();
            // TODO failed to decode the image. make sure you are using the URL of the image directly, not just the webpage it is hosted on              
            p.Message("&WThere was an error reading the downloaded image.");
            p.Message("&WThe url may need to end with its extension (such as .jpg).");
        }
        
        public static IBitmap2D DecodeImage(byte[] data, Player p) {
            IBitmap2D bmp = null;
            try {
                bmp = IBitmap2D.Create();
                bmp.Decode(data);
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
            IBitmap2D bmp = DecodeImage(data, p);
            if (bmp == null) return false;
            
            int index = 0, oneY = lvl.Width * lvl.Length;
            using (bmp) {
                if (lvl.Width != bmp.Width || lvl.Length != bmp.Height) {
                    p.Message("&cHeightmap size ({0}x{1}) does not match Width x Length ({2}x{3}) of the level",
                              bmp.Width, bmp.Height, lvl.Width, lvl.Length);
                    p.Message("&cAs such, the map may not look accurate.");
                    bmp.Resize(lvl.Width, lvl.Height, false);
                }
                bmp.LockBits();

                for (int z = 0; z < bmp.Height; z++)
                    for (int x = 0; x < bmp.Width; x++)
                {
                    int height = bmp.Get(x, z).R;
                    byte layer = Block.Dirt, top = Block.Grass;
                    
                    if (
                        IsCliff(height, bmp, x - 1, z) ||
                        IsCliff(height, bmp, x + 1, z) ||
                        IsCliff(height, bmp, x, z - 1) ||
                        IsCliff(height, bmp, x, z + 1))
                    {
                        layer = Block.Stone; top = Block.Stone;
                    }
                    
                    // remap from 0..255 to 0..lvl.Height
                    height = height * lvl.Height / 255;
                    for (int y = 0; y < height - 1; y++)
                        lvl.blocks[index + oneY * y] = layer;
                    if (height > 0)
                        lvl.blocks[index + oneY * (height - 1)] = top;
                    index++;
                }
            }
            return true;
        }
        
        static bool IsCliff(int height, IBitmap2D bmp, int x, int z) {
            if (x >= bmp.Width || x < 0 || z >= bmp.Height || z < 0) return false;
            int neighbourHeight = bmp.Get(x, z).R;
            return height >= neighbourHeight + 2;
        }
    }
}
