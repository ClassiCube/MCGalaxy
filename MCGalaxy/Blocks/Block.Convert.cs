/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
    public sealed partial class Block {
		
        public static string Name(byte block) { return Props[block].Name; }
        
        public static byte Byte(string type) {
            byte block;
            if (byte.TryParse(type, out block) && block < CpeCount)
                return block;
            if (Aliases.TryGetValue(type.ToLower(), out block))
                return block;
            return Invalid;
        }
        
        public static byte ConvertCPE(byte block) {
            switch (block) {
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
                default: return block;
            }
        }
        
        public static byte Convert(byte block) {
            switch (block) {
                case flagbase: return mushroom;
                case op_glass: return glass;
                case opsidian: return obsidian;
                case op_brick: return brick;
                case op_stone: return rock;
                case op_cobblestone: return stone;
                case op_air: return air; //Must be cuboided / replaced
                case op_water: return waterstill;
                case op_lava: return lavastill;

                case griefer_stone: return stone;
                case lava_sponge: return sponge;

                case wood_float: return wood;
                case lava_fast: return lava;
                case 71:
                case 72:
                    return white;
                case door_tree: return trunk;
                case door_obsidian: return obsidian;
                case door_glass: return glass;
                case door_stone: return rock;
                case door_leaves: return leaf;
                case door_sand: return sand;
                case door_wood: return wood;
                case door_green: return green;
                case door_tnt: return tnt;
                case door_stair: return staircasestep;
                case door_iron: return iron;
                case door_dirt: return dirt;
                case door_grass: return grass;
                case door_blue: return blue;
                case door_book: return bookcase;
                case door_gold: return goldsolid;
                case door_cobblestone: return stone;
                case door_red: return red;

                case door_orange: return orange;
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
                case WaterFaucet: return cyan;
                case LavaFaucet: return orange;

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

                case deathwater: return waterstill;
                case deathlava: return lavastill;
                case deathair: return air;
                case activedeathwater: return water;
                case activedeathlava: return lava;
                case fastdeathlava: return lava;

                case magma: return lava;
                case geyser: return water;
                case checkpoint: return air;

                case air_flood:
                case door_tree_air:
                case air_flood_layer:
                case air_flood_down:
                case air_flood_up:
                case door_obsidian_air:
                case door_glass_air:
                case door_stone_air:
                case door_leaves_air:
                case door_sand_air:
                case door_wood_air:
                case door_stair_air:
                case air_switch_air:
                case water_door_air:
                case lava_door_air:
                case air_door_air:
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
                case door_green_air: return red;
                case door_tnt_air: return lava;

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
                    return block < CpeCount ? block : orange;
            }
        }
        
        public static byte SaveConvert(byte b) { return b; }
    }
}
