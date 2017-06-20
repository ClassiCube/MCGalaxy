﻿/*
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
using System.Text;
using MCGalaxy.Blocks;

namespace MCGalaxy.Blocks {
    
    /// <summary> Stores default properties for blocks in Minecraft Classic. (and CPE blocks). </summary>
    public static class DefaultSet {
        
        /// <summary> Constructs a custom block, with the default properties of the given classic/CPE block. </summary>
        public static BlockDefinition MakeCustomBlock(byte b) {
            BlockDefinition def = new BlockDefinition();
            def.BlockID = b;
            def.Name = Name(b);
            def.CollideType = Collide(b);
            def.Speed = 1;
            def.BlocksLight = BlocksLight(b);
            
            def.TopTex = topTex[b];
            def.BottomTex = bottomTex[b];
            def.SetSideTex(sideTex[b]);
            def.WalkSound = (byte)StepSound(b);
            
            def.FullBright = FullBright(b);
            def.Shape = Draw(b) == DrawType.Sprite ? (byte)0 : (byte)1;
            def.BlockDraw = Draw(b);
            if (def.BlockDraw == DrawType.Sprite)
                def.BlockDraw = DrawType.Transparent;
            
            def.FogDensity = FogDensity(b);
            CustomColor fog = FogColor(b);
            def.FogR = fog.R; def.FogG = fog.G; def.FogB = fog.B;
            def.FallBack = b;
            
            def.MaxX = 16; def.MaxZ = Height(b); def.MaxY = 16;
            def.Version2 = true;
            return def;
        }
        
        /// <summary> Gets the default height of a block. A value of 16 is full height. </summary>
        public static byte Height(byte b) {
            if (b == Block.staircasestep) return 8;
            if (b == Block.cobblestoneslab) return 8;
            if (b == Block.snow) return 2;
            return 16;
        }
        
        /// <summary> Gets whether a block is full bright / light emitting by default. </summary>
        public static bool FullBright(byte b) {
            return b == Block.lava || b == Block.lavastill
                || b == Block.magmablock || b == Block.fire;
        }
        
        /// <summary> Gets the default fog density of a block, in packed form. </summary>
        public static byte FogDensity(byte b) {
            if (b == Block.water || b == Block.waterstill)
                return 11; // (128 * 0.1f - 1);
            if (b == Block.lava || b == Block.lavastill)
                return 255; // (128 * 2 - 1);
            return 0;
        }
        
        /// <summary> Gets the default fog colour of a block. </summary>
        public static CustomColor FogColor(byte b) {
            if (b == Block.water || b == Block.waterstill)
                return new CustomColor(5, 5, 51);
            if (b == Block.lava || b == Block.lavastill)
                return new CustomColor(153, 25, 0);
            return default(CustomColor);
        }
        
        /// <summary> Gets the default collide type of a block, see CollideType class. </summary>
        public static byte Collide(byte b) {
            if (b >= Block.water && b <= Block.lavastill)
                return CollideType.SwimThrough;
            if (b == Block.snow || b == Block.air || Draw(b) == DrawType.Sprite)
                return CollideType.WalkThrough;
            return CollideType.Solid;
        }
        
        /// <summary> Gets whether a block blocks light (prevents light passing through) by default. </summary>
        public static bool BlocksLight(byte b) {
            return !(b == Block.glass || b == Block.leaf
                     || b == Block.air || Draw(b) == DrawType.Sprite);
        }
        

        /// <summary> Gets the default step sound of a block. </summary>
        public static SoundType StepSound(byte b) {
            if (b == Block.glass) return SoundType.Stone;
            if (b == Block.rope) return SoundType.Cloth;
            if (Draw(b) == DrawType.Sprite) return SoundType.None;
            
            if (b >= Block.red && b <= Block.white)
                return SoundType.Cloth;
            if (b >= Block.lightpink && b <= Block.turquoise)
                return SoundType.Cloth;
            if (b == Block.iron || b == Block.goldsolid)
                return SoundType.Metal;
            
            if (b == Block.bookcase || b == Block.wood
                || b == Block.trunk || b == Block.crate || b == Block.fire)
                return SoundType.Wood;
            
            if (b == Block.rope) return SoundType.Cloth;
            if (b == Block.sand) return SoundType.Sand;
            if (b == Block.snow) return SoundType.Snow;
            if (b == Block.glass) return SoundType.Glass;
            if (b == Block.dirt || b == Block.gravel)
                return SoundType.Gravel;
            
            if (b == Block.grass || b == Block.shrub || b == Block.tnt
                || b == Block.leaf || b == Block.sponge)
                return SoundType.Grass;
            
            if (b >= Block.yellowflower && b <= Block.redmushroom)
                return SoundType.Grass;
            if (b >= Block.water && b <= Block.lavastill)
                return SoundType.None;
            if (b >= Block.rock && b <= Block.stonebrick)
                return SoundType.Stone;
            return SoundType.None;
        }
        

        /// <summary> Gets the default draw type of a block, see Draw class. </summary>        
        public static byte Draw(byte b) {
            if (b == Block.air || b == Block.Invalid) return DrawType.Gas;
            if (b == Block.leaf) return DrawType.TransparentThick;

            if (b == Block.ice || b == Block.water || b == Block.waterstill)
                return DrawType.Translucent;
            if (b == Block.glass || b == Block.leaf)
                return DrawType.Transparent;
            
            if (b >= Block.yellowflower && b <= Block.redmushroom)
                return DrawType.Sprite;
            if (b == Block.shrub || b == Block.rope || b == Block.fire)
                return DrawType.Sprite;
            return DrawType.Opaque;
        }
        
        
        const string Names = "Air Stone Grass Dirt Cobblestone Wood Sapling Bedrock Water StillWater Lava" +
            " StillLava Sand Gravel GoldOre IronOre CoalOre Log Leaves Sponge Glass Red Orange Yellow Lime Green" +
            " Teal Aqua Cyan Blue Indigo Violet Magenta Pink Black Gray White Dandelion Rose BrownMushroom RedMushroom" +
            " Gold Iron DoubleSlab Slab Brick TNT Bookshelf MossyRocks Obsidian CobblestoneSlab Rope Sandstone" +
            " Snow Fire LightPink ForestGreen Brown DeepBlue Turquoise Ice CeramicTile Magma Pillar Crate StoneBrick";
        
        static string Name(byte block) {
            // Find start and end of this particular block name
            int start = 0;
            for (int i = 0; i < block; i++)
                start = Names.IndexOf(' ', start) + 1;
            int end = Names.IndexOf(' ', start);
            if (end == -1) end = Names.Length;
            
            StringBuilder buffer = new StringBuilder();
            SplitUppercase(buffer, start, end);
            return buffer.ToString();
        }
        
        static void SplitUppercase(StringBuilder buffer, int start, int end) {
            for (int i = start; i < end; i++) {
                char c = Names[i];
                bool upper = Char.IsUpper(c) && i > start;
                bool nextLower = i < end - 1 && !Char.IsUpper(Names[i + 1]);
                
                if (upper && nextLower) {
                    buffer.Append(' ');
                    buffer.Append(Char.ToLower(c));
                } else {
                    buffer.Append(c);
                }
            }
        }
        
        
        static byte[] topTex = new byte[] { 0,  1,  0,  2, 16,  4, 15, 17, 14, 14,
            30, 30, 18, 19, 32, 33, 34, 21, 22, 48, 49, 64, 65, 66, 67, 68, 69, 70, 71,
            72, 73, 74, 75, 76, 77, 78, 79, 13, 12, 29, 28, 24, 23,  6,  6,  7,  9,  4,
            36, 37, 16, 11, 25, 50, 38, 80, 81, 82, 83, 84, 51, 54, 86, 26, 53, 52, };
        static byte[] sideTex = new byte[] { 0,  1,  3,  2, 16,  4, 15, 17, 14, 14,
            30, 30, 18, 19, 32, 33, 34, 20, 22, 48, 49, 64, 65, 66, 67, 68, 69, 70, 71,
            72, 73, 74, 75, 76, 77, 78, 79, 13, 12, 29, 28, 40, 39,  5,  5,  7,  8, 35,
            36, 37, 16, 11, 41, 50, 38, 80, 81, 82, 83, 84, 51, 54, 86, 42, 53, 52, };
        static byte[] bottomTex = new byte[] { 0,  1,  2,  2, 16,  4, 15, 17, 14, 14,
            30, 30, 18, 19, 32, 33, 34, 21, 22, 48, 49, 64, 65, 66, 67, 68, 69, 70, 71,
            72, 73, 74, 75, 76, 77, 78, 79, 13, 12, 29, 28, 56, 55,  6,  6,  7, 10,  4,
            36, 37, 16, 11, 57, 50, 38, 80, 81, 82, 83, 84, 51, 54, 86, 58, 53, 52 };
    }
    
    public static class DrawType {
        public const byte Opaque = 0;
        public const byte Transparent = 1;
        public const byte TransparentThick = 2; // e.g. leaves render all neighbours
        public const byte Translucent = 3;
        public const byte Gas = 4;
        public const byte Sprite = 5;
    }
    
    public static class CollideType {
        public const byte WalkThrough = 0; // i.e. gas or sprite
        public const byte SwimThrough = 1; // i.e. liquid
        public const byte Solid       = 2; // i.e. solid
    }
    
    public enum SoundType : byte {
        None, Wood, Gravel, Grass, Stone,
        Metal, Glass, Cloth, Sand, Snow,
    }
}