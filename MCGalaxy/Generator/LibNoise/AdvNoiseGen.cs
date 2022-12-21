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

namespace MCGalaxy.Generator 
{
    public static class AdvNoiseGen 
    {
        public static void RegisterGenerators() {
            const GenType type = GenType.Advanced;
            MapGen.Register("Billow",             type, GenBillow2D, MapGen.DEFAULT_HELP);
            MapGen.Register("RidgedMultifractal", type, GenRidged2D, MapGen.DEFAULT_HELP);
            MapGen.Register("Perlin",             type, GenPerlin2D, MapGen.DEFAULT_HELP);
            MapGen.Register("Checkerboard", type, GenCheckerboard, "&HSeed does nothing");
            MapGen.Register("Voronoi",         type, GenVoronoi,         MapGen.DEFAULT_HELP);          
            MapGen.Register("Perlin3D",        type, GenPerlin3D,        MapGen.DEFAULT_HELP);
            MapGen.Register("Perlin3Dyadjust", type, GenPerlin3DYAdjust, MapGen.DEFAULT_HELP);
            MapGen.Register("Billow3D",        type, GenBillow3D,        MapGen.DEFAULT_HELP);
        }
        
        
        #region Implementations
        
        static bool GenBillow2D(Player p, Level lvl, string seed) {
            Billow module = new Billow();
            return Gen2D(p, lvl, module, seed, out module.Seed);
        }
        
        static bool GenRidged2D(Player p, Level lvl, string seed) {
            RidgedMultifractal module = new RidgedMultifractal();
            return Gen2D(p, lvl, module, seed, out module.Seed);
        }
        
        static bool GenPerlin2D(Player p, Level lvl, string seed) {
            Perlin module = new Perlin();
            return Gen2D(p, lvl, module, seed, out module.Seed);
        }
        
        static bool GenCheckerboard(Player p, Level lvl, string seed) {
            int value;
            return Gen2D(p, lvl, new Checkerboard(), seed, out value);
        }
        
        static bool GenVoronoi(Player p, Level lvl, string seed) {
            Voronoi module = new Voronoi();
            return Gen2D(p, lvl, module, seed, out module.Seed);
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
        
        static bool Gen2D(Player p, Level lvl, IModule module, 
                          string seed, out int noiseSeed) {
            int width = lvl.Width, length = lvl.Length, half = lvl.Height / 2;
            int waterHeight = half - 1;
            
            MapGenThemeName theme = MapGenThemeName.Forest;
            if (!MapGen.ParseArgs(p, seed, out noiseSeed, ref theme)) return false;
            MapGenTheme biome = MapGenTheme.Get(theme);
            
            for (int z = 0; z < length; ++z)
                for (int x = 0; x < width; ++x)
            {
                double noise   = module.GetValue(x / 100.0, 0.1, z / 100.0);
                int dirtHeight = (int)Math.Floor(noise * 10) + half;
                
                if (dirtHeight < waterHeight) {
                    // column is underwater
                    for (int y = waterHeight; y >= dirtHeight; y--)
                    {
                        lvl.SetTile((ushort)x, (ushort)y, (ushort)z, biome.Water);
                    }
                } else {
                    // top of column is above water
                    int sandHeight = (int)Math.Floor(noise * 15) + half;
                    byte topBlock  = dirtHeight < sandHeight ? biome.Surface : biome.SeaFloor;
                    lvl.SetTile((ushort)x, (ushort)dirtHeight, (ushort)z, topBlock);
                }
                
                for (int y = dirtHeight - 1; y >= 0; y--) 
                {
                    byte block = (y > dirtHeight * 3 / 4) ? biome.Ground : biome.Cliff;
                    lvl.SetTile((ushort)x, (ushort)y, (ushort)z, block);
                }
            }
            
            biome.ApplyEnv(lvl.Config);
            return true;
        }
        
        static bool Gen3D(Level lvl, IModule module) {
            int width = lvl.Width, height = lvl.Height, length = lvl.Length;
            for (int y = 0; y < height; y++)
                for (int z = 0; z < length; ++z)
                    for (int x = 0; x < width; ++x)
            {
                double value = module.GetValue(x / 100.0, y / 100.0, z / 100.0);
                if (value >= 0.1)
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
                double value = Math.Floor((module.GetValue(x / 100.0, y / 100.0, z / 100.0) + 2) * 10);
                if (value > 30 * y / height)
                    lvl.SetTile((ushort)x, (ushort)y, (ushort)z, Block.Grass);
            }
            return true;
        }
    }
}
