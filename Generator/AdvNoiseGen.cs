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
using LibNoise;

namespace MCGalaxy {
    public static class AdvNoiseGen {
        
        public unsafe static void Generate(Level lvl, string type, bool useSeed, int seed) {
            switch (type) {
                case "billow":
                    Billow billow2D = new Billow();
                    billow2D.Seed = useSeed ? seed : new Random().Next();
                    Generate2D(lvl, billow2D); break;
                case "ridgedmultifractal":
                    RidgedMultifractal ridged2D = new RidgedMultifractal();
                    ridged2D.Seed = useSeed ? seed : new Random().Next();
                    Generate2D(lvl, ridged2D); break;
                case "perlin":
                    Perlin perlin2D = new Perlin();
                    perlin2D.Seed = useSeed ? seed : new Random().Next();
                    Generate2D(lvl, perlin2D); break;
                case "checkerboard":
                    Generate2D(lvl, new Checkerboard()); break;
                case "spheres":
                    Generate2D(lvl, new Spheres()); break;
                case "cylinders":
                    Generate2D(lvl, new Cylinders()); break;
                case "voronoi":
                    Voronoi voronoi2D = new Voronoi();
                    voronoi2D.Seed = useSeed ? seed : new Random().Next();
                    Generate2D(lvl, voronoi2D); break;
                case "perlin3d":
                    Perlin perlin3D = new Perlin();
                    perlin3D.Seed = useSeed ? seed : new Random().Next();
                    Generate3D(lvl, perlin3D); break;
                case "perlin3dyadjust":
                    Perlin adjNoise = new Perlin();
                    adjNoise.Seed = useSeed ? seed : new Random().Next();
                    Generate3DYAdjust(lvl, adjNoise); break;
                case "billow3d":
                    Billow billow3D = new Billow();
                    billow3D.Seed = useSeed ? seed : new Random().Next();
                    Generate3D(lvl, billow3D); break;
            }
        }
        
        static void Generate2D(Level lvl, IModule module) {
            int width = lvl.Width, length = lvl.Length, half = lvl.Height / 2;
            int waterHeight = half - 1;
            
            for (int z = 0; z < length; ++z)
                for (int x = 0; x < width; ++x)
            {
                double noise = module.GetValue(x / 100.0, 0.1, z / 100.0);
                int height2D = (int)System.Math.Floor((noise + 2) * 10) + (half - 20);
                int height2Dtex01 = (int)System.Math.Floor((noise + 2) * 15) + (half - 30);
                byte topBlock = height2D < height2Dtex01 ? Block.grass : Block.sand;
                lvl.SetTile((ushort)x, (ushort)height2D, (ushort)z, topBlock);
                
                if (height2D < waterHeight) {
                    for (int y = waterHeight; y >= height2D; y--)
                        lvl.SetTile((ushort)x, (ushort)y, (ushort)z, Block.water);
                }
                for (int y = height2D - 1; y >= 0; y--) {
                    byte block = (y > height2D * 3 / 4) ? Block.dirt : Block.rock;
                    lvl.SetTile((ushort)x, (ushort)y, (ushort)z, block);
                }
            }
        }
        
        static void Generate3D(Level lvl, IModule module) {
            int width = lvl.Width, height = lvl.Height, length = lvl.Length;
            for (int y = 0; y < height; y++)
                for (int z = 0; z < length; ++z)
                    for (int x = 0; x < width; ++x)
            {
                double value = System.Math.Floor((module.GetValue(x / 100.0, y / 100.0, z / 100.0) + 2) * 10);
                if (value > 20)
                    lvl.SetTile((ushort)x, (ushort)y, (ushort)z, Block.grass);
            }
        }
        
        static void Generate3DYAdjust(Level lvl, IModule module) {
            int width = lvl.Width, height = lvl.Height, length = lvl.Length;
            for (int y = 0; y < height; y++)
                for (int z = 0; z < length; ++z)
                    for (int x = 0; x < width; ++x)
            {
                double value = System.Math.Floor((module.GetValue(x / 100.0, y / 100.0, z / 100.0) + 2) * 10);
                if (value > 30 * y / height)
                    lvl.SetTile((ushort)x, (ushort)y, (ushort)z, Block.grass);
            }
        }
    }
}
