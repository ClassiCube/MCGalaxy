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
using System.Text;
using BlockID = System.UInt16;
using BlockRaw = System.Byte;

namespace MCGalaxy.Blocks {
    
    /// <summary> Stores default properties for blocks in Minecraft Classic. (and CPE blocks). </summary>
    public static class DefaultSet {
        
        /// <summary> Constructs a custom block, with the default properties of the given classic/CPE block. </summary>
        public static BlockDefinition MakeCustomBlock(BlockID b) {
            BlockDefinition def = new BlockDefinition();
            def.SetBlock(b);
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
            ColorDesc fog = FogColor(b);
            def.FogR = fog.R; def.FogG = fog.G; def.FogB = fog.B;
            def.FallBack = (BlockRaw)b;
            
            def.MaxX = 16; def.MaxZ = Height(b); def.MaxY = 16;
            return def;
        }
        
        /// <summary> Gets the default height of a block. A value of 16 is full height. </summary>
        public static byte Height(BlockID b) {
            if (b == Block.Slab) return 8;
            if (b == Block.CobblestoneSlab) return 8;
            if (b == Block.Snow) return 2;
            return 16;
        }
        
        /// <summary> Gets whether a block is full bright / light emitting by default. </summary>
        public static bool FullBright(BlockID b) {
            return b == Block.Lava || b == Block.StillLava
                || b == Block.MagmaBlock || b == Block.Fire;
        }
        
        /// <summary> Gets the default fog density of a block, in packed form. </summary>
        public static byte FogDensity(BlockID b) {
            if (b == Block.Water || b == Block.StillWater)
                return 11; // (128 * 0.1f - 1);
            if (b == Block.Lava || b == Block.StillLava)
                return 229; // (128 * 1.8f - 1);
            return 0;
        }
        
        /// <summary> Gets the default fog color of a block. </summary>
        public static ColorDesc FogColor(BlockID b) {
            if (b == Block.Water || b == Block.StillWater)
                return new ColorDesc(5, 5, 51);
            if (b == Block.Lava || b == Block.StillLava)
                return new ColorDesc(153, 25, 0);
            return default(ColorDesc);
        }
        
        /// <summary> Gets the default collide type of a block, see CollideType class. </summary>
        public static byte Collide(BlockID b) {
            if (b >= Block.Water && b <= Block.StillLava)
                return CollideType.SwimThrough;
            if (b == Block.Snow || b == Block.Air || Draw(b) == DrawType.Sprite)
                return CollideType.WalkThrough;
            return CollideType.Solid;
        }
        
        /// <summary> Gets whether a block blocks light (prevents light passing through) by default. </summary>
        public static bool BlocksLight(BlockID b) {
            return !(b == Block.Glass || b == Block.Leaves
                     || b == Block.Air || Draw(b) == DrawType.Sprite);
        }
        

        /// <summary> Gets the default step sound of a block. </summary>
        public static SoundType StepSound(BlockID b) {
            if (b == Block.Glass) return SoundType.Stone;
            if (b == Block.Rope) return SoundType.Cloth;
            if (Draw(b) == DrawType.Sprite) return SoundType.None;
            
            if (b >= Block.Red && b <= Block.White)
                return SoundType.Cloth;
            if (b >= Block.LightPink && b <= Block.Turquoise)
                return SoundType.Cloth;
            if (b == Block.Iron || b == Block.Gold)
                return SoundType.Metal;
            
            if (b == Block.Bookshelf || b == Block.Wood
                || b == Block.Log || b == Block.Crate || b == Block.Fire)
                return SoundType.Wood;
            
            if (b == Block.Rope) return SoundType.Cloth;
            if (b == Block.Sand) return SoundType.Sand;
            if (b == Block.Snow) return SoundType.Snow;
            if (b == Block.Glass) return SoundType.Glass;
            if (b == Block.Dirt || b == Block.Gravel)
                return SoundType.Gravel;
            
            if (b == Block.Grass || b == Block.Sapling || b == Block.TNT
                || b == Block.Leaves || b == Block.Sponge)
                return SoundType.Grass;
            
            if (b >= Block.Dandelion && b <= Block.RedMushroom)
                return SoundType.Grass;
            if (b >= Block.Water && b <= Block.StillLava)
                return SoundType.None;
            if (b >= Block.Stone && b <= Block.StoneBrick)
                return SoundType.Stone;
            return SoundType.None;
        }
        

        /// <summary> Gets the default draw type of a block, see Draw class. </summary>        
        public static byte Draw(BlockID b) {
            if (b == Block.Air || b == Block.Invalid) return DrawType.Gas;
            if (b == Block.Leaves) return DrawType.TransparentThick;

            if (b == Block.Ice || b == Block.Water || b == Block.StillWater)
                return DrawType.Translucent;
            if (b == Block.Glass || b == Block.Leaves)
                return DrawType.Transparent;
            
            if (b >= Block.Dandelion && b <= Block.RedMushroom)
                return DrawType.Sprite;
            if (b == Block.Sapling || b == Block.Rope || b == Block.Fire)
                return DrawType.Sprite;
            return DrawType.Opaque;
        }
        
        
        const string RawNames = "Air_Stone_Grass_Dirt_Cobblestone_Wood_Sapling_Bedrock_Water_Still water_Lava" +
            "_Still lava_Sand_Gravel_Gold ore_Iron ore_Coal ore_Log_Leaves_Sponge_Glass_Red_Orange_Yellow_Lime_Green" +
            "_Teal_Aqua_Cyan_Blue_Indigo_Violet_Magenta_Pink_Black_Gray_White_Dandelion_Rose_Brown mushroom_Red mushroom" +
            "_Gold_Iron_Double slab_Slab_Brick_TNT_Bookshelf_Mossy rocks_Obsidian_Cobblestone slab_Rope_Sandstone" +
            "_Snow_Fire_Light pink_Forest green_Brown_Deep blue_Turquoise_Ice_Ceramic tile_Magma_Pillar_Crate_Stone brick";    
        
        static string Name(BlockID block) {
            // Find start and end of this particular block name
            int start = 0;
            for (int i = 0; i < block; i++)
                start = RawNames.IndexOf('_', start) + 1;
            
            int end = RawNames.IndexOf('_', start);
            if (end == -1) end = RawNames.Length;
            
            return RawNames.Substring(start, end - start);
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
        public const byte WalkThrough = 0; // Gas (usually also used by sprite)
        public const byte SwimThrough = 1; // Liquid
        public const byte Solid       = 2; // Solid
        public const byte Ice         = 3; // Solid and partially slidable on.
        public const byte SlipperyIce = 4; // Solid and fully slidable on.        
        public const byte LiquidWater = 5; // Water style 'swimming'/'bobbing'    
        public const byte LiquidLava  = 6; // Lava style 'swimming'/'bobbing'
        public const byte ClimbRope   = 7; // Rope style 'climbing'
        
        public static bool IsSolid(byte collide) {
            return collide >= Solid && collide <= SlipperyIce;
        }
    }
    
    public enum SoundType : byte {
        None, Wood, Gravel, Grass, Stone,
        Metal, Glass, Cloth, Sand, Snow,
    }
}