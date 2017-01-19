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

        public static void RegisterGenerators() {
            MapGen.RegisterSimpleGen("island", GenSimple);
            MapGen.RegisterSimpleGen("mountains", GenSimple);
            MapGen.RegisterSimpleGen("forest", GenSimple);
            MapGen.RegisterSimpleGen("ocean", GenSimple);            
            MapGen.RegisterSimpleGen("flat", GenFlat);
            MapGen.RegisterSimpleGen("pixel", GenPixel);
            MapGen.RegisterSimpleGen("empty", GenEmpty);            
            MapGen.RegisterSimpleGen("desert", GenSimple);
            MapGen.RegisterSimpleGen("space", GenSpace);
            MapGen.RegisterSimpleGen("rainbow", GenRainbow);
            MapGen.RegisterSimpleGen("hell", GenHell);
        }
        
        unsafe static bool GenFlat(MapGenArgs args) {
            Level lvl = args.Level;
            int grassHeight = lvl.Height / 2;
            if (args.UseSeed && args.Seed >= 0 && args.Seed < lvl.Height)
                grassHeight = args.Seed;
            lvl.EdgeLevel = grassHeight + 1;
            
            fixed (byte* ptr = lvl.blocks) {
                if (grassHeight > 0)
                    MapSet(lvl.Width, lvl.Length, ptr, 0, grassHeight - 1, Block.dirt);
                if (grassHeight < lvl.Height)
                    MapSet(lvl.Width, lvl.Length, ptr, grassHeight, grassHeight, Block.grass);
            }
            return true;
        }

        static bool GenEmpty(MapGenArgs args) {
            int maxX = args.Level.Width - 1, maxZ = args.Level.Length - 1;
            Cuboid(args, 0, 0, 0, maxX, 0, maxZ, () => Block.blackrock);
            return true;
        }
        
        static bool GenPixel(MapGenArgs args) {
            int maxX = args.Level.Width - 1, maxY = args.Level.Height - 1, maxZ = args.Level.Length - 1;
            Func<byte> block = () => Block.white;
            
            // Cuboid the four walls
            Cuboid(args, 0, 1, 0,    maxX, maxY, 0, block);
            Cuboid(args, 0, 1, maxZ, maxX, maxY, maxZ, block);
            Cuboid(args, 0, 1, 0,    0, maxY, maxZ, block);
            Cuboid(args, maxX, 1, 0, maxX, maxY, maxZ, block);
            
            // Cuboid base
            Cuboid(args, 0, 0, 0, maxX, 0, maxZ, () => Block.blackrock);
            return true;
        }
        
        static bool GenSpace(MapGenArgs args) {
            int maxX = args.Level.Width - 1, maxY = args.Level.Height - 1, maxZ = args.Level.Length - 1;
            Random rand = args.UseSeed ? new Random(args.Seed) : new Random();
            Func<byte> block = () => rand.Next(100) == 0 ? Block.iron : Block.obsidian;

            // Cuboid the four walls
            Cuboid(args, 0, 2, 0,    maxX, maxY, 0, block);
            Cuboid(args, 0, 2, maxZ, maxX, maxY, maxZ, block);
            Cuboid(args, 0, 2, 0,    0, maxY, maxZ, block);
            Cuboid(args, maxX, 2, 0, maxX, maxY, maxZ, block);
            
            // Cuboid base and top
            Cuboid(args, 0, 0, 0,    maxX, 0, maxZ, () => Block.blackrock);
            Cuboid(args, 0, 1, 0,    maxX, 1, maxZ, block);
            Cuboid(args, 0, maxY, 0, maxX, maxY, maxZ, block);
            return true;
        }
        
        static bool GenRainbow(MapGenArgs args) {
            int maxX = args.Level.Width - 1, maxY = args.Level.Height - 1, maxZ = args.Level.Length - 1;
            Random rand = args.UseSeed ? new Random(args.Seed) : new Random();
            Func<byte> block = () => (byte)rand.Next(Block.red, Block.white);

            // Cuboid the four walls
            Cuboid(args, 0, 1, 0,    maxX, maxY, 0, block);
            Cuboid(args, 0, 1, maxZ, maxX, maxY, maxZ, block);
            Cuboid(args, 0, 1, 0,    0, maxY, maxZ, block);
            Cuboid(args, maxX, 1, 0, maxX, maxY, maxZ, block);
            
            // Cuboid base and top
            Cuboid(args, 0, 0, 0,    maxX, 0, maxZ, block);
            Cuboid(args, 0, maxY, 0, maxX, maxY, maxZ, block);
            return true;
        }
        
        static bool GenHell(MapGenArgs args) {
            Random rand = args.UseSeed ? new Random(args.Seed) : new Random();
            int width = args.Level.Width, height = args.Level.Height, length = args.Level.Length;
            int index = 0;
            byte[] blocks = args.Level.blocks;
            
            for (int y = 0; y < height; ++y)
                for (int z = 0; z < length; ++z)
                    for (int x = 0; x < width; ++x)
            {
                if (y == 0) {
                    blocks[index] = Block.blackrock;
                } else if (x == 0 || x == width - 1 || z == 0 || z == length - 1 || y == 0 || y == height - 1) {
                    blocks[index] = Block.obsidian;
                } else if (x == 1 || x == width - 2 || z == 1 || z == length - 2) {
                    if (rand.Next(1000) != 7) { index++; continue; }
                    
                    int colIndex = z * width + x;
                    for (int i = 1; i < (height - y); ++i) {
                        int yy = height - i;
                        blocks[colIndex + yy * width * length] = Block.lava;
                    }
                }
                index++;
            }
            return GenSimple(args);
        }
        
        
        static bool GenSimple(MapGenArgs args) {
            return new RealisticMapGen().GenerateMap(args);
        }
        
        unsafe static void MapSet(int width, int length, byte* ptr, 
                                  int yStart, int yEnd, byte block) {
            int start = (yStart * length) * width;
            int end = (yEnd * length + (length - 1)) * width + (width - 1);
            Utils.memset((IntPtr)ptr, block, start, end - start + 1);
        }
        
        static void Cuboid(MapGenArgs args, int minX, int minY, int minZ,
                           int maxX, int maxY, int maxZ, Func<byte> block) {
            int width = args.Level.Width, length = args.Level.Length;
            byte[] blocks = args.Level.blocks;
            
            for (int y = minY; y <= maxY; y++)
                for (int z = minZ; z <= maxZ; z++)
                    for (int x = minX; x <= maxX; x++)
            {
                blocks[x + width * (z + y * length)] = block();
            }
        }
    }
}
