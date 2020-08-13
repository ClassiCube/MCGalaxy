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

namespace MCGalaxy {
    public static partial class Block {
        public const byte OriginalMaxBlock = Obsidian;
        public const byte OriginalCount = OriginalMaxBlock + 1;
        public const byte CpeMaxBlock = StoneBrick;
        public const byte CpeCount = CpeMaxBlock + 1;
        public const int Count = 256;

        // 10 bit block ids are broken down into: 2 bits of class/type, 8 bits value
        // class | value meaning
        // --------------
        // 00    | Default blocks:
        //       |    0  to 65  are classic + CPE blocks
        //       |    66 to 255 are physics blocks
        // 01    | Custom blocks:
        //       |    0  to 65  are same as default blocks (not used)
        //       |    66 to 255 are custom blocks 66 to 255
        // 10    | Extended custom blocks 256 to 511
        // 11    | Extended custom blocks 512 to 767
        
        #if TEN_BIT_BLOCKS
        public const ushort MaxRaw = 767;
        public const int ExtendedCount = 256 * 4;
        public static ushort[] ExtendedBase = new ushort[Block.Count];
        public static byte[] ExtendedClass = new byte[4];
        
        static Block() {
            ExtendedBase[custom_block]   = Extended;
            ExtendedBase[custom_block_2] = Extended * 2;
            ExtendedBase[custom_block_3] = Extended * 3;
            
            ExtendedClass[0] = Air;
            ExtendedClass[1] = custom_block;
            ExtendedClass[2] = custom_block_2;
            ExtendedClass[3] = custom_block_3;
        }
        #else
        public const ushort MaxRaw = 255;
        public const int ExtendedCount = 256 * 2;
        #endif
        
        // Original blocks
        public const byte Air = 0;
        public const byte Stone = 1;
        public const byte Grass = 2;
        public const byte Dirt = 3;
        public const byte Cobblestone = 4;
        public const byte Wood = 5;
        public const byte Sapling = 6;
        public const byte Bedrock = 7;// adminium
        public const byte Water = 8;
        public const byte StillWater = 9;
        public const byte Lava = 10;
        public const byte StillLava = 11;
        public const byte Sand = 12;
        public const byte Gravel = 13;
        public const byte GoldOre = 14;
        public const byte IronOre = 15;
        public const byte CoalOre = 16;
        public const byte Log = 17;
        public const byte Leaves = 18;
        public const byte Sponge = 19;
        public const byte Glass = 20;
        public const byte Red = 21;
        public const byte Orange = 22;
        public const byte Yellow = 23;
        public const byte Lime = 24;
        public const byte Green = 25;
        public const byte Teal = 26;
        public const byte Aqua = 27;
        public const byte Cyan = 28;
        public const byte Blue = 29;
        public const byte Indigo = 30;
        public const byte Violet = 31;
        public const byte Magenta = 32;
        public const byte Pink = 33;
        public const byte Black = 34;
        public const byte Gray = 35;
        public const byte White = 36;
        public const byte Dandelion = 37;
        public const byte Rose = 38;
        public const byte Mushroom = 39;
        public const byte RedMushroom = 40;
        public const byte Gold = 41;
        public const byte Iron = 42;
        public const byte DoubleSlab = 43;
        public const byte Slab = 44;
        public const byte Brick = 45;
        public const byte TNT = 46;
        public const byte Bookshelf = 47;
        public const byte MossyRocks = 48;
        public const byte Obsidian = 49;
        
        // CPE blocks
        public const byte CobblestoneSlab = 50;
        public const byte Rope = 51;
        public const byte Sandstone = 52;
        public const byte Snow = 53;
        public const byte Fire = 54;
        public const byte LightPink = 55;
        public const byte ForestGreen = 56;
        public const byte Brown = 57;
        public const byte DeepBlue = 58;
        public const byte Turquoise = 59;
        public const byte Ice = 60;
        public const byte CeramicTile = 61;
        public const byte MagmaBlock = 62;
        public const byte Pillar = 63;
        public const byte Crate = 64;
        public const byte StoneBrick = 65;

        //public const byte Door_Pink_air  = 66;     // unused in core
        //public const byte Door_Black_Air = 67;     // unused in core
        //public const byte Door_Gray_Air  = 68;     // unused in core
        //public const byte Door_White_Air = 69;     // unused in core
        public const byte FlagBase = 70;

        //Seasons
        //public const byte FallSnow = 71;           // unused in core
        //public const byte Snow = 72;               // unused in core

        public const byte Deadly_FastLava = 73;

        public const byte C4 = 74;
        public const byte C4Detonator = 75;
        // 76, 77, 78, 79 free

        public const byte Door_Cobblestone = 80;
        //public const byte Door_Cobblestone_air = 81;// unused in core
        public const byte Door_Red = 83;
        //public const byte Door_Red_air = 84;        // unused in core

        public const byte Door_Orange = 85;
        public const byte Door_Yellow = 86;
        public const byte Door_Lime = 87;
        public const byte Door_Teal = 89;
        public const byte Door_Aqua = 90;
        public const byte Door_Cyan = 91;
        public const byte Door_Indigo = 92;
        public const byte Door_Purple = 93;
        public const byte Door_Magenta = 94;
        public const byte Door_Pink = 95;
        public const byte Door_Black = 96;
        public const byte Door_Gray = 97;
        public const byte Door_White = 98;

        public const byte Op_Glass = 100;
        public const byte Op_Obsidian = 101;
        public const byte Op_Brick = 102;
        public const byte Op_Stone = 103;
        public const byte Op_Cobblestone = 104;
        public const byte Op_Air = 105;
        public const byte Op_Water = 106;
        public const byte Op_Lava = 107;

        //public const byte GrieferStone = 108;        // unused in core
        public const byte LavaSponge = 109;

        public const byte FloatWood = 110;
        public const byte Door_Log = 111;
        public const byte FastLava = 112;
        public const byte Door_Obsidian = 113;
        public const byte Door_Glass = 114;
        public const byte Door_Stone = 115;
        public const byte Door_Leaves = 116;
        public const byte Door_Sand = 117;
        public const byte Door_Wood = 118;
        public const byte Door_Green = 119;
        public const byte Door_TNT = 120;
        public const byte Door_Slab = 121;

        // tdoors
        public const byte tDoor_Log = 122;
        public const byte tDoor_Obsidian = 123;
        public const byte tDoor_Glass = 124;
        public const byte tDoor_Stone = 125;
        public const byte tDoor_Leaves = 126;
        public const byte tDoor_Sand = 127;
        public const byte tDoor_Wood = 128;
        public const byte tDoor_Green = 129;

        // Message blocks
        public const byte MB_White = 130;
        public const byte MB_Black = 131;
        public const byte MB_Air = 132;
        public const byte MB_Water = 133;
        public const byte MB_Lava = 134;

        // More tdoors
        public const byte tDoor_TNT = 135;
        public const byte tDoor_Slab = 136;
        public const byte tDoor_Air = 137;
        public const byte tDoor_Water = 138;
        public const byte tDoor_Lava = 139;

        // "finite" liquids
        public const byte WaterDown = 140;
        public const byte LavaDown = 141;
        public const byte WaterFaucet = 143;
        public const byte LavaFaucet = 144;

        public const byte FiniteWater = 145;
        public const byte FiniteLava = 146;
        public const byte FiniteFaucet = 147;

        // ODoor blocks
        public const byte oDoor_Log = 148;
        public const byte oDoor_Obsidian = 149;
        public const byte oDoor_Glass = 150;
        public const byte oDoor_Stone = 151;
        public const byte oDoor_Leaves = 152;
        public const byte oDoor_Sand = 153;
        public const byte oDoor_Wood = 154;
        public const byte oDoor_Green = 155;
        public const byte oDoor_TNT = 156;
        public const byte oDoor_Slab = 157;
        public const byte oDoor_Lava = 158;
        public const byte oDoor_Water = 159;

        // Movement portals
        public const byte Portal_Air = 160;
        public const byte Portal_Water = 161;
        public const byte Portal_Lava = 162;

        // BlockDefinitions
        public const byte custom_block = 163;
        
        // Movement doors
        public const byte Door_Air = 164;
        public const byte Door_AirActivatable = 165; // air_switch
        public const byte Door_Water = 166;
        public const byte Door_Lava = 167;

        // Odoor air forms
        public const byte oDoor_Log_air = 168;
        public const byte oDoor_Obsidian_air = 169;
        public const byte oDoor_Glass_air = 170;
        public const byte oDoor_Stone_air = 171;
        public const byte oDoor_Leaves_air = 172;
        public const byte oDoor_Sand_air = 173;
        public const byte oDoor_Wood_air = 174;

        // Solid portals
        public const byte Portal_Blue = 175;
        public const byte Portal_Orange = 176;

        // More odoor air forms
        public const byte oDoor_Green_air = 177;
        public const byte oDoor_TNT_air = 178;
        public const byte oDoor_Slab_air = 179;
        public const byte oDoor_Lava_air = 180;
        public const byte oDoor_Water_air = 181;

        // Explosions
        public const byte TNT_Small = 182;
        public const byte TNT_Big = 183;
        public const byte TNT_Explosion = 184;
        public const byte LavaFire = 185;        
        public const byte TNT_Nuke = 186;
        public const byte RocketStart = 187;
        public const byte RocketHead = 188;
        public const byte Fireworks = 189;

        // Killer blocks
        public const byte Deadly_Lava = 190;
        public const byte Deadly_Water = 191;
        public const byte Deadly_Air = 192;
        public const byte Deadly_ActiveWater = 193;
        public const byte Deadly_ActiveLava = 194;

        // Special liquid blocks
        public const byte Magma = 195;
        public const byte Geyser = 196;
        
        public const byte Checkpoint = 197;
        #if TEN_BIT_BLOCKS
        public const byte custom_block_2 = 198;
        public const byte custom_block_3 = 199;
        #endif
        
        // Air type blocks
        public const byte Air_Flood = 200;
        public const byte Door_Log_air = 201;
        public const byte Air_FloodLayer = 202;
        public const byte Air_FloodDown = 203;
        public const byte Air_FloodUp = 204;
        //public const byte Door_Obsidian_air = 205;   // unused in core
        //public const byte Door_Glass_air = 206;      // unused in core
        //public const byte Door_Stone_air = 207;      // unused in core
        //public const byte Door_Leaves_air = 208;     // unused in core
        //public const byte Door_Sand_air = 209;       // unused in core
        //public const byte Door_Wood_air = 210;       // unused in core
        public const byte Door_Green_air = 211;
        public const byte Door_TNT_air = 212;
        //public const byte Door_Slab_air = 213;       // unused in core
        //public const byte Door_AirActivatable_air = 214;
        //public const byte Door_Water_air = 215;      // unused in core
        //public const byte Door_Lava_air = 216;       // unused in core
        public const byte Door_Air_air = 217;
        // 218, 219 free

        public const byte Door_Iron = 220;
        public const byte Door_Dirt = 221;
        public const byte Door_Grass = 222;
        public const byte Door_Blue = 223;
        public const byte Door_Bookshelf = 224;
        //public const byte Door_Iron_air = 225;       // unused in core
        //public const byte Door_Dirt_air = 226;       // unused in core
        //public const byte Door_Grass_air = 227;      // unused in core
        //public const byte Door_Blue_air = 228;       // unused in core
        //public const byte Door_Bookshelf_air = 229;  // unused in core

        public const byte Train = 230;

        public const byte Creeper = 231;
        public const byte ZombieBody = 232;
        public const byte ZombieHead = 233;        
        // 234 free

        // Bird blocks
        public const byte Bird_White = 235;
        public const byte Bird_Black = 236;
        public const byte Bird_Water = 237;
        public const byte Bird_Lava = 238;
        public const byte Bird_Red = 239;
        public const byte Bird_Blue = 240;
        public const byte Bird_Killer = 242;
        // 243, 244 free
        
        // Fish/Shark blocks
        public const byte Fish_Gold = 245;
        public const byte Fish_Sponge = 246;
        public const byte Fish_Shark = 247;
        public const byte Fish_Salmon = 248;
        public const byte Fish_Betta = 249;
        public const byte Fish_LavaShark = 250;

        public const byte Snake = 251;
        public const byte SnakeTail = 252;
        
        public const byte Door_Gold = 253;
        //public const byte Door_Gold_air = 254;       // unused in core
        
        public const byte Zero = 0xff; // backwards compatibility
        public const byte Invalid = 0xff;
        
        public const ushort Extended = 256;
        public const int ExtendedShift = 8;
    }
}