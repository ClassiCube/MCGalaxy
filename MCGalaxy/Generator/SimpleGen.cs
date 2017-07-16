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
            lvl.Config.EdgeLevel = grassHeight + 1;
            
            fixed (byte* ptr = lvl.blocks) {
                if (grassHeight > 0)
                    MapSet(lvl.Width, lvl.Length, ptr, 0, grassHeight - 1, Block.Dirt);
                if (grassHeight < lvl.Height)
                    MapSet(lvl.Width, lvl.Length, ptr, grassHeight, grassHeight, Block.Grass);
            }
            return true;
        }

        static bool GenEmpty(MapGenArgs args) {
            int maxX = args.Level.Width - 1, maxZ = args.Level.Length - 1;
            Cuboid(args, 0, 0, 0, maxX, 0, maxZ, () => Block.Bedrock);
            return true;
        }
        
        static bool GenPixel(MapGenArgs args) {
            int maxX = args.Level.Width - 1, maxY = args.Level.Height - 1, maxZ = args.Level.Length - 1;
            NextBlock nextBlock = () => Block.White;
            
            // Cuboid the four walls
            Cuboid(args, 0, 1, 0,    maxX, maxY, 0, nextBlock);
            Cuboid(args, 0, 1, maxZ, maxX, maxY, maxZ, nextBlock);
            Cuboid(args, 0, 1, 0,    0, maxY, maxZ, nextBlock);
            Cuboid(args, maxX, 1, 0, maxX, maxY, maxZ, nextBlock);
            
            // Cuboid base
            Cuboid(args, 0, 0, 0, maxX, 0, maxZ, () => Block.Bedrock);
            return true;
        }
        
        static bool GenSpace(MapGenArgs args) {
            int maxX = args.Level.Width - 1, maxY = args.Level.Height - 1, maxZ = args.Level.Length - 1;
            Random rand = args.UseSeed ? new Random(args.Seed) : new Random();
            NextBlock nextBlock = () => rand.Next(100) == 0 ? Block.Iron : Block.Obsidian;

            // Cuboid the four walls
            Cuboid(args, 0, 2, 0,    maxX, maxY, 0, nextBlock);
            Cuboid(args, 0, 2, maxZ, maxX, maxY, maxZ, nextBlock);
            Cuboid(args, 0, 2, 0,    0, maxY, maxZ, nextBlock);
            Cuboid(args, maxX, 2, 0, maxX, maxY, maxZ, nextBlock);
            
            // Cuboid base and top
            Cuboid(args, 0, 0, 0,    maxX, 0, maxZ, () => Block.Bedrock);
            Cuboid(args, 0, 1, 0,    maxX, 1, maxZ, nextBlock);
            Cuboid(args, 0, maxY, 0, maxX, maxY, maxZ, nextBlock);
            return true;
        }
        
        static bool GenRainbow(MapGenArgs args) {
            int maxX = args.Level.Width - 1, maxY = args.Level.Height - 1, maxZ = args.Level.Length - 1;
            Random rand = args.UseSeed ? new Random(args.Seed) : new Random();
            NextBlock nextBlock = () => (byte)rand.Next(Block.Red, Block.White);

            // Cuboid the four walls
            Cuboid(args, 0, 1, 0,    maxX, maxY, 0, nextBlock);
            Cuboid(args, 0, 1, maxZ, maxX, maxY, maxZ, nextBlock);
            Cuboid(args, 0, 1, 0,    0, maxY, maxZ, nextBlock);
            Cuboid(args, maxX, 1, 0, maxX, maxY, maxZ, nextBlock);
            
            // Cuboid base and top
            Cuboid(args, 0, 0, 0,    maxX, 0, maxZ, nextBlock);
            Cuboid(args, 0, maxY, 0, maxX, maxY, maxZ, nextBlock);
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
                    blocks[index] = Block.Bedrock;
                } else if (x == 0 || x == width - 1 || z == 0 || z == length - 1 || y == 0 || y == height - 1) {
                    blocks[index] = Block.Obsidian;
                } else if (x == 1 || x == width - 2 || z == 1 || z == length - 2) {
                    if (rand.Next(1000) != 7) { index++; continue; }
                    
                    int colIndex = z * width + x;
                    for (int i = 1; i < (height - y); ++i) {
                        int yy = height - i;
                        blocks[colIndex + yy * width * length] = Block.Lava;
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
                           int maxX, int maxY, int maxZ, NextBlock nextBlock) {
            int width = args.Level.Width, length = args.Level.Length;
            byte[] blocks = args.Level.blocks;
            
            for (int y = minY; y <= maxY; y++)
                for (int z = minZ; z <= maxZ; z++)
                    for (int x = minX; x <= maxX; x++)
            {
                blocks[x + width * (z + y * length)] = nextBlock();
            }
        }
    }
}
