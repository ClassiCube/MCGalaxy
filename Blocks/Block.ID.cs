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

namespace MCGalaxy
{
    public sealed partial class Block
    {
        public const byte OriginalMaxBlock = obsidian;
        public const byte OriginalCount = OriginalMaxBlock + 1;
        public const byte CpeMaxBlock = stonebrick;
        public const byte CpeCount = CpeMaxBlock + 1;
        
        public const byte air = 0;
        public const byte rock = 1;
        public const byte grass = 2;
        public const byte dirt = 3;
        public const byte stone = 4;
        public const byte wood = 5;
        public const byte shrub = 6;
        public const byte blackrock = 7;// adminium
        public const byte water = 8;
        public const byte waterstill = 9;
        public const byte lava = 10;
        public const byte lavastill = 11;
        public const byte sand = 12;
        public const byte gravel = 13;
        public const byte goldrock = 14;
        public const byte ironrock = 15;
        public const byte coal = 16;
        public const byte trunk = 17;
        public const byte leaf = 18;
        public const byte sponge = 19;
        public const byte glass = 20;
        public const byte red = 21;
        public const byte orange = 22;
        public const byte yellow = 23;
        public const byte lightgreen = 24;
        public const byte green = 25;
        public const byte aquagreen = 26;
        public const byte cyan = 27;
        public const byte lightblue = 28;
        public const byte blue = 29;
        public const byte purple = 30;
        public const byte lightpurple = 31;
        public const byte pink = 32;
        public const byte darkpink = 33;
        public const byte darkgrey = 34;
        public const byte lightgrey = 35;
        public const byte white = 36;
        public const byte yellowflower = 37;
        public const byte redflower = 38;
        public const byte mushroom = 39;
        public const byte redmushroom = 40;
        public const byte goldsolid = 41;
        public const byte iron = 42;
        public const byte staircasefull = 43;
        public const byte staircasestep = 44;
        public const byte brick = 45;
        public const byte tnt = 46;
        public const byte bookcase = 47;
        public const byte stonevine = 48;
        public const byte obsidian = 49;
        public const byte cobblestoneslab = 50;
        public const byte rope = 51;
        public const byte sandstone = 52;
        public const byte snow = 53;
        public const byte fire = 54;
        public const byte lightpink = 55;
        public const byte forestgreen = 56;
        public const byte brown = 57;
        public const byte deepblue = 58;
        public const byte turquoise = 59;
        public const byte ice = 60;
        public const byte ceramictile = 61;
        public const byte magmablock = 62;
        public const byte pillar = 63;
        public const byte crate = 64;
        public const byte stonebrick = 65;
        public const byte Zero = 0xff;

        public const byte door_darkpink_air = 66;
        public const byte door_darkgrey_air = 67;
        public const byte door_lightgrey_air = 68;
        public const byte door_white_air = 69;
        public const byte flagbase = 70;

        //Seasons
        //public const byte fallsnow = 71;
        //public const byte snow = 72;

        public const byte fastdeathlava = 73;

        public const byte c4 = 74;
        public const byte c4det = 75;

        public const byte door_cobblestone = 80;
        public const byte door_cobblestone_air = 81;
        public const byte door_red = 83;
        public const byte door_red_air = 84;

        public const byte door_orange = 85;
        public const byte door_yellow = 86;
        public const byte door_lightgreen = 87;
        public const byte door_aquagreen = 89;
        public const byte door_cyan = 90;
        public const byte door_lightblue = 91;
        public const byte door_purple = 92;
        public const byte door_lightpurple = 93;
        public const byte door_pink = 94;
        public const byte door_darkpink = 95;
        public const byte door_darkgrey = 96;
        public const byte door_lightgrey = 97;
        public const byte door_white = 98;

        public const byte op_glass = 100;
        public const byte opsidian = 101;
        public const byte op_brick = 102;
        public const byte op_stone = 103;
        public const byte op_cobblestone = 104;
        public const byte op_air = 105;
        public const byte op_water = 106;
        public const byte op_lava = 107;

        public const byte griefer_stone = 108;
        public const byte lava_sponge = 109;

        public const byte wood_float = 110;
        public const byte door_tree = 111;
        public const byte lava_fast = 112;
        public const byte door_obsidian = 113;
        public const byte door_glass = 114;
        public const byte door_stone = 115;
        public const byte door_leaves = 116;
        public const byte door_sand = 117;
        public const byte door_wood = 118;
        public const byte door_green = 119;
        public const byte door_tnt = 120;
        public const byte door_stair = 121;

        public const byte tdoor = 122;
        public const byte tdoor2 = 123;
        public const byte tdoor3 = 124;
        public const byte tdoor4 = 125;
        public const byte tdoor5 = 126;
        public const byte tdoor6 = 127;
        public const byte tdoor7 = 128;
        public const byte tdoor8 = 129;

        //Messages
        public const byte MsgWhite = 130;
        public const byte MsgBlack = 131;
        public const byte MsgAir = 132;
        public const byte MsgWater = 133;
        public const byte MsgLava = 134;

        public const byte tdoor9 = 135;
        public const byte tdoor10 = 136;
        public const byte tdoor11 = 137;
        public const byte tdoor12 = 138;
        public const byte tdoor13 = 139;

        //"finite"
        public const byte WaterDown = 140;
        public const byte LavaDown = 141;
        public const byte WaterFaucet = 143;
        public const byte LavaFaucet = 144;

        public const byte finiteWater = 145;
        public const byte finiteLava = 146;
        public const byte finiteFaucet = 147;

        public const byte odoor1 = 148;
        public const byte odoor2 = 149;
        public const byte odoor3 = 150;
        public const byte odoor4 = 151;
        public const byte odoor5 = 152;
        public const byte odoor6 = 153;
        public const byte odoor7 = 154;
        public const byte odoor8 = 155;
        public const byte odoor9 = 156;
        public const byte odoor10 = 157;
        public const byte odoor11 = 158;
        public const byte odoor12 = 159;

        //movement
        public const byte air_portal = 160;
        public const byte water_portal = 161;
        public const byte lava_portal = 162;

        //BlockDefinitions
        public const byte custom_block = 163;
        //Movement doors
        public const byte air_door = 164;
        public const byte air_switch = 165;
        public const byte water_door = 166;
        public const byte lava_door = 167;

        public const byte odoor1_air = 168;
        public const byte odoor2_air = 169;
        public const byte odoor3_air = 170;
        public const byte odoor4_air = 171;
        public const byte odoor5_air = 172;
        public const byte odoor6_air = 173;
        public const byte odoor7_air = 174;

        //portals
        public const byte blue_portal = 175;
        public const byte orange_portal = 176;

        public const byte odoor8_air = 177;
        public const byte odoor9_air = 178;
        public const byte odoor10_air = 179;
        public const byte odoor11_air = 180;
        public const byte odoor12_air = 181;

        //Explosions
        public const byte smalltnt = 182;
        public const byte bigtnt = 183;
        public const byte tntexplosion = 184;

        public const byte lava_fire = 185;
        
        public const byte nuketnt = 186;

        public const byte rocketstart = 187;
        public const byte rockethead = 188;
        public const byte firework = 189;

        //Death
        public const byte deathlava = 190;
        public const byte deathwater = 191;
        public const byte deathair = 192;

        public const byte activedeathwater = 193;
        public const byte activedeathlava = 194;

        public const byte magma = 195;
        public const byte geyser = 196;
        public const byte checkpoint = 197;

        public const byte air_flood = 200;
        public const byte door_tree_air = 201;
        public const byte air_flood_layer = 202;
        public const byte air_flood_down = 203;
        public const byte air_flood_up = 204;
        public const byte door_obsidian_air = 205;
        public const byte door_glass_air = 206;
        public const byte door_stone_air = 207;
        public const byte door_leaves_air = 208;
        public const byte door_sand_air = 209;
        public const byte door_wood_air = 210;
        public const byte door_green_air = 211;
        public const byte door_tnt_air = 212;
        public const byte door_stair_air = 213;
        public const byte air_switch_air = 214;
        public const byte water_door_air = 215;
        public const byte lava_door_air = 216;
        public const byte air_door_air = 217;

        public const byte door_iron = 220;
        public const byte door_dirt = 221;
        public const byte door_grass = 222;
        public const byte door_blue = 223;
        public const byte door_book = 224;
        public const byte door_iron_air = 225;
        public const byte door_dirt_air = 226;
        public const byte door_grass_air = 227;
        public const byte door_blue_air = 228;
        public const byte door_book_air = 229;

        public const byte train = 230;

        public const byte creeper = 231;
        public const byte zombiebody = 232;
        public const byte zombiehead = 233;

        public const byte birdwhite = 235;
        public const byte birdblack = 236;
        public const byte birdwater = 237;
        public const byte birdlava = 238;
        public const byte birdred = 239;
        public const byte birdblue = 240;
        public const byte birdkill = 242;

        public const byte fishgold = 245;
        public const byte fishsponge = 246;
        public const byte fishshark = 247;
        public const byte fishsalmon = 248;
        public const byte fishbetta = 249;
        public const byte fishlavashark = 250;

        public const byte snake = 251;
        public const byte snaketail = 252;
        
        public const byte door_gold = 253;
        public const byte door_gold_air = 254;
    }
}
