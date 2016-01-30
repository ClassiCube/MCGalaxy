/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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

namespace MCGalaxy {

    public static class MapGen {
        
        public static bool IsRecognisedFormat(string s) {
            s = s.ToLower();
            return s == "flat" || s == "pixel" || s == "empty" || s == "hell" ||
                s == "island" || s == "mountains" || s == "ocean" || s == "forest" 
            	|| s == "desert" || s == "space" || s == "rainbow";
        }
        
        public static void PrintValidFormats(Player p) {
            Player.SendMessage(p, "Valid types: island, mountains, forest, ocean, " +
                               "flat, pixel, empty, desert, space, rainbow, and hell");
        }
		
		public static bool OkayAxis(int len) {
			return len == 16 || len == 32 || len == 64 || len == 128 || len == 256 ||
				len == 512 || len == 1024 || len == 2048 || len == 4096 || len == 8192;
		}
        
        public static void Generate(Level lvl, string type, int seed, bool useSeed) {
            int index = 0;
            int width = lvl.Width, height = lvl.Height, length = lvl.Length;
            byte[] blocks = lvl.blocks;
            int half = height / 2;
            RealisticMapGen generator = new RealisticMapGen();
            
            switch (type)
            {
                case "flat":
                    for (int y = 0; y <= half; ++y)
                        for (int z = 0; z < length; ++z)
                            for (int x = 0; x < width; ++x)
                    {
                        blocks[index++] = y < half ? Block.dirt : Block.grass;
                    } break;
                case "pixel":
                    for (int y = 0; y < height; ++y)
                        for (int z = 0; z < length; ++z)
                            for (int x = 0; x < width; ++x)
                    {
                        if (y == 0)
                            blocks[index] = Block.blackrock;
                        else if (x == 0 || x == width - 1 || z == 0 || z == length - 1)
                            blocks[index] = Block.white;
                        index++;
                    } break;
                case "empty":
                    for (int x = 0; x < width; ++x)
                        for (int z = 0; z < length; ++z)
                    {
                        blocks[index++] = Block.blackrock;
                    } break;
                case "space":
                    Random rand = useSeed ? new Random(seed) : new Random();
                    for (int y = 0; y < height; ++y)
                        for (int z = 0; z < length; ++z)
                            for (int x = 0; x < width; ++x)
                    {
                        if (y == 0)
                            blocks[index] = Block.blackrock;
                        else if (x == 0 || x == width - 1 || z == 0 || z == length - 1 || y == 1 || y == height - 1)
                            blocks[index] = rand.Next(100) == 0 ? Block.iron : Block.obsidian;
                        index++;
                    } break;
                case "rainbow":
                    Random random = useSeed ? new Random(seed) : new Random();
                    for (int y = 0; y < height; ++y)
                        for (int z = 0; z < length; ++z)
                            for (int x = 0; x < width; ++x)
                    {
                        if (y == 0 || y == height - 1 || x == 0 || x == width - 1 || z == 0 || z == length - 1)
                            blocks[index] = (byte)random.Next(Block.red, Block.white);
                        index++;
                    } break;
                case "hell":
                    Random random2 = useSeed ? new Random(seed) : new Random();
                    for (int y = 0; y < height; ++y)
                        for (int z = 0; z < length; ++z)
                            for (int x = 0; x < width; ++x)
                    {
                        if (y == 0) {
                            blocks[index] = Block.blackrock;
                        } else if (x == 0 || x == width - 1 || z == 0 || z == length - 1 || y == 0 || y == height - 1) {
                            blocks[index] = Block.obsidian;
                        } else if (x == 1 || x == width - 2 || z == 1 || z == length - 2) {
                            if (random2.Next(1000) != 7) {
                                index++; continue;
                            }
                            int colIndex = z * width + x;
                            for (int i = 1; i < (height - y); ++i) {
                                int yy = height - i;
                                blocks[colIndex + yy * width * length] = Block.lava;
                            }
                        }
                        index++;
                    }
                    generator.GenerateMap(lvl, type, seed, useSeed); break;
                case "island":
                case "mountains":
                case "ocean":
                case "forest":
                case "desert":
                    generator.GenerateMap(lvl, type, seed, useSeed); break;
            }
        }
    }
}
