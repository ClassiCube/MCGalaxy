/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using LibNoise;

namespace MCGalaxy {
    
    public static class MapGen {
        
        public static bool IsRecognisedTheme(string s) {
            s = s.ToLower();
            return Array.IndexOf<string>(types, s) >= 0 || Array.IndexOf<string>(advTypes, s) >= 0;
        }
        
        static string[] types = { "island", "mountains", "forest", "ocean", "flat", 
            "pixel", "empty", "desert", "space", "rainbow", "hell" };
        static string[] advTypes = { "billow", "perlin", "checkerboard", "spheres", "cylinders",
            "voronoi", "ridgedmultifractal", "perlin3d", "perlin3dyadjust" };

        public static void PrintThemes(Player p) {
            Player.Message(p, "Simple themes: " + String.Join(", ", types));
            Player.Message(p, "Advanced themes: " + String.Join(", ", advTypes));
        }
        
        public static bool OkayAxis(int len) {
			return len >= 16 && len <= 8192 && (len % 16) == 0;
        }
        
        unsafe static void MapSet(int width, int length, byte* ptr, int yStart, int yEnd, byte block) {
            int start = yStart * length * width;
            int end = (yEnd * length + (length - 1)) * width + (width - 1);
            Extensions.memset((IntPtr)ptr, block, start, end - start + 1);
        }

        public unsafe static void Generate(Level lvl, string type, string args) {
            int index = 0, width = lvl.Width, height = lvl.Height, length = lvl.Length;
            byte[] blocks = lvl.blocks;
            int half = height / 2;            
            RealisticMapGen generator = new RealisticMapGen();
            IModule module2D = null, module3D = null;
            
            int seed = 0; 
            bool useSeed = args != "";
            if (useSeed && !int.TryParse(args, out seed))
                seed = args.GetHashCode();
            
            switch (type) {
                case "flat":
                    int grassHeight = height / 2;
                    if (useSeed && seed >= 0 && seed < height) {
                        lvl.EdgeLevel = (short)seed;
                        grassHeight = seed;
                    }
                    
                    fixed (byte* ptr = blocks) {
                        if (grassHeight > 0)
                            MapSet(lvl.Width, lvl.Length, ptr, 0, grassHeight - 1, Block.dirt);
                        if (grassHeight < lvl.Height)
                            MapSet(lvl.Width, lvl.Length, ptr, grassHeight, grassHeight, Block.grass);
                    }
                    return;
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
                    } return;
                case "empty":
                    for (int z = 0; z < length; ++z)
                        for (int x = 0; x < width; ++x)
                    {
                        blocks[index++] = Block.blackrock;
                    } return;
                case "space":
                    Random random = useSeed ? new Random(seed) : new Random();
                    for (int y = 0; y < height; ++y)
                        for (int z = 0; z < length; ++z)
                            for (int x = 0; x < width; ++x)
                    {
                        if (y == 0)
                            blocks[index] = Block.blackrock;
                        else if (x == 0 || x == width - 1 || z == 0 || z == length - 1 || y == 1 || y == height - 1)
                            blocks[index] = random.Next(100) == 0 ? Block.iron : Block.obsidian;
                        index++;
                    } break;
                case "rainbow":
                    Random random2 = useSeed ? new Random(seed) : new Random();
                    for (int y = 0; y < height; ++y)
                        for (int z = 0; z < length; ++z)
                            for (int x = 0; x < width; ++x)
                    {
                        if (y == 0 || y == height - 1 || x == 0 || x == width - 1 || z == 0 || z == length - 1)
                            blocks[index] = (byte)random2.Next(Block.red, Block.white);
                        index++;
                    } return;
                case "hell":
                    Random random3 = useSeed ? new Random(seed) : new Random();
                    for (int y = 0; y < height; ++y)
                        for (int z = 0; z < length; ++z)
                            for (int x = 0; x < width; ++x)
                    {
                        if (y == 0) {
                            blocks[index] = Block.blackrock;
                        } else if (x == 0 || x == width - 1 || z == 0 || z == length - 1 || y == 0 || y == height - 1) {
                            blocks[index] = Block.obsidian;
                        } else if (x == 1 || x == width - 2 || z == 1 || z == length - 2) {
                            if (random3.Next(1000) != 7) {
                                index++; continue;
                            }
                            int colIndex = z * width + x;
                            for (int i = 1; i < (height - y); ++i) {
                                int yy = height - i;
                                blocks[colIndex + yy * width * length] = Block.lava;
                            }
                        }
                        index++;
                    } generator.GenerateMap(lvl, type, seed, useSeed); return;
                case "billow":
                    module2D = new Billow();
                    ((Billow)module2D).Seed = useSeed ? seed : new Random().Next(); break;
                case "ridgedmultifractal":
                    module2D = new RidgedMultifractal();
                    ((RidgedMultifractal)module2D).Seed = useSeed ? seed : new Random().Next(); break;
                case "perlin":
                    module2D = new Perlin();
                    ((Perlin)module2D).Seed = useSeed ? seed : new Random().Next(); break;
                case "checkerboard":
                    module2D = new Checkerboard(); break;
                case "spheres":
                    module2D = new Spheres(); break;
                case "cylinders":
                    module2D = new Cylinders(); break;
                case "voronoi":
                    module2D = new Voronoi();
                    ((Voronoi)module2D).Seed = useSeed ? seed : new Random().Next(); break;
                case "perlin3d":
                    module3D = new Perlin();
                    ((Perlin)module3D).Seed = useSeed ? seed : new Random().Next(); break;
                case "perlin3dyadjust":
                    Perlin adjNoise = new Perlin();
                    adjNoise.Seed = useSeed ? seed : new Random().Next();
                    for (int y = 0; y < height; y++)
                        for (int z = 0; z < length; ++z)
                            for (int x = 0; x < width; ++x)
                    {                        
                        double value = System.Math.Floor((adjNoise.GetValue(x / 100.0, y / 100.0, z / 100.0) + 2) * 10);
                        if (value > 30 * y / height)
                            lvl.SetTile((ushort)x, (ushort)y, (ushort)z, Block.grass);
                    }
                    break;
                case "billow3d":
                    module3D = new Billow();
                    ((Billow)module3D).Seed = useSeed ? seed : new Random().Next(); break;
                case "island":
                case "mountains":
                case "ocean":
                case "forest":
                case "desert":
                    generator.GenerateMap(lvl, type, seed, useSeed); return;
            }
            
            if (module2D != null) {
                int waterlvl = half - 1;
                for (int z = 0; z < length; ++z)
                    for (int x = 0; x < width; ++x)
                {
                    double noise = module2D.GetValue(x / 100.0, 0.1, z / 100.0);
                    int height2D = (int)System.Math.Floor((noise + 2) * 10) + (half-20);
                    int height2Dtex01 = (int)System.Math.Floor((noise + 2) * 15) + (half- 30);
                    byte topBlock = height2D < height2Dtex01 ? Block.grass : Block.sand;
                    lvl.SetTile((ushort)x, (ushort)height2D, (ushort)z, topBlock);
                    
                    if (height2D < waterlvl) {
                        for (int y = waterlvl; y >= height2D; y--)
                            lvl.SetTile((ushort)x, (ushort)y, (ushort)z, Block.water);
                    }
                    for (int y = height2D - 1; y >= 0; y--) {
                        byte block = (y > height2D * 3 / 4) ? Block.dirt : Block.rock;
                        lvl.SetTile((ushort)x, (ushort)y, (ushort)z, block);
                    }
                }
            }
            
            if (module3D != null) {
                for (int y = 0; y < height; y++)
                    for (int z = 0; z < length; ++z)
                        for (int x = 0; x < width; ++x)
                {
                    double value = System.Math.Floor((module3D.GetValue(x / 100.0, y / 100.0, z / 100.0) + 2) * 10);
                    if (value > 20)
                        lvl.SetTile((ushort)x, (ushort)y, (ushort)z, Block.grass);
                }
            }
        }
    }
}
