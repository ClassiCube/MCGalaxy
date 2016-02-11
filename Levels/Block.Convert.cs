/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
        public static string Name(byte type)
        {
            switch (type)
            {
                case air: return "air";
                case rock: return "stone";
                case grass: return "grass";
                case dirt: return "dirt";
                case stone: return "cobblestone";
                case wood: return "wood";
                case shrub: return "plant";
                case blackrock: return "adminium";
                case water: return "active_water";
                case waterstill: return "water";
                case lava: return "active_lava";
                case lavastill: return "lava";
                case sand: return "sand";
                case gravel: return "gravel";
                case goldrock: return "gold_ore";
                case ironrock: return "iron_ore";
                case coal: return "coal";
                case trunk: return "tree";
                case leaf: return "leaves";
                case sponge: return "sponge";
                case glass: return "glass";
                case red: return "red";
                case orange: return "orange";
                case yellow: return "yellow";
                case lightgreen: return "greenyellow";
                case green: return "green";
                case aquagreen: return "springgreen";
                case cyan: return "cyan";
                case lightblue: return "blue";
                case blue: return "blueviolet";
                case purple: return "indigo";
                case lightpurple: return "purple";
                case pink: return "magenta";
                case darkpink: return "pink";
                case darkgrey: return "black";
                case lightgrey: return "gray";
                case white: return "white";
                case yellowflower: return "yellow_flower";
                case redflower: return "red_flower";
                case mushroom: return "brown_shroom";
                case redmushroom: return "red_shroom";
                case goldsolid: return "gold";
                case iron: return "iron";
                case staircasefull: return "double_stair";
                case staircasestep: return "stair";
                case brick: return "brick";
                case tnt: return "tnt";
                case bookcase: return "bookcase";
                case stonevine: return "mossy_cobblestone";
                case obsidian: return "obsidian";
                case cobblestoneslab: return "cobblestoneslab";
				case rope: return "rope";
				case sandstone: return "sandstone";
				case snow: return "snow";
				case fire: return "fire";
				case lightpink: return "lightpink";
				case forestgreen: return "forestgreen";
				case brown: return "brown";
				case deepblue: return "deepblue";
				case turquoise: return "turquoise";
				case ice: return "ice";
				case ceramictile: return "ceramictile";
				case magmablock: return "magmablock";
				case pillar: return "pillar";
				case crate: return "crate";
				case stonebrick: return "stonebrick";
                case flagbase: return "flagbase";
                case fastdeathlava: return "fast_hot_lava";
                case op_glass: return "op_glass";
                case opsidian: return "opsidian";              //TODO Add command or just use bind?
                case op_brick: return "op_brick";              //TODO
                case op_stone: return "op_stone";              //TODO
                case op_cobblestone: return "op_cobblestone";        //TODO
                case op_air: return "op_air";                //TODO
                case op_water: return "op_water";              //TODO
                case op_lava: return "op_lava";

                case griefer_stone: return "griefer_stone";
                case lava_sponge: return "lava_sponge";

                case wood_float: return "wood_float";            //TODO
                case door: return "door_wood";
                case lava_fast: return "lava_fast";
                case door2: return "door_obsidian";
                case door3: return "door_glass";
                case door4: return "door_stone";
                case door5: return "door_leaves";
                case door6: return "door_sand";
                case door7: return "door_wood";
                case door8: return "door_green";
                case door9: return "door_tnt";
                case door10: return "door_stair";
                case door_iron: return "door_iron";
				case door_gold: return "door_gold";
                case door_cobblestone: return "door_cobblestone";
                case door_red: return "door_red";
                case door_grass: return "door_grass";
                case door_dirt: return "door_dirt";
                case door_blue: return "door_blue";
                case door_book: return "door_book";

                case door_orange: return "door_orange";
                case door_yellow: return "door_yellow";
                case door_lightgreen: return "door_lightgreen";
                case door_aquagreen: return "door_aquagreen";
                case door_cyan: return "door_cyan";
                case door_lightblue: return "door_lightblue";
                case door_purple: return "door_purple";
                case door_lightpurple: return "door_lightpurple";
                case door_pink: return "door_pink";
                case door_darkpink: return "door_darkpink";
                case door_darkgrey: return "door_darkgrey";
                case door_lightgrey: return "door_lightgrey";
                case door_white: return "door_white";

                case tdoor: return "tdoor_wood";
                case tdoor2: return "tdoor_obsidian";
                case tdoor3: return "tdoor_glass";
                case tdoor4: return "tdoor_stone";
                case tdoor5: return "tdoor_leaves";
                case tdoor6: return "tdoor_sand";
                case tdoor7: return "tdoor_wood";
                case tdoor8: return "tdoor_green";
                case tdoor9: return "tdoor_tnt";
                case tdoor10: return "tdoor_stair";
                case tdoor11: return "tdoor_air";
                case tdoor12: return "tdoor_water";
                case tdoor13: return "tdoor_lava";

                case odoor1: return "odoor_wood";
                case odoor2: return "odoor_obsidian";
                case odoor3: return "odoor_glass";
                case odoor4: return "odoor_stone";
                case odoor5: return "odoor_leaves";
                case odoor6: return "odoor_sand";
                case odoor7: return "odoor_wood";
                case odoor8: return "odoor_green";
                case odoor9: return "odoor_tnt";
                case odoor10: return "odoor_stair";
                case odoor11: return "odoor_lava";
                case odoor12: return "odoor_water";

                case odoor1_air: return "odoor_wood_air";
                case odoor2_air: return "odoor_obsidian_air";
                case odoor3_air: return "odoor_glass_air";
                case odoor4_air: return "odoor_stone_air";
                case odoor5_air: return "odoor_leaves_air";
                case odoor6_air: return "odoor_sand_air";
                case odoor7_air: return "odoor_wood_air";
                case odoor8_air: return "odoor_red";
                case odoor9_air: return "odoor_tnt_air";
                case odoor10_air: return "odoor_stair_air";
                case odoor11_air: return "odoor_lava_air";
                case odoor12_air: return "odoor_water_air";

                case MsgWhite: return "white_message";
                case MsgBlack: return "black_message";
                case MsgAir: return "air_message";
                case MsgWater: return "water_message";
                case MsgLava: return "lava_message";

                case WaterDown: return "waterfall";
                case LavaDown: return "lavafall";
                case WaterFaucet: return "water_faucet";
                case LavaFaucet: return "lava_faucet";

                case finiteWater: return "finite_water";
                case finiteLava: return "finite_lava";
                case finiteFaucet: return "finite_faucet";

                case air_portal: return "air_portal";
                case water_portal: return "water_portal";
                case lava_portal: return "lava_portal";
                case custom_block: return "custom_block";
                case air_door: return "air_door";
                case air_switch: return "air_switch";
                case water_door: return "door_water";
                case lava_door: return "door_lava";

                case blue_portal: return "blue_portal";
                case orange_portal: return "orange_portal";

                case c4: return "c4";
                case c4det: return "c4_det";
                case smalltnt: return "small_tnt";
                case bigtnt: return "big_tnt";
                case nuketnt: return "nuke_tnt";
                case tntexplosion: return "tnt_explosion";

                case lava_fire: return "lava_fire";

                case rocketstart: return "rocketstart";
                case rockethead: return "rockethead";
                case firework: return "firework";

                case Block.deathlava: return "hot_lava";
                case Block.deathwater: return "cold_water";
                case Block.deathair: return "nerve_gas";
                case activedeathwater: return "active_cold_water";
                case activedeathlava: return "active_hot_lava";

                case Block.magma: return "magma";
                case Block.geyser: return "geyser";
                case Block.checkpoint: return "checkpoint";

                //Blocks after this are converted before saving
                case air_flood: return "air_flood";
                case door_air: return "door_air";
                case air_flood_layer: return "air_flood_layer";
                case air_flood_down: return "air_flood_down";
                case air_flood_up: return "air_flood_up";
                case door2_air: return "door2_air";
                case door3_air: return "door3_air";
                case door4_air: return "door4_air";
                case door5_air: return "door5_air";
                case door6_air: return "door6_air";
                case door7_air: return "door7_air";
                case door8_air: return "door8_air";
                case door9_air: return "door9_air";
                case door10_air: return "door10_air";
                case door11_air: return "door11_air";
                case door12_air: return "door12_air";
                case door13_air: return "door13_air";
                case door14_air: return "door14_air";
                case door_iron_air: return "door_iron_air";
				case door_gold_air: return "door_gold_air";
                case door_dirt_air: return "door_dirt_air";
                case door_grass_air: return "door_grass_air";
                case door_blue_air: return "door_blue_air";
                case door_book_air: return "door_book_air";
                case door_cobblestone_air: return "door_cobblestone_air";
                case door_red_air: return "door_red_air";
                case door_darkpink_air: return "door_darkpink_air";
                case door_darkgrey_air: return "door_darkgrey_air";
                case door_lightgrey_air: return "door_lightgrey_air";
                case door_white_air: return "door_white_air";

                //"AI" blocks
                case train: return "train";

                case snake: return "snake";
                case snaketail: return "snake_tail";

                case creeper: return "creeper";
                case zombiebody: return "zombie";
                case zombiehead: return "zombie_head";

                case Block.birdblue: return "blue_bird";
                case Block.birdred: return "red_robin";
                case Block.birdwhite: return "dove";
                case Block.birdblack: return "pidgeon";
                case Block.birdwater: return "duck";
                case Block.birdlava: return "phoenix";
                case Block.birdkill: return "killer_phoenix";

                case fishbetta: return "betta_fish";
                case fishgold: return "goldfish";
                case fishsalmon: return "salmon";
                case fishshark: return "shark";
                case fishsponge: return "sea_sponge";
                case fishlavashark: return "lava_shark";

                default: return "unknown";
            }
        }
        
        public static byte Byte(string type)
        {
            byte block;
            if (byte.TryParse(type, out block) && block < CpeCount)
                return block;
        	
            switch (type.ToLower())
            {
                case "air": return air;
                case "stone": return rock;
                case "grass": return grass;
                case "dirt": return dirt;
                case "cobblestone": return stone;
                case "wood": return wood;
                case "plant": return shrub;
                case "solid":
                case "admintite":
                case "blackrock":
                case "adminium": return blackrock;
                case "activewater":
                case "active_water": return water;
                case "water": return waterstill;
                case "activelava":
                case "active_lava": return lava;
                case "lava": return lavastill;
                case "sand": return sand;
                case "gravel": return gravel;
                case "gold_ore": return goldrock;
                case "iron_ore": return ironrock;
                case "coal": return coal;
                case "tree": return trunk;
                case "leaves": return leaf;
                case "sponge": return sponge;
                case "glass": return glass;
                case "red": return red;
                case "orange": return orange;
                case "yellow": return yellow;
                case "greenyellow": return lightgreen;
                case "green": return green;
                case "springgreen": return aquagreen;
                case "cyan": return cyan;
                case "blue": return lightblue;
                case "blueviolet": return blue;
                case "indigo": return purple;
                case "purple": return lightpurple;
                case "magenta": return pink;
                case "pink": return darkpink;
                case "black": return darkgrey;
                case "gray": return lightgrey;
                case "white": return white;
                case "yellow_flower": return yellowflower;
                case "red_flower": return redflower;
                case "brown_shroom": return mushroom;
                case "red_shroom": return redmushroom;
                case "gold": return goldsolid;
                case "iron": return iron;
                case "double_stair": return staircasefull;
                case "stair": return staircasestep;
                case "brick": return brick;
                case "tnt": return tnt;
                case "bookcase": return bookcase;
                case "mossy_cobblestone": return stonevine;
                case "obsidian": return obsidian;
                case "cobblestoneslab": return cobblestoneslab;
                case "rope": return rope;
                case "sandstone": return sandstone;
                case "snow": return snow;
                case "fire": return fire;
                case "lightpink": return lightpink;
                case "forestgreen": return forestgreen;
                case "brown": return brown;
                case "deepblue": return deepblue;
                case "turquoise": return turquoise;
                case "ice": return ice;
                case "ceramictile": return ceramictile;
                case "magmablock": return magmablock;
                case "pillar": return pillar;
                case "crate": return crate;
                case "stonebrick": return stonebrick;
                case "fhl":
                case "fast_hot_lava": return fastdeathlava;
                case "op_glass": return op_glass;
                case "opsidian": return opsidian;              //TODO Add command or just use bind?
                case "op_brick": return op_brick;              //TODO
                case "op_stone": return op_stone;              //TODO
                case "op_cobblestone": return op_cobblestone;        //TODO
                case "op_air": return op_air;                //TODO
                case "op_water": return op_water;              //TODO
                case "op_lava": return op_lava;

                case "griefer_stone": return griefer_stone;
                case "lava_sponge": return lava_sponge;

                case "wood_float": return wood_float;            //TODO
                case "lava_fast": return lava_fast;

                case "door_tree":
                case "door": return door;
                case "door_obsidian":
                case "door2": return door2;
                case "door_glass":
                case "door3": return door3;
                case "door_stone":
                case "door4": return door4;
                case "door_leaves":
                case "door5": return door5;
                case "door_sand":
                case "door6": return door6;
                case "door_wood":
                case "door7": return door7;
                case "door_green":
                case "door8": return door8;
                case "door_tnt":
                case "door9": return door9;
                case "door_stair":
                case "door10": return door10;
                case "door11":
                case "door_iron": return door_iron;
                case "door12":
                case "door_dirt": return door_dirt;
                case "door13":
                case "door_grass": return door_grass;
                case "door14":
                case "door_blue": return door_blue;
                case "door15":
                case "door_book": return door_book;
				case "door16":
				case "door_gold": return door_gold;
                case "door17":
                case "door_cobblestone": return door_cobblestone;
                case "door18":
                case "door_red": return door_red;

                case "door_orange": return door_orange;
                case "door_yellow": return door_yellow;
                case "door_lightgreen": return door_lightgreen;
                case "door_aquagreen": return door_aquagreen;
                case "door_cyan": return door_cyan;
                case "door_lightblue": return door_lightblue;
                case "door_purple": return door_purple;
                case "door_lightpurple": return door_lightpurple;
                case "door_pink": return door_pink;
                case "door_darkpink": return door_darkpink;
                case "door_darkgrey": return door_darkgrey;
                case "door_lightgrey": return door_lightgrey;
                case "door_white": return door_white;

                case "tdoor_tree":
                case "tdoor": return tdoor;
                case "tdoor_obsidian":
                case "tdoor2": return tdoor2;
                case "tdoor_glass":
                case "tdoor3": return tdoor3;
                case "tdoor_stone":
                case "tdoor4": return tdoor4;
                case "tdoor_leaves":
                case "tdoor5": return tdoor5;
                case "tdoor_sand":
                case "tdoor6": return tdoor6;
                case "tdoor_wood":
                case "tdoor7": return tdoor7;
                case "tdoor_green":
                case "tdoor8": return tdoor8;
                case "tdoor_tnt":
                case "tdoor9": return tdoor9;
                case "tdoor_stair":
                case "tdoor10": return tdoor10;
                case "tair_switch":
                case "tdoor11": return tdoor11;
                case "tdoor_water":
                case "tdoor12": return tdoor12;
                case "tdoor_lava":
                case "tdoor13": return tdoor13;

                case "odoor_tree":
                case "odoor": return odoor1;
                case "odoor_obsidian":
                case "odoor2": return odoor2;
                case "odoor_glass":
                case "odoor3": return odoor3;
                case "odoor_stone":
                case "odoor4": return odoor4;
                case "odoor_leaves":
                case "odoor5": return odoor5;
                case "odoor_sand":
                case "odoor6": return odoor6;
                case "odoor_wood":
                case "odoor7": return odoor7;
                case "odoor_green":
                case "odoor8": return odoor8;
                case "odoor_tnt":
                case "odoor9": return odoor9;
                case "odoor_stair":
                case "odoor10": return odoor10;
                case "odoor_lava":
                case "odoor11": return odoor11;
                case "odoor_water":
                case "odoor12": return odoor12;
                case "odoor_red": return odoor8_air;

                case "white_message": return MsgWhite;
                case "black_message": return MsgBlack;
                case "air_message": return MsgAir;
                case "water_message": return MsgWater;
                case "lava_message": return MsgLava;

                case "waterfall": return 140;
                case "lavafall": return 141;
                case "water_faucet": return WaterFaucet;
                case "lava_faucet": return LavaFaucet;

                case "finite_water": return finiteWater;
                case "finite_lava": return finiteLava;
                case "finite_faucet": return finiteFaucet;

                case "air_portal": return 160;
                case "water_portal": return 161;
                case "lava_portal": return 162;

                case "air_door": return air_door;
                case "air_switch": return air_switch;
                case "door_water":
                case "water_door": return water_door;
                case "door_lava":
                case "lava_door": return lava_door;

                case "blue_portal": return 175;
                case "orange_portal": return 176;

                case "c4": return c4;
                case "c4_det": return c4det;
                case "small_tnt": return 182;
                case "big_tnt": return 183;
                case "nuke_tnt": return 186;
                case "tnt_explosion": return 184;

                case "lava_fire": return lava_fire;

                case "rocketstart": return rocketstart;
                case "rockethead": return rockethead;
                case "firework": return firework;

                case "hot_lava": return deathlava;
                case "cold_water": return deathwater;
                case "nerve_gas": return deathair;
                case "acw":
                case "active_cold_water": return activedeathwater;
                case "ahl":
                case "active_hot_lava": return activedeathlava;

                case "magma": return magma;
                case "geyser": return geyser;
                case "checkpoint": return checkpoint;

                //Blocks after this are converted before saving
                case "air_flood": return air_flood;
                case "air_flood_layer": return air_flood_layer;
                case "air_flood_down": return air_flood_down;
                case "air_flood_up": return air_flood_up;
                case "door_air": return door_air;
                case "door2_air": return door2_air;
                case "door3_air": return door3_air;
                case "door4_air": return door4_air;
                case "door5_air": return door5_air;
                case "door6_air": return door6_air;
                case "door7_air": return door7_air;
                case "door8_air": return door8_air;
                case "door9_air": return door9_air;
                case "door10_air": return door10_air;
                case "door11_air": return door11_air;
                case "door12_air": return door12_air;
                case "door13_air": return door13_air;
                case "door14_air": return door14_air;
                case "door_iron_air": return door_iron_air;
                case "door_dirt_air": return door_dirt_air;
                case "door_grass_air": return door_grass_air;
                case "door_blue_air": return door_blue_air;
                case "door_book_air": return door_book_air;
				case "door_gold_air": return door_gold_air;
                case "door_cobblestone_air": return door_cobblestone_air;
                case "door_red_air": return door_red_air;
                case "door_darkpink_air": return door_darkpink_air;
                case "door_darkgrey_air": return door_darkgrey_air;
                case "door_lightgrey_air": return door_lightgrey_air;
                case "door_white_air": return door_white_air;

                case "train": return train;

                case "snake": return snake;
                case "snake_tail": return snaketail;

                case "creeper": return creeper;
                case "zombie": return zombiebody;
                case "zombie_head": return zombiehead;

                case "blue_bird": return Block.birdblue;
                case "red_robin": return Block.birdred;
                case "dove": return Block.birdwhite;
                case "pidgeon": return Block.birdblack;
                case "duck": return Block.birdwater;
                case "phoenix": return Block.birdlava;
                case "killer_phoenix": return Block.birdkill;

                case "betta_fish": return fishbetta;
                case "goldfish": return fishgold;
                case "salmon": return fishsalmon;
                case "shark": return fishshark;
                case "sea_sponge": return fishsponge;
                case "lava_shark": return fishlavashark;

                default: return Zero;
            }
        }
        
		public static byte ConvertCPE( byte b ) {
			switch ( b ) {
				case cobblestoneslab: return staircasestep;
				case rope: return mushroom;
				case sandstone: return sand;
				case snow: return air;
				case fire: return lava;
				case lightpink: return darkpink;
				case forestgreen: return green;
				case brown: return dirt;
				case deepblue: return blue;
				case turquoise: return lightblue;
				case ice: return glass;
				case ceramictile: return iron;
				case magmablock: return obsidian;
				case pillar: return white;
				case crate: return wood;
				case stonebrick: return rock;
				default:
				return b;
			}
		}
        
        public static byte Convert(byte b)
        {
            switch (b)
            {
                case flagbase: return mushroom;
                case op_glass: return glass;
                case opsidian: return obsidian;
                case op_brick: return brick;
                case op_stone: return rock;
                case op_cobblestone: return stone;
                case op_air: return air; //Must be cuboided / replaced
                case op_water: return waterstill;
                case op_lava: return lavastill;

                case griefer_stone: return stone; //Griefer_stone
                case lava_sponge: return sponge;

                case wood_float: return wood; //wood_float
                case lava_fast: return lava;
                case 71:
                case 72:
                    return Block.white;
                case door: return trunk;//door show by treetype
                case door2: return obsidian;//door show by obsidian
                case door3: return glass;//door show by glass
                case door4: return rock;//door show by stone
                case door5: return leaf;//door show by leaves
                case door6: return sand;//door show by sand
                case door7: return wood;//door show by wood
                case door8: return green;
                case door9: return tnt;//door show by TNT
                case door10: return staircasestep;//door show by Stair
                case door_iron: return iron;
                case door_dirt: return dirt;
                case door_grass: return grass;
                case door_blue: return blue;
                case door_book: return bookcase;
				case door_gold: return goldsolid;
                case door_cobblestone: return 4;
                case door_red: return red;

                case door_orange: return Block.orange;
                case door_yellow: return yellow;
                case door_lightgreen: return lightgreen;
                case door_aquagreen: return aquagreen;
                case door_cyan: return cyan;
                case door_lightblue: return lightblue;
                case door_purple: return purple;
                case door_lightpurple: return lightpurple;
                case door_pink: return pink;
                case door_darkpink: return darkpink;
                case door_darkgrey: return darkgrey;
                case door_lightgrey: return lightgrey;
                case door_white: return white;

                case tdoor: return trunk;//tdoor show by treetype
                case tdoor2: return obsidian;//tdoor show by obsidian
                case tdoor3: return glass;//tdoor show by glass
                case tdoor4: return rock;//tdoor show by stone
                case tdoor5: return leaf;//tdoor show by leaves
                case tdoor6: return sand;//tdoor show by sand
                case tdoor7: return wood;//tdoor show by wood
                case tdoor8: return green;
                case tdoor9: return tnt;//tdoor show by TNT
                case tdoor10: return staircasestep;//tdoor show by Stair
                case tdoor11: return air;
                case tdoor12: return waterstill;
                case tdoor13: return lavastill;

                case odoor1: return trunk;//odoor show by treetype
                case odoor2: return obsidian;//odoor show by obsidian
                case odoor3: return glass;//odoor show by glass
                case odoor4: return rock;//odoor show by stone
                case odoor5: return leaf;//odoor show by leaves
                case odoor6: return sand;//odoor show by sand
                case odoor7: return wood;//odoor show by wood
                case odoor8: return green;
                case odoor9: return tnt;//odoor show by TNT
                case odoor10: return staircasestep;//odoor show by Stair
                case odoor11: return lavastill;
                case odoor12: return waterstill;

                case MsgWhite: return white;
                case MsgBlack: return darkgrey;
                case MsgAir: return air;
                case MsgWater: return waterstill;
                case MsgLava: return lavastill;

                case WaterDown: return water;
                case LavaDown: return lava;
                case WaterFaucet: return Block.cyan;
                case LavaFaucet: return Block.orange;

                case finiteWater: return water;
                case finiteLava: return lava;
                case finiteFaucet: return lightblue;

                case air_portal: return air;
                case water_portal: return waterstill;
                case lava_portal: return lavastill;

                case air_door: return air;
                case air_switch: return air;//air door
                case water_door: return waterstill;//water door
                case lava_door: return lavastill;

                case blue_portal: return lightblue;
                case orange_portal: return orange;

                case c4: return tnt;
                case c4det: return red;
                case smalltnt: return tnt;
                case bigtnt: return tnt;
                case tntexplosion: return lava;

                case lava_fire: return lava;
                case nuketnt: return tnt;

                case rocketstart: return glass;
                case rockethead: return goldsolid;
                case firework: return iron;

                case Block.deathwater: return waterstill;
                case Block.deathlava: return lavastill;
                case Block.deathair: return air;
                case activedeathwater: return water;
                case activedeathlava: return lava;
                case fastdeathlava: return lava;

                case Block.magma: return Block.lava;
                case Block.geyser: return Block.water;
                case Block.checkpoint: return Block.air;

                case air_flood:
                case door_air:
                case air_flood_layer:
                case air_flood_down:
               	case air_flood_up:
                case door2_air:
                case door3_air:
                case door4_air:
                case door5_air:
                case door6_air:
                case door7_air:
                case door10_air:
                case door11_air:
                case door12_air:
                case door13_air:
                case door14_air:
                case door_iron_air:
				case door_gold_air:
                case door_cobblestone_air:
                case door_dirt_air:
                case door_grass_air:
                case door_blue_air:
                case door_book_air:
                case door_red_air:
                case door_darkpink_air:
                case door_darkgrey_air:
                case door_lightgrey_air:
                case door_white_air:
                    return air;
                case door8_air: return red;
                case door9_air: return lava;

                case odoor1_air:
                case odoor2_air:
                case odoor3_air:
                case odoor4_air:
                case odoor5_air:
                case odoor6_air:
                case odoor7_air:
                case odoor10_air:
                case odoor11_air:
                case odoor12_air:
                    return air;
                case odoor8_air: return red;
                case odoor9_air: return lavastill;

                case train: return cyan;

                case snake: return darkgrey;
                case snaketail: return coal;

                case creeper: return tnt;
                case zombiebody: return stonevine;
                case zombiehead: return lightgreen;

                case birdwhite: return white;
                case birdblack: return darkgrey;
                case birdlava: return lava;
                case birdred: return red;
                case birdwater: return water;
                case birdblue: return blue;
                case birdkill: return lava;

                case fishbetta: return blue;
                case fishgold: return goldsolid;
                case fishsalmon: return red;
                case fishshark: return lightgrey;
                case fishsponge: return sponge;
                case fishlavashark: return obsidian;
                
                case custom_block: return custom_block;
                default:
                    return b < CpeCount ? b : orange;
            }
        }
        public static byte SaveConvert(byte b)
        {
            switch (b)
            {
            	case air_flood:
            	case air_flood_layer:
            	case air_flood_down:
            	case air_flood_up:
                    return (byte)0; //air_flood must be converted to air on save to prevent issues
                case door_air: return door;
                case door2_air: return door2;
                case door3_air: return door3;
                case door4_air: return door4;
                case door5_air: return door5;
                case door6_air: return door6;
                case door7_air: return door7;
                case door8_air: return door8;
                case door9_air: return door9;
                case door10_air: return door10;
                case door11_air: return air_switch;
                case door12_air: return water_door;
                case door13_air: return lava_door;
                case door14_air: return air_door;
                case door_iron_air: return door_iron;
				case door_gold_air: return door_gold;
                case door_dirt_air: return door_dirt;
                case door_grass_air: return door_grass;
                case door_blue_air: return door_blue;
                case door_book_air: return door_book;
                case door_cobblestone_air: return door_cobblestone;
                case door_red_air: return door_red;
                case door_darkpink_air: return door_darkpink;
                case door_darkgrey_air: return door_darkgrey;
                case door_lightgrey_air: return door_lightgrey; 
                case door_white_air: return door_white; 
                
                case odoor1_air:
                case odoor2_air:
                case odoor3_air:
                case odoor4_air:
                case odoor5_air:
                case odoor6_air:
                case odoor7_air:
                case odoor8_air:
                case odoor9_air:
                case odoor10_air:
                case odoor11_air:
                case odoor12_air:
                    return odoor(b);

                default: return b;
            }
        }
    }
}
