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
        
        public const byte air = (byte)0;
        public const byte rock = (byte)1;
        public const byte grass = (byte)2;
        public const byte dirt = (byte)3;
        public const byte stone = (byte)4;
        public const byte wood = (byte)5;
        public const byte shrub = (byte)6;
        public const byte blackrock = (byte)7;// adminium
        public const byte water = (byte)8;
        public const byte waterstill = (byte)9;
        public const byte lava = (byte)10;
        public const byte lavastill = (byte)11;
        public const byte sand = (byte)12;
        public const byte gravel = (byte)13;
        public const byte goldrock = (byte)14;
        public const byte ironrock = (byte)15;
        public const byte coal = (byte)16;
        public const byte trunk = (byte)17;
        public const byte leaf = (byte)18;
        public const byte sponge = (byte)19;
        public const byte glass = (byte)20;
        public const byte red = (byte)21;
        public const byte orange = (byte)22;
        public const byte yellow = (byte)23;
        public const byte lightgreen = (byte)24;
        public const byte green = (byte)25;
        public const byte aquagreen = (byte)26;
        public const byte cyan = (byte)27;
        public const byte lightblue = (byte)28;
        public const byte blue = (byte)29;
        public const byte purple = (byte)30;
        public const byte lightpurple = (byte)31;
        public const byte pink = (byte)32;
        public const byte darkpink = (byte)33;
        public const byte darkgrey = (byte)34;
        public const byte lightgrey = (byte)35;
        public const byte white = (byte)36;
        public const byte yellowflower = (byte)37;
        public const byte redflower = (byte)38;
        public const byte mushroom = (byte)39;
        public const byte redmushroom = (byte)40;
        public const byte goldsolid = (byte)41;
        public const byte iron = (byte)42;
        public const byte staircasefull = (byte)43;
        public const byte staircasestep = (byte)44;
        public const byte brick = (byte)45;
        public const byte tnt = (byte)46;
        public const byte bookcase = (byte)47;
        public const byte stonevine = (byte)48;
        public const byte obsidian = (byte)49;
        public const byte cobblestoneslab = (byte)50;
        public const byte rope = (byte)51;
        public const byte sandstone = (byte)52;
        public const byte snow = (byte)53;
        public const byte fire = (byte)54;
        public const byte lightpink = (byte)55;
        public const byte forestgreen = (byte)56;
        public const byte brown = (byte)57;
        public const byte deepblue = (byte)58;
        public const byte turquoise = (byte)59;
        public const byte ice = (byte)60;
        public const byte ceramictile = (byte)61;
        public const byte magmablock = (byte)62;
        public const byte pillar = (byte)63;
        public const byte crate = (byte)64;
        public const byte stonebrick = (byte)65;
        public const byte Zero = 0xff;

        public const byte door_darkpink_air = (byte)66;
        public const byte door_darkgrey_air = (byte)67;
        public const byte door_lightgrey_air = (byte)68;
        public const byte door_white_air = (byte)69;
        public const byte flagbase = (byte)70;

        //Seasons
        //public const byte fallsnow = (byte)71;
        //public const byte snow = (byte)72;

        public const byte fastdeathlava = (byte)73;

        public const byte c4 = (byte)74;
        public const byte c4det = (byte)75;

        public const byte door_cobblestone = (byte)80;
        public const byte door_cobblestone_air = (byte)81;
        public const byte door_red = (byte)83;
        public const byte door_red_air = (byte)84;

        public const byte door_orange = (byte)85;
        public const byte door_yellow = (byte)86;
        public const byte door_lightgreen = (byte)87;
        public const byte door_aquagreen = (byte)89;
        public const byte door_cyan = (byte)90;
        public const byte door_lightblue = (byte)91;
        public const byte door_purple = (byte)92;
        public const byte door_lightpurple = (byte)93;
        public const byte door_pink = (byte)94;
        public const byte door_darkpink = (byte)95;
        public const byte door_darkgrey = (byte)96;
        public const byte door_lightgrey = (byte)97;
        public const byte door_white = (byte)98;

        public const byte op_glass = (byte)100;
        public const byte opsidian = (byte)101;
        public const byte op_brick = (byte)102;
        public const byte op_stone = (byte)103;
        public const byte op_cobblestone = (byte)104;
        public const byte op_air = (byte)105;
        public const byte op_water = (byte)106;
        public const byte op_lava = (byte)107;

        public const byte griefer_stone = (byte)108;
        public const byte lava_sponge = (byte)109;

        public const byte wood_float = (byte)110;
        public const byte door = (byte)111;
        public const byte lava_fast = (byte)112;
        public const byte door2 = (byte)113;
        public const byte door3 = (byte)114;
        public const byte door4 = (byte)115;
        public const byte door5 = (byte)116;
        public const byte door6 = (byte)117;
        public const byte door7 = (byte)118;
        public const byte door8 = (byte)119;
        public const byte door9 = (byte)120;
        public const byte door10 = (byte)121;

        public const byte tdoor = (byte)122;
        public const byte tdoor2 = (byte)123;
        public const byte tdoor3 = (byte)124;
        public const byte tdoor4 = (byte)125;
        public const byte tdoor5 = (byte)126;
        public const byte tdoor6 = (byte)127;
        public const byte tdoor7 = (byte)128;
        public const byte tdoor8 = (byte)129;

        //Messages
        public const byte MsgWhite = (byte)130;
        public const byte MsgBlack = (byte)131;
        public const byte MsgAir = (byte)132;
        public const byte MsgWater = (byte)133;
        public const byte MsgLava = (byte)134;

        public const byte tdoor9 = (byte)135;
        public const byte tdoor10 = (byte)136;
        public const byte tdoor11 = (byte)137;
        public const byte tdoor12 = (byte)138;
        public const byte tdoor13 = (byte)139;

        //"finite"
        public const byte WaterDown = (byte)140;
        public const byte LavaDown = (byte)141;
        public const byte WaterFaucet = (byte)143;
        public const byte LavaFaucet = (byte)144;

        public const byte finiteWater = (byte)145;
        public const byte finiteLava = (byte)146;
        public const byte finiteFaucet = (byte)147;

        public const byte odoor1 = (byte)148;
        public const byte odoor2 = (byte)149;
        public const byte odoor3 = (byte)150;
        public const byte odoor4 = (byte)151;
        public const byte odoor5 = (byte)152;
        public const byte odoor6 = (byte)153;
        public const byte odoor7 = (byte)154;
        public const byte odoor8 = (byte)155;
        public const byte odoor9 = (byte)156;
        public const byte odoor10 = (byte)157;
        public const byte odoor11 = (byte)158;
        public const byte odoor12 = (byte)159;

        //movement
        public const byte air_portal = (byte)160;
        public const byte water_portal = (byte)161;
        public const byte lava_portal = (byte)162;

        //BlockDefinitions
        public const byte custom_block = (byte)163;
        //Movement doors
        public const byte air_door = (byte)164;
        public const byte air_switch = (byte)165;
        public const byte water_door = (byte)166;
        public const byte lava_door = (byte)167;

        public const byte odoor1_air = (byte)168;
        public const byte odoor2_air = (byte)169;
        public const byte odoor3_air = (byte)170;
        public const byte odoor4_air = (byte)171;
        public const byte odoor5_air = (byte)172;
        public const byte odoor6_air = (byte)173;
        public const byte odoor7_air = (byte)174;

        //portals
        public const byte blue_portal = (byte)175;
        public const byte orange_portal = (byte)176;

        public const byte odoor8_air = (byte)177;
        public const byte odoor9_air = (byte)178;
        public const byte odoor10_air = (byte)179;
        public const byte odoor11_air = (byte)180;
        public const byte odoor12_air = (byte)181;

        //Explosions
        public const byte smalltnt = (byte)182;
        public const byte bigtnt = (byte)183;
        public const byte tntexplosion = (byte)184;

        public const byte lava_fire = (byte)185;
        
        public const byte nuketnt = (byte)186;

        public const byte rocketstart = (byte)187;
        public const byte rockethead = (byte)188;
        public const byte firework = (byte)189;

        //Death
        public const byte deathlava = (byte)190;
        public const byte deathwater = (byte)191;
        public const byte deathair = (byte)192;

        public const byte activedeathwater = (byte)193;
        public const byte activedeathlava = (byte)194;

        public const byte magma = (byte)195;
        public const byte geyser = (byte)196;
        public const byte checkpoint = (byte)197;

        public const byte air_flood = (byte)200;
        public const byte door_air = (byte)201;
        public const byte air_flood_layer = (byte)202;
        public const byte air_flood_down = (byte)203;
        public const byte air_flood_up = (byte)204;
        public const byte door2_air = (byte)205;
        public const byte door3_air = (byte)206;
        public const byte door4_air = (byte)207;
        public const byte door5_air = (byte)208;
        public const byte door6_air = (byte)209;
        public const byte door7_air = (byte)210;
        public const byte door8_air = (byte)211;
        public const byte door9_air = (byte)212;
        public const byte door10_air = (byte)213;
        public const byte door11_air = (byte)214;
        public const byte door12_air = (byte)215;
        public const byte door13_air = (byte)216;
        public const byte door14_air = (byte)217;

        public const byte door_iron = (byte)220;
        public const byte door_dirt = (byte)221;
        public const byte door_grass = (byte)222;
        public const byte door_blue = (byte)223;
        public const byte door_book = (byte)224;
        public const byte door_iron_air = (byte)225;
        public const byte door_dirt_air = (byte)226;
        public const byte door_grass_air = (byte)227;
        public const byte door_blue_air = (byte)228;
        public const byte door_book_air = (byte)229;

        public const byte train = (byte)230;

        public const byte creeper = (byte)231;
        public const byte zombiebody = (byte)232;
        public const byte zombiehead = (byte)233;

        public const byte birdwhite = (byte)235;
        public const byte birdblack = (byte)236;
        public const byte birdwater = (byte)237;
        public const byte birdlava = (byte)238;
        public const byte birdred = (byte)239;
        public const byte birdblue = (byte)240;
        public const byte birdkill = (byte)242;

        public const byte fishgold = (byte)245;
        public const byte fishsponge = (byte)246;
        public const byte fishshark = (byte)247;
        public const byte fishsalmon = (byte)248;
        public const byte fishbetta = (byte)249;
        public const byte fishlavashark = (byte)250;

        public const byte snake = (byte)251;
        public const byte snaketail = (byte)252;
        
        public const byte door_gold = (byte)253;
        public const byte door_gold_air = (byte)254;
    }
}
