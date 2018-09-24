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
        const string defHelp = "%HSeed affects how terrain is generated. If seed is the same, the generated level will be the same.";
        
        public static void RegisterGenerators() {
            const GenType type = GenType.Advanced;
            MapGen.Register("Billow",             type, GenBillow2D, defHelp);
            MapGen.Register("RidgedMultifractal", type, GenRidged2D, defHelp);
            MapGen.Register("Perlin",             type, GenPerlin2D, defHelp);
            MapGen.Register("Checkerboard", type, GenCheckerboard, "%HSeed does nothing");
            MapGen.Register("Voronoi",         type, GenVoronoi,         defHelp);          
            MapGen.Register("Perlin3D",        type, GenPerlin3D,        defHelp);
            MapGen.Register("Perlin3Dyadjust", type, GenPerlin3DYAdjust, defHelp);
            MapGen.Register("Billow3D",        type, GenBillow3D,        defHelp);
        }
        
        
        #region Implementations
        
        static bool GenBillow2D(Player p, Level lvl, string seed) {
            Billow billow2D = new Billow();
            billow2D.Seed   = MapGen.MakeInt(seed);
            return Gen2D(lvl, billow2D);
        }
        
        static bool GenRidged2D(Player p, Level lvl, string seed) {
            RidgedMultifractal ridged2D = new RidgedMultifractal();
            ridged2D.Seed = MapGen.MakeInt(seed);
            return Gen2D(lvl, ridged2D);
        }
        
        static bool GenPerlin2D(Player p, Level lvl, string seed) {
            Perlin perlin2D = new Perlin();
            perlin2D.Seed   = MapGen.MakeInt(seed);
            return Gen2D(lvl, perlin2D);
        }
        
        static bool GenCheckerboard(Player p, Level lvl, string seed) {
            return Gen2D(lvl, new Checkerboard());
        }
        
        static bool GenVoronoi(Player p, Level lvl, string seed) {
            Voronoi voronoi2D = new Voronoi();
            voronoi2D.Seed    = MapGen.MakeInt(seed);
            return Gen2D(lvl, voronoi2D);
        }
        
        static bool GenPerlin3D(Player p, Level lvl, string seed) {
            Perlin perlin3D = new Perlin();
            perlin3D.Seed   = MapGen.MakeInt(seed);
            return Gen3D(lvl, perlin3D);
        }
        
        static bool GenPerlin3DYAdjust(Player p, Level lvl, string seed) {
            Perlin adjNoise = new Perlin();
            adjNoise.Seed   = MapGen.MakeInt(seed);
            return Gen3DYAdjust(lvl, adjNoise);
        }
        
        static bool GenBillow3D(Player p, Level lvl, string seed) {
            Billow billow3D = new Billow();
            billow3D.Seed   = MapGen.MakeInt(seed);
            return Gen3D(lvl, billow3D);
        }
        
        #endregion
        
        static bool Gen2D(Level lvl, IModule module) {
            int width = lvl.Width, length = lvl.Length, half = lvl.Height / 2;
            int waterHeight = half - 1;
            
            for (int z = 0; z < length; ++z)
                for (int x = 0; x < width; ++x)
            {
                double noise = module.GetValue(x / 100.0, 0.1, z / 100.0);
                int dirtHeight = (int)System.Math.Floor((noise + 2) * 10) + (half - 20);
                int sandHeight = (int)System.Math.Floor((noise + 2) * 15) + (half - 30);
                byte topBlock = dirtHeight < sandHeight ? Block.Grass : Block.Sand;
                lvl.SetTile((ushort)x, (ushort)dirtHeight, (ushort)z, topBlock);
                
                if (dirtHeight < waterHeight) {
                    for (int y = waterHeight; y >= dirtHeight; y--)
                        lvl.SetTile((ushort)x, (ushort)y, (ushort)z, Block.StillWater);
                }
                for (int y = dirtHeight - 1; y >= 0; y--) {
                    byte block = (y > dirtHeight * 3 / 4) ? Block.Dirt : Block.Stone;
                    lvl.SetTile((ushort)x, (ushort)y, (ushort)z, block);
                }
            }
            return true;
        }
        
        static bool Gen3D(Level lvl, IModule module) {
            int width = lvl.Width, height = lvl.Height, length = lvl.Length;
            for (int y = 0; y < height; y++)
                for (int z = 0; z < length; ++z)
                    for (int x = 0; x < width; ++x)
            {
                double value = System.Math.Floor((module.GetValue(x / 100.0, y / 100.0, z / 100.0) + 2) * 10);
                if (value > 20)
                    lvl.SetTile((ushort)x, (ushort)y, (ushort)z, Block.Grass);
            }
            return true;
        }
        
        static bool Gen3DYAdjust(Level lvl, IModule module) { 
            int width = lvl.Width, height = lvl.Height, length = lvl.Length;
            for (int y = 0; y < height; y++)
                for (int z = 0; z < length; ++z)
                    for (int x = 0; x < width; ++x)
            {
                double value = System.Math.Floor((module.GetValue(x / 100.0, y / 100.0, z / 100.0) + 2) * 10);
                if (value > 30 * y / height)
                    lvl.SetTile((ushort)x, (ushort)y, (ushort)z, Block.Grass);
            }
            return true;
        }
    }
}
