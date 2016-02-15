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
using LibNoise;

namespace MCGalaxy {

    public static class MapGen {
        
        public static bool IsRecognisedFormat(string s) {
            s = s.ToLower();
            return s == "flat" || s == "pixel" || s == "empty" || s == "hell" ||
                s == "island" || s == "mountains" || s == "ocean" || s == "forest"
                || s == "desert" || s == "space" || s == "rainbow" || s == "billow"
                || s == "perlin" || s == "checkerboard" || s == "spheres" ||
                s == "cylinders" || s == "voronoi" || s == "ridgedmultifractal"
                || s == "billow3d" || s == "perlin3d" || s == "perlin3dyadjust";
        }
        
        public static void PrintValidFormats(Player p) {
            Player.SendMessage(p, "Valid types: island, mountains, forest, ocean, " +
                               "flat, pixel, empty, desert, space, rainbow, hell, billow, perlin,checkerboard,spheres,cylinders,voronoi,ridgedmultifractal,billow3d,perlin3d,perlin3dyadjust");
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
                    Server.MapGen.GenerateMap(lvl, type, seed); break;
 +                case "billow":
+                    Billow BillowNoise = new Billow();
+                    BillowNoise.Seed = useSeed ? BillowNoise.Seed = seed : BillowNoise.Seed = 5;
+                    for (double z = 0; z < length; ++z)
+                        for (double x = 0; x < width; ++x)
+                        {
+                            double noiseValTemp = System.Math.Floor((BillowNoise.GetValue(x / 100, 0.1, z / 100) + 2) * 10) + ((height / 2) - 10);
+                            lvl.SetTile((ushort)(x), (ushort)(noiseValTemp), (ushort)(z), Block.grass);
+                            for (double i = noiseValTemp - 1; i >= 0; i--)
+                            {
+                                if (i > noiseValTemp / 4 * 3)
+                                {
+                                    lvl.SetTile((ushort)(x), (ushort)(i), (ushort)(z), Block.dirt);
+                                }
+                                else
+                                {
+                                    lvl.SetTile((ushort)(x), (ushort)(i), (ushort)(z), Block.rock);
+                                }
+                            }
+                        }
+                    break;
+                case "ridgedmultifractal":
+                    RidgedMultifractal RidgedMultifractalNoise = new RidgedMultifractal();
+                    RidgedMultifractalNoise.Seed = useSeed ? RidgedMultifractalNoise.Seed = seed : RidgedMultifractalNoise.Seed = 5;
+                    for (double z = 0; z < length; ++z)
+                        for (double x = 0; x < width; ++x)
+                        {
+                            double noiseValTemp = System.Math.Floor((RidgedMultifractalNoise.GetValue(x / 100, 0.1, z / 100) + 2) * 10) + ((height / 2) - 10);
+                            lvl.SetTile((ushort)(x), (ushort)(noiseValTemp), (ushort)(z), Block.grass);
+                            for (double i = noiseValTemp - 1; i >= 0; i--)
+                            {
+                                if (i > noiseValTemp / 4 * 3) { lvl.SetTile((ushort)(x), (ushort)(i), (ushort)(z), Block.dirt); }
+                                else { lvl.SetTile((ushort)(x), (ushort)(i), (ushort)(z), Block.rock); }
+                            }
+                        }
+                    break;
+                case "perlin":
+                    for (double z = 0; z < length; ++z)
+                        for (double x = 0; x < width; ++x)
+                        {
+                            Perlin PerlinNoise = new Perlin();
+                            PerlinNoise.Seed = useSeed ? PerlinNoise.Seed = seed : PerlinNoise.Seed = 5;
+                            double noiseValTemp = System.Math.Floor((PerlinNoise.GetValue(x / 100, 0.1, z / 100) + 2) * 10) + ((height / 2) - 10);
+                            lvl.SetTile((ushort)(x), (ushort)(noiseValTemp), (ushort)(z), Block.grass);
+                            for (double i = noiseValTemp - 1; i >= 0; i--)
+                            {
+                                if (i > noiseValTemp / 4 * 3)
+                                {
+                                    lvl.SetTile((ushort)(x), (ushort)(i), (ushort)(z), Block.dirt);
+                                }
+                                else
+                                {
+                                    lvl.SetTile((ushort)(x), (ushort)(i), (ushort)(z), Block.rock);
+                                }
+                            }
+                        }
+                    break;
+                case "checkerboard":
+                    for (double z = 0; z < length; ++z)
+                        for (double x = 0; x < width; ++x)
+                        {
+                            Checkerboard CheckerboardNoise = new Checkerboard();
+                            double noiseValTemp = System.Math.Floor((CheckerboardNoise.GetValue(x / 100, 0.1, z / 100) + 2) * 10) + ((height / 2) - 10);
+                            lvl.SetTile((ushort)(x), (ushort)(noiseValTemp), (ushort)(z), Block.grass);
+                            for (double i = noiseValTemp - 1; i >= 0; i--)
+                            {
+                                if (i > noiseValTemp / 4 * 3)
+                                {
+                                    lvl.SetTile((ushort)(x), (ushort)(i), (ushort)(z), Block.dirt);
+                                }
+                                else
+                                {
+                                    lvl.SetTile((ushort)(x), (ushort)(i), (ushort)(z), Block.rock);
+                                }
                             }
-                            int colIndex = z * width + x;
-                            for (int i = 1; i < (height - y); ++i) {
-                                int yy = height - i;
-                                blocks[colIndex + yy * width * length] = Block.lava;
+                        }
+                    break;
+                case "spheres":
+                    for (double z = 0; z < length; ++z)
+                        for (double x = 0; x < width; ++x)
+                        {
+                            Spheres SpheresNoise = new Spheres();
+                            double noiseValTemp = System.Math.Floor((SpheresNoise.GetValue(x / 100, 0.1, z / 100) + 2) * 10) + ((height / 2) - 10);
+                            lvl.SetTile((ushort)(x), (ushort)(noiseValTemp), (ushort)(z), Block.grass);
+                            for (double i = noiseValTemp - 1; i >= 0; i--)
+                            {
+                                if (i > noiseValTemp / 4 * 3)
+                                {
+                                    lvl.SetTile((ushort)(x), (ushort)(i), (ushort)(z), Block.dirt);
+                                }
+                                else
+                                {
+                                    lvl.SetTile((ushort)(x), (ushort)(i), (ushort)(z), Block.rock);
+                                }
                             }
                         }
-                        index++;
-                    }
+                    break;
+                case "cylinders":
+                    for (double z = 0; z < length; ++z)
+                        for (double x = 0; x < width; ++x)
+                        {
+                            Cylinders CylindersNoise = new Cylinders();
+                            double noiseValTemp = System.Math.Floor((CylindersNoise.GetValue(x / 100, 0.1, z / 100) + 2) * 10) + ((height / 2) - 10);
+                            lvl.SetTile((ushort)(x), (ushort)(noiseValTemp), (ushort)(z), Block.grass);
+                            for (double i = noiseValTemp - 1; i >= 0; i--)
+                            {
+                                if (i > noiseValTemp / 4 * 3)
+                                {
+                                    lvl.SetTile((ushort)(x), (ushort)(i), (ushort)(z), Block.dirt);
+                                }
+                                else
+                                {
+                                    lvl.SetTile((ushort)(x), (ushort)(i), (ushort)(z), Block.rock);
+                                }
+                            }
+                        }
+                    break;
+                case "voronoi":
+                    for (double z = 0; z < length; ++z)
+                        for (double x = 0; x < width; ++x)
+                        {
+                            Voronoi VoronoiNoise = new Voronoi();
+                            double noiseValTemp = System.Math.Floor((VoronoiNoise.GetValue(x / 100, 0.1, z / 100) + 2) * 10) + ((height / 2) - 10);
+                            lvl.SetTile((ushort)(x), (ushort)(noiseValTemp), (ushort)(z), Block.grass);
+                            for (double i = noiseValTemp - 1; i >= 0; i--)
+                            {
+                                if (i > noiseValTemp / 4 * 3)
+                                {
+                                    lvl.SetTile((ushort)(x), (ushort)(i), (ushort)(z), Block.dirt);
+                                }
+                                else
+                                {
+                                    lvl.SetTile((ushort)(x), (ushort)(i), (ushort)(z), Block.rock);
+                                }
+                            }
+                        }
+                    break;
+                case "perlin3d":
+                    Perlin perlin3dNoise = new Perlin();
+                    for (double z = 0; z < length; ++z)
+                        for (double y = 0; y < height; ++y)
+                            for (double x = 0; x < width; ++x)
+                            {
+                                perlin3dNoise.Seed = useSeed ? perlin3dNoise.Seed = seed : perlin3dNoise.Seed = 5;
+                                double noiseValTemp = System.Math.Floor((perlin3dNoise.GetValue(x / 100, y / 100, z / 100) + 2) * 10);
+                                if (noiseValTemp > 20)
+                                {
+                                    lvl.SetTile((ushort)(x), (ushort)(y), (ushort)(z), Block.grass);
+                                }
+                            }
+                    break;
+                case "perlin3dyadjust":
+                    Perlin perlin3dYadjustNoise = new Perlin();
+                    for (double z = 0; z < length; ++z)
+                        for (double y = 0; y < height; ++y)
+                            for (double x = 0; x < width; ++x)
+                            {
+                                perlin3dYadjustNoise.Seed = useSeed ? perlin3dYadjustNoise.Seed = seed : perlin3dYadjustNoise.Seed = 5;
+                                double noiseValTemp = System.Math.Floor((perlin3dYadjustNoise.GetValue(x / 100, y / 100, z / 100) + 2) * 10);
+                                if (noiseValTemp > 30 * (y / height))
+                                {
+                                    lvl.SetTile((ushort)(x), (ushort)(y), (ushort)(z), Block.grass);
+                                }
+                            }
+                    break;
+                case "billow3d":
+                    Billow Billow3dNoise = new Billow();
+                    for (double z = 0; z < length; ++z)
+                        for (double y = 0; y < height; ++y)
+                            for (double x = 0; x < width; ++x)
+                            {
+                                Billow3dNoise.Seed = useSeed ? Billow3dNoise.Seed = seed : Billow3dNoise.Seed = 5;
+                                double noiseValTemp = System.Math.Floor((Billow3dNoise.GetValue(x / 100, y / 100, z / 100) + 2) * 10);
+                                if (noiseValTemp > 20)
+                                {
+                                    lvl.SetTile((ushort)(x), (ushort)(y), (ushort)(z), Block.grass);
+                                }
+                            }
+                    break;
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
