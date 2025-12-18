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
using MCGalaxy.Network;
using MCGalaxy.Util;
using MCGalaxy.Util.Imaging;

namespace MCGalaxy.Generator
{
    public static class HeightmapGen
    {
        public static bool Generate(Player p, Level lvl, MapGenArgs args) {
            string url = null;
            int argIdx = 0;
            
            // only parse first argument
            args.ArgFilter = (arg) => argIdx++ == 0;
            args.ArgParser = (arg) => { url = arg; return true; };
            
            if (!args.ParseArgs(p)) return false;
            if (url == null) { p.Message("You need to provide a url for the image."); return false; }
            MapGenBiome biome = MapGenBiome.Get(args.Biome);
            
            byte[] data = HttpUtil.DownloadImage(url, p);
            if (data == null) return false;
            
            Bitmap2D bmp = ImageUtils.DecodeImage(data, p);
            if (bmp == null) return false;
            
            int index = 0, oneY = lvl.Width * lvl.Length;
            int lvlWidth  = lvl.Width;
            int lvlLength = lvl.Length;
            
            bool resized = bmp.Width != lvlWidth || bmp.Height != lvlLength;
            
            if (resized) {
                p.Message("&cHeightmap size ({0}x{1}) does not match Width x Length ({2}x{3}) of the level",
                          bmp.Width, bmp.Height, lvlWidth, lvlLength);
                p.Message("&cAs such, the map may not look accurate.");
            }
            
            byte[] hmap = resized ? ResizeHeightmap(bmp, lvlWidth, lvlLength)
                : ComputeHeightmap(bmp);

            for (int z = 0; z < lvlLength; z++)
                for (int x = 0; x < lvlWidth; x++)
            {
                int height = hmap[index];
                byte layer = biome.Ground, top = biome.Surface;
                
                if (
                    IsCliff(height, hmap, lvl, x - 1, z) ||
                    IsCliff(height, hmap, lvl, x + 1, z) ||
                    IsCliff(height, hmap, lvl, x, z - 1) ||
                    IsCliff(height, hmap, lvl, x, z + 1))
                {
                    layer = biome.Cliff; top = biome.Cliff;
                }
                
                // remap from 0..255 to 0..lvl.Height
                height = height * lvl.Height / 255;
                for (int y = 0; y < height - 1; y++)
                    lvl.blocks[index + oneY * y] = layer;
                if (height > 0)
                    lvl.blocks[index + oneY * (height - 1)] = top;
                index++;
            }
            return true;
        }
        
        static bool IsCliff(int height, byte[] hmap, Level lvl, int x, int z) {
            if (x >= lvl.Width || x < 0 || z >= lvl.Length || z < 0) return false;
            
            int neighbourHeight = hmap[z * lvl.Width + x];
            return height >= neighbourHeight + 2;
        }
        
        static byte[] ComputeHeightmap(Bitmap2D bmp) {
            byte[] hmap = new byte[bmp.Width * bmp.Height];
            int i = 0;
            
            for (int y = 0; y < bmp.Height; y++)
                for (int x = 0; x < bmp.Width; x++)
            {
                hmap[i] = bmp.Pixels[i].R;
                i++;
            }
            return hmap;
        }
        
        // Calculates adjusted X/Y coordinates using nearest neighbour resizing
        static byte[] ResizeHeightmap(Bitmap2D bmp, int dstWidth, int dstHeight) {
            byte[] hmap = new byte[dstWidth * dstHeight];
            int i = 0;
            
            for (int y = 0; y < dstHeight; y++)
            {
                int srcY = y * bmp.Height / dstHeight;
                for (int x = 0; x < dstWidth; x++)
                {
                    int srcX  = x * bmp.Width / dstWidth;
                    hmap[i++] = bmp.Get(srcX, srcY).R;
                }
            }
            return hmap;
        }
    }
}
