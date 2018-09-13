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
using System.Collections.Generic;

namespace MCGalaxy.Generator {
    public static class SimpleGen {
        delegate byte NextBlock();
        const string defHelp = "%HSeed affects how terrain is generated. If seed is the same, the generated level will be the same.";
        
        public static void RegisterGenerators() {
            const GenType type = GenType.Simple;
            MapGen.Register("Island",    type, GenIsland,    defHelp);
            MapGen.Register("Mountains", type, GenMountains, defHelp);
            MapGen.Register("Forest",    type, GenForest,    defHelp);
            MapGen.Register("Ocean",     type, GenOcean,     defHelp);
            MapGen.Register("Flat",  type, GenFlat,  "%HSeed specifies grass height (default half of level height)");
            MapGen.Register("Pixel", type, GenPixel, "%HSeed does nothing");
            MapGen.Register("Empty", type, GenEmpty, "%HSeed does nothing");
            MapGen.Register("Desert",  type, GenDesert,  defHelp);
            MapGen.Register("Space",   type, GenSpace,   defHelp);
            MapGen.Register("Rainbow", type, GenRainbow, defHelp);
            MapGen.Register("Hell",    type, GenHell,    defHelp);
        }
        
        static bool GenIsland(Player p, Level lvl, string seed) {
            return new RealisticMapGen().Gen(p, lvl, seed, RealisticMapGenArgs.island);
        }
        
        static bool GenMountains(Player p, Level lvl, string seed) {
            return new RealisticMapGen().Gen(p, lvl, seed, RealisticMapGenArgs.mountains);
        }
        
        static bool GenForest(Player p, Level lvl, string seed) {
            return new RealisticMapGen().Gen(p, lvl, seed, RealisticMapGenArgs.forest);
        }
        
        static bool GenOcean(Player p, Level lvl, string seed) {
            return new RealisticMapGen().Gen(p, lvl, seed, RealisticMapGenArgs.ocean);
        }
        
        unsafe static bool GenFlat(Player p, Level lvl, string seed) {
            int grassHeight = lvl.Height / 2, v;
            if (int.TryParse(seed, out v) && v >= 0 && v < lvl.Height) grassHeight = v;
            lvl.Config.EdgeLevel = grassHeight + 1;
            
            fixed (byte* ptr = lvl.blocks) {
                if (grassHeight > 0)
                    MapSet(lvl.Width, lvl.Length, ptr, 0, grassHeight - 1, Block.Dirt);
                if (grassHeight < lvl.Height)
                    MapSet(lvl.Width, lvl.Length, ptr, grassHeight, grassHeight, Block.Grass);
            }
            return true;
        }

        static bool GenEmpty(Player p, Level lvl, string seed) {
            int maxX = lvl.Width - 1, maxZ = lvl.Length - 1;
            Cuboid(lvl, 0, 0, 0, maxX, 0, maxZ, () => Block.Bedrock);
            lvl.Config.EdgeLevel = 1;
            return true;
        }
        
        static bool GenDesert(Player p, Level lvl, string seed) {
            return new RealisticMapGen().Gen(p, lvl, seed, RealisticMapGenArgs.desert);
        }
        
        static bool GenPixel(Player p, Level lvl, string seed) {
            int maxX = lvl.Width - 1, maxY = lvl.Height - 1, maxZ = lvl.Length - 1;
            NextBlock nextBlock = () => Block.White;
            
            // Cuboid the four walls
            Cuboid(lvl, 0, 1, 0,    maxX, maxY, 0,    nextBlock);
            Cuboid(lvl, 0, 1, maxZ, maxX, maxY, maxZ, nextBlock);
            Cuboid(lvl, 0, 1, 0,    0, maxY, maxZ,    nextBlock);
            Cuboid(lvl, maxX, 1, 0, maxX, maxY, maxZ, nextBlock);
            
            // Cuboid base
            Cuboid(lvl, 0, 0, 0, maxX, 0, maxZ, () => Block.Bedrock);
            return true;
        }
        
        static bool GenSpace(Player p, Level lvl, string seed) {
            int maxX = lvl.Width - 1, maxY = lvl.Height - 1, maxZ = lvl.Length - 1;
            Random rng = MapGen.MakeRng(seed);
            NextBlock nextBlock = () => rng.Next(100) == 0 ? Block.Iron : Block.Obsidian;

            // Cuboid the four walls
            Cuboid(lvl, 0, 2, 0,    maxX, maxY, 0,    nextBlock);
            Cuboid(lvl, 0, 2, maxZ, maxX, maxY, maxZ, nextBlock);
            Cuboid(lvl, 0, 2, 0,    0, maxY, maxZ,    nextBlock);
            Cuboid(lvl, maxX, 2, 0, maxX, maxY, maxZ, nextBlock);
            
            // Cuboid base and top
            Cuboid(lvl, 0, 0, 0,    maxX, 0, maxZ, () => Block.Bedrock);
            Cuboid(lvl, 0, 1, 0,    maxX, 1, maxZ,    nextBlock);
            Cuboid(lvl, 0, maxY, 0, maxX, maxY, maxZ, nextBlock);
            return true;
        }
        
        static bool GenRainbow(Player p, Level lvl, string seed) {
            int maxX = lvl.Width - 1, maxY = lvl.Height - 1, maxZ = lvl.Length - 1;
            Random rng = MapGen.MakeRng(seed);
            NextBlock nextBlock = () => (byte)rng.Next(Block.Red, Block.White);

            // Cuboid the four walls
            Cuboid(lvl, 0, 1, 0,    maxX, maxY, 0,    nextBlock);
            Cuboid(lvl, 0, 1, maxZ, maxX, maxY, maxZ, nextBlock);
            Cuboid(lvl, 0, 1, 0,    0, maxY, maxZ,    nextBlock);
            Cuboid(lvl, maxX, 1, 0, maxX, maxY, maxZ, nextBlock);
            
            // Cuboid base and top
            Cuboid(lvl, 0, 0, 0,    maxX, 0, maxZ,    nextBlock);
            Cuboid(lvl, 0, maxY, 0, maxX, maxY, maxZ, nextBlock);
            return true;
        }
        
        static bool GenHell(Player p, Level lvl, string seed) {
            Random rng = MapGen.MakeRng(seed);
            int width = lvl.Width, height = lvl.Height, length = lvl.Length;
            int index = 0;
            byte[] blocks = lvl.blocks;
            
            for (int y = 0; y < height; ++y)
                for (int z = 0; z < length; ++z)
                    for (int x = 0; x < width; ++x)
            {
                if (y == 0) {
                    blocks[index] = Block.Bedrock;
                } else if (x == 0 || x == width - 1 || z == 0 || z == length - 1 || y == 0 || y == height - 1) {
                    blocks[index] = Block.Obsidian;
                } else if (x == 1 || x == width - 2 || z == 1 || z == length - 2) {
                    if (rng.Next(1000) != 7) { index++; continue; }
                    
                    int colIndex = z * width + x;
                    for (int i = 1; i < (height - y); ++i) {
                        int yy = height - i;
                        blocks[colIndex + yy * width * length] = Block.Lava;
                    }
                }
                index++;
            }
            return new RealisticMapGen().Gen(p, lvl, seed, RealisticMapGenArgs.hell);
        }
        
        unsafe static void MapSet(int width, int length, byte* ptr,
                                  int yStart, int yEnd, byte block) {
            int start = (yStart * length) * width;
            int end = (yEnd * length + (length - 1)) * width + (width - 1);
            Utils.memset((IntPtr)ptr, block, start, end - start + 1);
        }
        
        static void Cuboid(Level lvl, int minX, int minY, int minZ,
                           int maxX, int maxY, int maxZ, NextBlock nextBlock) {
            int width = lvl.Width, length = lvl.Length;
            byte[] blocks = lvl.blocks;
            
            for (int y = minY; y <= maxY; y++)
                for (int z = minZ; z <= maxZ; z++)
                    for (int x = minX; x <= maxX; x++)
            {
                blocks[x + width * (z + y * length)] = nextBlock();
            }
        }
    }
}
