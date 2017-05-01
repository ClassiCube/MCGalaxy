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
using System.Collections.Generic;
using LibNoise;

namespace MCGalaxy.Generator {
    public static class AdvNoiseGen {

        public static void RegisterGenerators() {
            MapGen.RegisterAdvancedGen("billow", GenBillow2D);
            MapGen.RegisterAdvancedGen("ridgedmultifractal", GenRidged2D);
            MapGen.RegisterAdvancedGen("perlin", GenPerlin2D);
            MapGen.RegisterAdvancedGen("checkerboard", GenCheckerboard);          
            MapGen.RegisterAdvancedGen("spheres", GenSpheres);
            MapGen.RegisterAdvancedGen("cylinders", GenCylinders);
            MapGen.RegisterAdvancedGen("voronoi", GenVoronoi);            
            MapGen.RegisterAdvancedGen("perlin3d", GenPerlin3D);
            MapGen.RegisterAdvancedGen("perlin3dyadjust", GenPerlin3DYAdjust);
            MapGen.RegisterAdvancedGen("billow3d", GenBillow3D);
        }
        
        
        #region Implementations
        
        static bool GenBillow2D(MapGenArgs args) {
            Billow billow2D = new Billow();
            billow2D.Seed = args.UseSeed ? args.Seed : new Random().Next();
            return Gen2D(args, billow2D);
        }
        
        static bool GenRidged2D(MapGenArgs args) {
            RidgedMultifractal ridged2D = new RidgedMultifractal();
            ridged2D.Seed = args.UseSeed ? args.Seed : new Random().Next();
            return Gen2D(args, ridged2D);
        }
        
        static bool GenPerlin2D(MapGenArgs args) {
            Perlin perlin2D = new Perlin();
            perlin2D.Seed = args.UseSeed ? args.Seed : new Random().Next();
            return Gen2D(args, perlin2D);
        }
        
        static bool GenCheckerboard(MapGenArgs args) {
            return Gen2D(args, new Checkerboard());
        }
        
        static bool GenSpheres(MapGenArgs args) {
            return Gen2D(args, new Spheres());
        }
        
        static bool GenCylinders(MapGenArgs args) {
            return Gen2D(args, new Cylinders());
        }
        
        static bool GenVoronoi(MapGenArgs args) {
            Voronoi voronoi2D = new Voronoi();
            voronoi2D.Seed = args.UseSeed ? args.Seed : new Random().Next();
            return Gen2D(args, voronoi2D);
        }
        
        static bool GenPerlin3D(MapGenArgs args) {
            Perlin perlin3D = new Perlin();
            perlin3D.Seed = args.UseSeed ? args.Seed : new Random().Next();
            return Gen3D(args, perlin3D);
        }
        
        static bool GenPerlin3DYAdjust(MapGenArgs args) {
            Perlin adjNoise = new Perlin();
            adjNoise.Seed = args.UseSeed ? args.Seed : new Random().Next();
            return Gen3DYAdjust(args, adjNoise);
        }
        
        static bool GenBillow3D(MapGenArgs args) {
            Billow billow3D = new Billow();
            billow3D.Seed = args.UseSeed ? args.Seed : new Random().Next();
            return Gen3D(args, billow3D);
        }
        
        #endregion
        
        static bool Gen2D(MapGenArgs args, IModule module) {
            Level lvl = args.Level;
            int width = lvl.Width, length = lvl.Length, half = lvl.Height / 2;
            int waterHeight = half - 1;
            
            for (int z = 0; z < length; ++z)
                for (int x = 0; x < width; ++x)
            {
                double noise = module.GetValue(x / 100.0, 0.1, z / 100.0);
                int dirtHeight = (int)System.Math.Floor((noise + 2) * 10) + (half - 20);
                int sandHeight = (int)System.Math.Floor((noise + 2) * 15) + (half - 30);
                byte topBlock = dirtHeight < sandHeight ? Block.grass : Block.sand;
                lvl.SetTile((ushort)x, (ushort)dirtHeight, (ushort)z, topBlock);
                
                if (dirtHeight < waterHeight) {
                    for (int y = waterHeight; y >= dirtHeight; y--)
                        lvl.SetTile((ushort)x, (ushort)y, (ushort)z, Block.waterstill);
                }
                for (int y = dirtHeight - 1; y >= 0; y--) {
                    byte block = (y > dirtHeight * 3 / 4) ? Block.dirt : Block.rock;
                    lvl.SetTile((ushort)x, (ushort)y, (ushort)z, block);
                }
            }
            return true;
        }
        
        static bool Gen3D(MapGenArgs args, IModule module) {
            Level lvl = args.Level;
            int width = lvl.Width, height = lvl.Height, length = lvl.Length;
            for (int y = 0; y < height; y++)
                for (int z = 0; z < length; ++z)
                    for (int x = 0; x < width; ++x)
            {
                double value = System.Math.Floor((module.GetValue(x / 100.0, y / 100.0, z / 100.0) + 2) * 10);
                if (value > 20)
                    lvl.SetTile((ushort)x, (ushort)y, (ushort)z, Block.grass);
            }
            return true;
        }
        
        static bool Gen3DYAdjust(MapGenArgs args, IModule module) {
            Level lvl = args.Level;
            int width = lvl.Width, height = lvl.Height, length = lvl.Length;
            for (int y = 0; y < height; y++)
                for (int z = 0; z < length; ++z)
                    for (int x = 0; x < width; ++x)
            {
                double value = System.Math.Floor((module.GetValue(x / 100.0, y / 100.0, z / 100.0) + 2) * 10);
                if (value > 30 * y / height)
                    lvl.SetTile((ushort)x, (ushort)y, (ushort)z, Block.grass);
            }
            return true;
        }
    }
}
