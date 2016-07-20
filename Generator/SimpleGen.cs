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
        
        static Dictionary<string, Action<MapGenArgs>> generators
            = new Dictionary<string, Action<MapGenArgs>>{
            { "island", GenSimple }, { "mountains", GenSimple },
            { "forest", GenSimple }, { "ocean", GenSimple }, 
            { "flat", GenFlat }, { "pixel", GenPixel },
            { "empty", GenEmpty }, { "desert", GenSimple },
            { "space", GenSpace }, { "rainbow", GenRainbow }, 
            { "hell", GenHell },
        };
        
        public static bool Generate(MapGenArgs args) {
            Action<MapGenArgs> generator;
            generators.TryGetValue(args.Type, out generator);
            if (generator != null) generator(args);
            return generator != null;
        }
        
        
        unsafe static void GenFlat(MapGenArgs args) {
            Level lvl = args.Level;
            int grassHeight = lvl.Height / 2;
            if (args.UseSeed && args.Seed >= 0 && args.Seed < lvl.Height) {
                lvl.EdgeLevel = (short)args.Seed;
                grassHeight = args.Seed;
            }
            
            fixed (byte* ptr = lvl.blocks) {
                if (grassHeight > 0)
                    MapSet(lvl.Width, lvl.Length, ptr, 0, grassHeight - 1, Block.dirt);
                if (grassHeight < lvl.Height)
                    MapSet(lvl.Width, lvl.Length, ptr, grassHeight, grassHeight, Block.grass);
            }
        }
        
        unsafe static void MapSet(int width, int length, byte* ptr, int yStart, int yEnd, byte block) {
            int start = yStart * length * width;
            int end = (yEnd * length + (length - 1)) * width + (width - 1);
            Extensions.memset((IntPtr)ptr, block, start, end - start + 1);
        }
        
        static void GenPixel(MapGenArgs args) {
            int width = args.Level.Width, height = args.Level.Height, length = args.Level.Length;
            int index = 0;
            byte[] blocks = args.Level.blocks;
            
            for (int y = 0; y < height; ++y)
                for (int z = 0; z < length; ++z)
                    for (int x = 0; x < width; ++x)
            {
                if (y == 0)
                    blocks[index] = Block.blackrock;
                else if (x == 0 || x == width - 1 || z == 0 || z == length - 1)
                    blocks[index] = Block.white;
                index++;
            }
        }

        static void GenEmpty(MapGenArgs args) {
            int width = args.Level.Width, length = args.Level.Length;
            int index = 0;
            byte[] blocks = args.Level.blocks;
            
            for (int z = 0; z < length; ++z)
                for (int x = 0; x < width; ++x)
            {
                blocks[index++] = Block.blackrock;
            }
        }
        
        static void GenSpace(MapGenArgs args) {
            Random rand = args.UseSeed ? new Random(args.Seed) : new Random();
            int width = args.Level.Width, height = args.Level.Height, length = args.Level.Length;
            int index = 0;
            byte[] blocks = args.Level.blocks;
            
            for (int y = 0; y < height; ++y)
                for (int z = 0; z < length; ++z)
                    for (int x = 0; x < width; ++x)
            {
                if (y == 0)
                    blocks[index] = Block.blackrock;
                else if (x == 0 || x == width - 1 || z == 0 || z == length - 1 || y == 1 || y == height - 1)
                    blocks[index] = rand.Next(100) == 0 ? Block.iron : Block.obsidian;
                index++;
            }
        }
        
        static void GenRainbow(MapGenArgs args) {
            Random rand = args.UseSeed ? new Random(args.Seed) : new Random();
            int width = args.Level.Width, height = args.Level.Height, length = args.Level.Length;
            int index = 0;
            byte[] blocks = args.Level.blocks;
            
            for (int y = 0; y < height; ++y)
                for (int z = 0; z < length; ++z)
                    for (int x = 0; x < width; ++x)
            {
                if (y == 0 || y == height - 1 || x == 0 || x == width - 1 || z == 0 || z == length - 1)
                    blocks[index] = (byte)rand.Next(Block.red, Block.white);
                index++;
            }
        }
        
        static void GenHell(MapGenArgs args) {
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
            GenSimple(args);
        }
        
        static void GenSimple(MapGenArgs args) {
            RealisticMapGen gen = new RealisticMapGen();
            gen.GenerateMap(args.Level, args.Type, args.Seed, args.UseSeed);
        }
    }
}
