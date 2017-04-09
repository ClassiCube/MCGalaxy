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
using System.IO;
using System.Net;

namespace MCGalaxy.Generator {
    public static class HeightmapGen {
        
        public static bool DownloadImage(string url, string dir, Player p) {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (!url.CaselessStarts("http://") && !url.CaselessStarts("https://"))
                url = "http://" + url;
            
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri)) {
                Player.Message(p, "{0} is not a valid URL.", url); return false;
            }
            
            try {
                using (WebClient client = new WebClient()) {
                    Player.Message(p, "Downloading file from: &f" + url);
                    client.DownloadFile(uri, dir + "tempImage_" + p.name + ".bmp");
                }
                Player.Message(p, "Finished downloading image.");
                return true;
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                Player.Message(p, "&cFailed to download the image from the given url.");
                Player.Message(p, "&cThe url may need to end with its extension (such as .jpg).");
                return false;
            }
        }
        
        public static Bitmap ReadBitmap(string name, string dir, Player p) {
            Bitmap bmp = null;
            try {
                bmp = new Bitmap(dir + name + ".bmp");
                int width = bmp.Width;
                // sometimes Mono will return an invalid bitmap instance that throws ArgumentNullException,
                // so we make sure to check for that here rather than later.
                return bmp;
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                if (bmp != null) bmp.Dispose();
                Player.Message(p, "&cThere was an error reading the downloaded image.");
                Player.Message(p, "&cThe url may need to end with its extension (such as .jpg).");
                return null;
            }
        }
        
        public static bool Generate(MapGenArgs args) {
            Player p = args.Player;
            Level lvl = args.Level;
            if (args.Args == "") { Player.Message(p, "You need to provide a url for the image."); return false; }
            
            if (!DownloadImage(args.Args, "extra/heightmap/", p )) return false;
            Bitmap bmp = ReadBitmap("tempImage_" + p.name, "extra/heightmap/", p);
            if (bmp == null) return false;
            int index = 0, oneY = lvl.Width * lvl.Length;
            
            using (bmp) {
                if (lvl.Width != bmp.Width || lvl.Length != bmp.Height) {
                    Player.Message(p, "The size of the heightmap is {0} by {1}.", bmp.Width, bmp.Height);
                    Player.Message(p, "The width and length of the new level must match that size.");
                    return false;
                }
                
                for (int z = 0; z < bmp.Height; z++)
                    for (int x = 0; x < bmp.Width; x++)
                {
                    int height = bmp.GetPixel(x, z).R;     
                    byte layer = Block.dirt, top = Block.grass;
                    
                    if (
                        IsShorterBy(height, bmp, x - 1, z) ||
                        IsShorterBy(height, bmp, x + 1, z) ||
                        IsShorterBy(height, bmp, x, z - 1) ||
                        IsShorterBy(height, bmp, x, z + 1)) 
                    {
                        layer = Block.rock; top = Block.rock;
                    }
                    
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
        
        static bool IsShorterBy(int height, Bitmap bmp, int x, int z) {
            if (x >= bmp.Width || x < 0 || z >= bmp.Height || z < 0) return false;            
            int neighbourHeight = bmp.GetPixel(x, z).R;
            return height >= neighbourHeight + 2;
        }
    }
}
